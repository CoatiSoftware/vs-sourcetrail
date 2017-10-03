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

using CoatiSoftware.SourcetrailExtension.IntegrationTests.Helpers;
using CoatiSoftware.SourcetrailExtension.SolutionParser;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;

namespace CoatiSoftware.SourcetrailExtension.IntegrationTests
{
	[TestClass]
	public class CreateCdbTests
	{
		private bool _updateExpectedOutput = false;

		[TestMethod]
		[HostType("VS IDE")]
		public void TestSourcetrailExtensionPackageGetsLoaded()
		{
			UIThreadInvoker.Invoke(new Action(() =>
			{
				// Load the package into the shell.
				Assert.IsNotNull(VsIdeTestHostContext.ServiceProvider);
				IVsShell shellService = (IVsShell)VsIdeTestHostContext.ServiceProvider.GetService(typeof(SVsShell));
				Guid packageGuid = new Guid(GuidList.guidSourcetrailExtensionPkgString);

				IVsPackage package;
				shellService.IsPackageLoaded(ref packageGuid, out package);
				if (package == null)
				{
					shellService.LoadPackage(ref packageGuid, out package);
				}

				Assert.IsNotNull(package);
				Assert.IsTrue(package is SourcetrailExtensionPackage);
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForAllFilesInSameFolder()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/all_in_same_folder/test.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForCinderSolution()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/cinder/cinder.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForNMakeProjectWithForcedInclude()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/nmake_project_with_forced_include/nmake_project_with_forced_include.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForNMakeProjectWithIncludeSearchPath()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/nmake_project_with_include_search_path/nmake_project_with_include_search_path.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForNMakeProjectWithPreprocessorDefinition()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/nmake_project_with_preprocessor_definition/nmake_project_with_preprocessor_definition.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForProjectWithCompileAsDefaultOption()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/project_with_compile_as_default_option/project_with_compile_as_default_option.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForProjectWithForcedInclude()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/project_with_forced_include/project_with_forced_include.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForProjectWithPropertySheetUsage()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/project_with_property_sheet_usage/project_with_property_sheet_usage.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForProjectWithDifferentItemTypes()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/project_with_different_item_types/project_with_different_item_types.sln");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void TestCompilationDatabaseCreationForSolutionWithCustomNamedProjectConfiguration()
		{
			UIThreadInvoker.Initialize();
			UIThreadInvoker.Invoke(new Action(() =>
			{
				TestCompilationDatabaseForSolution("../../../SourcetrailExtensionTests/data/solution_with_custom_named_project_configuration/solution_with_custom_named_project_configuration.sln");
			}));
		}

		private void TestCompilationDatabaseForSolution(string solutionPath)
		{
			Assert.IsTrue(File.Exists(solutionPath), "solution path does not exist");

			Console.WriteLine("opening solution: " + solutionPath);
			Helpers.TestUtility.OpenSolution(solutionPath);

			Console.WriteLine("creating compilation database");

			CompilationDatabase output = null;
			try
			{
				output = CreateCompilationDatabaseForCurrentSolution();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.Message);
				Console.WriteLine("Stack Trace: " + e.StackTrace);
				Assert.Fail("Caught an exception while creating compilation database.");
			}

			Assert.IsNotNull(output);

			string cdbPath = Path.ChangeExtension(solutionPath, "json");
			if (_updateExpectedOutput)
			{
				output.SortAlphabetically();
				Console.WriteLine("writing compilation database to file: " + cdbPath);
				File.WriteAllText(cdbPath, output.SerializeToJson());
				Assert.IsTrue(File.Exists(cdbPath));
			}
			else
			{
				Console.WriteLine("reading compilation database from file: " + cdbPath);
				CompilationDatabase expectedOutput = CompilationDatabase.LoadFromFile(cdbPath);
				Assert.IsNotNull(expectedOutput);

				Console.WriteLine("comparing generated compilation database to expected output");
				Assert.IsTrue(output == expectedOutput, "The created compilation database differs from the expected output");
			}

			Console.WriteLine("closing solution");
			Helpers.TestUtility.CloseCurrentSolution();
		}

		private static CompilationDatabase CreateCompilationDatabaseForCurrentSolution()
		{
			DTE dte = (DTE)VsIdeTestHostContext.ServiceProvider.GetService(typeof(DTE));
			Assert.IsNotNull(dte);

			CompilationDatabase cdb = new CompilationDatabase();
			foreach (Project project in dte.Solution.Projects)
			{
				List<string> configurationNames = Utility.SolutionUtility.GetConfigurationNames(dte);
				Assert.IsTrue(configurationNames.Count > 0, "No target configurations found in loaded solution.");

				List<string> platformNames = Utility.SolutionUtility.GetPlatformNames(dte);
				Assert.IsTrue(platformNames.Count > 0, "No target platforms found in loaded solution.");

				SolutionParser.SolutionParser solutionParser = new SolutionParser.SolutionParser(new TestPathResolver());
				solutionParser.CreateCompileCommands(
					project, configurationNames[0], platformNames[0], "c11", null,
					(CompileCommand command, bool lastFile) => {
						cdb.AddCompileCommand(command);
					}
				);
			}

			return cdb;
		}
	}
}
