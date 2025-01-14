using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	public static class BezierUtil
	{
		public static List<Vector3> InterpolatePath(List<Vector3> path, int segments, float radius, float angleThreshold)
		{
			if (path.Count >= 3)
			{
				List<Vector3> list = new List<Vector3>();
				int num = path.Count - 1;
				list.Add(path[0]);
				int num2 = 0;
				for (int i = 2; i < path.Count; i++)
				{
					Vector3 b = path[i - 2];
					Vector3 vector = path[i - 1];
					Vector3 a = path[i];
					Vector3 vector2 = vector - b;
					Vector3 vector3 = a - vector;
					if (!(Mathf.Abs(Vector3.Angle(vector2, vector3)) <= angleThreshold))
					{
						float magnitude = vector2.magnitude;
						float magnitude2 = vector3.magnitude;
						vector2.Normalize();
						vector3.Normalize();
						magnitude = Mathf.Min(magnitude * 0.5f, radius);
						magnitude2 = Mathf.Min(magnitude2 * 0.5f, radius);
						Vector3 a2 = vector - vector2 * magnitude;
						Vector3 a3 = vector;
						Vector3 a4 = vector + vector3 * magnitude2;
						for (int j = 0; j < segments; j++)
						{
							float num3 = (float)j / ((float)segments - 1f);
							float num4 = 1f - num3;
							Vector3 item = num4 * num4 * a2 + 2f * num4 * num3 * a3 + num3 * num3 * a4;
							list.Add(item);
						}
						num2 = i;
					}
				}
				if (num2 <= num)
				{
					list.Add(path[num]);
				}
				return list;
			}
			return path;
		}

		public static Vector3[] GetBezierApproximation(Vector3[] controlPoints, int outputSegmentCount)
		{
			Vector3[] array = new Vector3[outputSegmentCount + 1];
			for (int i = 0; i < outputSegmentCount; i++)
			{
				float t = (float)i / (float)outputSegmentCount;
				array[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
			}
			return array;
		}

		public static Vector3 GetBezierPoint(float t, Vector3[] controlPoints, int index, int count)
		{
			if (count == 1)
			{
				return controlPoints[index];
			}
			Vector3 bezierPoint = GetBezierPoint(t, controlPoints, index - 1, count - 1);
			Vector3 bezierPoint2 = GetBezierPoint(t, controlPoints, index, count - 1);
			Vector3 bezierPoint3 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
			return (1f - t) * (1f - t) * bezierPoint + 2f * (1f - t) * t * bezierPoint2 + t * t * bezierPoint3;
		}
	}
}
