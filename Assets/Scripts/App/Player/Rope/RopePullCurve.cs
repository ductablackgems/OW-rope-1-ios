using UnityEngine;

namespace App.Player.Rope
{
	public class RopePullCurve : ScriptableObject
	{
		public AnimationCurve humanCurve;

		public float GetHumanStopDistance(float totalDistance)
		{
			if (totalDistance > 40f)
			{
				totalDistance = 40f;
			}
			return humanCurve.Evaluate(totalDistance);
		}
	}
}
