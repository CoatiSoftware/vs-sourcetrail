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

using System.Runtime.CompilerServices;

namespace CoatiSoftware.SourcetrailExtension.Logging
{
	public class Logging
	{
		public static void LogInfo(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
		{
			int idx = file.LastIndexOf('\\');
			if(idx > -1)
			{
				file = file.Substring(idx + 1);
			}

			LogManager.GetInstance().LogInfo(message, file, member, line);
		}

		public static void LogWarning(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
		{
			int idx = file.LastIndexOf('\\');
			if (idx > -1)
			{
				file = file.Substring(idx + 1);
			}

			LogManager.GetInstance().LogWarning(message, file, member, line);
		}

		public static void LogError(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
		{
			int idx = file.LastIndexOf('\\');
			if (idx > -1)
			{
				file = file.Substring(idx + 1);
			}

			LogManager.GetInstance().LogError(message, file, member, line);
		}
	}
}
