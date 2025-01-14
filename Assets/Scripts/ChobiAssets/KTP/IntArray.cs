using System;

namespace ChobiAssets.KTP
{
	[Serializable]
	public class IntArray
	{
		public int[] intArray;

		public IntArray(int[] newIntArray)
		{
			intArray = newIntArray;
		}
	}
}
