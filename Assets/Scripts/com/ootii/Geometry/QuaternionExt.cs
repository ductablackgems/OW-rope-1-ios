using UnityEngine;

namespace com.ootii.Geometry
{
	public static class QuaternionExt
	{
		public static float EPSILON = 1E-06f;

		public static Quaternion InverseIdentity = new Quaternion(0f, 0f, 0f, -1f);

		public static bool IsEqual(Quaternion rLeft, Quaternion rRight)
		{
			if (Mathf.Abs(rLeft.x - rRight.x) > EPSILON)
			{
				return false;
			}
			if (Mathf.Abs(rLeft.y - rRight.y) > EPSILON)
			{
				return false;
			}
			if (Mathf.Abs(rLeft.z - rRight.z) > EPSILON)
			{
				return false;
			}
			if (Mathf.Abs(rLeft.w - rRight.w) > EPSILON)
			{
				return false;
			}
			return true;
		}

		public static bool IsEqual(ref Quaternion rLeft, ref Quaternion rRight)
		{
			if (Mathf.Abs(rLeft.x - rRight.x) > EPSILON)
			{
				return false;
			}
			if (Mathf.Abs(rLeft.y - rRight.y) > EPSILON)
			{
				return false;
			}
			if (Mathf.Abs(rLeft.z - rRight.z) > EPSILON)
			{
				return false;
			}
			if (Mathf.Abs(rLeft.w - rRight.w) > EPSILON)
			{
				return false;
			}
			return true;
		}

		public static bool IsIdentity(this Quaternion rThis)
		{
			if (rThis.x != 0f)
			{
				return false;
			}
			if (rThis.y != 0f)
			{
				return false;
			}
			if (rThis.z != 0f)
			{
				return false;
			}
			if (rThis.w == 1f || rThis.w == -1f)
			{
				return true;
			}
			return false;
		}

		public static Quaternion RotationTo(this Quaternion rFrom, Quaternion rTo)
		{
			return Quaternion.Inverse(rFrom) * rTo;
		}

		public static Quaternion OrientTo(this Quaternion rFrom, Quaternion rTo)
		{
			Quaternion rhs = Quaternion.Inverse(rFrom);
			return rTo * rhs;
		}

		public static Quaternion Subtract(this Quaternion rLHS, Quaternion rRHS)
		{
			Quaternion result = default(Quaternion);
			result.x = rLHS.x - rRHS.x;
			result.y = rLHS.y - rRHS.y;
			result.z = rLHS.z - rRHS.z;
			result.w = rLHS.w - rRHS.w;
			return result;
		}

		public static Quaternion Negate(this Quaternion rThis)
		{
			Quaternion result = default(Quaternion);
			result.x = 0f - rThis.x;
			result.y = 0f - rThis.y;
			result.z = 0f - rThis.z;
			result.w = 0f - rThis.w;
			return result;
		}

		public static Quaternion Conjugate(this Quaternion rThis)
		{
			Quaternion result = default(Quaternion);
			result.x = 0f - rThis.x;
			result.y = 0f - rThis.y;
			result.z = 0f - rThis.z;
			result.w = rThis.w;
			return result;
		}

		public static Vector3 Forward(this Quaternion rThis)
		{
			return new Vector3(2f * (rThis.x * rThis.z + rThis.w * rThis.y), 2f * (rThis.y * rThis.z - rThis.w * rThis.x), 1f - 2f * (rThis.x * rThis.x + rThis.y * rThis.y));
		}

		public static Vector3 Up(this Quaternion rThis)
		{
			return new Vector3(2f * (rThis.x * rThis.y - rThis.w * rThis.z), 1f - 2f * (rThis.x * rThis.x + rThis.z * rThis.z), 2f * (rThis.y * rThis.z + rThis.w * rThis.x));
		}

		public static Vector3 Right(this Quaternion rThis)
		{
			return new Vector3(1f - 2f * (rThis.y * rThis.y + rThis.z * rThis.z), 2f * (rThis.x * rThis.y + rThis.w * rThis.z), 2f * (rThis.x * rThis.z - rThis.w * rThis.y));
		}

		public static Quaternion FromToRotation(Vector3 u, Vector3 v)
		{
			float num = Vector3.Dot(u.normalized, v.normalized);
			if (num >= 1f)
			{
				return Quaternion.identity;
			}
			if (num <= -1f)
			{
				Vector3 axis = Vector3.Cross(u, Vector3.right);
				if (axis.sqrMagnitude == 0f)
				{
					axis = Vector3.Cross(u, Vector3.up);
				}
				return Quaternion.AngleAxis(180f, axis);
			}
			float num2 = Mathf.Acos(num);
			Vector3 normalized = Vector3.Cross(u, v).normalized;
			return Quaternion.AngleAxis(num2 * 57.29578f, normalized);
		}

		public static void DecomposeSwingTwist(this Quaternion rThis, Vector3 rAxis, ref Quaternion rSwing, ref Quaternion rTwist)
		{
			Vector3 normalized = rAxis.normalized;
			float d = Vector3.Dot(new Vector3(rThis.x, rThis.y, rThis.z), normalized);
			normalized *= d;
			rTwist.x = normalized.x;
			rTwist.y = normalized.y;
			rTwist.z = normalized.z;
			rTwist.w = rThis.w;
			rTwist = rTwist.normalized;
			rSwing = rThis * Quaternion.Inverse(rTwist);
		}

		public static void DecomposeTwistSwingAxisAngles(this Quaternion rThis, Vector3 rTwistAxis, ref float rTwistAngle, ref Vector3 rSwingAxis, ref float rSwingAngle)
		{
			rTwistAngle = 2f * Mathf.Atan2(rThis.y, rThis.w) * 57.29578f;
			Vector4 vector = new Vector4(0f, rThis.y, 0f, rThis.w) / Mathf.Sqrt(rThis.y * rThis.y + rThis.w * rThis.w);
			Quaternion rThis2 = new Quaternion(vector.x, vector.y, vector.z, vector.w);
			Quaternion quaternion = rThis * rThis2.Conjugate();
			float num = Mathf.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z);
			if (num > 1E-06f)
			{
				float num2 = 1f / num;
				rSwingAxis.x = quaternion.x * num2;
				rSwingAxis.y = quaternion.y * num2;
				rSwingAxis.z = quaternion.z * num2;
				if (quaternion.w < 0f)
				{
					rSwingAngle = 2f * Mathf.Atan2(0f - num, 0f - quaternion.w) * 57.29578f;
				}
				else
				{
					rSwingAngle = 2f * Mathf.Atan2(num, quaternion.w) * 57.29578f;
				}
			}
			else
			{
				rSwingAxis = Vector3.right;
				rSwingAngle = 0f;
			}
		}

		public static Quaternion FromString(this Quaternion rThis, string rString)
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
	}
}
