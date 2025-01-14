using UnityEngine;

namespace App
{
	public class MathHelper
	{
		public static bool HitProbabilityPerMinute(float hitPerMinute, float deltaTime)
		{
			float num = hitPerMinute / 60f * deltaTime;
			return Random.Range(0f, 1f) <= num;
		}

		public static Vector3[] LineCircleIntersection(Vector3 center, float radius, Vector3 rayStart, Vector3 rayEnd)
		{
			Vector3 vector = rayEnd - rayStart;
			Vector3 vector2 = rayStart - center;
			float num = Vector3.Dot(vector, vector);
			float num2 = 2f * Vector3.Dot(vector2, vector);
			float num3 = Vector3.Dot(vector2, vector2) - radius * radius;
			float num4 = num2 * num2 - 4f * num * num3;
			if (num4 >= 0f)
			{
				num4 = Mathf.Sqrt(num4);
				float num5 = (0f - num2 - num4) / (2f * num);
				float num6 = (0f - num2 + num4) / (2f * num);
				return (num5 == num6) ? new Vector3[1]
				{
					rayStart + vector * num5
				} : new Vector3[2]
				{
					rayStart + vector * num5,
					rayStart + vector * num6
				};
			}
			return null;
		}

		public static bool GetParallelIntersection(Vector3 begin1, Vector3 end1, Vector3 begin2, Vector3 end2, out Vector3 begin, out Vector3 end)
		{
			Vector3[] array = new Vector3[4]
			{
				begin1,
				end1,
				begin2,
				end2
			};
			float[] array2 = new float[4]
			{
				begin1.magnitude,
				end1.magnitude,
				begin2.magnitude,
				end2.magnitude
			};
			int num = 0;
			for (int i = 1; i < 4; i++)
			{
				if (array2[i] < array2[num])
				{
					num = i;
				}
			}
			int num2 = -1;
			for (int j = 0; j < 4; j++)
			{
				if (j != num && (num2 == -1 || array2[j] < array2[num2]))
				{
					num2 = j;
				}
			}
			int num3 = num + num2;
			if (num3 == 1 || num3 == 5)
			{
				begin = default(Vector3);
				end = default(Vector3);
				return false;
			}
			int num4 = -1;
			for (int k = 0; k < 4; k++)
			{
				if (k != num && k != num2 && (num4 == -1 || array2[k] < array2[num4]))
				{
					num4 = k;
				}
			}
			begin = array[num2];
			end = array[num4];
			return true;
		}
	}
}
