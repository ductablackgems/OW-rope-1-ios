using System;
using UnityEngine;

namespace com.ootii.Geometry
{
	public class BezierSpline : MonoBehaviour
	{
		[SerializeField]
		private Vector3[] mPoints;

		[SerializeField]
		private int[] mControlConstraints;

		[SerializeField]
		private int mSegments = 10;

		[SerializeField]
		private bool mLoop;

		private float mLength;

		private float[] mCurveLengths;

		public Vector3[] Points => mPoints;

		public int Segments
		{
			get
			{
				return mSegments;
			}
			set
			{
				mSegments = value;
			}
		}

		public bool Loop
		{
			get
			{
				return mLoop;
			}
			set
			{
				mLoop = value;
				if (mLoop)
				{
					mControlConstraints[mControlConstraints.Length - 1] = mControlConstraints[0];
					SetControlPoint(ControlPointCount - 1, GetControlPoint(0));
				}
			}
		}

		public float Length
		{
			get
			{
				if (mLength == 0f)
				{
					CalculateCurveLengths();
				}
				return mLength;
			}
		}

		public int ControlPointCount => (mPoints.Length - 1) / 3 + 1;

		public int CurveCount => (mPoints.Length - 1) / 3;

		public void Awake()
		{
			CalculateCurveLengths();
		}

