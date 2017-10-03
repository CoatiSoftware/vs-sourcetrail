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

using Microsoft.VisualStudio.VCProjectEngine;

namespace VCProjectEngineWrapper
{
	public class
#if (VS2012)
		VCCLCompilerToolWrapperVs2012
#elif (VS2013)
		VCCLCompilerToolWrapperVs2013
#elif (VS2015)
		VCCLCompilerToolWrapperVs2015
#elif (VS2017)
		VCCLCompilerToolWrapperVs2017
#endif
		: IVCCLCompilerToolWrapper
	{
		private VCCLCompilerTool _wrapped = null;
		private IVCRulePropertyStorage _wrappedRules = null;

		public
#if (VS2012)
			VCCLCompilerToolWrapperVs2012
#elif (VS2013)
			VCCLCompilerToolWrapperVs2013
#elif (VS2015)
			VCCLCompilerToolWrapperVs2015
#elif (VS2017)
			VCCLCompilerToolWrapperVs2017
#endif
			(object wrapped)
		{
			_wrapped = wrapped as VCCLCompilerTool;
			_wrappedRules = wrapped as IVCRulePropertyStorage;
		}

		public bool isValid()
		{
			return (_wrapped != null && _wrappedRules != null);
		}

		public string GetWrappedVersion()
		{
			return Utility.GetWrappedVersion();
		}

		public string GetAdditionalOptions()
		{
			return _wrapped.AdditionalOptions;
		}

		public bool GetCompilesAsC()
		{
			return _wrapped.CompileAs == CompileAsOptions.compileAsC;
		}

		public bool GetCompilesAsCPlusPlus()
		{
			return _wrapped.CompileAs == CompileAsOptions.compileAsCPlusPlus;
		}

		public string GetToolPath()
		{
			return _wrapped.ToolPath;
		}

		public string[] GetAdditionalIncludeDirectories()
		{
			return _wrappedRules.GetEvaluatedPropertyValue("AdditionalIncludeDirectories").Split(';');
		}

		public string[] GetPreprocessorDefinitions()
		{
			return _wrappedRules.GetEvaluatedPropertyValue("PreprocessorDefinitions").Split(';');
		}

		public string[] GetForcedIncludeFiles()
		{
			return _wrappedRules.GetEvaluatedPropertyValue("ForcedIncludeFiles").Split(';');
		}
	}
}
