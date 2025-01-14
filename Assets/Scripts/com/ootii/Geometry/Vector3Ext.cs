using System.Globalization;
using System.Threading;
using UnityEngine;

namespace com.ootii.Geometry
{
	public static class Vector3Ext
	{
		public static Vector3 Null;

		static Vector3Ext()
		{
			Null = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		public static float SignedAngle(Vector3 rFrom, Vector3 rTo, Vector3 rAxis)
		{
			if (rTo == rFrom)
			{
				return 0f;
			}
			Vector3 rhs = Vector3.Cross(rFrom, rTo);
			float x = Vector3.Dot(rFrom, rTo);
			return (float)((!(Vector3.Dot(rAxis, rhs) < 0f)) ? 1 : (-1)) * Mathf.Atan2(rhs.magnitude, x) * 57.29578f;
		}

		public static float SignedAngle(Vector3 rFrom, Vector3 rTo)
		{
			if (rTo == rFrom)
			{
				return 0f;
			}
			Vector3 vector = Vector3.Cross(rFrom, rTo);
			return (float)((!(vector.y < -0.0001f)) ? 1 : (-1)) * Mathf.Atan2(x: Vector3.Dot(rFrom, rTo), y: vector.magnitude) * 57.29578f;
		}

		public static float AngleTo(this Vector3 rFrom, Vector3 rTo)
		{
			return SignedAngle(rFrom, rTo);
		}

		public static void DecomposeYawPitch(Transform rOwner, Vector3 rFrom, Vector3 rTo, ref float rYaw, ref float rPitch)
		{
			Vector3 vector = rTo - rFrom;
			rPitch = (0f - Mathf.Atan2(vector.y, Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z))) * 57.29578f + rOwner.rotation.eulerAngles.x;
			rYaw = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f + 90f - rOwner.rotation.eulerAngles.y;
		}

		public static float HorizontalMagnitude(this Vector3 rVector)
		{
			return Mathf.Sqrt(rVector.x * rVector.x + rVector.z * rVector.z);
		}

		public static float HorizontalSqrMagnitude(this Vector3 rVector)
		{
			return rVector.x * rVector.x + rVector.z * rVector.z;
		}

		public static float HorizontalAngleTo(this Vector3 rFrom, Vector3 rTo)
		{
			float num = Mathf.Atan2(Vector3.Dot(Vector3.up, Vector3.Cross(rFrom, rTo)), Vector3.Dot(rFrom, rTo));
			num *= 57.29578f;
			if ((double)Mathf.Abs(num) < 0.0001)
			{
				num = 0f;
			}
			return num;
		}

		public static float HorizontalAngleTo(this Vector3 rFrom, Vector3 rTo, Vector3 rUp)
		{
			float num = Mathf.Atan2(Vector3.Dot(rUp, Vector3.Cross(rFrom, rTo)), Vector3.Dot(rFrom, rTo));
			num *= 57.29578f;
			if ((double)Mathf.Abs(num) < 0.0001)
			{
				num = 0f;
			}
			return num;
		}

		public static float HorizontalAngleFrom(this Vector3 rTo, Vector3 rFrom)
		{
			float num = Mathf.Atan2(Vector3.Dot(Vector3.up, Vector3.Cross(rFrom, rTo)), Vector3.Dot(rFrom, rTo));
			num *= 57.29578f;
			if ((double)Mathf.Abs(num) < 0.0001)
			{
				num = 0f;
			}
			return num;
		}

		public static float DistanceTo(this Vector3 rFrom, Vector3 rTo, float rYTolerance)
		{
			float num = rTo.y - rFrom.y;
			if (num > 0f)
			{
				num = Mathf.Max(num - rYTolerance, 0f);
			}
			else if (num < 0f)
			{
				num = Mathf.Min(num + rYTolerance, 0f);
			}
			rTo.y = rFrom.y + num;
			return Vector3.Distance(rFrom, rTo);
		}

		public static Vector3 DirectionTo(this Vector3 rFrom, Vector3 rTo)
		{
			return (rTo - rFrom).normalized;
		}

		public static Vector3 NormalizeRotations(this Vector3 rThis)
		{
			Vector3 result = rThis;
			rThis.x = ((rThis.x < -180f) ? (rThis.x + 360f) : ((rThis.x > 180f) ? (rThis.x - 360f) : rThis.x));
			rThis.y = ((rThis.y < -180f) ? (rThis.y + 360f) : ((rThis.y > 180f) ? (rThis.y - 360f) : rThis.y));
			rThis.z = ((rThis.z < -180f) ? (rThis.z + 360f) : ((rThis.z > 180f) ? (rThis.z - 360f) : rThis.z));
			return result;
		}

		public static Vector3 AddRotation(this Vector3 rFrom, Vector3 rTo)
		{
			return rFrom + rTo;
		}

		public static Vector3 AddRotation(this Vector3 rFrom, float rX, float rY, float rZ)
		{
			Vector3 vector = rFrom;
			vector.x += rX;
			vector.y += rY;
			vector.z += rZ;
			return vector;
		}

		public static void FindOrthogonals(Vector3 rNormal, ref Vector3 rOrthoUp, ref Vector3 rOrthoRight)
		{
			rNormal.Normalize();
			rOrthoRight = Quaternion.AngleAxis(90f, Vector3.right) * rNormal;
			if (Mathf.Abs(Vector3.Dot(rNormal, rOrthoRight)) > 0.6f)
			{
				rOrthoRight = Quaternion.AngleAxis(90f, Vector3.up) * rNormal;
			}
			rOrthoRight.Normalize();
			rOrthoRight = Vector3.Cross(rNormal, rOrthoRight).normalized;
			rOrthoUp = Vector3.Cross(rNormal, rOrthoRight).normalized;
		}

