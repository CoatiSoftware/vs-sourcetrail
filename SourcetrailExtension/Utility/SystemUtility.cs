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

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	public class SystemUtility
	{
		private static class NativeMethods
		{
			[System.Runtime.InteropServices.DllImport("user32.dll")]
			public static extern bool SetForegroundWindow(IntPtr hWnd);
			[System.Runtime.InteropServices.DllImport("user32.dll")]
			public static extern IntPtr SetActiveWindow(IntPtr hWnd);
		}

		public static void GetWindowFocus()
		{
			try
			{
				System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
				IntPtr windowHandle = process.MainWindowHandle;

				if (windowHandle != null)
				{
					NativeMethods.SetForegroundWindow(windowHandle);
					NativeMethods.SetActiveWindow(windowHandle);
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
		}

		public static void OpenWindowsExplorerAtDirectory(string directory)
		{
			System.Diagnostics.Process.Start(directory);
		}
	}
}
