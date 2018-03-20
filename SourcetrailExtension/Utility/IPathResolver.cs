/*
 * Copyright 2018 Coati Software KG
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

using VCProjectEngineWrapper;
using System;
using System.Collections.Generic;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	public abstract class IPathResolver
	{
		public List<string> ResolveVsMacroInPath(string path, IVCConfigurationWrapper vcProjectConfig)
		{
			string resolvedPaths = path;

			try
			{
				Tuple<int, int> potentialMacroPosition = Utility.StringUtility.FindFirstRange(path, "$(", ")");
				if (potentialMacroPosition != null)
				{
					string potentialMacro = path.Substring(potentialMacroPosition.Item1, potentialMacroPosition.Item2 - potentialMacroPosition.Item1 + 1);
					string resolvedMacro = ResolveVsMacro(potentialMacro, vcProjectConfig);
					resolvedPaths = path.Substring(0, potentialMacroPosition.Item1) + resolvedMacro + path.Substring(potentialMacroPosition.Item2 + 1);
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
			try
			{
				Tuple<int, int> potentialMacroPosition = Utility.StringUtility.FindFirstRange(path, "%(", ")");
				if (potentialMacroPosition != null)
				{
					string potentialMacro = path.Substring(potentialMacroPosition.Item1, potentialMacroPosition.Item2 - potentialMacroPosition.Item1 + 1);
					string resolvedMacro = ResolveVsMacro(potentialMacro, vcProjectConfig);
					resolvedPaths = path.Substring(0, potentialMacroPosition.Item1) + resolvedMacro + path.Substring(potentialMacroPosition.Item2 + 1);
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}

			List<string> pathList = new List<string>();
			foreach (string resolvedPath in resolvedPaths.SplitPaths())
			{
				pathList.Add(resolvedPath);
			}

			return pathList;
		}

		public abstract string GetCompilationDatabaseFilePath();

		public abstract string GetAsAbsoluteCanonicalPath(string path, IVCProjectWrapper project);

		protected abstract string ResolveVsMacro(string potentialMacro, IVCConfigurationWrapper vcProjectConfig);
	}
}
