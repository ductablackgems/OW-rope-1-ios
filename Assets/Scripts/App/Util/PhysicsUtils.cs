using System.Collections.Generic;
using UnityEngine;

namespace App.Util
{
	public static class PhysicsUtils
	{
		private static Collider[] colliderHits = new Collider[40];

		public static bool IsValidPosition(Vector3 position, float radius, int layerMask)
		{
			return Physics.OverlapSphereNonAlloc(position, radius, colliderHits, layerMask, QueryTriggerInteraction.Ignore) == 0;
		}

		public static int GetNumberOfObjectsInRadius(Vector3 position, float radius, int layerMask)
		{
			return Physics.OverlapSphereNonAlloc(position, radius, colliderHits, layerMask, QueryTriggerInteraction.Ignore);
		}

		public static Vector3 GetValidSpherePosition(Vector3 pivot, Vector3 forward, float sphereRadius, float startDistance, float maxDistance, float scanAngle, int layerMask, List<Vector3> checkedPositions = null)
		{
			if (maxDistance <= 0f)
			{
				return pivot;
			}
			maxDistance = ((maxDistance < startDistance) ? startDistance : maxDistance);
			Vector3 result = pivot;
			bool flag = false;
			float num = 360f;
			scanAngle = ((scanAngle <= 0f) ? (num / 12f) : scanAngle);
			Vector3 angles = new Vector3(0f, scanAngle, 0f);
			float num2 = startDistance;
			while (num2 < maxDistance)
			{
				float num3 = 0f;
				Vector3 vector = pivot + forward * num2;
				for (; num3 < num; num3 += scanAngle)
				{
					if (flag)
					{
						break;
					}
					checkedPositions?.Add(vector);
					flag = IsValidPosition(vector, sphereRadius, layerMask);
					vector = RotatePointAroundPivot(vector, pivot, angles);
				}
				num2 += 1f;
				if (flag)
				{
					result = vector;
					break;
				}
			}
			return result;
		}

		private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			return Quaternion.Euler(angles) * (point - pivot) + pivot;
		}
	}
}
