using System;
using UnityEngine;

namespace App.Util
{
	[Serializable]
	public class FloatRange
	{
		public float Min;

		public float Max;

		public FloatRange(float min, float max)
		{
			Min = min;
			Max = max;
		}

		public bool IsInRange(float value)
		{
			if (value >= Min)
			{
				return value <= Max;
			}
			return false;
		}

		public float GetRandomValue()
		{
			return UnityEngine.Random.Range(Min, Max);
		}
	}
}
