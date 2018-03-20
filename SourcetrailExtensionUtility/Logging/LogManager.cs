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
using System.Collections.Generic;

namespace CoatiSoftware.SourcetrailExtension.Logging
{
	public class LogManager
	{
		private static LogManager _instance = null;

		private List<ILogger> _loggers = new List<ILogger>();

		private bool _loggingEnabled = false;

		public List<ILogger> Loggers
		{
			get { return _loggers; }
			set { _loggers = value; }
		}

		public bool LoggingEnabled
		{
			get { return _loggingEnabled; }
			set { _loggingEnabled = value; }
		}

		private LogManager()
		{
			
		}

		public static LogManager GetInstance()
		{
			if(_instance == null)
			{
				_instance = new LogManager();
			}

			return _instance;
		}

		public void LogInfo(string message, string sourceFile, string callingFunction, int lineNumber)
		{
			LogMessage logMessage = new LogMessage();
			logMessage.Message = message;
			logMessage.MessageType = LogMessage.LogMessageType.INFO;
			logMessage.Time = DateTime.Now;
			logMessage.SourceFile = sourceFile;
			logMessage.CallingFunction = callingFunction;
			logMessage.LineNumber = lineNumber;

			Log(logMessage);
		}

		public void LogWarning(string message, string sourceFile, string callingFunction, int lineNumber)
		{
			LogMessage logMessage = new LogMessage();
			logMessage.Message = message;
			logMessage.MessageType = LogMessage.LogMessageType.WARNING;
			logMessage.Time = DateTime.Now;
			logMessage.SourceFile = sourceFile;
			logMessage.CallingFunction = callingFunction;
			logMessage.LineNumber = lineNumber;

			Log(logMessage);
		}

		public void LogError(string message, string sourceFile, string callingFunction, int lineNumber)
		{
			LogMessage logMessage = new LogMessage();
			logMessage.Message = message;
			logMessage.MessageType = LogMessage.LogMessageType.ERROR;
			logMessage.Time = DateTime.Now;
			logMessage.SourceFile = sourceFile;
			logMessage.CallingFunction = callingFunction;
			logMessage.LineNumber = lineNumber;

			Log(logMessage);
		}

		private void Log(LogMessage message)
		{
			if(_loggingEnabled == true)
			{
				foreach (ILogger logger in _loggers)
				{
					logger.LogMessage(message);
				}
			}
		}
	}
}
