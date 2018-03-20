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
using System.Threading;
using System.Threading.Tasks;

// based on: https://msdn.microsoft.com/en-us/library/system.threading.tasks.taskscheduler(v=vs.110).aspx

namespace CoatiSoftware.SourcetrailExtension.Multitasking
{
	class LimitedThreadsTaskScheduler : TaskScheduler
	{
		[ThreadStatic]
		private static bool _currentThreadIsProcessingItems;

		private readonly LinkedList<Task> _tasks = new LinkedList<Task>();

		private readonly int _maxNumberOfRunningThreads = 0;

		private int _delegatesQueuedOrRunning = 0;

		public LimitedThreadsTaskScheduler(int maxNumberOfRunningThreads)
		{
			if (maxNumberOfRunningThreads < 1)
			{
				maxNumberOfRunningThreads = 1;
			}

			_maxNumberOfRunningThreads = maxNumberOfRunningThreads;
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			bool lockTaken = false;

			try
			{
				Monitor.TryEnter(_tasks, ref lockTaken);
				if(lockTaken)
				{
					return _tasks;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(_tasks);
				}
			}
		}

		protected override void QueueTask(Task task)
		{
			lock(_tasks)
			{
				_tasks.AddLast(task);
				
				if(_delegatesQueuedOrRunning < _maxNumberOfRunningThreads)
				{
					++_delegatesQueuedOrRunning;
					NotifyThreadPoolOfPendingWork();
				}
			}
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			if(!_currentThreadIsProcessingItems)
			{
				return false;
			}

			if(taskWasPreviouslyQueued)
			{
				if(TryDequeue(task))
				{
					return base.TryExecuteTask(task);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return base.TryExecuteTask(task);
			}
		}

		private void NotifyThreadPoolOfPendingWork()
		{
			ThreadPool.UnsafeQueueUserWorkItem(_ =>
			{
				_currentThreadIsProcessingItems = true;
				try
				{
					while(true)
					{
						Task item;

						lock(_tasks)
						{
							if(_tasks.Count <= 0)
							{
								--_delegatesQueuedOrRunning;
								break;
							}

							item = _tasks.First.Value;
							_tasks.RemoveFirst();
						}

						base.TryExecuteTask(item);
					}
				}
				finally
				{
					_currentThreadIsProcessingItems = false;
				}

			}, null);
		}

		protected sealed override bool TryDequeue(Task task)
		{
			lock(_tasks)
			{
				return _tasks.Remove(task);
			}
		}
	}
}
