using UnityEngine;

namespace Forge3D
{
	public class F3DPredictTrajectory : MonoBehaviour
	{
		public static Vector3 Predict(Vector3 sPos, Vector3 tPos, Vector3 tLastPos, float pSpeed)
		{
			Vector3 vector = (tPos - tLastPos) / Time.deltaTime;
			float projFlightTime = GetProjFlightTime(tPos - sPos, vector, pSpeed);
			if (projFlightTime > 0f)
			{
				return tPos + projFlightTime * vector;
			}
			return tPos;
		}

		private static float GetProjFlightTime(Vector3 dist, Vector3 tVel, float pSpeed)
		{
			float num = Vector3.Dot(tVel, tVel) - pSpeed * pSpeed;
			float num2 = 2f * Vector3.Dot(tVel, dist);
			float num3 = Vector3.Dot(dist, dist);
			float num4 = num2 * num2 - 4f * num * num3;
			if (num4 > 0f)
			{
				return 2f * num3 / (Mathf.Sqrt(num4) - num2);
			}
			return -1f;
		}
	}
}
