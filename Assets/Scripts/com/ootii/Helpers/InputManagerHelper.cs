using UnityEngine;

namespace com.ootii.Helpers
{
	public class InputManagerHelper
	{
		public static void ConvertToRadialInput(ref float rInputX, ref float rInputY, ref float rMagnitude, float rMultiplier = 1f)
		{
			if (rMagnitude > 1f)
			{
				rMagnitude = 1f;
			}
			float f = Mathf.Atan2(rInputX, rInputY) - 1.5708f;
			rInputX = rMagnitude * Mathf.Cos(f);
			rInputY = rMagnitude * (0f - Mathf.Sin(f));
			rMagnitude = Mathf.Sqrt(rInputX * rInputX + rInputY * rInputY);
			if (rMultiplier != 1f)
			{
				rInputX *= rMultiplier;
				rInputY *= rMultiplier;
				rMagnitude *= rMultiplier;
			}
		}
	}
}
