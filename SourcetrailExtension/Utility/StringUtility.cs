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

using System;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	public class StringUtility
	{
		static public Tuple<int, int> FindFirstRange(string text, string startTag, string endTag)
		{
			if(startTag.Length > 0 && endTag.Length > 0 && (startTag.Length + endTag.Length) < text.Length)
			{
				int start = text.IndexOf(startTag);
				int end = text.IndexOf(endTag);

				// just check wheter both, start and end, are valid and the end tag occurs after the start tag
				if(start > -1 && end > -1 && end > start)
				{
					return new Tuple<int, int>(start, end);
				}
			}

			return null;
		}

		static public int GetMatchingCharsFromStart(string a, string b)
		{
			int matchingChars = 0;

			if (a != string.Empty)
			{
				a = a.ToLower();
			}
			else
			{
				return matchingChars;
			}

			if (b != string.Empty)
			{
				b = b.ToLower();
			}
			else
			{
				return matchingChars;
			}

			for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
			{
				if (!char.Equals(a[i], b[i]))
				{
					break;
				}
				else
				{
					matchingChars++;
				}
			}

			return matchingChars;
		}
	}
}
