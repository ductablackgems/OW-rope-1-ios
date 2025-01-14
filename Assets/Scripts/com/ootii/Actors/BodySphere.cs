using com.ootii.Data.Serializers;
using com.ootii.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Actors
{
	[Serializable]
	public class BodySphere : BodyShape
	{
		public override Vector3 Offset
		{
			get
			{
				return _Offset;
			}
			set
			{
				if (!(_Offset == value))
				{
					_Offset = value;
					if (mColliders != null && Collider != null)
					{
						Transform transform = (_Transform != null) ? _Transform : _Parent;
						float d = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
						Collider.center = _Offset / d;
					}
				}
			}
		}

		public override float Radius
		{
			get
			{
				return _Radius;
			}
			set
			{
				if (_Radius != value)
				{
					_Radius = value;
					if (mColliders != null && Collider != null)
					{
						Transform transform = (_Transform != null) ? _Transform : _Parent;
						float num = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
						Collider.radius = _Radius / num;
					}
				}
			}
		}

		[SerializationIgnore]
		public new SphereCollider Collider
		{
			get
			{
				if (mColliders == null || mColliders.Length == 0)
				{
					return null;
				}
				return mColliders[0] as SphereCollider;
			}
			set
			{
				if (mColliders == null || mColliders.Length == 0)
				{
					mColliders = new Collider[1];
				}
				mColliders[0] = value;
			}
		}

		public override List<BodyShapeHit> CollisionOverlap(Vector3 rPositionDelta, Quaternion rRotationDelta, int rLayerMask)
		{
			List<BodyShapeHit> list = new List<BodyShapeHit>();
			Vector3 vector = rPositionDelta + ((_Transform == null) ? (_Parent.position + _Parent.rotation * rRotationDelta * _Offset) : (_Transform.position + _Transform.rotation * rRotationDelta * _Offset));
			GeometryExt.Ignore = _Parent;
			Collider[] rColliderArray = null;
			int num = RaycastExt.SafeOverlapSphere(vector, _Radius, out rColliderArray, rLayerMask, _Parent);
			for (int i = 0; i < num; i++)
			{
				if (rColliderArray[i].transform == _Transform || (_CharacterController != null && _CharacterController.IsIgnoringCollision(rColliderArray[i])))
				{
					continue;
				}
				Vector3 vector2 = vector;
				Vector3 vector3 = GeometryExt.ClosestPoint(vector, _Radius, rColliderArray[i], rLayerMask);
				if (vector3 != Vector3Ext.Null)
				{
					float num2 = Vector3.Distance(vector2, vector3);
					if (num2 < _Radius + 0.001f)
					{
						BodyShapeHit bodyShapeHit = BodyShapeHit.Allocate();
						bodyShapeHit.StartPosition = vector;
						bodyShapeHit.HitCollider = rColliderArray[i];
						bodyShapeHit.HitOrigin = vector2;
						bodyShapeHit.HitPoint = vector3;
						bodyShapeHit.HitDistance = num2 - _Radius - 0.001f;
						list.Add(bodyShapeHit);
					}
				}
			}
			GeometryExt.Ignore = null;
			GeometryExt.IgnoreArray = null;
			return list;
		}

		public override BodyShapeHit[] CollisionCastAll(Vector3 rPositionDelta, Vector3 rDirection, float rDistance, int rLayerMask, float rMaxStepHeight = 0f)
		{
			Vector3 vector = rPositionDelta + ((_Transform == null) ? (_Parent.position + _Parent.rotation * _Offset) : (_Transform.position + _Transform.rotation * _Offset));
			for (int i = 0; i < mBodyShapeHitArray.Length; i++)
			{
				mBodyShapeHitArray[i] = null;
			}
			int num = 0;
			num = Physics.SphereCastNonAlloc(vector, _Radius, rDirection, mRaycastHitArray, rDistance + 0.001f, rLayerMask, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			for (int j = 0; j < num; j++)
			{
				if (mRaycastHitArray[j].collider.isTrigger || (_CharacterController != null && _CharacterController.IsIgnoringCollision(mRaycastHitArray[j].collider)))
				{
					continue;
				}
				Transform transform = mRaycastHitArray[j].collider.transform;
				if (transform == _Transform)
				{
					continue;
				}
				bool flag = true;
				while (transform != null)
				{
					if (transform == _Parent)
					{
						flag = false;
						break;
					}
					transform = transform.parent;
				}
				if (!flag)
				{
					continue;
				}
				BodyShapeHit bodyShapeHit = BodyShapeHit.Allocate();
				bodyShapeHit.StartPosition = vector;
				bodyShapeHit.Shape = this;
				bodyShapeHit.Hit = mRaycastHitArray[j];
				bodyShapeHit.HitOrigin = vector;
				bodyShapeHit.HitCollider = mRaycastHitArray[j].collider;
				bodyShapeHit.HitPoint = mRaycastHitArray[j].point;
				bodyShapeHit.HitNormal = mRaycastHitArray[j].normal;
				bodyShapeHit.HitDistance = mRaycastHitArray[j].distance;
				if (mRaycastHitArray[j].distance == 0f)
				{
					Vector3 zero = Vector3.zero;
					zero = GeometryExt.ClosestPoint(vector, _Radius, bodyShapeHit.HitCollider, rLayerMask);
					if (zero == Vector3Ext.Null)
					{
						BodyShapeHit.Release(bodyShapeHit);
						continue;
					}
					Vector3 vector2 = zero - vector;
					if (Mathf.Abs(vector2.magnitude - _Radius) < 0.001f && Vector3.Dot(vector2.normalized, rDirection) <= -0.8f)
					{
						BodyShapeHit.Release(bodyShapeHit);
						continue;
					}
					bodyShapeHit.HitOrigin = vector;
					bodyShapeHit.HitPoint = zero;
					bodyShapeHit.HitDistance = vector2.magnitude - _Radius;
					bodyShapeHit.HitPenetration = (bodyShapeHit.HitDistance < 0f);
					if (RaycastExt.SafeRaycast(vector, vector2.normalized, out RaycastHit rHitInfo, Mathf.Max(bodyShapeHit.HitDistance + _Radius, _Radius + 0.01f)))
					{
						bodyShapeHit.HitNormal = rHitInfo.normal;
					}
					else if (bodyShapeHit.HitDistance < 0.001f)
					{
						bodyShapeHit.HitNormal = (vector - zero).normalized;
					}
				}
				if (bodyShapeHit != null)
				{
					if (rDirection != Vector3.down && RaycastExt.SafeRaycast(bodyShapeHit.HitPoint - rDirection * rDistance, rDirection, out RaycastHit rHitInfo2, rDistance + _Radius, -1, _Parent))
					{
						bodyShapeHit.HitNormal = rHitInfo2.normal;
					}
					bodyShapeHit.HitRootDistance = _Parent.InverseTransformPoint(bodyShapeHit.HitPoint).y;
					mBodyShapeHitArray[num2] = bodyShapeHit;
					num2++;
				}
			}
			return mBodyShapeHitArray;
		}

		public override bool ClosestPoint(Collider rCollider, Vector3 rMovement, bool rProcessTerrain, out Vector3 rShapePoint, out Vector3 rContactPoint)
		{
			Transform transform = (_Transform != null) ? _Transform : _Parent;
			rShapePoint = transform.position + transform.rotation * _Offset;
			rContactPoint = Vector3.zero;
			rContactPoint = GeometryExt.ClosestPoint(rShapePoint, _Radius, rCollider);
			return rContactPoint != Vector3Ext.Null;
		}

		public override Vector3 CalculateHitOrigin(Vector3 rHitPoint, Vector3 rStartPosition, Vector3 rEndPosition)
		{
			return rStartPosition;
		}

		public override void CreateUnityColliders()
		{
			if (!(_Parent == null))
			{
				DestroyUnityColliders();
				Transform transform = (_Transform != null) ? _Transform : _Parent;
				float num = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
				SphereCollider sphereCollider = transform.gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = _Radius / num;
				sphereCollider.center = _Offset / num;
				if (mColliders == null || mColliders.Length == 0)
				{
					mColliders = new Collider[1];
				}
				mColliders[0] = sphereCollider;
			}
		}
	}
}
