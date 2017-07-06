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

using EnvDTE;
using System;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	class FileUtility
	{
		public delegate void ErrorCallback(string message);

		public static ErrorCallback _errorCallback = null;

		/**
		 * Returns true when file was found, false otherwise
		 */
		public static bool OpenSourceFile(DTE dte, string fileName)
		{
			try
			{
				dte.ItemOperations.OpenFile(fileName);
				return true;
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);

				if (_errorCallback != null)
				{
					_errorCallback("Failed to open file at " + fileName);
				}

				string message = "Failed to open file at " + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(fileName);
				Logging.Logging.LogError(message);

				return false;
			}
		}

		public static void GoToLine(DTE dte, int lineNumber, int columnNumber)
		{
			try
			{
				((EnvDTE.TextSelection)dte.ActiveDocument.Selection).MoveToLineAndOffset(lineNumber, columnNumber);
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);

				if (_errorCallback != null)
				{
					_errorCallback("Failed to set cursor to position [" + lineNumber.ToString() + "," + columnNumber.ToString() + "]");

					string message = "Failed to set cursor to position [" + lineNumber.ToString() + "," + columnNumber.ToString() + "]";
					Logging.Logging.LogError(message);
				}
			}
		}

		public static string GetActiveDocumentName(DTE dte)
		{
			return dte.ActiveDocument.Name;
		}

		public static string GetActiveDocumentPath(DTE dte)
		{
			return dte.ActiveDocument.Path;
		}

		public static int GetActiveLineNumber(DTE dte)
		{
			return ((EnvDTE.TextSelection)dte.ActiveDocument.Selection).ActivePoint.Line;
		}

		public static int GetActiveColumnNumber(DTE dte)
		{
			return ((EnvDTE.TextSelection)dte.ActiveDocument.Selection).ActivePoint.LineCharOffset;
		}
	}
}
