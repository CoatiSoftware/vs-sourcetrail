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

using CoatiSoftware.SourcetrailExtension;
using Microsoft.VisualStudio.VCProjectEngine;

namespace VCProjectEngineWrapper
{
	public class
#if (VS2012)
		VCPlatformWrapperVs2012
#elif (VS2013)
		VCPlatformWrapperVs2013
#elif (VS2015)
		VCPlatformWrapperVs2015
#elif (VS2017)
		VCPlatformWrapperVs2017
#endif
		: IVCPlatformWrapper
	{
		private VCPlatform _wrapped = null;

		public
#if (VS2012)
			VCPlatformWrapperVs2012
#elif (VS2013)
			VCPlatformWrapperVs2013
#elif (VS2015)
			VCPlatformWrapperVs2015
#elif (VS2017)
			VCPlatformWrapperVs2017
#endif
			(object wrapped)
		{
			_wrapped = wrapped as VCPlatform;
		}

		public bool isValid()
		{
			return (_wrapped != null);
		}

		public string GetWrappedVersion()
		{
			return Utility.GetWrappedVersion();
		}

		public string GetExecutableDirectories()
		{
			return _wrapped.ExecutableDirectories;
		}

		public string[] GetIncludeDirectories()
		{
			return _wrapped.IncludeDirectories.SplitPaths();
		}
	}
}
