using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public static class NavMeshUtils
	{
		public class ScanInput
		{
			public Vector3 Pivot;

			public Vector3 StartDirection;

			public float MinScanDistance;

			public float MaxScanDistance;

			public float ScanAngle = 10f;

			public float ScanStep = 3f;

			public Func<Vector3, bool> Validate;
		}

		public static Vector3 FindValidNavMeshPosition(ScanInput input)
		{
			if (input == null)
			{
				return Vector3.zero;
			}
			if (input.MaxScanDistance <= 0f)
			{
				return Vector3.zero;
			}
			float num = 360f;
			float num2 = (input.ScanAngle <= 0f) ? (num / 12f) : input.ScanAngle;
			Vector3 angles = new Vector3(0f, num2, 0f);
			NavMeshHit hit = default(NavMeshHit);
			for (float num3 = input.MinScanDistance; num3 < input.MaxScanDistance; num3 += input.ScanStep)
			{
				float num4 = 0f;
				Vector3 vector = input.Pivot + input.StartDirection * num3;
				for (; num4 < num; num4 += num2)
				{
					if (NavMesh.SamplePosition(vector, out hit, input.MaxScanDistance, -1) & (input.Validate == null || input.Validate(hit.position)))
					{
						return hit.position;
					}
					vector = vector.RotateAround(input.Pivot, angles);
				}
			}
			return Vector3.zero;
		}
	}
}
