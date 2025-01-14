using System;
using UnityEngine;

namespace App.Vehicles.Skateboard
{
	[Serializable]
	public class SkateboardRotationFix
	{
		public float maxSpeed = 20f;

		public float minFixSpeed = 20f;

		public float maxFixSpeed = 50f;

		public float speedCoeff = 2f;

		public float rampMinFixSpeed = 20f;

		public float rampMaxFixSpeed = 50f;

		public float rampSpeedCoeff = 4f;

		public float GetMaxSpeed()
		{
			return maxFixSpeed * rampSpeedCoeff;
		}

		public float GetFixSpeed(float angleDifference, float velocityMagnitude, bool useRampSpeeds, bool useRampSpeedCoeff)
		{
			float a = useRampSpeeds ? rampMinFixSpeed : minFixSpeed;
			float b = useRampSpeeds ? rampMaxFixSpeed : maxFixSpeed;
			float b2 = useRampSpeedCoeff ? rampSpeedCoeff : speedCoeff;
			return Mathf.Lerp(a, b, angleDifference / 45f) * Mathf.Lerp(1f, b2, velocityMagnitude / maxSpeed);
		}
	}
}
