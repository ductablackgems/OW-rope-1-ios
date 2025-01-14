using com.ootii.Collections;
using UnityEngine;

namespace com.ootii.Graphics
{
	public class Triangle
	{
		public Transform Transform;

		public Vector3 Point1 = Vector3.zero;

		public Vector3 Point2 = Vector3.zero;

		public Vector3 Point3 = Vector3.zero;

		public Color Color = Color.white;

		public float ExpirationTime;

		private static ObjectPool<Triangle> sPool = new ObjectPool<Triangle>(20, 5);

		public static int Length => sPool.Length;

		public static Triangle Allocate()
		{
			return sPool.Allocate();
		}

		public static void Release(Triangle rInstance)
		{
			if (rInstance != null)
			{
				rInstance.Transform = null;
				rInstance.Point1 = Vector3.zero;
				rInstance.Point2 = Vector3.zero;
				rInstance.Point3 = Vector3.zero;
				rInstance.Color = Color.white;
				rInstance.ExpirationTime = 0f;
				sPool.Release(rInstance);
			}
		}
	}
}
