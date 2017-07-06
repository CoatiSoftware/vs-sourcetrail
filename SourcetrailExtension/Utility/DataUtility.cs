﻿/*
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

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	class DataUtility
	{
		static private string _standardFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Coati Software\\Plugins\\VS\\";
		static private string _standardFileName = "cdbs.sourcetraildata";

		static private DataUtility _instance = null;

		static private bool _valid = true; // stores if a file system operation failed, indicating that there is something wrong

		static public string GetStandardFolderDirectory()
		{
			return _standardFolder;
		}

		static bool Valid
		{
			get { return _valid; }
		}

		static public DataUtility GetInstance()
		{
			if(_instance == null)
			{
				_instance = new DataUtility();
			}

			return _instance;
		}

		private DataUtility()
		{
			CreateStandardFolderIfNotExists();
			CreateStandardFileIfNotExists();
		}

		public void AppendData(string data)
		{
			try
			{
				using (System.IO.StreamWriter file = System.IO.File.AppendText(_standardFolder + _standardFileName))
				{
					file.WriteLine(data);
				}
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Failed to write data to file '" + _standardFolder + _standardFileName + "':" + e.Message);
				_valid = false;
			}
		}

		public string GetData()
		{
			string result = "";

			try
			{
				string data = "";
				if (System.IO.File.Exists(_standardFolder + _standardFileName))
				{
					using (System.IO.StreamReader file = new System.IO.StreamReader(_standardFolder + _standardFileName))
					{
						string line = "";

						while ((line = file.ReadLine()) != null)
						{
							data += line;
						}
					}
				}
				result = data;
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Failed to read data from file '" + _standardFolder + _standardFileName + "':" + e.Message);
				_valid = false;
			}

			return result;
		}

		public void ClearData()
		{
			try
			{
				System.IO.File.WriteAllText(_standardFolder + _standardFileName, "");
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Failed to clear data: " + e.Message);
			}
		}

		private void CreateStandardFolderIfNotExists()
		{
			try
			{
				if (System.IO.Directory.Exists(_standardFolder) == false)
				{
					System.IO.Directory.CreateDirectory(_standardFolder);
				}
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Failed to create folder: " + e.Message);
				_valid = false;
			}
		}

		private void CreateStandardFileIfNotExists()
		{
			try
			{
				if (System.IO.File.Exists(_standardFolder + _standardFileName) == false)
				{
					System.IO.File.Create(_standardFolder + _standardFileName);
				}
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Failed to create file: " + e.Message);
				_valid = false;
			}
		}
	}
}
