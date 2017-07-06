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

using VCProjectEngineWrapper;
using System;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	public abstract class IPathResolver
	{
		public string ResolveVsMacroInPath(string path, IVCConfigurationWrapper vcProjectConfig)
		{
			string result = path;

			try
			{
				Tuple<int, int> potentialMacroPosition = Utility.StringUtility.FindFirstRange(path, "$(", ")");

				if (potentialMacroPosition != null)
				{
					string potentialMacro = path.Substring(potentialMacroPosition.Item1, potentialMacroPosition.Item2 - potentialMacroPosition.Item1 + 1);

					string resolvedMacro = ResolveVsMacro(potentialMacro, vcProjectConfig);

					result = path.Substring(0, potentialMacroPosition.Item1) + resolvedMacro + path.Substring(potentialMacroPosition.Item2 + 1);
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}

			return result;
		}

		public string GetAsAbsoluteCanonicalPath(string path, IVCProjectWrapper project)
		{
			if (path.Length > 0 && !System.IO.Path.IsPathRooted(path))
			{
				path = DoGetAsAbsoluteCanonicalPath(path, project);
			}
			return path;
		}

		public abstract string GetCompilationDatabaseFilePath();

		protected abstract string DoGetAsAbsoluteCanonicalPath(string path, IVCProjectWrapper project);

		protected abstract string ResolveVsMacro(string potentialMacro, IVCConfigurationWrapper vcProjectConfig);
	}
}
