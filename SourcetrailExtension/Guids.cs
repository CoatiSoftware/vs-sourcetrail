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

// Guids.cs
// MUST match guids.h
using System;

namespace CoatiSoftware.SourcetrailExtension
{
	public static class GuidList
	{
		public const string guidSourcetrailExtensionPkgString = "acf15780-03b5-440e-a41e-db79b7043fc2";
		public const string guidSourcetrailExtensionCmdSetString = "0efb005b-715c-4a62-8a9b-1e5a870e6c34";

		public static readonly Guid guidSourcetrailExtensionCmdSet = new Guid(guidSourcetrailExtensionCmdSetString);
	};
}