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
using VCProjectEngineWrapper;

namespace CoatiSoftware.SourcetrailExtension.IntegrationTests.Helpers
{
	class TestPathResolver : IPathResolver
	{
		public override string GetCompilationDatabaseFilePath()
		{
			return "<CompilationDatabaseFilePath>";
		}

		public override string GetAsAbsoluteCanonicalPath(string path, IVCProjectWrapper project)
		{
			if (path.Length > 0 && !path.StartsWith("<") && !System.IO.Path.IsPathRooted(path))
			{
				path = "<ProjectBaseDirectory>/" + path;
			}

			return path;
		}

		protected override string ResolveVsMacro(string potentialMacro, IVCConfigurationWrapper vcProjectConfig)
		{
			return "<Macro " + potentialMacro + ">";
		}
	}
}
