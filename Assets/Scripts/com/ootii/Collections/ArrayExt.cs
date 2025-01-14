using System;

namespace com.ootii.Collections
{
	public static class ArrayExt
	{
		public static void RemoveAt<T>(ref T[] rSource, int rIndex)
		{
			if (rSource.Length != 0 && rIndex >= 0 && rIndex < rSource.Length)
			{
				T[] array = new T[rSource.Length - 1];
				if (rIndex > 0)
				{
					Array.Copy(rSource, 0, array, 0, rIndex);
				}
				if (rIndex < rSource.Length - 1)
				{
					Array.Copy(rSource, rIndex + 1, array, rIndex, rSource.Length - rIndex - 1);
				}
				rSource = array;
			}
		}

		public static T[] RemoveAt<T>(T[] rSource, int rIndex)
		{
			int num = rSource.Length;
			if (num == 0)
			{
				return null;
			}
			if (rIndex < 0 || rIndex >= num)
			{
				return null;
			}
			int i = 0;
			int num2 = 0;
			T[] array = new T[num - 1];
			for (; i < num; i++)
			{
				if (i != rIndex)
				{
					array[num2] = rSource[i];
					num2++;
				}
			}
			return array;
		}

		public static void Sort<T>(this T[] rSource, Comparison<T> rComparison)
		{
			if (rSource.Length > 1)
			{
				Array.Sort(rSource, rComparison);
			}
		}

		public static bool Contains<T>(this T[] rArray, T rValue)
		{
			return Array.Exists(rArray, (T item) => item.Equals(rValue));
		}
	}
}
