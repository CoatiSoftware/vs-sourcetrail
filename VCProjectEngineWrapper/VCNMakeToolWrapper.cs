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

namespace VCProjectEngineWrapper
{
	public class
#if (VS2012)
		VCNMakeToolWrapperVs2012
#elif (VS2013)
        VCNMakeToolWrapperVs2013
#elif (VS2015)
		VCNMakeToolWrapperVs2015
#elif (VS2017)
		VCNMakeToolWrapperVs2017
#endif
        : IVCNMakeToolWrapper
	{
		private VCNMakeTool _wrapped = null;

		public
#if (VS2012)
			VCNMakeToolWrapperVs2012
#elif (VS2013)
            VCNMakeToolWrapperVs2013
#elif (VS2015)
			VCNMakeToolWrapperVs2015
#elif (VS2017)
			VCNMakeToolWrapperVs2017
#endif
            (object wrapped)
		{
			_wrapped = wrapped as VCNMakeTool;
		}

		public bool isValid()
		{
			return (_wrapped != null);
		}

		public string GetWrappedVersion()
		{
			return Utility.GetWrappedVersion();
		}

		public string GetToolPath()
		{
			return _wrapped.ToolPath;
		}

		public string[] GetIncludeSearchPaths()
		{
			return _wrapped.IncludeSearchPath.Split(';');
		}

		public string[] GetPreprocessorDefinitions()
		{
			return _wrapped.PreprocessorDefinitions.Split(';');
		}

		public string GetForcedIncludes()
		{
			return _wrapped.ForcedIncludes;
		}

	}
}