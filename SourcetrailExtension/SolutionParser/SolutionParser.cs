﻿/*
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
		static private List<string> _sourceExtensionWhiteList = new List<string>() { "c", "cc", "cpp", "cxx" };
		static private List<string> _headerExtensionWhiteList = new List<string>() { "h", "hpp" };

		private string _compatibilityVersionFlag = _compatibilityVersionFlagBase + "19"; // This default version would correspond to VS2015

		private List<string> _headerDirectories = new List<string>();

		private IPathResolver _pathResolver = null;

		public List<string> HeaderDirectories
		{
			get { return _headerDirectories; }
		}

		public SolutionParser(IPathResolver pathResolver)
		{
			_pathResolver = pathResolver;
		}

		public List<CompileCommand> CreateCompileCommands(Project project, string configurationName, string platformName, string cStandard)
		{
			Logging.Logging.LogInfo("Creating command objects for project \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "\".");

			List<CompileCommand> result = new List<CompileCommand>();

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
				return result;
			}

			SetCompatibilityVersionFlag(vcProject, configurationName, platformName);

			// gather include paths and preprocessor definitions of the project
			List<string> includeDirectories = Utility.ProjectUtility.GetProjectIncludeDirectories(vcProject, configurationName, platformName, _pathResolver);
			List<string> preprocessorDefinitions = Utility.ProjectUtility.GetProjectPreprocessorDefinitions(vcProject, configurationName, platformName);

			string cppStandard = Utility.ProjectUtility.GetCppStandardForProject(vcProject, configurationName, platformName);
			Logging.Logging.LogInfo("Found C++ standard " + cppStandard + ".");

			// create command objects for all applicable project items
			foreach (EnvDTE.ProjectItem item in Utility.ProjectUtility.GetProjectItems(project))
			{
				CompileCommand command = CreateCompileCommand(item, includeDirectories, preprocessorDefinitions, cppStandard, cStandard, configurationName, platformName);
				if (command != null)
				{
					result.Add(command);
				}
			}

			if (projectGuid != Guid.Empty)
			{
				Utility.ProjectUtility.UnloadProject(projectGuid, dte);
			}

			_headerDirectories = _headerDirectories.Distinct().ToList();

			return result;
		}

		private CompileCommand CreateCompileCommand(EnvDTE.ProjectItem item, List<string> includeDirectories, List<string> preprocessorDefinitions, string vcStandard, string cStandard, string configurationName, string platformName)
		{
			Logging.Logging.LogInfo("Starting to create Command Object from item \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(item.Name) + "\"");

			try
			{
				DTE dte = item.DTE;

				if (dte == null)
				{
					Logging.Logging.LogError("Failed to retreive DTE object. Abort creating command object.");
				}

				IVCCLCompilerToolWrapper compilerTool = null;

				IVCFileWrapper vcFile = VCFileWrapperFactory.create(item.Object);
				List<IVCFileConfigurationWrapper> vcFileConfigurations = vcFile.GetFileConfigurations();
				foreach (IVCFileConfigurationWrapper vcFileConfiguration in vcFileConfigurations)
				{
					if (vcFileConfiguration != null && vcFileConfiguration.isValid())
					{
						compilerTool = vcFileConfiguration.GetTool();
						if (compilerTool != null && compilerTool.isValid())
						{
							break;
						}
					}
				}

				if (compilerTool == null || !compilerTool.isValid())
				{
					Logging.Logging.LogInfo("Unable to retrieve build tool. Using extension white-list to determine file type.");
				}

				if (CheckIsSourceFile(item, compilerTool))
				{
					CompileCommand command = new CompileCommand();
					command.File = item.Name;

					string additionalOptions = "";
					if (compilerTool != null && compilerTool.isValid())
					{
						additionalOptions = compilerTool.GetAdditionalOptions();

						if (additionalOptions == "$(NOINHERIT)")
						{
							additionalOptions = "";
						}

						// check wheter it's a .c file, we don't want that...
						// TODO: there is a property for comilation as .c or .cpp file (/TC and /TP), try to retrieve it
						string extension = item.Properties.Item("Extension").Value.ToString();
						if (compilerTool.GetCompilesAsC())
						{
							vcStandard = "-std=" + cStandard;
						}
					}

					// if a language standard was defined in the additional options the 'vcStandard' string is not used
					if (additionalOptions.Contains("-std="))
					{
						vcStandard = "";
					}

					string relativeFilePath = item.Properties.Item("RelativePath").Value.ToString();
					command.File = _pathResolver.GetAsAbsoluteCanonicalPath(relativeFilePath, vcFile.GetProject());
					command.File = command.File.Replace('\\', '/');

					command.Directory = _pathResolver.GetCompilationDatabaseFilePath();

					command.Command = "clang-tool ";

					foreach (string flag in _compatibilityFlags)
					{
						command.Command += flag + " ";
					}

					command.Command += _compatibilityVersionFlag + " ";

					foreach (string dir in includeDirectories)
					{
						command.Command += " -isystem '" + dir + "' "; // using '-isystem' because it allows for use of quotes and pointy brackets in source files. In other words it's more robust. It's slower than '-I' though
					}

					foreach (string prepDef in preprocessorDefinitions)
					{
						command.Command += " -D " + prepDef + " ";
					}

					command.Command += vcStandard + " ";

					command.Command += additionalOptions + " ";

					command.Command += "'" + command.File + "'";

					return command;
				}
				else if (CheckIsHeaderFile(item, compilerTool))
				{
					if (ProjectUtility.HasProperty(item.Properties, "FullPath"))
					{
						string propValue = item.Properties.Item("FullPath").Value.ToString();

						int i = propValue.LastIndexOf('\\');

						propValue = propValue.Substring(0, i);

						_headerDirectories.Add(propValue);
					}
					return null;
				}
				else
				{
					Logging.Logging.LogInfo("Item discarded, wrong code model");
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}

			Logging.Logging.LogError("Failed to create command object.");

			return null;
		}

		static private bool CheckIsSourceFile(ProjectItem item, IVCCLCompilerToolWrapper tool)
		{
			if (tool != null && tool.isValid()) // if the tool is null it's probably not a normal VC project, indicating that the file code model is unavailable
			{
				try
				{
					if (ProjectUtility.HasProperty(item.Properties, "ContentType") && item.Properties.Item("ContentType").Value.ToString() == "CppCode")
					{
						Logging.Logging.LogInfo("Accepting item because of its \"ContentType\" property");
						return true;
					}
					else if(ProjectUtility.HasProperty(item.Properties, "ItemType") && item.Properties.Item("ItemType").Value.ToString() == "ClCompile")
					{
						Logging.Logging.LogInfo("Accepting item because of its \"ItemType\" property");
						return true;
					}
				}
				catch (Exception e)
				{
					Logging.Logging.LogError("Exception: " + e.Message);
				}

				if (item.FileCodeModel != null && item.FileCodeModel.Language == CodeModelLanguageConstants.vsCMLanguageVC)
				{
					return true;
				}
			}
			else if (_sourceExtensionWhiteList.Contains(GetFileExtension(item).ToLower()))
			{
				return true;
			}

			return false;
		}

		static private bool CheckIsHeaderFile(EnvDTE.ProjectItem item, IVCCLCompilerToolWrapper tool)
		{
			if (tool != null && tool.isValid()) // if the tool is null it's probably not a normal VC project, indicating that the file code model is unavailable
			{
				try
				{
					if (ProjectUtility.HasProperty(item.Properties, "ItemType") && item.Properties.Item("ItemType").Value.ToString() == "ClInclude")
					{
						Logging.Logging.LogInfo("Accepting item because of its \"ItemType\" property");
						return true;
					}
				}
				catch (Exception e)
				{
					Logging.Logging.LogError("Exception: " + e.Message);
				}
			}
			else if (_headerExtensionWhiteList.Contains(GetFileExtension(item).ToLower()))
			{
				return true;
			}
			
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

		private void SetCompatibilityVersionFlag(IVCProjectWrapper project, string configurationName, string platformName)
		{
			Logging.Logging.LogInfo("Determining CL.exe (C++ compiler) version");

			IVCConfigurationWrapper vcProjectConfig = project.getConfiguration(configurationName, platformName);

			IVCCLCompilerToolWrapper compilerTool = vcProjectConfig.GetCompilerTool();

			if (compilerTool != null && compilerTool.isValid())
			{
				int majorCompilerVersion = GetCLMajorVersion(compilerTool, vcProjectConfig);

				if (majorCompilerVersion > -1)
				{
					Logging.Logging.LogInfo("Found compiler version " + majorCompilerVersion.ToString());

					_compatibilityVersionFlag = _compatibilityVersionFlagBase + majorCompilerVersion.ToString();
					return;
				}
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
					string resolvedDirectory = resolver.ResolveVsMacroInPath(directory, vcProjectConfig);
					string[] splitResolvedDirectory = resolvedDirectory.Split(';'); // resolved macros might result in concatenated paths

					foreach (string d in splitResolvedDirectory)
					{
						finalDirectories.Add(d);
					}
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
