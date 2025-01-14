using com.ootii.Collections;
using com.ootii.Geometry;
using UnityEngine;

namespace com.ootii.Actors
{
	public class BodyShapeHit
	{
		public BodyShape Shape;

		public Vector3 StartPosition;

		public Vector3 EndPosition;

		public Collider HitCollider;

		public Vector3 HitOrigin;

		public Vector3 HitPoint;

		public Vector3 HitNormal;

		public float HitDistance;

		public float HitRootDistance;

		public bool HitPenetration;

		public bool IsPlatformHit;

		public RaycastHit Hit;

		private static ObjectPool<BodyShapeHit> sPool = new ObjectPool<BodyShapeHit>(20, 5);

		public static int Length => sPool.Length;

		public void CalculateHitOrigin()
		{
			HitOrigin = Shape.CalculateHitOrigin(HitPoint, StartPosition, EndPosition);
		}

		public static BodyShapeHit Allocate()
		{
			return sPool.Allocate();
		}

		public static BodyShapeHit Allocate(BodyShapeHit rInstance)
		{
			if (rInstance == null)
			{
				return sPool.Allocate();
			}
			BodyShapeHit bodyShapeHit = sPool.Allocate();
			bodyShapeHit.Shape = rInstance.Shape;
			bodyShapeHit.StartPosition = rInstance.StartPosition;
			bodyShapeHit.EndPosition = rInstance.EndPosition;
			bodyShapeHit.HitCollider = rInstance.HitCollider;
			bodyShapeHit.HitOrigin = rInstance.HitOrigin;
			bodyShapeHit.HitPoint = rInstance.HitPoint;
			bodyShapeHit.HitNormal = rInstance.HitNormal;
			bodyShapeHit.HitDistance = rInstance.HitDistance;
			bodyShapeHit.HitRootDistance = rInstance.HitRootDistance;
			bodyShapeHit.HitPenetration = rInstance.HitPenetration;
			bodyShapeHit.IsPlatformHit = rInstance.IsPlatformHit;
			bodyShapeHit.Hit = rInstance.Hit;
			return bodyShapeHit;
		}

		public static void Release(BodyShapeHit rInstance)
		{
			if (rInstance != null)
			{
				rInstance.Shape = null;
				rInstance.StartPosition = Vector3.zero;
				rInstance.EndPosition = Vector3.zero;
				rInstance.HitCollider = null;
				rInstance.HitOrigin = Vector3.zero;
				rInstance.HitPoint = Vector3.zero;
				rInstance.HitNormal = Vector3.zero;
				rInstance.HitDistance = 0f;
				rInstance.HitPenetration = false;
				rInstance.Hit = RaycastExt.EmptyHitInfo;
				sPool.Release(rInstance);
			}
		}
	}
}
