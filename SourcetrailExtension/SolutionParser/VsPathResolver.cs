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

using CoatiSoftware.SourcetrailExtension.Utility;
using System;
using System.IO;
using VCProjectEngineWrapper;

namespace CoatiSoftware.SourcetrailExtension.SolutionParser
{
	public class VsPathResolver : IPathResolver
	{
		private string _compilationDatabaseFilePath = "";

		public VsPathResolver(string compilationDatabaseFilePath)
		{
			_compilationDatabaseFilePath = compilationDatabaseFilePath.Replace('\\', '/');
		}

		public override string GetCompilationDatabaseFilePath()
		{
			return _compilationDatabaseFilePath;
		}

		public override string GetAsAbsoluteCanonicalPath(string path, IVCProjectWrapper project)
		{
			if (!string.IsNullOrEmpty(path))
			{
				if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
				{
					Logging.Logging.LogError("Path \"" + path + "\" contains invalid characters.");
				}

				if (!Path.IsPathRooted(path))
				{
					path = project.GetProjectDirectory() + path;
					path = new Uri(path).LocalPath;
				}
			}
			return path;
		}

		protected override string ResolveVsMacro(string potentialMacro, IVCConfigurationWrapper vcProjectConfig)
		{
			return vcProjectConfig.EvaluateMacro(potentialMacro);
		}
	}
}
