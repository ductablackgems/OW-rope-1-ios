using System;
using UnityEngine;

public static class Vector3Extensions
{
	public static Vector3 RotateX(this Vector3 vector, float angle)
	{
		angle *= (float)Math.PI / 180f;
		float num = Mathf.Sin(angle);
		float num2 = Mathf.Cos(angle);
		vector.y = num2 * vector.y - num * vector.z;
		vector.z = num2 * vector.z + num * vector.y;
		return vector;
	}

	public static Vector3 RotateY(this Vector3 vector, float angle)
	{
		angle *= (float)Math.PI / 180f;
		float num = Mathf.Sin(angle);
		float num2 = Mathf.Cos(angle);
		vector.x = num2 * vector.x + num * vector.z;
		vector.z = num2 * vector.z - num * vector.x;
		return vector;
	}

	public static Vector3 RotateZ(this Vector3 vector, float angle)
	{
		angle *= (float)Math.PI / 180f;
		float num = Mathf.Sin(angle);
		float num2 = Mathf.Cos(angle);
		vector.x = num2 * vector.x - num * vector.y;
		vector.y = num2 * vector.y + num * vector.x;
		return vector;
	}

	public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

	public static Vector3 RemoveVertical(this Vector3 vector)
	{
		vector.y = 0f;
		return vector;
	}
}
