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
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections;

namespace VCProjectEngineWrapper
{
	public class
#if (VS2012)
		VCProjectWrapperVs2012
#elif (VS2013)
		VCProjectWrapperVs2013
#elif (VS2015)
		VCProjectWrapperVs2015
#elif (VS2017)
		VCProjectWrapperVs2017
#endif
		: IVCProjectWrapper
	{
		private VCProject _wrapped = null;

		public
#if (VS2012)
			VCProjectWrapperVs2012
#elif (VS2013)
			VCProjectWrapperVs2013
#elif (VS2015)
			VCProjectWrapperVs2015
#elif (VS2017)
			VCProjectWrapperVs2017
#endif
			(object wrapped)
		{
			_wrapped = wrapped as VCProject;
		}

		public bool isValid()
		{
			return (_wrapped != null);
		}

		public string GetWrappedVersion()
		{
			return Utility.GetWrappedVersion();
		}

		public IVCConfigurationWrapper getConfiguration(string configurationName, string platformName)
		{
			try
			{
				IEnumerable configurations = _wrapped.Configurations as IEnumerable;
				foreach (Object configuration in configurations)
				{
					VCConfiguration vcProjectConfig = configuration as VCConfiguration;

					if (vcProjectConfig != null &&
						vcProjectConfig.ConfigurationName == configurationName &&
						vcProjectConfig.Platform.Name == platformName)
					{
						return new 
#if (VS2012)
						VCConfigurationWrapperVs2012
#elif (VS2013)
						VCConfigurationWrapperVs2013
#elif (VS2015)
						VCConfigurationWrapperVs2015
#elif (VS2017)
						VCConfigurationWrapperVs2017
#endif
						(vcProjectConfig);
					}
				}
			}
			catch (Exception e)
			{
				Logging.LogError("Failed to retreive project configuration: " + e.Message);
			}
			Logging.LogError("Failed to find project config matching with \"" + configurationName + "\"");

			return new 
#if (VS2012)
			VCConfigurationWrapperVs2012
#elif (VS2013)
			VCConfigurationWrapperVs2013
#elif (VS2015)
			VCConfigurationWrapperVs2015
#elif (VS2017)
			VCConfigurationWrapperVs2017
#endif
			(null);
		}

		public string GetProjectDirectory()
		{
			return _wrapped.ProjectDirectory;
		}

		public string GetName()
		{
			return _wrapped.Name;
		}

		public string GetProjectFile()
		{
			return _wrapped.ProjectFile;
		}
	}
}
