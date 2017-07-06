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

using System;
using System.Collections.Generic;

namespace CoatiSoftware.SourcetrailExtension.Logging
{
	public class FileLogger : ILogger
	{
		private static string _directory = "";
		private static string _fileNamePrefix = "Log_SourcetrailExtension_";
		private static string _fileNameSufix = ".txt";

		private string _fileName = "";

		private Queue<string> _messageBacklog = new Queue<string>(); // stores messages if the log file was in use at original logging time

		public FileLogger()
		{
			DateTime time = DateTime.Now;

			string dateString = "";
			dateString += time.Year.ToString() + "-" + time.Month.ToString() + "-" + time.Day.ToString() + "_";
			dateString += time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();

			_fileName = _fileNamePrefix + dateString + _fileNameSufix;

			_directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			_directory += "\\Coati Software\\Plugins\\VS\\";

			if(System.IO.Directory.Exists(_directory) == false)
			{
				System.IO.Directory.CreateDirectory(_directory);
			}
		}

		public void LogMessage(LogMessage message)
		{
			System.IO.StreamWriter writer = null;

			try
			{
				writer = System.IO.File.AppendText(_directory + _fileName);

				// write backlog to file first
				while(_messageBacklog.Count > 0)
				{
					string bm = _messageBacklog.Dequeue();
					writer.WriteLine(bm);
				}

				writer.WriteLine(message.ToString());
				writer.Close();
			}
			catch(Exception e)
			{
				// well...file is still in use
				_messageBacklog.Enqueue(message.ToString());
			}
			finally
			{
				if(writer != null)
				{
					writer.Close();
				}
			}
		}
	}
}
