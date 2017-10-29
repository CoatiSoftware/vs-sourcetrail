using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoatiSoftware.SourcetrailExtension
{
	public static class StringExtensions
	{
		public static string[] SplitPaths(this string @this)
		{
			return @this.Split(';').Select(x => x.Trim()).ToArray();
		}
	}
}
