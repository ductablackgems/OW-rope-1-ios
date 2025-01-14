using System;
using UnityEngine;

namespace com.ootii.Helpers
{
	public class NumberHelper
	{
		public static System.Random Randomizer = new System.Random();

		public static float GetRandom(float rMin, float rMax)
		{
			return UnityEngine.Random.Range(rMin, rMax);
		}

		public static Vector3 GetRandom(Vector3 rCenter, float rRadius, bool rRandomizeY)
		{
			float num = rRadius * 2f * (float)Randomizer.NextDouble() - rRadius;
			rCenter.x += num;
			if (rRandomizeY)
			{
				num = rRadius * 2f * (float)Randomizer.NextDouble() - rRadius;
				rCenter.y += num;
			}
			num = rRadius * 2f * (float)Randomizer.NextDouble() - rRadius;
			rCenter.z += num;
			return rCenter;
		}

		public static float EaseInQuadratic(float rTime)
		{
			return rTime * rTime;
		}

		public static float EaseOutQuadratic(float rTime)
		{
			return rTime * (2f - rTime);
		}

		public static float EaseInOutQuadratic(float rTime)
		{
			if ((rTime *= 2f) < 1f)
			{
				return 0.5f * rTime * rTime;
			}
			return -0.5f * ((rTime -= 1f) * (rTime - 2f) - 1f);
		}

		public static float EaseInOutCubic(float rTime)
		{
			return rTime * rTime * rTime * (rTime * (6f * rTime - 15f) + 10f);
		}

		public static float SmoothStep(float rStart, float rEnd, float rTime)
		{
			if (rTime <= 0f)
			{
				return rStart;
			}
			if (rTime >= 1f)
			{
				return rEnd;
			}
			float num = rTime * rTime * rTime * (rTime * (6f * rTime - 15f) + 10f);
			float num2 = (rEnd - rStart) * num;
			return rStart + num2;
		}

		public static float SmoothStepTime(float rStart, float rEnd, float rValue)
		{
			if (rValue <= rStart)
			{
				return 0f;
			}
			if (rValue >= rEnd)
			{
				return 1f;
			}
			int num = 0;
			float num2 = 0f;
			float num3 = 1f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			do
			{
				num++;
				num4 = num2 + (num3 - num2) * 0.5f;
				num5 = SmoothStep(rStart, rEnd, num4);
				num6 = rValue - num5;
				if (num6 > 0f)
				{
					num2 = num4;
				}
				else if (num6 < 0f)
				{
					num3 = num4;
				}
			}
			while (num < 40 && Mathf.Abs(num6) > 0.0001f);
			return num4;
		}

		public static float ClampAngle(float rAngle, float rMin, float rMax)
		{
			if (rAngle < -360f)
			{
				rAngle += 360f;
			}
			if (rAngle > 360f)
			{
				rAngle -= 360f;
			}
			return Mathf.Clamp(rAngle, rMin, rMax);
		}

		public static float NormalizeAngle(float rAngle)
		{
			if (rAngle < -360f)
			{
				rAngle += 360f;
			}
			if (rAngle > 360f)
			{
				rAngle -= 360f;
			}
			return rAngle;
		}

		public static float GetHorizontalDistance(Vector3 rFrom, Vector3 rTo)
		{
			rFrom.y = 0f;
			rTo.y = 0f;
			return (rTo - rFrom).magnitude;
		}

		public static void GetHorizontalDifference(Vector3 rFrom, Vector3 rTo, ref Vector3 rResult)
		{
			rFrom.y = 0f;
			rTo.y = 0f;
			rResult = rTo - rFrom;
		}

		public static float GetHorizontalAngle(Vector3 rFrom, Vector3 rTo)
		{
			float num = Mathf.Atan2(Vector3.Dot(Vector3.up, Vector3.Cross(rFrom, rTo)), Vector3.Dot(rFrom, rTo));
			num *= 57.29578f;
			if (Mathf.Abs(num) < 0.0001f)
			{
				num = 0f;
			}
			return num;
		}

		public static float GetHorizontalAngle(Vector3 rFrom, Vector3 rTo, Vector3 rUp)
		{
			float num = Mathf.Atan2(Vector3.Dot(rUp, Vector3.Cross(rFrom, rTo)), Vector3.Dot(rFrom, rTo));
			num *= 57.29578f;
			if (Mathf.Abs(num) < 0.0001f)
			{
				num = 0f;
			}
			return num;
		}

		public static void GetHorizontalQuaternion(Vector3 rFrom, Vector3 rTo, ref Quaternion rResult)
		{
			rFrom.y = 0f;
			rTo.y = 0f;
			rResult = Quaternion.LookRotation(rTo - rFrom);
		}

		public static float Pow(float rBase, int rExponent)
		{
			switch (rExponent)
			{
			case 0:
				return 0f;
			case 1:
				return rBase;
			}
			while (rExponent > 1)
			{
				rBase *= rBase;
				rExponent--;
			}
			return rBase;
		}
	}
}
