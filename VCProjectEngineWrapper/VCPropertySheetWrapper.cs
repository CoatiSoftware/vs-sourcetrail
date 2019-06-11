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
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections;

namespace VCProjectEngineWrapper
{
	public class
#if (VS2015)
		VCPropertySheetWrapperVs2015
#elif (VS2017)
		VCPropertySheetWrapperVs2017
#elif (VS2019)
		VCPropertySheetWrapperVs2019
#endif
		: IVCPropertySheetWrapper
	{
		private VCPropertySheet _wrapped = null;

		public
#if (VS2015)
			VCPropertySheetWrapperVs2015
#elif (VS2017)
			VCPropertySheetWrapperVs2017
#elif (VS2019)
			VCPropertySheetWrapperVs2019
#endif
			(object wrapped)
		{
			_wrapped = wrapped as VCPropertySheet;
		}

		public bool isValid()
		{
			return (_wrapped != null);
		}

		public string GetWrappedVersion()
		{
			return Utility.GetWrappedVersion();
		}

		public string getName()
		{
			return _wrapped.Name;
		}

		public IVCCLCompilerToolWrapper GetCLCompilerTool()
		{
			try
			{
				IEnumerable tools = _wrapped.Tools as IEnumerable;
				foreach (Object tool in tools)
				{
					VCCLCompilerTool compilerTool = tool as VCCLCompilerTool;
					if (compilerTool != null)
					{
						return new
#if (VS2015)
							VCCLCompilerToolWrapperVs2015
#elif (VS2017)
							VCCLCompilerToolWrapperVs2017
#elif (VS2019)
							VCCLCompilerToolWrapperVs2019
#endif
							(compilerTool);
					}
				}
			}
			catch (Exception e)
			{
				Logging.LogError("Property Sheet failed to retreive cl compiler tool: " + e.Message);
			}
			return new
#if (VS2015)
				VCCLCompilerToolWrapperVs2015
#elif (VS2017)
				VCCLCompilerToolWrapperVs2017
#elif (VS2019)
				VCCLCompilerToolWrapperVs2019
#endif
				(null);
		}

		public IVCResourceCompilerToolWrapper GetResourceCompilerTool()
		{
			try
			{
				IEnumerable tools = _wrapped.Tools as IEnumerable;
				foreach (Object tool in tools)
				{
					VCResourceCompilerTool compilerTool = tool as VCResourceCompilerTool;
					if (compilerTool != null)
					{
						return new
#if (VS2015)
							VCResourceCompilerToolWrapperVs2015
#elif (VS2017)
							VCResourceCompilerToolWrapperVs2017
#elif (VS2019)
							VCResourceCompilerToolWrapperVs2019
#endif
							(compilerTool);
					}
				}
			}
			catch (Exception e)
			{
				Logging.LogError("Property Sheet failed to retreive resource compiler tool: " + e.Message);
			}
			return new
#if (VS2015)
				VCResourceCompilerToolWrapperVs2015
#elif (VS2017)
				VCResourceCompilerToolWrapperVs2017
#elif (VS2019)
				VCResourceCompilerToolWrapperVs2019
#endif
				(null);
		}

	}
}
