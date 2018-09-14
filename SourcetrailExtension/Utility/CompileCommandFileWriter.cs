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

using CoatiSoftware.SourcetrailExtension.SolutionParser;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	class CompileCommandFileWriter
	{
		private bool _isFirstCommand;
		private QueuedFileWriter _fileWriter = null;

		public CompileCommandFileWriter(string fileName, string targetDirectory)
		{
			_isFirstCommand = true;
			_fileWriter = new QueuedFileWriter(fileName, targetDirectory);
		}

		public void PushCommand(CompileCommand command)
		{
			string serializedCommand = "";

			if (!_isFirstCommand)
			{
				serializedCommand += ",\n";
			}
			else
			{
				_isFirstCommand = false;
			}

			foreach (string line in command.SerializeToJson().Split('\n'))
			{
				serializedCommand += "  " + line + "\n";
			}

			serializedCommand = serializedCommand.TrimEnd('\n');

			_fileWriter.PushMessage(serializedCommand);
		}

		public void StartWorking()
		{
			_fileWriter.StartWorking();
		}

		public void StopWorking()
		{
			_fileWriter.StopWorking();
		}
	}
}
