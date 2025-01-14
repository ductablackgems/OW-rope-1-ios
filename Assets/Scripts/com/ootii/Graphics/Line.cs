using com.ootii.Collections;
using UnityEngine;

namespace com.ootii.Graphics
{
	public class Line
	{
		public Transform Transform;

		public Vector3 Start = Vector3.zero;

		public Vector3 End = Vector3.zero;

		public Color Color = Color.white;

		public float ExpirationTime;

		private static ObjectPool<Line> sPool = new ObjectPool<Line>(20, 5);

		public static int Length => sPool.Length;

		public static Line Allocate()
		{
			return sPool.Allocate();
		}

		public static void Release(Line rInstance)
		{
			if (rInstance != null)
			{
				rInstance.Transform = null;
				rInstance.Start = Vector3.zero;
				rInstance.End = Vector3.zero;
				rInstance.Color = Color.white;
				rInstance.ExpirationTime = 0f;
				sPool.Release(rInstance);
			}
		}
	}
}
