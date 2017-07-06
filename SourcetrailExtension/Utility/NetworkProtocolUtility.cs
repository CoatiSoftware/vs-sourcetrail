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

using System.Collections.Generic;
using System.Linq;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	class NetworkProtocolUtility
	{
		private static string s_divider = ">>";
		private static string s_setActiveTokenPrefix = "setActiveToken";
		private static string s_moveCursorPrefix = "moveCursor";
		private static string s_endOfMessageToken = "<EOM>";
		
		private static string s_createCdbProjectPrefix = "createCDBProject";
		private static string s_ideId = "vs";

		private static string s_createCdbPrefix = "createCDB";

		private static string s_pingPrefix = "ping";

		public enum MESSAGE_TYPE
		{
			UNKNOWN = 0,
			MOVE_CURSOR,
			CREATE_CDB,
			PING
		}

		public class CursorPosition
		{
			private string _filePath = "";
			private int _lineNumber = 0;
			private int _columnNumber = 0;
			private bool _valid = false;

			public string FilePath
			{
				get { return _filePath; }
				set { _filePath = value; }
			}

			public int LineNumber
			{
				get { return _lineNumber; }
				set { _lineNumber = value; }
			}

			public int ColumnNumber
			{
				get { return _columnNumber; }
				set { _columnNumber = value; }
			}

			public bool Valid
			{
				get { return _valid; }
				set { _valid = value; }
			}
		}

		public class Ping
		{
			private string _id = "";
			private bool _valid = false;
		
			public string Id
			{
				get { return _id; }
				set { _id = value; }
			}

			public bool Valid
			{
				get { return _valid; }
				set { _valid = value; }
			}
		}

		public static string CreateActivateTokenMessage(string filePath, int lineNumber, int columnNumber)
		{
			string message = s_setActiveTokenPrefix;

			message += s_divider;

			message += filePath;

			message += s_divider;

			message += lineNumber.ToString();

			message += s_divider;

			message += columnNumber.ToString();

			message += s_endOfMessageToken;

			return message;
		}

		public static string CreateCreateProjectMessage(string cdbPath, List<string> headerPaths)
		{
			string message = s_createCdbProjectPrefix;

			message += s_divider;

			message += cdbPath;

			message += s_divider;

			foreach(string path in headerPaths)
			{
				message += path;

				message += s_divider;
			}

			message += s_ideId;

			message += s_endOfMessageToken;

			return message;
		}

		public static string CreatePingMessage()
		{
			string message = s_pingPrefix;

			message += s_divider;

			message += s_ideId;

			message += s_endOfMessageToken;

			return message;
		}

		public static MESSAGE_TYPE GetMessageType(string message)
		{
			List<string> tokens = GetMessageTokens(message);

			if (tokens.Count > 0)
			{
				if (tokens[0] == s_createCdbPrefix)
				{
					return MESSAGE_TYPE.CREATE_CDB;
				}
				else if (tokens[0] == s_moveCursorPrefix)
				{
					return MESSAGE_TYPE.MOVE_CURSOR;
				}
				else if (tokens[0] == s_pingPrefix)
				{
					return MESSAGE_TYPE.PING;
				}
				else
				{
					return MESSAGE_TYPE.UNKNOWN;
				}
			}

			return MESSAGE_TYPE.UNKNOWN;
		}

		public static CursorPosition ParseSetCursorMessage(string message)
		{
			CursorPosition result = new CursorPosition();

			List<string> tokens = GetMessageTokens(message);

			if(tokens.Count != 4)
			{
				Logging.Logging.LogError("Invalid message: " + message);
				Logging.Logging.LogError("Invalid token count for 'move cursor' message. Expected 4, but got " + tokens.Count.ToString());
				return result;
			}

			if(tokens[0] != s_moveCursorPrefix)
			{
				Logging.Logging.LogError("Invalid message: " + message);
				Logging.Logging.LogError("Invalid message type. Expected '" + s_moveCursorPrefix + "' but got '" + tokens[0] + "'");
				return result;
			}

			result.FilePath = tokens[1];

			int lineNumber = 0;
			if(int.TryParse(tokens[2], out lineNumber))
			{
				result.LineNumber = lineNumber;
			}
			else
			{
				Logging.Logging.LogError("Invalid message: " + message);
				Logging.Logging.LogError("Failed to parse line number");
				return result;
			}

			int columnNumber = 0;
			if(int.TryParse(tokens[3], out columnNumber))
			{
				result.ColumnNumber = columnNumber;
			}
			else
			{
				Logging.Logging.LogError("Invalid message: " + message);
				Logging.Logging.LogError("Failed to parse column number");
				return result;
			}

			result.Valid = true;
			return result;
		}

		public static Ping ParsePingMessage(string message)
		{
			Ping result = new Ping();

			List<string> tokens = GetMessageTokens(message);

			if(tokens.Count != 2)
			{
				Logging.Logging.LogError("Invalid message: " + message);
				Logging.Logging.LogError("Invalid token count for 'ping' message. Expected 2, but got " + tokens.Count.ToString());
				return result;
			}

			if (tokens[0] != s_pingPrefix)
			{
				Logging.Logging.LogError("Invalid message: " + message);
				Logging.Logging.LogError("Invalid message type. Expected '" + s_pingPrefix + "' but got '" + tokens[0] + "'");
				return result;
			}

			result.Id = tokens[1];
			result.Valid = true;

			return result;
		}

		private static List<string> GetMessageTokens(string message)
		{
			List<string> tokens = new List<string>();

			if(RemoveEOMString(ref message))
			{
				tokens = message.Split(s_divider.ToCharArray()).ToList();

				// removing empty strings
				List<string> cleanTokens = new List<string>();
				foreach(string token in tokens)
				{
					if(token.Length > 0)
					{
						cleanTokens.Add(token);
					}
				}

				tokens = cleanTokens;
			}
			else
			{
				Logging.Logging.LogError("");
			}

			return tokens;
		}

		// returns false if no EOM string was found
		private static bool RemoveEOMString(ref string message)
		{
			if (message.IndexOf(s_endOfMessageToken) > -1)
			{
				message = message.Substring(0, message.IndexOf(s_endOfMessageToken));

				return true;
			}

			return false;
		}
	}
}
