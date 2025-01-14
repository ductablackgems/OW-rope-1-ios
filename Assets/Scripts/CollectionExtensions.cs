using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtensions
{
	public static void Shuffle<T>(this IList<T> list)
	{
		if (list != null)
		{
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int index = Random.Range(0, num + 1);
				T value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}
	}

	public static T GetRandomElement<T>(this IList<T> list, bool allowRepeat = true)
	{
		if (list == null)
		{
			return default(T);
		}
		int count = list.Count;
		switch (count)
		{
		case 0:
			return default(T);
		case 1:
			return list[0];
		default:
		{
			int index = Random.Range(1, count);
			T val = list[index];
			if (allowRepeat)
			{
				return val;
			}
			list[index] = list[0];
			list[0] = val;
			return val;
		}
		}
	}
}
