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

using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoatiSoftware.SourcetrailExtension.SolutionParser
{
	public class CompilationDatabase
	{
		private List<CompileCommand> _compileCommands = new List<CompileCommand>();

		private bool TryLoadData(string filePath)
		{
			if (filePath.Length > 0)
			{
				if (System.IO.File.Exists(filePath))
				{
					string data = "";

					using (System.IO.StreamReader file = new System.IO.StreamReader(filePath))
					{
						string line = "";

						while ((line = file.ReadLine()) != null)
						{
							data += line;
						}
					}

					DeserializeFromJson(data);

					return true;
				}
				else
				{
					Logging.Logging.LogError("The cdb file at \"" + filePath + "\" does not exist.");
				}
			}
			else
			{
				Logging.Logging.LogWarning("Can't load cdb data. Filepath has not been set.");
			}

			return false;
		}



		public static CompilationDatabase LoadFromFile(string filePath)
		{
			CompilationDatabase cdb = new CompilationDatabase();
			bool success = cdb.TryLoadData(filePath);
			return cdb;
		}


		public string SerializeToJson()
		{
			return JsonConvert.SerializeObject(_compileCommands, Newtonsoft.Json.Formatting.Indented);
		}

		public void DeserializeFromJson(string serialized)
		{
			_compileCommands.Clear();

			if (serialized.Length > 0)
			{
				_compileCommands = JsonConvert.DeserializeObject<List<CompileCommand>>(serialized);
			}
		}


		public int CompileCommandCount
		{
			get { return _compileCommands.Count; }
		}

		public static bool operator ==(CompilationDatabase a, CompilationDatabase b)
		{
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			if (a.CompileCommandCount != b.CompileCommandCount)
			{
				return false;
			}

			foreach (CompileCommand aCommand in a._compileCommands)
			{
				CompileCommand bCommand = b._compileCommands.Find(x => x == aCommand);
				if (bCommand == null || aCommand.File != bCommand.File)
				{
					return false;
				}
			}

			return true;
		}

		public static bool operator !=(CompilationDatabase a, CompilationDatabase b)
		{
			return !(a == b);
		}

		public void AddCompileCommand(CompileCommand commandObject)
		{
			_compileCommands.Add(commandObject);
		}

		public void SortAlphabetically()
		{
			_compileCommands.Sort((c1, c2) => c1.File.CompareTo(c2.File));
		}

		public void Clear()
		{
			_compileCommands.Clear();
		}

		public override bool Equals(object obj)
		{
			var database = obj as CompilationDatabase;
			if (ReferenceEquals(this, database))
			{
				return true;
			}

			return database != null &&
				   EqualityComparer<List<CompileCommand>>.Default.Equals(_compileCommands, database._compileCommands) &&
				   CompileCommandCount == database.CompileCommandCount;
		}

		public override int GetHashCode()
		{
			var hashCode = 548601099;
			hashCode = hashCode * -1521134295 + EqualityComparer<List<CompileCommand>>.Default.GetHashCode(_compileCommands);
			hashCode = hashCode * -1521134295 + CompileCommandCount.GetHashCode();
			return hashCode;
		}
	}
}
