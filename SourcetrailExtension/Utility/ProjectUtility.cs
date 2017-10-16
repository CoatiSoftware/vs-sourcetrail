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

using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VCProjectEngineWrapper;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	public class ProjectUtility
	{
		public static List<string> GetPropertyNamesAndValues(Properties properties)
		{
			List<string> ret = new List<string>();
			foreach (Property propertiy in properties)
			{
				string name = propertiy.Name;
				string value = "";

				try
				{
					value = propertiy.Value.ToString();
				}
				catch (Exception)
				{
					value = "Error occurred while converting value to string.";
				}

				ret.Add(name + ": " + value);
			}

			return ret;
		}

		public static bool HasProperty(Properties properties, string propertyName)
		{
			if (properties != null)
			{
				foreach (Property item in properties)
				{
					if (item != null && item.Name == propertyName)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool ContainsCFiles(Project project)
		{
			List<ProjectItem> projectItems = GetProjectItems(project);

			try
			{
				foreach (EnvDTE.ProjectItem item in projectItems)
				{
					if (item.FileCodeModel != null && item.FileCodeModel.Language == CodeModelLanguageConstants.vsCMLanguageVC)
					{
						string extension = item.Properties.Item("Extension").Value.ToString();
						if (extension == ".c")
						{
							return true;
						}
					}
				}
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}

			return false;
		}

		static public List<ProjectItem> GetProjectItems(Project project)
		{
			List<ProjectItem> items = new List<ProjectItem>();

			IEnumerator itemEnumerator = project.ProjectItems.GetEnumerator();

			while (itemEnumerator.MoveNext())
			{
				ProjectItem currentItem = (ProjectItem)itemEnumerator.Current;
				items.Add(GetProjectSubItemsRecursive(currentItem, ref items));
			}

			return items;
		}

		static private ProjectItem GetProjectSubItemsRecursive(ProjectItem item, ref List<ProjectItem> projectItems)
		{
			if (item.ProjectItems == null)
			{
				return item;
			}

			IEnumerator items = item.ProjectItems.GetEnumerator();

			while (items.MoveNext())
			{
				ProjectItem currentItem = (ProjectItem)items.Current;
				projectItems.Add(GetProjectSubItemsRecursive(currentItem, ref projectItems));
			}

			return item;
		}

		static private List<string> CleanIncludeDirectories(IVCProjectWrapper project, IPathResolver pathResolver, List<string> includeDirectories)
		{
			return includeDirectories
					.Select(x => x.Replace("\\\"", ""))
					.Select(x => pathResolver.GetAsAbsoluteCanonicalPath(x, project))
					.Select(x => x.Replace("\\", "/")) // backslashes would cause some string-escaping hassles...
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Distinct()
					.ToList();
		}

		static public List<string> GetProjectIncludeDirectories(IVCProjectWrapper project, IVCConfigurationWrapper vcProjectConfig, IPathResolver pathResolver)
		{
			// get additional include directories
			// source: http://www.mztools.com/articles/2014/MZ2014005.aspx

			Logging.Logging.LogInfo("Attempting to retrieve project Include Directories for project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.GetName()) + "'");

			List<string> includeDirectories = new List<string>();

			if (vcProjectConfig != null && vcProjectConfig.isValid())
			{
				if (vcProjectConfig.GetCLCompilerTool() != null && vcProjectConfig.GetCLCompilerTool().isValid())
				{
					includeDirectories.AddRange(GetIncludeDirectories(vcProjectConfig.GetCLCompilerTool()));
				}
				else if (vcProjectConfig.GetNMakeTool() != null && vcProjectConfig.GetNMakeTool().isValid())
				{
					includeDirectories.AddRange(GetIncludeDirectories(vcProjectConfig.GetNMakeTool()));
				}
			}
			else
			{
				Logging.Logging.LogWarning("Could not retrieve Project Configuration. No include directories could be retrieved.");
				return new List<string>();
			}

			Logging.Logging.LogInfo("Attempting to clean up.");
			includeDirectories = CleanIncludeDirectories(project, pathResolver, includeDirectories);

			Logging.Logging.LogInfo("Found " + includeDirectories.Count.ToString() + " distinct project include directories.");
			return includeDirectories;
		}

		static public List<string> GetSystemIncludeDirectories(IVCProjectWrapper project, IVCConfigurationWrapper vcProjectConfig, IPathResolver pathResolver)
		{
			Logging.Logging.LogInfo("Attempting to retrieve system Include Directories for project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.GetName()) + "'");

			List<string> includeDirectories = new List<string>();

			try
			{
				IVCPlatformWrapper platform = vcProjectConfig.GetPlatform();
				if (platform != null && platform.isValid())
				{
					foreach (string directory in platform.GetIncludeDirectories())
					{
						includeDirectories.AddRange(pathResolver.ResolveVsMacroInPath(directory, vcProjectConfig));
					}
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Failed to retrieve platform include directories: " + e.Message);
				return new List<string>();
			}

			Logging.Logging.LogInfo("Attempting to clean up.");
			includeDirectories = CleanIncludeDirectories(project, pathResolver, includeDirectories);

			Logging.Logging.LogInfo("Found " + includeDirectories.Count.ToString() + " distinct system include directories.");
			return includeDirectories;
		}

		static private List<string> GetIncludeDirectories(IVCCLCompilerToolWrapper compilerTool)
		{
			List<string> includeDirectories = new List<string>();
			if (compilerTool != null && compilerTool.isValid())
			{
				includeDirectories.AddRange(compilerTool.GetAdditionalIncludeDirectories());
			}
			else
			{
				Logging.Logging.LogWarning("No CL Compiler Tool provided");
			}
			return includeDirectories;
		}

		static private List<string> GetIncludeDirectories(IVCNMakeToolWrapper nmakeTool)
		{
			List<string> includeDirectories = new List<string>();
			if (nmakeTool != null && nmakeTool.isValid())
			{
				includeDirectories.AddRange(nmakeTool.GetIncludeSearchPaths());
			}
			else
			{
				Logging.Logging.LogWarning("No NMake Tool provided");
			}
			return includeDirectories;
		}

		static private List<string> GetIncludeDirectories(IVCResourceCompilerToolWrapper compilerTool)
		{
			List<string> includeDirectories = new List<string>();
			if (compilerTool != null && compilerTool.isValid())
			{
				includeDirectories.AddRange(compilerTool.GetAdditionalIncludeDirectories());
			}
			else
			{
				Logging.Logging.LogWarning("No Resource Compiler Tool provided");
			}
			return includeDirectories;
		}

		static public List<string> GetPreprocessorDefinitions(IVCProjectWrapper project, IVCConfigurationWrapper vcProjectConfiguration)
		{
			Logging.Logging.LogInfo("Attempting to retrieve Preprocessor Definitions for project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.GetName()) + "'");

			List<string> preprocessorDefinitions = new List<string>();

			if (vcProjectConfiguration != null && vcProjectConfiguration.isValid())
			{
				if (vcProjectConfiguration.GetCLCompilerTool() != null && vcProjectConfiguration.GetCLCompilerTool().isValid())
				{
					preprocessorDefinitions = GetPreprocessorDefinitions(vcProjectConfiguration.GetCLCompilerTool());
				}
				else if (vcProjectConfiguration.GetNMakeTool() != null && vcProjectConfiguration.GetNMakeTool().isValid())
				{
					preprocessorDefinitions = GetPreprocessorDefinitions(vcProjectConfiguration.GetNMakeTool());
				}
			}
			else
			{
				Logging.Logging.LogWarning("Could not retrieve project configuration.");
			}

			Logging.Logging.LogInfo("Found " + preprocessorDefinitions.Count.ToString() + " distinct preprocessor definitions.");

			return preprocessorDefinitions;
		}

		static private List<string> GetPreprocessorDefinitions(IVCCLCompilerToolWrapper compilerTool)
		{
			List<string> preprocessorDefinitions = new List<string>();
			if (compilerTool != null && compilerTool.isValid())
			{
				preprocessorDefinitions = compilerTool.GetPreprocessorDefinitions()
					.Select(x => x.Replace("\\\"", "\""))
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Distinct()
					.ToList();
			}
			else
			{
				Logging.Logging.LogWarning("No CL Compiler Tool provided");
			}
			return preprocessorDefinitions;
		}

		static private List<string> GetPreprocessorDefinitions(IVCNMakeToolWrapper nmakeTool)
		{
			List<string> preprocessorDefinitions = new List<string>();
			if (nmakeTool != null && nmakeTool.isValid())
			{
				preprocessorDefinitions = nmakeTool.GetPreprocessorDefinitions()
					.Select(x => x.Replace("\\\"", "\""))
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Distinct()
					.ToList();
			}
			else
			{
				Logging.Logging.LogWarning("No NMake Tool provided");
			}
			return preprocessorDefinitions;
		}

		static private List<string> GetPreprocessorDefinitions(IVCResourceCompilerToolWrapper compilerTool)
		{
			List<string> preprocessorDefinitions = new List<string>();
			if (compilerTool != null && compilerTool.isValid())
			{
				preprocessorDefinitions = compilerTool.GetPreprocessorDefinitions()
					.Select(x => x.Replace("\\\"", "\""))
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Distinct()
					.ToList();
			}
			else
			{
				Logging.Logging.LogWarning("No Resource Compiler Tool provided");
			}
			return preprocessorDefinitions;
		}

		static public List<string> GetForcedIncludeFiles(IVCProjectWrapper project, IVCConfigurationWrapper vcProjectConfiguration, IPathResolver pathResolver)
		{
			Logging.Logging.LogInfo("Attempting to retrieve Forced Include Files for project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.GetName()) + "'");

			List<string> forcedIncludeFiles = new List<string>();

			if (vcProjectConfiguration != null && vcProjectConfiguration.isValid())
			{
				if (vcProjectConfiguration.GetCLCompilerTool() != null && vcProjectConfiguration.GetCLCompilerTool().isValid())
				{
					forcedIncludeFiles = vcProjectConfiguration.GetCLCompilerTool().GetForcedIncludeFiles()
						.Select(x => x.Replace("\\", "/")) // backslashes would cause some string-escaping hassles...
						.Where(x => !string.IsNullOrWhiteSpace(x))
						.Distinct()
						.ToList();
				}
				else if(vcProjectConfiguration.GetNMakeTool() != null && vcProjectConfiguration.GetNMakeTool().isValid())
				{
					forcedIncludeFiles = vcProjectConfiguration.GetNMakeTool().GetForcedIncludes()
						.Select(x => x.Replace("\\", "/")) // backslashes would cause some string-escaping hassles...
						.Where(x => !string.IsNullOrWhiteSpace(x))
						.Distinct()
						.ToList();
				}
			}

			Logging.Logging.LogInfo("Found " + forcedIncludeFiles.Count.ToString() + " distinct forced include files.");

			return forcedIncludeFiles;
		}

		// AFAIK there is no way to programmatically get the c++ standard version supported by any given VS version
		// instead I'm refearing to the VS docu for that information
		// https://msdn.microsoft.com/en-us/library/hh567368.aspx
		static public string GetCppStandardForProject(IVCProjectWrapper project)
		{
			string result = "";

			string toolset = project.GetWrappedVersion();
			string justNumbers = new String(toolset.Where(Char.IsDigit).ToArray());
			int versionNumber = int.Parse(justNumbers);

			if (versionNumber < 120) // version 11 (2012)
			{
				result = "c++11";
			}
			else if (versionNumber < 130) // version 12 (2013)
			{
				result = "c++14";
			}
			else if (versionNumber < 150) // version 14 (2015)
			{
				result = "c++14";
			}
			else if (versionNumber < 160) // version 15 (2017)
			{
				result = "c++14";
			}

			return result;
		}


		// returns a valid Guid if the project was reloaded or an empty Guid if the project did not need to be reloaded
		static public Guid ReloadProject(Project project)
		{
			Logging.Logging.LogInfo("Attempting to reload project");

			try
			{
				if (project != null && project.Kind == EnvDTE.Constants.vsProjectKindUnmodeled)
				{
					DTE dte = project.DTE;

					ServiceProvider sp = new ServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
					IVsSolution vsSolution = sp.GetService(typeof(IVsSolution)) as IVsSolution;

					IVsHierarchy hierarchy;

					string solutionDirectory = "";
					string solutionFile = "";
					string userOptions = "";
					vsSolution.GetSolutionInfo(out solutionDirectory, out solutionFile, out userOptions);

					vsSolution.GetProjectOfUniqueName(solutionDirectory + project.UniqueName, out hierarchy);

					if (hierarchy != null)
					{
						Guid projectGuid;

						hierarchy.GetGuidProperty(
									VSConstants.VSITEMID_ROOT,
									(int)__VSHPROPID.VSHPROPID_ProjectIDGuid,
									out projectGuid);

						if (projectGuid != null)
						{
							Logging.Logging.LogInfo("Project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "' with GUID {" + projectGuid.ToString() + "} loaded.");
							(vsSolution as IVsSolution4).ReloadProject(projectGuid);
							return projectGuid;
						}
						else
						{
							Logging.Logging.LogError("Failed to load project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "'");
						}
					}
					else
					{
						Logging.Logging.LogError("Failed to retrieve IVsHierarchy. Can't load project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "'");
					}
				}
				else
				{
					if (project == null)
					{
						Logging.Logging.LogWarning("Project is null");
					}
					else
					{
						Logging.Logging.LogInfo("Project '" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "' is already loaded");
					}
				}
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);

				return Guid.Empty;
			}

			return Guid.Empty;
		}

		static public void UnloadProject(Guid guid, DTE dte)
		{
			Logging.Logging.LogInfo("Attempting to unload project with GUID {" + guid.ToString() + "}");

			if (dte == null)
			{
				return;
			}

			try
			{
				ServiceProvider sp = new ServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
				IVsSolution vsSolution = sp.GetService(typeof(SVsSolution)) as IVsSolution;

				(vsSolution as IVsSolution4).UnloadProject(guid, (uint)_VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser);

				Logging.Logging.LogInfo("Done unloading project with GUID {" + guid.ToString() + "}");
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
		}
	}
}
