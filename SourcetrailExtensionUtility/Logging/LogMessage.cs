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

using System;

namespace CoatiSoftware.SourcetrailExtension.Logging
{
	public class LogMessage
	{
		public enum LogMessageType
		{
			UNKNOWN = 0,
			INFO,
			WARNING,
			ERROR
		}

		private string _message = "";
		private DateTime _time = new DateTime();
		private LogMessageType _messageType = LogMessageType.UNKNOWN;

		private string _sourceFile = "";
		private string _callingFunction = "";
		private int _lineNumber = -1;

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public DateTime Time
		{
			get { return _time; }
			set { _time = value; }
		}

		public LogMessageType MessageType
		{
			get { return _messageType; }
			set { _messageType = value; }
		}

		public string SourceFile
		{
			get { return _sourceFile; }
			set { _sourceFile = value; }
		}

		public string CallingFunction
		{
			get { return _callingFunction; }
			set { _callingFunction = value; }
		}

		public int LineNumber
		{
			get { return _lineNumber; }
			set { _lineNumber = value; }
		}

		public override string ToString()
		{
			string result = "";

			result += _time.Hour.ToString() + ":" + _time.Minute.ToString() + ":"+ _time.Second.ToString();

			result += "\t";

			result += _messageType.ToString();

			result += "\t";

			result += _sourceFile + ":" + _lineNumber.ToString();

			result += " (";

			result += _callingFunction;

			result += ")";

			result += "\t\t";

			result += _message;

			return result;
		}
	}
}
