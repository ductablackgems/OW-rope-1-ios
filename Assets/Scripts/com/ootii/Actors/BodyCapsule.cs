using com.ootii.Data.Serializers;
using com.ootii.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Actors
{
	[Serializable]
	public class BodyCapsule : BodyShape
	{
		public Transform _EndTransform;

		public Vector3 _EndOffset = Vector3.zero;

		protected SphereCollider mEndCollider;

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
					if (_UseUnityColliders && mColliders != null)
					{
						CreateUnityColliders();
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
				if (_Radius == value)
				{
					return;
				}
				_Radius = value;
				if (!_UseUnityColliders || mColliders == null)
				{
					return;
				}
				Transform transform = (_Transform != null) ? _Transform : _Parent;
				float num = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
				for (int i = 0; i < mColliders.Length; i++)
				{
					if (mColliders[i] is SphereCollider)
					{
						((SphereCollider)mColliders[i]).radius = _Radius / num;
					}
					else if (mColliders[i] is CapsuleCollider)
					{
						((CapsuleCollider)mColliders[i]).radius = _Radius / num;
					}
				}
			}
		}

		public Transform EndTransform
		{
			get
			{
				return _EndTransform;
			}
			set
			{
				if (!(_EndTransform == value))
				{
					_EndTransform = value;
					if (_UseUnityColliders && mColliders != null)
					{
						CreateUnityColliders();
					}
				}
			}
		}

		public Vector3 EndOffset
		{
			get
			{
				return _EndOffset;
			}
			set
			{
				if (!(_EndOffset == value))
				{
					_EndOffset = value;
					if (_UseUnityColliders && mColliders != null)
					{
						CreateUnityColliders();
					}
				}
			}
		}

		[SerializationIgnore]
		public new CapsuleCollider Collider
		{
			get
			{
				if (mColliders == null || mColliders.Length == 0)
				{
					return null;
				}
				return mColliders[0] as CapsuleCollider;
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

		[SerializationIgnore]
		public SphereCollider EndCollider
		{
			get
			{
				return mEndCollider;
			}
			set
			{
				mEndCollider = value;
			}
		}

		public override void LateUpdate()
		{
			if (mColliders != null && mColliders.Length != 0 && !(mColliders[0] is CapsuleCollider))
			{
				Transform transform = (_Transform != null) ? _Transform : _Parent;
				Transform transform2 = (_EndTransform != null) ? _EndTransform : _Parent;
				Vector3 position = transform.position + transform.rotation * _Offset;
				Vector3 position2 = transform2.position + transform2.rotation * _EndOffset;
				transform = _Parent;
				Vector3 vector = transform.InverseTransformPoint(position);
				Vector3 b = (transform.InverseTransformPoint(position2) - vector) / (mColliders.Length - 1);
				Vector3 vector2 = Vector3.zero;
				for (int i = 0; i < mColliders.Length; i++)
				{
					(mColliders[i] as SphereCollider).center = vector + vector2;
					vector2 += b;
				}
			}
		}

		public override List<BodyShapeHit> CollisionOverlap(Vector3 rPositionDelta, Quaternion rRotationDelta, int rLayerMask)
		{
			List<BodyShapeHit> list = new List<BodyShapeHit>();
			Vector3 vector = rPositionDelta + ((_Transform == null) ? (_Parent.position + _Parent.rotation * rRotationDelta * _Offset) : (_Transform.position + _Transform.rotation * rRotationDelta * _Offset));
			Vector3 vector2 = rPositionDelta + ((_EndTransform == null) ? (_Parent.position + _Parent.rotation * rRotationDelta * _EndOffset) : (_EndTransform.position + _EndTransform.rotation * rRotationDelta * _EndOffset));
			Vector3 rPosition = vector + (vector2 - vector) / 2f;
			float rRadius = Vector3.Distance(vector, vector2) / 2f + _Radius;
			GeometryExt.Ignore = _Parent;
			Collider[] rColliderArray = null;
			int num = RaycastExt.SafeOverlapSphere(rPosition, rRadius, out rColliderArray, rLayerMask, _Parent);
			for (int i = 0; i < num; i++)
			{
				Transform transform = rColliderArray[i].transform;
				if (transform == _Transform || transform == _EndTransform || (_CharacterController != null && _CharacterController.IsIgnoringCollision(rColliderArray[i])))
				{
					continue;
				}
				Vector3 rLinePoint = Vector3.zero;
				Vector3 rColliderPoint = Vector3.zero;
				GeometryExt.ClosestPoints(vector, vector2, _Radius, rColliderArray[i], ref rLinePoint, ref rColliderPoint, rLayerMask);
				if (rLinePoint != Vector3Ext.Null && rColliderPoint != Vector3Ext.Null)
				{
					float num2 = Vector3.Distance(rLinePoint, rColliderPoint);
					if (num2 < _Radius + 0.001f)
					{
						BodyShapeHit bodyShapeHit = BodyShapeHit.Allocate();
						bodyShapeHit.StartPosition = vector;
						bodyShapeHit.EndPosition = vector2;
						bodyShapeHit.HitCollider = rColliderArray[i];
						bodyShapeHit.HitOrigin = rLinePoint;
						bodyShapeHit.HitPoint = rColliderPoint;
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
			Vector3 b = (rMaxStepHeight >= _Radius) ? new Vector3(0f, rMaxStepHeight - _Radius + 0.01f, 0f) : Vector3.zero;
			Vector3 vector = rPositionDelta + ((_Transform == null) ? (_Parent.position + _Parent.rotation * (_Offset + b)) : (_Transform.position + _Transform.rotation * (_Offset + b)));
			Vector3 vector2 = rPositionDelta + ((_EndTransform == null) ? (_Parent.position + _Parent.rotation * _EndOffset) : (_EndTransform.position + _EndTransform.rotation * _EndOffset));
			for (int i = 0; i < mBodyShapeHitArray.Length; i++)
			{
				mBodyShapeHitArray[i] = null;
			}
			int num = 0;
			num = Physics.CapsuleCastNonAlloc(vector, vector2, _Radius, rDirection, mRaycastHitArray, rDistance + 0.001f, rLayerMask, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			for (int j = 0; j < num; j++)
			{
				if (mRaycastHitArray[j].collider.isTrigger)
				{
					continue;
				}
				Transform transform = mRaycastHitArray[j].collider.transform;
				if (transform == _Transform || transform == _EndTransform || (_CharacterController != null && _CharacterController.IsIgnoringCollision(mRaycastHitArray[j].collider)))
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
				bodyShapeHit.EndPosition = vector2;
				bodyShapeHit.Shape = this;
				bodyShapeHit.Hit = mRaycastHitArray[j];
				bodyShapeHit.HitCollider = mRaycastHitArray[j].collider;
				bodyShapeHit.HitPoint = mRaycastHitArray[j].point;
				bodyShapeHit.HitNormal = mRaycastHitArray[j].normal;
				bodyShapeHit.HitDistance = mRaycastHitArray[j].distance;
				if (mRaycastHitArray[j].distance == 0f)
				{
					Vector3 rLinePoint = Vector3.zero;
					Vector3 rColliderPoint = Vector3.zero;
					GeometryExt.ClosestPoints(vector, vector2, _Radius, bodyShapeHit.HitCollider, ref rLinePoint, ref rColliderPoint, rLayerMask);
					if (rColliderPoint == Vector3Ext.Null)
					{
						BodyShapeHit.Release(bodyShapeHit);
						continue;
					}
					Vector3 vector3 = rColliderPoint - rLinePoint;
					bodyShapeHit.HitOrigin = rLinePoint;
					bodyShapeHit.HitPoint = rColliderPoint;
					bodyShapeHit.HitDistance = vector3.magnitude - _Radius;
					bodyShapeHit.HitPenetration = (bodyShapeHit.HitDistance < 0f);
					if (RaycastExt.SafeRaycast(rLinePoint, vector3.normalized, out RaycastHit rHitInfo, Mathf.Max(bodyShapeHit.HitDistance + _Radius, _Radius + 0.01f)))
					{
						bodyShapeHit.HitNormal = rHitInfo.normal;
					}
					else if (bodyShapeHit.HitDistance < 0.001f)
					{
						bodyShapeHit.HitNormal = (rLinePoint - rColliderPoint).normalized;
					}
				}
				else
				{
					bodyShapeHit.CalculateHitOrigin();
				}
				if (bodyShapeHit != null)
				{
					if (rDirection != Vector3.down && RaycastExt.SafeRaycast(bodyShapeHit.HitPoint - rDirection * rDistance, rDirection, out RaycastHit rHitInfo2, rDistance + _Radius, -1, _Parent))
					{
						bodyShapeHit.HitNormal = rHitInfo2.normal;
					}
					bodyShapeHit.HitNormal = (bodyShapeHit.HitOrigin - bodyShapeHit.HitPoint).normalized;
					bodyShapeHit.HitRootDistance = _Parent.InverseTransformPoint(bodyShapeHit.HitPoint).y;
					mBodyShapeHitArray[num2] = bodyShapeHit;
					num2++;
				}
			}
			return mBodyShapeHitArray;
		}

		public override Vector3 ClosestPoint(Vector3 rOrigin)
		{
			Transform transform = (_Transform != null) ? _Transform : _Parent;
			Vector3 rLineStart = transform.position + transform.rotation * _Offset;
			Transform transform2 = (_EndTransform != null) ? _EndTransform : _Parent;
			Vector3 rLineEnd = transform2.position + transform2.rotation * _EndOffset;
			return GeometryExt.ClosestPoint(rOrigin, rLineStart, rLineEnd, _Radius);
		}

		public override bool ClosestPoint(Collider rCollider, Vector3 rMovement, bool rProcessTerrain, out Vector3 rShapePoint, out Vector3 rContactPoint)
		{
			Transform transform = (_Transform != null) ? _Transform : _Parent;
			Vector3 vector = transform.position + transform.rotation * _Offset;
			Transform transform2 = (_EndTransform != null) ? _EndTransform : _Parent;
			Vector3 rEnd = transform2.position + transform2.rotation * _EndOffset;
			rShapePoint = vector;
			rContactPoint = Vector3.zero;
			GeometryExt.ClosestPoints(vector, rEnd, _Radius, rCollider, ref rShapePoint, ref rContactPoint);
			return rContactPoint != Vector3Ext.Null;
		}

		public override void CreateUnityColliders()
		{
			if (_Parent == null)
			{
				return;
			}
			DestroyUnityColliders();
			Transform transform = (_Transform != null) ? _Transform : _Parent;
			Transform transform2 = (_EndTransform != null) ? _EndTransform : _Parent;
			float num = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
			int num2 = DetermineDirection();
			if (transform == transform2 && num2 > -1)
			{
				CapsuleCollider capsuleCollider = transform.gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.radius = _Radius / num;
				capsuleCollider.center = (_Offset + (_EndOffset - _Offset) * 0.5f) / num;
				capsuleCollider.height = (Vector3.Distance(_Offset, _EndOffset) + _Radius * 2f) / num;
				capsuleCollider.direction = num2;
				if (mColliders == null || mColliders.Length == 0)
				{
					mColliders = new Collider[1];
				}
				mColliders[0] = capsuleCollider;
				return;
			}
			Vector3 position = transform.position + transform.rotation * _Offset;
			Vector3 position2 = transform2.position + transform2.rotation * _EndOffset;
			transform = _Parent;
			Vector3 vector = transform.InverseTransformPoint(position);
			Vector3 vector2 = transform.InverseTransformPoint(position2);
			if (vector2 == vector)
			{
				if (mColliders == null || mColliders.Length != 1)
				{
					mColliders = new Collider[1];
				}
				SphereCollider sphereCollider = transform.gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = _Radius / num;
				sphereCollider.center = vector;
				mColliders[0] = sphereCollider;
				return;
			}
			Vector3 vector3 = vector2 - vector;
			int num3 = (int)(vector3.magnitude / (_Radius / num));
			if (mColliders == null || mColliders.Length != num3)
			{
				mColliders = new Collider[num3 + 2];
			}
			SphereCollider sphereCollider2 = transform.gameObject.AddComponent<SphereCollider>();
			sphereCollider2.radius = _Radius / num;
			sphereCollider2.center = vector;
			mColliders[0] = sphereCollider2;
			SphereCollider sphereCollider3 = transform.gameObject.AddComponent<SphereCollider>();
			sphereCollider3.radius = _Radius / num;
			sphereCollider3.center = vector2;
			mColliders[mColliders.Length - 1] = sphereCollider3;
			for (int i = 1; i < mColliders.Length - 1; i++)
			{
				SphereCollider sphereCollider4 = transform.gameObject.AddComponent<SphereCollider>();
				sphereCollider4.radius = _Radius / num;
				sphereCollider4.center = vector + vector3.normalized * (_Radius * (float)i);
				mColliders[i] = sphereCollider4;
			}
		}

		private int DetermineDirection()
		{
			int result = -1;
			if (_Offset.sqrMagnitude == 0f)
			{
				if (_EndOffset.normalized == Vector3.right)
				{
					result = 0;
				}
				else if (_EndOffset.normalized == Vector3.up)
				{
					result = 1;
				}
				else if (_EndOffset.normalized == Vector3.forward)
				{
					result = 2;
				}
			}
			else if (_EndOffset.sqrMagnitude == 0f)
			{
				if (_Offset.normalized == Vector3.right)
				{
					result = 0;
				}
				else if (_Offset.normalized == Vector3.up)
				{
					result = 1;
				}
				else if (_Offset.normalized == Vector3.forward)
				{
					result = 2;
				}
			}
			else if (_Offset.normalized == Vector3.right && _EndOffset.normalized == Vector3.right)
			{
				result = 0;
			}
			else if (_Offset.normalized == Vector3.up && _EndOffset.normalized == Vector3.up)
			{
				result = 1;
			}
			else if (_Offset.normalized == Vector3.forward && _EndOffset.normalized == Vector3.forward)
			{
				result = 2;
			}
			return result;
		}
	}
}
