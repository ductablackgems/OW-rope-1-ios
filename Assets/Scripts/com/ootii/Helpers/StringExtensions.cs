using System;

namespace com.ootii.Helpers
{
	public static class StringExtensions
	{
		public static bool Contains(this string rSource, string rValue, StringComparison rComparison)
		{
			return rSource.IndexOf(rValue, rComparison) >= 0;
		}
	}
}
