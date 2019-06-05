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

using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;

namespace VCProjectEngineWrapper
{
	public class
#if (VS2012)
	VCFileWrapperVs2012
#elif (VS2013)
	VCFileWrapperVs2013
#elif (VS2015)
	VCFileWrapperVs2015
#elif (VS2017)
	VCFileWrapperVs2017
#elif (VS2019)
	VCFileWrapperVs2019
#endif
		: IVCFileWrapper
	{
		private VCFile _wrapped = null;

		public
#if (VS2012)
			VCFileWrapperVs2012
#elif (VS2013)
			VCFileWrapperVs2013
#elif (VS2015)
			VCFileWrapperVs2015
#elif (VS2017)
			VCFileWrapperVs2017
#elif (VS2019)
			VCFileWrapperVs2019
#endif
			(object wrapped)
		{
			_wrapped = wrapped as VCFile;
		}

		public bool isValid()
		{
			return (_wrapped != null);
		}

		public string GetWrappedVersion()
		{
			return Utility.GetWrappedVersion();
		}

		public string GetSubType()
		{
			return _wrapped.SubType;
		}

		public IVCProjectWrapper GetProject()
		{
			return new
#if (VS2012)
			VCProjectWrapperVs2012
#elif (VS2013)
			VCProjectWrapperVs2013
#elif (VS2015)
			VCProjectWrapperVs2015
#elif (VS2017)
			VCProjectWrapperVs2017
#elif (VS2019)
			VCProjectWrapperVs2019
#endif
			(_wrapped.project);
		}

		public List<IVCFileConfigurationWrapper> GetFileConfigurations()
		{
			List<IVCFileConfigurationWrapper> fileConfigurations = new List<IVCFileConfigurationWrapper>();
			foreach (Object configuration in _wrapped.FileConfigurations)
			{
				IVCFileConfigurationWrapper vcFileConfig = new
#if (VS2012)
					VCFileConfigurationWrapperVs2012
#elif (VS2013)
					VCFileConfigurationWrapperVs2013
#elif (VS2015)
					VCFileConfigurationWrapperVs2015
#elif (VS2017)
					VCFileConfigurationWrapperVs2017
#elif (VS2019)
					VCFileConfigurationWrapperVs2019
#endif
					(configuration);
				if (vcFileConfig.isValid())
				{
					fileConfigurations.Add(vcFileConfig);
				}
			}
			return fileConfigurations;
		}
	}
}
