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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using System;

namespace CoatiSoftware.SourcetrailExtension.IntegrationTests.Helpers
{
	public static class TestUtility
	{
		public static void OpenSolution(string solutionFilePath)
		{
			IVsSolution solutionService = (IVsSolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IVsSolution));
			int ret = solutionService.OpenSolutionFile((uint)__VSSLNOPENOPTIONS.SLNOPENOPT_DontConvertSLN, solutionFilePath);

			DTE dte = (DTE)VsIdeTestHostContext.ServiceProvider.GetService(typeof(DTE));
			Console.WriteLine("opened solution contains " + dte.Solution.Projects.Count.ToString() + " projects");

			Assert.AreEqual(VSConstants.S_OK, ret);
		}

		public static void CloseCurrentSolution()
		{
			IVsSolution solutionService = (IVsSolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IVsSolution));
			int ret = solutionService.CloseSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave, null, 0);
			Assert.AreEqual(VSConstants.S_OK, ret);
		}
	}
}
