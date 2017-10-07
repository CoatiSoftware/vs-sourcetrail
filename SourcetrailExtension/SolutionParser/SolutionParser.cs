/*
 * Copyright 2017 Coati Software OG
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using CoatiSoftware.SourcetrailExtension.Utility;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VCProjectEngineWrapper;

namespace CoatiSoftware.SourcetrailExtension.SolutionParser
{
	public class SolutionParser
	{
		static private List<Guid> _reloadedProjectGuids = new List<Guid>();
		static private List<string> _compatibilityFlags = new List<string>() { "-fms-extensions", "-fms-compatibility" };
		static private string _compatibilityVersionFlagBase = "-fms-compatibility-version="; // We want to get the exact version at runtime for this flag, therefore keeping it seperate from the others makes things easier
		static private List<string> _cFileExtensions = new List<string>() { "c" };
		static private List<string> _cppFileExtensions = new List<string>() { "cc", "cpp", "cxx" };
		static private List<string> _sourceExtensionWhiteList = new List<string>(_cFileExtensions.Concat(_cppFileExtensions));
		static private List<string> _headerExtensionWhiteList = new List<string>() { "h", "hpp" };

		private string _compatibilityVersionFlag = _compatibilityVersionFlagBase + "19"; // This default version would correspond to VS2015

		private IPathResolver _pathResolver = null;

		public SolutionParser(IPathResolver pathResolver)
		{
			_pathResolver = pathResolver;
		}

		public void CreateCompileCommands(Project project, string solutionConfigurationName, string solutionPlatformName, string cStandard, string additionalClangOptions, bool nonSystemIncludesUseAngleBrackets, Action<CompileCommand, bool> lambda)
		{
			Logging.Logging.LogInfo("Creating command objects for project \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "\".");

			DTE dte = project.DTE;
			Guid projectGuid = Utility.ProjectUtility.ReloadProject(project);

			IVCProjectWrapper vcProject = VCProjectEngineWrapper.VCProjectWrapperFactory.create(project.Object);
			if (vcProject != null && vcProject.isValid())
			{
				Logging.Logging.LogInfo("Project \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "\" has been converted to VCProject " + vcProject.GetWrappedVersion() + ".");
			}
			else
			{
				Logging.Logging.LogWarning("Project \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "\" could not be converted to VCProject, skipping.");
				return;
			}

			string projectConfigurationName = "";
			string projectPlatformName = "";

			foreach (SolutionConfiguration2 solutionConfiguration in SolutionUtility.GetSolutionBuild2(dte).SolutionConfigurations)
			{
				if (solutionConfiguration.Name == solutionConfigurationName && solutionConfiguration.PlatformName == solutionPlatformName)
				{
					foreach (SolutionContext solutionContext in solutionConfiguration.SolutionContexts)
					{
						if (vcProject.GetProjectFile().EndsWith(solutionContext.ProjectName))
						{
							projectConfigurationName = solutionContext.ConfigurationName;
							projectPlatformName = solutionContext.PlatformName;
						}
					}
				}
			}

			if (projectConfigurationName.Length == 0 || projectPlatformName.Length == 0)
			{
				Logging.Logging.LogWarning("No project configuration found for solution configuration, trying to use solution configuration on project.");
				projectConfigurationName = solutionConfigurationName;
				projectPlatformName = solutionPlatformName;
			}

			IVCConfigurationWrapper vcProjectConfiguration = vcProject.getConfiguration(projectConfigurationName, projectPlatformName);

			if (vcProjectConfiguration != null && vcProjectConfiguration.isValid())
			{
				SetCompatibilityVersionFlag(vcProject, vcProjectConfiguration);

				string commandFlags = "";
				{
					// gather include paths and preprocessor definitions of the project
					List<string> projectIncludeDirectories = ProjectUtility.GetProjectIncludeDirectories(vcProject, vcProjectConfiguration, _pathResolver);
					List<string> systemIncludeDirectories = ProjectUtility.GetSystemIncludeDirectories(vcProject, vcProjectConfiguration, _pathResolver);
					List<string> preprocessorDefinitions = ProjectUtility.GetPreprocessorDefinitions(vcProject, vcProjectConfiguration);
					List<string> forcedIncludeFiles = ProjectUtility.GetForcedIncludeFiles(vcProject, vcProjectConfiguration, _pathResolver);

					foreach (string flag in _compatibilityFlags)
					{
						commandFlags += flag + " ";
					}

					commandFlags += _compatibilityVersionFlag + " ";

					if (!string.IsNullOrWhiteSpace(additionalClangOptions))
					{
						commandFlags += additionalClangOptions;
					}

					if (nonSystemIncludesUseAngleBrackets)
					{
						systemIncludeDirectories.InsertRange(0, projectIncludeDirectories);
						projectIncludeDirectories.Clear();
					}

					foreach (string dir in projectIncludeDirectories)
					{
						commandFlags += " -I \"" + dir + "\" ";
					}

					foreach (string dir in systemIncludeDirectories)
					{
						commandFlags += " -isystem \"" + dir + "\" ";
					}

					foreach (string prepDef in preprocessorDefinitions)
					{
						commandFlags += " -D " + prepDef + " ";
					}

					foreach (string file in forcedIncludeFiles)
					{
						commandFlags += " -include \"" + file + "\" ";
					}
				}

				string cppStandard = Utility.ProjectUtility.GetCppStandardForProject(vcProject);
				Logging.Logging.LogInfo("Found C++ standard " + cppStandard + ".");

				bool isMakefileProject = vcProjectConfiguration.isMakefileConfiguration();

				// create command objects for all applicable project items
				{
					List<CompileCommand> compileCommands = Utility.ProjectUtility.GetProjectItems(project).Select(item =>
					{
						return CreateCompileCommand(item, commandFlags, cppStandard, cStandard, isMakefileProject);
					}).Where(command => command != null).ToList();

					for (int i = 0; i < compileCommands.Count; i++)
					{
						lambda(compileCommands[i], i == compileCommands.Count - 1);
					}
				}

				if (projectGuid != Guid.Empty)
				{
					Utility.ProjectUtility.UnloadProject(projectGuid, dte);
				}
			}
			else
			{
				Logging.Logging.LogError("No project configuration found. Skipping this project");
			}
		}

		private CompileCommand CreateCompileCommand(ProjectItem item, string commandFlags, string vcStandard, string cStandard, bool isMakefileProject)
		{
			Logging.Logging.LogInfo("Starting to create Command Object from item \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(item.Name) + "\"");
			try
			{
				DTE dte = item.DTE;

				if (dte == null)
				{
					Logging.Logging.LogError("Failed to retrieve DTE object. Abort creating command object.");
				}

				IVCFileWrapper vcFile = VCFileWrapperFactory.create(item.Object);

				IVCCLCompilerToolWrapper compilerTool = null;
				if (!isMakefileProject)
				{
					List<IVCFileConfigurationWrapper> vcFileConfigurations = vcFile.GetFileConfigurations();
					foreach (IVCFileConfigurationWrapper vcFileConfiguration in vcFileConfigurations)
					{
						if (vcFileConfiguration != null && vcFileConfiguration.isValid())
						{
							compilerTool = vcFileConfiguration.GetCLCompilerTool();
							if (compilerTool != null && compilerTool.isValid())
							{
								break;
							}
						}
					}
				}

				if (CheckIsSourceFile(item))
				{
					string additionalOptions = "";
					if (compilerTool != null && compilerTool.isValid())
					{
						additionalOptions = compilerTool.GetAdditionalOptions();

						if (additionalOptions == "$(NOINHERIT)")
						{
							additionalOptions = "";
						}
					}

					string languageStandardOption;
					if (additionalOptions.Contains("-std="))
					{
						// if a language standard was defined in the additional options we do not need to set the language standard again.
						languageStandardOption = "";
					}
					else
					{
						if (compilerTool != null && compilerTool.isValid() && compilerTool.GetCompilesAsC())
						{
							languageStandardOption = "-std=" + cStandard;
						}
						else if (compilerTool != null && compilerTool.isValid() && compilerTool.GetCompilesAsCPlusPlus())
						{
							languageStandardOption = "-std=" + vcStandard;
						}
						else
						{
							// we cannot derive the language from the compiler tool, so we need to check the file extension
							if (ProjectUtility.HasProperty(item.Properties, "Extension") &&
								_cFileExtensions.Contains(item.Properties.Item("Extension").Value.ToString().Substring(1)) // extension property starts with "."
							)
							{
								languageStandardOption = "-std=" + cStandard;
							}
							else
							{
								languageStandardOption = "-std=" + vcStandard;
							}
						}
					}

					CompileCommand command = new CompileCommand();

					string relativeFilePath = item.Properties.Item("RelativePath").Value.ToString();
					command.File = _pathResolver.GetAsAbsoluteCanonicalPath(relativeFilePath, vcFile.GetProject());
					command.File = command.File.Replace('\\', '/');

					command.Directory = _pathResolver.GetCompilationDatabaseFilePath();

					command.Command = "clang-tool " + commandFlags;

					command.Command += languageStandardOption + " ";

					command.Command += additionalOptions + " ";

					command.Command += "\"" + command.File + "\"";

					return command;
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}

			Logging.Logging.LogError("Failed to create command object.");

			return null;
		}

		static private bool CheckIsSourceFile(ProjectItem item)
		{
			try
			{
				string itemType = "";
				if (ProjectUtility.HasProperty(item.Properties, "ItemType"))
				{
					itemType = item.Properties.Item("ItemType").Value.ToString();
				}

				if (itemType == "ClCompile")
				{
					Logging.Logging.LogInfo("Accepting item because of its \"ItemType\" property");
					return true;
				}

				if (itemType == "None")
				{
					Logging.Logging.LogInfo("Discarding item because \"ItemType\" has been set to \"Does not participate in build\"");
					return false;
				}

				string contentType = "";
				if (ProjectUtility.HasProperty(item.Properties, "ContentType"))
				{
					contentType = item.Properties.Item("ContentType").Value.ToString();
				}

				if (contentType == "CppCode")
				{
					Logging.Logging.LogInfo("Accepting item because of its \"ContentType\" property");
					return true;
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
			
			if (_sourceExtensionWhiteList.Contains(GetFileExtension(item).ToLower()))
			{
				Logging.Logging.LogInfo("Accepting item because of its file extension");
				return true;
			}

			Logging.Logging.LogInfo("Discarding item because of wrong code model");
			return false;
		}

		static private void ReloadAll(DTE dte)
		{
			if (dte == null)
			{
				return;
			}

			EnvDTE.Solution solution = dte.Solution;

			List<EnvDTE.Project> projects = Utility.SolutionUtility.GetSolutionProjectList(dte);

			foreach (EnvDTE.Project project in projects)
			{
				_reloadedProjectGuids.Add(Utility.ProjectUtility.ReloadProject(project));
			}
		}

		static private void UnloadReloadedProjects(DTE dte)
		{
			foreach (Guid guid in _reloadedProjectGuids)
			{
				Utility.ProjectUtility.UnloadProject(guid, dte);
			}
		}

		private void SetCompatibilityVersionFlag(IVCProjectWrapper project, IVCConfigurationWrapper vcProjectConfiguration)
		{
			Logging.Logging.LogInfo("Determining CL.exe (C++ compiler) version");

			int majorCompilerVersion = -1;

			{
				IVCCLCompilerToolWrapper compilerTool = vcProjectConfiguration.GetCLCompilerTool();
				if (compilerTool != null && compilerTool.isValid())
				{
					majorCompilerVersion = GetCLMajorVersion(compilerTool, vcProjectConfiguration);
				}
			}

			if (majorCompilerVersion > -1)
			{
				Logging.Logging.LogInfo("Found compiler version " + majorCompilerVersion.ToString());

				_compatibilityVersionFlag = _compatibilityVersionFlagBase + majorCompilerVersion.ToString();
				return;
			}
		}

		static private int GetCLMajorVersion(IVCCLCompilerToolWrapper compilerTool, IVCConfigurationWrapper vcProjectConfig)
		{
			Logging.Logging.LogInfo("Looking up CL.exe (C++ compiler)");

			if (compilerTool == null || !compilerTool.isValid() || vcProjectConfig == null || !vcProjectConfig.isValid())
			{
				return -1;
			}

			try
			{
				IVCPlatformWrapper platform = vcProjectConfig.GetPlatform();

				List<string> finalDirectories = new List<string>();
				foreach (string directory in platform.GetExecutableDirectories().Split(';'))
				{
					IPathResolver resolver = new VsPathResolver("");
					finalDirectories.AddRange(resolver.ResolveVsMacroInPath(directory, vcProjectConfig));
				}

				string toolPath = compilerTool.GetToolPath();

				Logging.Logging.LogInfo("Found " + finalDirectories.Count.ToString() + " possible compiler directories.");

				foreach (string fd in finalDirectories)
				{
					string path = fd + "\\" + toolPath;

					if (File.Exists(path))
					{
						FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);
						int version = info.FileMajorPart;

						Logging.Logging.LogInfo("Found compiler location. Compiler tool version is " + version.ToString());

						return version;
					}
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}

			Logging.Logging.LogWarning("Failed to find C++ compiler tool.");

			return -1;
		}

		static private string GetFileExtension(ProjectItem item)
		{
			if (item == null)
			{
				return "";
			}

			string name = item.Name;

			int idx = name.LastIndexOf(".");

			if (idx > -1 && idx < name.Length - 1)
			{
				return name.Substring(idx + 1);
			}
			else
			{
				return "";
			}
		}
	}
}
