﻿/*
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

using Microsoft.VisualStudio.VCProjectEngine;
using System;

namespace VCProjectEngineWrapper
{
	public class
#if (VS2012)
		VCFileConfigurationWrapperVs2012
#elif (VS2013)
		VCFileConfigurationWrapperVs2013
#elif (VS2015)
		VCFileConfigurationWrapperVs2015
#elif (VS2017)
		VCFileConfigurationWrapperVs2017
#endif
		: IVCFileConfigurationWrapper
	{
		private VCFileConfiguration _wrapped = null;

		public
#if (VS2012)
			VCFileConfigurationWrapperVs2012
#elif (VS2013)
			VCFileConfigurationWrapperVs2013
#elif (VS2015)
			VCFileConfigurationWrapperVs2015
#elif (VS2017)
			VCFileConfigurationWrapperVs2017
#endif
			(object wrapped)
		{
			_wrapped = wrapped as VCFileConfiguration;
		}

		public bool isValid()
		{
			return (_wrapped != null);
		}

		public string GetWrappedVersion()
		{
			return Utility.GetWrappedVersion();
		}

		public IVCCLCompilerToolWrapper GetCLCompilerTool()
		{
			object tool;
			try
			{
				tool = _wrapped.Tool;
			}
			catch (Exception)
			{
				tool = null;
			}

			return new
#if (VS2012)
				VCCLCompilerToolWrapperVs2012
#elif (VS2013)
				VCCLCompilerToolWrapperVs2013
#elif (VS2015)
				VCCLCompilerToolWrapperVs2015
#elif (VS2017)
				VCCLCompilerToolWrapperVs2017
#endif
				(tool);
		}
	}
}
