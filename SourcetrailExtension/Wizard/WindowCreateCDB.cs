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

using CoatiSoftware.SourcetrailExtension.SolutionParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace CoatiSoftware.SourcetrailExtension.Wizard
{
	public partial class WindowCreateCdb : Form
	{
		public struct CreationResult
		{
			public SolutionParser.CompilationDatabaseSettings _cdbSettings;
			public string _cdbDirectory;
			public string _cdbName;
		}

		public delegate void OnFinishedCreatingCdb(CreationResult result);

		private OnFinishedCreatingCdb _onFinishedCreateCdb = null;

		private List<EnvDTE.Project> _projects = new List<EnvDTE.Project>();

		private string _configurationName = "";
		private string _platformName = "";
		private string _targetDir = "";
		private string _fileName = "";
		private string _cStandard = "";
		private string _solutionDir = "";
		private string _additionalClangOptions = "";
		private bool _nonSystemIncludesUseAngleBrackets = false;

		private SolutionParser.CompilationDatabaseSettings _cdb = null;

		private CreationResult _result = new CreationResult();

		private int _threadCount = 1;

		private static object _lockObject = new object();
		private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

		public OnFinishedCreatingCdb CallbackOnFinishedCreatingCdb
		{
			get { return _onFinishedCreateCdb; }
			set { _onFinishedCreateCdb = value; }
		}

		public List<EnvDTE.Project> Projects
		{
			get { return _projects; }
			set { _projects = value; }
		}

		public string ConfigurationName
		{
			get { return _configurationName; }
			set { _configurationName = value; }
		}

		public string PlatformName
		{
			get { return _platformName; }
			set { _platformName = value; }
		}

		public string TargetDir
		{
			get { return _targetDir; }
			set { _targetDir = value; }
		}

		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		public string CStandard
		{
			get { return _cStandard; }
			set { _cStandard = value; }
		}

		public int ThreadCount
		{
			get { return _threadCount; }
			set { _threadCount = value; }
		}

		public string SolutionDir
		{
			get { return _solutionDir; }
			set { _solutionDir = value; }
		}

		public string AdditionalClangOptions
		{
			get { return _additionalClangOptions; }
			set { _additionalClangOptions = value; }
		}

		public bool NonSystemIncludesUseAngleBrackets
		{
			get { return _nonSystemIncludesUseAngleBrackets; }
			set { _nonSystemIncludesUseAngleBrackets = value; }
		}

		public SolutionParser.CompilationDatabaseSettings Cdb
		{
			get { return _cdb; }
			set { _cdb = value; }
		}


		public WindowCreateCdb()
		{
			InitializeComponent();

			progressBar.Maximum = 100;
			progressBar.Value = 0;

			backgroundWorker1.WorkerReportsProgress = true;
			backgroundWorker1.WorkerSupportsCancellation = true;
		}

		public void StartWorking()
		{
			backgroundWorker1.RunWorkerAsync();
		}

		private CreationResult CreateCdb()
		{
			CreationResult result = new CreationResult();
			result._cdbSettings = null;
			result._cdbDirectory = "";
			result._cdbName = "";

			Logging.Logging.LogInfo("Starting to create CDB");

			SolutionParser.CompilationDatabaseSettings cdbSettings = null;

			try
			{
				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
				watch.Start();
				CreateCompilationDatabase();
				watch.Stop();

				Logging.Logging.LogInfo("Finished creating CDB, elapsed time: " + watch.ElapsedMilliseconds.ToString() + " ms");

				cdbSettings = new CompilationDatabaseSettings();
				cdbSettings.Name = _fileName;
				cdbSettings.Directory = _targetDir;
				cdbSettings.SourceProject = _solutionDir;
				cdbSettings.LastUpdated = DateTime.Now;
				cdbSettings.ConfigurationName = _configurationName;
				cdbSettings.PlatformName = _platformName;
				cdbSettings.AdditionalClangOptions = _additionalClangOptions;
				cdbSettings.NonSystemIncludesUseAngleBrackets = _nonSystemIncludesUseAngleBrackets;

				cdbSettings.IncludedProjects = new List<string>();
				foreach (EnvDTE.Project p in _projects)
				{
					cdbSettings.IncludedProjects.Add(p.Name);
				}
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Failed to create CDB: " + e.Message);
			}

			result._cdbSettings = cdbSettings;
			result._cdbDirectory = _targetDir;
			result._cdbName = _fileName;

			Logging.Logging.LogInfo("Done creating CDB");

			return result;
		}

		private void CreateCompilationCommands(Utility.QueuedFileWriter fileWriter, EnvDTE.Project project, ref int projectsProcessed, bool lastProject = false)
		{
			try
			{
				SolutionParser.SolutionParser solutionParser = new SolutionParser.SolutionParser(new VsPathResolver(_targetDir));

				solutionParser.CreateCompileCommands(
					project, _configurationName, _platformName, _cStandard, _additionalClangOptions, _nonSystemIncludesUseAngleBrackets,
					(CompileCommand command, bool lastFile) => {
						string serializedCommand = "";
						foreach (string line in command.SerializeToJson().Split('\n'))
						{
							serializedCommand += "  " + line + "\n";
						}
						serializedCommand = serializedCommand.TrimEnd('\n');

						fileWriter.PushMessage(serializedCommand + (lastProject && lastFile ? "\n" : ",\n"));
					}
				);

				lock (_lockObject)
				{
					projectsProcessed++;
				}

				float relativProgress = (float)projectsProcessed / (float)_projects.Count;
				Logging.Logging.LogInfo("Processing project \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "\"");
				backgroundWorker1.ReportProgress((int)(relativProgress * 100), "Processing project \"" + project.Name + "\"");
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Failed to create CDB for project with exception: " + e.Message);
				Logging.Logging.LogError("Stack Trace: " + e.StackTrace);
				throw;
			}
		}

		private void CreateCompilationDatabase()
		{
			File.WriteAllText(_targetDir + "\\" + _fileName + ".json", "");
			File.AppendAllText(_targetDir + "\\" + _fileName + ".json", "[\n");

			Utility.QueuedFileWriter fileWriter = new Utility.QueuedFileWriter(_fileName + ".json", _targetDir);
			fileWriter.StartWorking();

			try
			{
				Multitasking.LimitedThreadsTaskScheduler scheduler = new Multitasking.LimitedThreadsTaskScheduler(_threadCount);
				TaskFactory factory = new TaskFactory(scheduler);
				List<Task> tasks = new List<Task>();

				int projectsProcessed = 0;
				foreach (EnvDTE.Project project in _projects.GetRange(0, _projects.Count - 1))
				{
					Logging.Logging.LogInfo("Scheduling project \"" + Logging.Obfuscation.NameObfuscator.GetObfuscatedName(project.Name) + "\" for parsing.");

					Task task = factory.StartNew(() =>
					{
						CreateCompilationCommands(fileWriter, project, ref projectsProcessed);
					});

					tasks.Add(task);
				}

				int threadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;

				Task.WaitAll(tasks.ToArray());

				CreateCompilationCommands(fileWriter, _projects[_projects.Count - 1], ref projectsProcessed, true);

				fileWriter.StopWorking();

				backgroundWorker1.ReportProgress(100, "Writing data to file. This might take several minutes...");
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Failed to create CDB for solution with exception: " + e.Message);
				Logging.Logging.LogError("Stack Trace: " + e.StackTrace);
			}
			finally
			{
				File.AppendAllText(_targetDir + "\\" + _fileName + ".json", "\n]");
			}
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			_result = CreateCdb();
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if(backgroundWorker1.CancellationPending == false)
			{
				progressBar.Value = e.ProgressPercentage;

				labelStatus.Text = e.UserState as string;
			}
			else
			{
				progressBar.Value = 0;

				labelStatus.Text = "Cancelling...";
			}
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if(e.Cancelled == false && e.Error == null /*&& progressBar.Value >= 100*/)
			{
				_onFinishedCreateCdb?.Invoke(_result);
			}
			else
			{
				Logging.Logging.LogWarning("Cdb creation was aborted by user");
			}

			Close();
		}

		private void WindowCreateCdb_FormClosed(object sender, FormClosedEventArgs e)
		{
			backgroundWorker1.CancelAsync();
		}
	}
}