		public void Reset()
		{
			mLoop = false;
			mLength = 0f;
			mPoints = new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, 2f),
				new Vector3(0f, 0f, 3f)
			};
			mCurveLengths = new float[1]
			{
				3f
			};
			mControlConstraints = new int[2];
		}

		public void AddControlPoint()
		{
			if (mPoints == null)
			{
				Reset();
			}
			int num = mPoints.Length;
			Vector3 vector = mPoints[mPoints.Length - 1];
			Array.Resize(ref mPoints, num + 3);
			Array.Resize(ref mCurveLengths, (mPoints.Length - 1) / 3);
			Array.Resize(ref mControlConstraints, mCurveLengths.Length + 1);
			vector.z += 1f;
			mPoints[num++] = vector;
			vector.z += 1f;
			mPoints[num++] = vector;
			vector.z += 1f;
			mPoints[num++] = vector;
			mControlConstraints[mControlConstraints.Length - 1] = mControlConstraints[mControlConstraints.Length - 2];
			if (mLoop)
			{
				mControlConstraints[mControlConstraints.Length - 1] = mControlConstraints[0];
				SetControlPoint(ControlPointCount - 1, GetControlPoint(0));
			}
			CalculateCurveLengths();
		}

		public void InsertControlPoint(int rIndex)
		{
			if (mPoints == null)
			{
				Reset();
			}
			if (rIndex < 0 || rIndex >= ControlPointCount)
			{
				AddControlPoint();
				return;
			}
			if (rIndex == 0)
			{
				rIndex = 1;
			}
			Array.Resize(ref mPoints, mPoints.Length + 3);
			for (int num = mPoints.Length - 4; num >= rIndex * 3; num--)
			{
				mPoints[num + 3] = mPoints[num];
			}
			int num2 = rIndex * 3;
			int num3 = (rIndex - 1) * 3;
			int num4 = (rIndex + 1) * 3;
			mPoints[num4 - 1] = mPoints[num2 - 1];
			mPoints[num2] = (mPoints[num2 - 3] + mPoints[num2 + 3]) / 2f;
			mPoints[num2 + 1] = mPoints[num2] + (mPoints[num4] - mPoints[num2]) / 2f;
			mPoints[num2 - 1] = mPoints[num2] + (mPoints[num3] - mPoints[num2]) / 2f;
			Array.Resize(ref mCurveLengths, mCurveLengths.Length + 1);
			for (int num5 = mCurveLengths.Length - 1; num5 >= rIndex; num5--)
			{
				mCurveLengths[num5] = mCurveLengths[num5 - 1];
			}
			Array.Resize(ref mControlConstraints, mCurveLengths.Length + 1);
			for (int num6 = mControlConstraints.Length - 1; num6 >= rIndex; num6--)
			{
				mControlConstraints[num6] = mControlConstraints[num6 - 1];
			}
			CalculateCurveLengths();
		}

		public void DeleteControlPoint(int rIndex)
		{
			if (rIndex >= 0 && rIndex < ControlPointCount && mPoints.Length > 4)
			{
				for (int i = (rIndex != 0) ? (rIndex * 3 - 1) : 0; i < mPoints.Length - 3; i++)
				{
					mPoints[i] = mPoints[i + 3];
				}
				Array.Resize(ref mPoints, mPoints.Length - 3);
				for (int j = rIndex; j < mCurveLengths.Length - 1; j++)
				{
					mCurveLengths[j] = mCurveLengths[j + 1];
				}
				Array.Resize(ref mCurveLengths, mCurveLengths.Length - 1);
				for (int k = rIndex; k < mControlConstraints.Length - 1; k++)
				{
					mControlConstraints[k] = mControlConstraints[k + 1];
				}
				Array.Resize(ref mControlConstraints, mControlConstraints.Length - 1);
				CalculateCurveLengths();
			}
		}

		public Vector3 GetControlPoint(int rIndex)
		{
			rIndex *= 3;
			if (rIndex < 0 || rIndex >= mPoints.Length)
			{
				return Vector3.zero;
			}
			return mPoints[rIndex];
		}

		public void SetControlPoint(int rIndex, Vector3 rPoint)
		{
			int controlPointCount = ControlPointCount;
			int num = rIndex * 3;
			if (num < 0 || num >= mPoints.Length)
			{
				return;
			}
			Vector3 b = rPoint - mPoints[num];
			mPoints[num] = rPoint;
			if (mLoop)
			{
				if (rIndex == 0)
				{
					mPoints[mPoints.Length - 1] = mPoints[0];
				}
				else if (rIndex == controlPointCount - 1)
				{
					mPoints[0] = mPoints[mPoints.Length - 1];
				}
			}
			SetBackwardTangentPoint(rIndex, GetBackwardTangentPoint(rIndex) + b);
			if (GetControlPointConstraint(rIndex) == 1)
			{
				SetForwardTangentPoint(rIndex, GetForwardTangentPoint(rIndex) + b);
			}
			CalculateCurveLengths();
		}

		public int GetControlPointConstraint(int rIndex)
		{
			if (rIndex < 0 || rIndex >= mControlConstraints.Length)
			{
				return 0;
			}
			return mControlConstraints[rIndex];
		}

		public void SetControlPointConstraint(int rIndex, int rConstraint)
		{
			if (rIndex < 0 || rIndex >= mControlConstraints.Length)
			{
				return;
			}
			mControlConstraints[rIndex] = rConstraint;
			if (mLoop)
			{
				if (rIndex == 0)
				{
					mControlConstraints[mControlConstraints.Length - 1] = rConstraint;
				}
				else if (rIndex == mControlConstraints.Length - 1)
				{
					mControlConstraints[0] = rConstraint;
				}
			}
			ApplyControlPointConstraint(rIndex, rLeadWithBackwardCP: true);
		}

		public Vector3 GetBackwardTangentPoint(int rIndex)
		{
			rIndex *= 3;
			if (rIndex < 0 || rIndex >= mPoints.Length)
			{
				return Vector3.zero;
			}
			rIndex--;
			if (rIndex < 0)
			{
				if (!mLoop)
				{
					return Vector3.zero;
				}
				rIndex = mPoints.Length - 2;
			}
			return mPoints[rIndex];
		}

		public void SetBackwardTangentPoint(int rIndex, Vector3 rPoint)
		{
			int num = rIndex * 3;
			if (num < 0 || num >= mPoints.Length)
			{
				return;
			}
			num--;
			if (num < 0)
			{
				if (!mLoop)
				{
					return;
				}
				num = mPoints.Length - 2;
			}
			mPoints[num] = rPoint;
			ApplyControlPointConstraint(rIndex, rLeadWithBackwardCP: true);
		}

		public Vector3 GetForwardTangentPoint(int rIndex)
		{
			rIndex *= 3;
			if (rIndex < 0 || rIndex >= mPoints.Length)
			{
				return Vector3.zero;
			}
			rIndex++;
			if (rIndex >= mPoints.Length)
			{
				if (!mLoop)
				{
					return Vector3.zero;
				}
				rIndex = 1;
			}
			return mPoints[rIndex];
		}

		public void SetForwardTangentPoint(int rIndex, Vector3 rPoint)
		{
			int num = rIndex * 3;
			if (num < 0 || num >= mPoints.Length)
			{
				return;
			}
			num++;
			if (num >= mPoints.Length)
			{
				if (!mLoop)
				{
					return;
				}
				num = 1;
			}
			mPoints[num] = rPoint;
			ApplyControlPointConstraint(rIndex, rLeadWithBackwardCP: false);
		}

		public Vector3 GetPoint(float rPercent)
		{
			int rCurveIndex = 0;
			float rCurvePercent = 0f;
			GetCurvePercent(rPercent, ref rCurveIndex, ref rCurvePercent);
			return GetPoint(rCurveIndex, rCurvePercent);
		}

		public Vector3 GetPoint(int rCurveIndex, float rPercent)
		{
			if (rPercent < -1f || rPercent > 1f)
			{
				rPercent %= 1f;
			}
			if (rPercent < 0f)
			{
				rPercent = 1f + rPercent;
			}
			rCurveIndex *= 3;
			Vector3 cubicPoint = GetCubicPoint(mPoints[rCurveIndex], mPoints[rCurveIndex + 1], mPoints[rCurveIndex + 2], mPoints[rCurveIndex + 3], rPercent);
			return base.transform.TransformPoint(cubicPoint);
		}

		public Vector3 GetVelocity(float rPercent)
		{
			int rCurveIndex = 0;
			float rCurvePercent = 0f;
			GetCurvePercent(rPercent, ref rCurveIndex, ref rCurvePercent);
			return GetVelocity(rCurveIndex, rCurvePercent);
		}

		public Vector3 GetVelocity(int rCurveIndex, float rPercent)
		{
			if (rPercent < -1f || rPercent > 1f)
			{
				rPercent %= 1f;
			}
			if (rPercent < 0f)
			{
				rPercent = 1f + rPercent;
			}
			rCurveIndex *= 3;
			Vector3 position = GetFirstCubicDerivative(mPoints[rCurveIndex], mPoints[rCurveIndex + 1], mPoints[rCurveIndex + 2], mPoints[rCurveIndex + 3], rPercent) - base.transform.position;
			return base.transform.TransformPoint(position);
		}

		public Vector3 GetDirection(float rPercent)
		{
			int rCurveIndex = 0;
			float rCurvePercent = 0f;
			GetCurvePercent(rPercent, ref rCurveIndex, ref rCurvePercent);
			return GetDirection(rCurveIndex, rCurvePercent);
		}

		public Vector3 GetDirection(int rCurveIndex, float rPercent)
		{
			return GetVelocity(rCurveIndex, rPercent).normalized;
		}

		public void GetCurvePercent(float rPercent, ref int rCurveIndex, ref float rCurvePercent)
		{
			if (mLength == 0f)
			{
				CalculateCurveLengths();
			}
			if (rPercent < -1f || rPercent > 1f)
			{
				rPercent %= 1f;
			}
			if (rPercent < 0f)
			{
				rPercent = 1f + rPercent;
			}
			if (rPercent == 0f)
			{
				rCurveIndex = 0;
				rCurvePercent = 0f;
				return;
			}
			if (rPercent == 1f)
			{
				rCurveIndex = CurveCount - 1;
				rCurvePercent = 1f;
				return;
			}
			float num = mLength * rPercent;
			int num2 = 0;
			float num3 = 0f;
			for (num2 = 0; num2 < CurveCount && !(num < num3 + mCurveLengths[num2]); num2++)
			{
				num3 += mCurveLengths[num2];
			}
			num -= num3;
			rCurveIndex = num2;
			rCurvePercent = num / mCurveLengths[num2];
		}

		public void ApplyControlPointConstraint(int rIndex, bool rLeadWithBackwardCP)
		{
			if ((!mLoop && rIndex <= 0) || (!mLoop && rIndex >= ControlPointCount - 1))
			{
				return;
			}
			int controlPointConstraint = GetControlPointConstraint(rIndex);
			if (controlPointConstraint != 1)
			{
				Vector3 controlPoint = GetControlPoint(rIndex);
				int num = rIndex * 3 - 1;
				if (num < 0)
				{
					num = mPoints.Length - 2;
				}
				Vector3 backwardTangentPoint = GetBackwardTangentPoint(rIndex);
				int num2 = rIndex * 3 + 1;
				if (num2 >= mPoints.Length)
				{
					num2 = 1;
				}
				Vector3 forwardTangentPoint = GetForwardTangentPoint(rIndex);
				switch (controlPointConstraint)
				{
				case 0:
					if (rLeadWithBackwardCP)
					{
						Vector3 b3 = backwardTangentPoint - controlPoint;
						mPoints[num2] = controlPoint - b3;
					}
					else
					{
						Vector3 b4 = forwardTangentPoint - controlPoint;
						mPoints[num] = controlPoint - b4;
					}
					break;
				case 2:
					if (rLeadWithBackwardCP)
					{
						Vector3 b = (controlPoint - backwardTangentPoint).normalized * Vector3.Distance(controlPoint, forwardTangentPoint);
						mPoints[num2] = controlPoint - b;
					}
					else
					{
						Vector3 b2 = (controlPoint - forwardTangentPoint).normalized * Vector3.Distance(controlPoint, backwardTangentPoint);
						mPoints[num] = controlPoint - b2;
					}
					break;
				}
			}
			CalculateCurveLengths();
		}

		public float CalculateCurveLengths()
		{
			mLength = 0f;
			if (mCurveLengths == null || mCurveLengths.Length == 0)
			{
				mCurveLengths = new float[CurveCount];
			}
			Vector3 a = GetPoint(0, 0f);
			for (int i = 0; i < CurveCount; i++)
			{
				float num = 0f;
				for (int j = 1; j <= mSegments; j++)
				{
					float rPercent = (float)j / (float)mSegments;
					Vector3 point = GetPoint(i, rPercent);
					num += Vector3.Distance(a, point);
					a = point;
				}
				mCurveLengths[i] = num;
				mLength += num;
			}
			return mLength;
		}

		public static Vector3 GetQuadradicPoint(Vector3 rP0, Vector3 rP1, Vector3 rP2, float rTime)
		{
			rTime = Mathf.Clamp01(rTime);
			float num = 1f - rTime;
			return num * num * rP0 + 2f * num * rTime * rP1 + rTime * rTime * rP2;
		}

		public static Vector3 GetFirstQuadradicDerivative(Vector3 rP0, Vector3 rP1, Vector3 rP2, float rTime)
		{
			return 2f * (1f - rTime) * (rP1 - rP0) + 2f * rTime * (rP2 - rP1);
		}

		public static Vector3 GetCubicPoint(Vector3 rP0, Vector3 rP1, Vector3 rP2, Vector3 rP3, float rTime)
		{
			rTime = Mathf.Clamp01(rTime);
			float num = 1f - rTime;
			return num * num * num * rP0 + 3f * num * num * rTime * rP1 + 3f * num * rTime * rTime * rP2 + rTime * rTime * rTime * rP3;
		}

		public static Vector3 GetFirstCubicDerivative(Vector3 rP0, Vector3 rP1, Vector3 rP2, Vector3 rP3, float rTime)
		{
			rTime = Mathf.Clamp01(rTime);
			float num = 1f - rTime;
			return 3f * num * num * (rP1 - rP0) + 6f * num * rTime * (rP2 - rP1) + 3f * rTime * rTime * (rP3 - rP2);
		}
	}
}
