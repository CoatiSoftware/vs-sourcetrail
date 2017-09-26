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

using CoatiSoftware.SourcetrailExtension.Logging;
using System;
using System.Collections.Generic;

namespace VCProjectEngineWrapper
{
	public static class VCProjectWrapperFactory
	{
		private interface IFactoryModule
		{
			IVCProjectWrapper Create(object wrapped);
		}

		private class FactoryModule2017 : IFactoryModule
		{
			public IVCProjectWrapper Create(object wrapped)
			{
				return new VCProjectWrapperVs2017(wrapped);
			}
		}

		private class FactoryModule2015 : IFactoryModule
		{
			public IVCProjectWrapper Create(object wrapped)
			{
				return new VCProjectWrapperVs2015(wrapped);
			}
		}

		private class FactoryModule2013 : IFactoryModule
		{
			public IVCProjectWrapper Create(object wrapped)
			{
				return new VCProjectWrapperVs2013(wrapped);
			}
		}

		private class FactoryModule2012 : IFactoryModule
		{
			public IVCProjectWrapper Create(object wrapped)
			{
				return new VCProjectWrapperVs2012(wrapped);
			}
		}

		private static Queue<IFactoryModule> modules = null;

		public static IVCProjectWrapper create(object wrapped)
		{
			if (modules == null)
			{
				modules = new Queue<IFactoryModule>();

				// One of these modules will be working for each version of Visual Studio.
				modules.Enqueue(new FactoryModule2017());
				modules.Enqueue(new FactoryModule2015());
				modules.Enqueue(new FactoryModule2013());
				modules.Enqueue(new FactoryModule2012());
			}

			IVCProjectWrapper wrapper = null;
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
					Logging.LogInfo("Discarcing " + failedModule.GetType().Name + " while creating project wrapper.");
					modules.Enqueue(failedModule);
					wrapper = null;
				}
			}

			return wrapper;
		}
	}
}