		public static Vector3 PlaneNormal(Vector3 rVertexA, Vector3 rVertexB, Vector3 rVertexC)
		{
			Vector3 rhs = rVertexB - rVertexA;
			return Vector3.Cross(rVertexC - rVertexA, rhs).normalized;
		}

		public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
		{
			planeNormal = Vector3.zero;
			planePoint = Vector3.zero;
			Vector3 vector = pointB - pointA;
			Vector3 vector2 = pointC - pointA;
			planeNormal = Vector3.Normalize(Vector3.Cross(vector, vector2));
			Vector3 vector3 = pointA + vector / 2f;
			Vector3 vector4 = pointA + vector2 / 2f;
			Vector3 lineVec = pointC - vector3;
			Vector3 lineVec2 = pointB - vector4;
			ClosestPointsOnTwoLines(out planePoint, out Vector3 _, vector3, lineVec, vector4, lineVec2);
		}

		public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			closestPointLine1 = Vector3.zero;
			closestPointLine2 = Vector3.zero;
			float num = Vector3.Dot(lineVec1, lineVec1);
			float num2 = Vector3.Dot(lineVec1, lineVec2);
			float num3 = Vector3.Dot(lineVec2, lineVec2);
			float num4 = num * num3 - num2 * num2;
			if (num4 != 0f)
			{
				Vector3 rhs = linePoint1 - linePoint2;
				float num5 = Vector3.Dot(lineVec1, rhs);
				float num6 = Vector3.Dot(lineVec2, rhs);
				float d = (num2 * num6 - num5 * num3) / num4;
				float d2 = (num * num6 - num5 * num2) / num4;
				closestPointLine1 = linePoint1 + lineVec1 * d;
				closestPointLine2 = linePoint2 + lineVec2 * d2;
				return true;
			}
			return false;
		}

		public static Vector3 MoveTo(Vector3 rValue, Vector3 rTarget, float rVelocity, float rDeltaTime)
		{
			if (rValue == rTarget)
			{
				return rTarget;
			}
			Vector3 a = (rTarget - rValue).normalized * rVelocity;
			Vector3 result = rValue + a * rDeltaTime;
			if (result.sqrMagnitude > rTarget.sqrMagnitude)
			{
				return rTarget;
			}
			return result;
		}

		public static Vector2 FromString(this Vector2 rThis, string rString)
		{
			string obj = (rString.IndexOf(' ') >= 0) ? rString.Substring(0, rString.IndexOf(' ')) : rString;
			char c = ',';
			int num = obj.IndexOf(',');
			int num2 = obj.IndexOf('.');
			if (num2 < 0)
			{
				c = ',';
			}
			if (num < 0)
			{
				c = '.';
			}
			else if (num < num2)
			{
				c = '.';
			}
			string[] array = rString.Substring(1, rString.Length - 2).Split(c);
			if (array.Length != 2)
			{
				return rThis;
			}
			rThis.x = float.Parse(array[0]);
			rThis.y = float.Parse(array[1]);
			return rThis;
		}

		public static Vector3 FromString(this Vector3 rThis, string rString)
		{
			string obj = (rString.IndexOf(' ') >= 0) ? rString.Substring(0, rString.IndexOf(' ')) : rString;
			char c = ',';
			int num = obj.IndexOf(',');
			int num2 = obj.IndexOf('.');
			if (num2 < 0)
			{
				c = ',';
			}
			if (num < 0)
			{
				c = '.';
			}
			else if (num < num2)
			{
				c = '.';
			}
			string[] array = rString.Substring(1, rString.Length - 2).Split(c);
			if (array.Length != 3)
			{
				return rThis;
			}
			rThis.x = float.Parse(array[0]);
			rThis.y = float.Parse(array[1]);
			rThis.z = float.Parse(array[2]);
			return rThis;
		}

		public static Vector4 FromString(this Vector4 rThis, string rString)
		{
			string obj = (rString.IndexOf(' ') >= 0) ? rString.Substring(0, rString.IndexOf(' ')) : rString;
			char c = ',';
			int num = obj.IndexOf(',');
			int num2 = obj.IndexOf('.');
			if (num2 < 0)
			{
				c = ',';
			}
			if (num < 0)
			{
				c = '.';
			}
			else if (num < num2)
			{
				c = '.';
			}
			string[] array = rString.Substring(1, rString.Length - 2).Split(c);
			if (array.Length != 4)
			{
				return rThis;
			}
			rThis.x = float.Parse(array[0]);
			rThis.y = float.Parse(array[1]);
			rThis.z = float.Parse(array[2]);
			rThis.w = float.Parse(array[3]);
			return rThis;
		}

		public static float Dot(this Vector3 rThis, Vector3 rTarget)
		{
			return rThis.x * rTarget.x + rThis.y * rTarget.y + rThis.z * rTarget.z;
		}

		public static Vector3 SmoothStep(Vector3 rStart, Vector3 rEnd, float rTime)
		{
			if (rTime <= 0f)
			{
				return rStart;
			}
			if (rTime >= 1f)
			{
				return rEnd;
			}
			rTime = rTime * rTime * rTime * (rTime * (6f * rTime - 15f) + 10f);
			Vector3 vector = rEnd - rStart;
			float d = vector.magnitude * rTime;
			return rStart + vector.normalized * d;
		}
	}
}
