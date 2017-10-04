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
	public class CompileCommand
	{
		private string _directory = "";
		private string _command = "";
		private string _file = "";

		[JsonProperty(propertyName: "directory")]
		public string Directory
		{
			get { return _directory; }
			set { _directory = value; }
		}

		[JsonProperty(propertyName: "command")]
		public string Command
		{
			get { return _command; }
			set
			{
				_command = value;
				//_command = _command.Replace('"', '\'');
				//_command = _command.Replace("\"", "'");
				//_command = _command.Replace("\\\"", "'");
			}
		}

		[JsonProperty(propertyName: "file")]
		public string File
		{
			get { return _file; }
			set
			{
				_file = value;
				//_file = _file.Replace('"', '\'');
				//_file = _file.Replace('\"', '\'');
				//_file = _file.Replace("\\\"", "'");
			}
		}

		public static bool operator ==(CompileCommand a, CompileCommand b)
		{
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			if (a.File != b.File || a.Command != b.Command || a.Directory != b.Directory)
			{
				return false;
			}

			return true;
		}

		public static bool operator !=(CompileCommand a, CompileCommand b)
		{
			return !(a == b);
		}

		public string SerializeToJson()
		{
			return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
		}

		public static CompileCommand DeserializeFromJson(string serialized)
		{
			CompileCommand command = null;
			if (serialized.Length > 0)
			{
				command = JsonConvert.DeserializeObject<CompileCommand>(serialized);
			}
			return command;
		}

		public override bool Equals(object obj)
		{
			var command = obj as CompileCommand;
			if (ReferenceEquals(this, command))
			{
				return true;
			}

			return command != null &&
				   Directory == command.Directory &&
				   Command == command.Command &&
				   File == command.File;
		}

		public override int GetHashCode()
		{
			var hashCode = -1659665107;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Directory);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Command);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(File);
			return hashCode;
		}
	}
}
