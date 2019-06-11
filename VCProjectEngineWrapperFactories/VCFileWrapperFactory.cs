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

using CoatiSoftware.SourcetrailExtension.Logging;
using System;
using System.Collections.Generic;

namespace VCProjectEngineWrapper
{
	public static class VCFileWrapperFactory
	{
		private interface IFactoryModule
		{
			IVCFileWrapper Create(object wrapped);
		}

		private class FactoryModule2019 : IFactoryModule
		{
			public IVCFileWrapper Create(object wrapped)
			{
				return new VCFileWrapperVs2019(wrapped);
			}
		}

		private class FactoryModule2017 : IFactoryModule
		{
			public IVCFileWrapper Create(object wrapped)
			{
				return new VCFileWrapperVs2017(wrapped);
			}
		}

		private class FactoryModule2015 : IFactoryModule
		{
			public IVCFileWrapper Create(object wrapped)
			{
				return new VCFileWrapperVs2015(wrapped);
			}
		}

		private class FactoryModule2013 : IFactoryModule
		{
			public IVCFileWrapper Create(object wrapped)
			{
				return new VCFileWrapperVs2013(wrapped);
			}
		}

		private class FactoryModule2012 : IFactoryModule
		{
			public IVCFileWrapper Create(object wrapped)
			{
				return new VCFileWrapperVs2012(wrapped);
			}
		}

		private static Queue<IFactoryModule> modules = null;

		public static IVCFileWrapper create(object wrapped)
		{
			if (modules == null)
			{
				modules = new Queue<IFactoryModule>();

				// One of these modules will be working for each version of Visual Studio.
				modules.Enqueue(new FactoryModule2019());
				modules.Enqueue(new FactoryModule2017());
				modules.Enqueue(new FactoryModule2015());
				modules.Enqueue(new FactoryModule2013());
				modules.Enqueue(new FactoryModule2012());
			}

			IVCFileWrapper wrapper = null;
			int testedModuleCount = 0;

			while (wrapper == null && testedModuleCount < modules.Count)
			{
				try
				{
					wrapper = modules.Peek().Create(wrapped);
				}
				catch (Exception)
				{
					wrapper = null;
				}

				testedModuleCount++;

				if (wrapper == null || !wrapper.isValid())
				{
					// Moving the failing module to the end of the queue.
					// This causes the working module to finall end up in front.
					IFactoryModule failedModule = modules.Dequeue();
					Logging.LogInfo("Discarcing " + failedModule.GetType().Name + " while creating file wrapper.");
					modules.Enqueue(failedModule);
					wrapper = null;
				}
			}

			return wrapper;
		}
	}
}
