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
using System.Diagnostics;

namespace CoatiSoftware.SourcetrailExtension.Logging
{
	public class VSOutputLogger : ILogger
	{
		private EnvDTE.DTE _dte = null;
		private OutputWindowPane _pane = null;

		public VSOutputLogger(EnvDTE.DTE dte)
		{
			_dte = dte;
		}

		public void LogMessage(LogMessage message)
		{
			if (message.MessageType == SourcetrailExtension.Logging.LogMessage.LogMessageType.INFO)
			{
				Debug.WriteLine(message.Message, "Info");
				WriteToOutputWindow("Info: " + message.Message);
			}
			if (message.MessageType == SourcetrailExtension.Logging.LogMessage.LogMessageType.WARNING)
			{
				Debug.WriteLine(message.Message, "Warning");
				WriteToOutputWindow("Warning: " + message.Message);
			}
			if (message.MessageType == SourcetrailExtension.Logging.LogMessage.LogMessageType.ERROR)
			{
				Debug.WriteLine(message.Message, "Error");
				WriteToOutputWindow("Error: " + message.Message);
			}
		}

		private void WriteToOutputWindow(string message)
		{
			string paneName = "Sourcetrail Log";

			if (_dte.Windows.Count > 0)
			{
				Window window = _dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
				OutputWindow outputWindow = (OutputWindow)window.Object;

				OutputWindowPanes panes = outputWindow.OutputWindowPanes;

				if(_pane == null)
				{
					try
					{
						for (int i = 0; i < panes.Count; i++)
						{
							OutputWindowPane pane = panes.Item(i);

							if (pane.Name.Equals(paneName, StringComparison.CurrentCultureIgnoreCase))
							{
								_pane = outputWindow.OutputWindowPanes.Item(i);
								break;
							}
						}
					}
					catch (Exception)
					{
						// don't log here, otherwise we create an infinite loop ;)
					}
				}

				if (_pane == null)
				{
					_pane = outputWindow.OutputWindowPanes.Add(paneName);
				}

				_pane.OutputString(message + '\n');
			}
		}
	}
}
