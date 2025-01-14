using UnityEngine;

namespace com.ootii.Geometry
{
	public static class Matrix4x4Ext
	{
		public static Vector3 Position(this Matrix4x4 rMatrix)
		{
			return rMatrix.GetColumn(3);
		}

		public static Quaternion Rotation(this Matrix4x4 rMatrix)
		{
			return Quaternion.LookRotation(rMatrix.GetColumn(2), rMatrix.GetColumn(1));
		}

		public static Vector3 Scale(this Matrix4x4 rMatrix)
		{
			return new Vector3(rMatrix.GetColumn(0).magnitude, rMatrix.GetColumn(1).magnitude, rMatrix.GetColumn(2).magnitude);
		}
	}
}
