using com.ootii.Data.Serializers;
using com.ootii.Geometry;
using com.ootii.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace com.ootii.Actors
{
	public abstract class BodyShape
	{
		public const float EPSILON = 0.001f;

		public string _Name = "";

		public bool _UseUnityColliders = true;

		public bool _IsEnabledOnGround = true;

		public bool _IsEnabledOnSlope = true;

		public bool _IsEnabledAboveGround = true;

		[NonSerialized]
		public Transform _Parent;

		[NonSerialized]
		public ICharacterController _CharacterController;

		[NonSerialized]
		public Transform _Transform;

		public Vector3 _Offset = Vector3.zero;

		public float _Radius = 0.25f;

		protected Collider[] mColliders;

		protected RaycastHit[] mRaycastHitArray = new RaycastHit[15];

		protected BodyShapeHit[] mBodyShapeHitArray = new BodyShapeHit[15];

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public bool UseUnityColliders
		{
			get
			{
				return _UseUnityColliders;
			}
			set
			{
				_UseUnityColliders = value;
				if (_UseUnityColliders)
				{
					if (mColliders == null || mColliders.Length == 0)
					{
						CreateUnityColliders();
					}
				}
				else if (mColliders != null)
				{
					DestroyUnityColliders();
				}
			}
		}

		public bool IsEnabledOnGround
		{
			get
			{
				return _IsEnabledOnGround;
			}
			set
			{
				_IsEnabledOnGround = value;
			}
		}

		public bool IsEnabledOnSlope
		{
			get
			{
				return _IsEnabledOnSlope;
			}
			set
			{
				_IsEnabledOnSlope = value;
			}
		}

		public bool IsEnabledAboveGround
		{
			get
			{
				return _IsEnabledAboveGround;
			}
			set
			{
				_IsEnabledAboveGround = value;
			}
		}

		[SerializationIgnore]
		public Transform Parent => _Parent;

		[SerializationIgnore]
		public ICharacterController CharacterController => _CharacterController;

		[SerializationIgnore]
		public virtual Transform Transform
		{
			get
			{
				return _Transform;
			}
			set
			{
				if (!(_Transform == value))
				{
					_Transform = value;
					if (_UseUnityColliders && mColliders != null)
					{
						CreateUnityColliders();
					}
				}
			}
		}

		public virtual Vector3 Offset
		{
			get
			{
				return _Offset;
			}
			set
			{
				_Offset = value;
			}
		}

		public virtual float Radius
		{
			get
			{
				return _Radius;
			}
			set
			{
				_Radius = value;
			}
		}

		[SerializationIgnore]
		public virtual Collider[] Colliders
		{
			get
			{
				return mColliders;
			}
			set
			{
				mColliders = value;
			}
		}

		[SerializationIgnore]
		public virtual Collider Collider
		{
			get
			{
				if (mColliders == null || mColliders.Length == 0)
				{
					return null;
				}
				return mColliders[0];
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

		public virtual void LateUpdate()
		{
		}

		public virtual List<BodyShapeHit> CollisionOverlap(Vector3 rPositionDelta, Quaternion rRotationDelta, int rLayerMask)
		{
			return null;
		}

		public virtual BodyShapeHit[] CollisionCastAll(Vector3 rPositionDelta, Vector3 rDirection, float rDistance, int rLayerMask, float rMaxStepHeight = 0f)
		{
			return null;
		}

		public virtual Vector3 ClosestPoint(Vector3 rOrigin)
		{
			Transform transform = (_Transform != null) ? _Transform : _Parent;
			Vector3 rPosition = transform.position + transform.rotation * _Offset;
			return GeometryExt.ClosestPoint(rOrigin, rPosition, _Radius);
		}

		public virtual bool ClosestPoint(Collider rCollider, Vector3 rMovement, bool rProcessTerrain, out Vector3 rShapePoint, out Vector3 rContactPoint)
		{
			Transform transform = (_Transform != null) ? _Transform : _Parent;
			rShapePoint = transform.position + transform.rotation * _Offset;
			rContactPoint = Vector3.zero;
			return false;
		}

		public virtual Vector3 CalculateHitOrigin(Vector3 rHitPoint, Vector3 rStartPosition, Vector3 rEndPosition)
		{
			return GeometryExt.ClosestPoint(rHitPoint, rStartPosition, rEndPosition);
		}

		public virtual void CreateUnityColliders()
		{
		}

		public virtual void DestroyUnityColliders()
		{
			if (mColliders != null)
			{
				for (int i = 0; i < mColliders.Length; i++)
				{
					Collider obj = mColliders[i];
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(obj);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(obj);
					}
				}
			}
			mColliders = null;
		}

		public virtual string Serialize()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(", \"Type\" : \"" + GetType().AssemblyQualifiedName + "\"");
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.CanWrite || propertyInfo.GetValue(this, null) == null)
				{
					continue;
				}
				object value = propertyInfo.GetValue(this, null);
				if (propertyInfo.PropertyType == typeof(Vector2))
				{
					stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + ((Vector2)value).Serialize() + "\"");
				}
				else if (propertyInfo.PropertyType == typeof(Vector3))
				{
					stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + ((Vector3)value).Serialize() + "\"");
				}
				else if (propertyInfo.PropertyType == typeof(Vector4))
				{
					stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + ((Vector4)value).Serialize() + "\"");
				}
				else if (propertyInfo.PropertyType == typeof(Transform))
				{
					string text = "";
					Transform transform = (Transform)value;
					while (transform != null)
					{
						if (transform.parent != null)
						{
							text = transform.name + ((text.Length > 0) ? "/" : "") + text;
						}
						transform = transform.parent;
						if (transform == _Parent)
						{
							break;
						}
					}
					if (text.Length == 0)
					{
						text = ".";
					}
					stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + text + "\"");
				}
				else
				{
					stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + value.ToString() + "\"");
				}
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public virtual void Deserialize(string rDefinition)
		{
			JSONNode jSONNode = JSONNode.Parse(rDefinition);
			if (jSONNode == null)
			{
				return;
			}
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.CanWrite)
				{
					continue;
				}
				JSONNode jSONNode2 = jSONNode[propertyInfo.Name];
				if (jSONNode2 == null)
				{
					if (propertyInfo.PropertyType == typeof(string))
					{
						propertyInfo.SetValue(this, "", null);
					}
				}
				else if (propertyInfo.PropertyType == typeof(string))
				{
					propertyInfo.SetValue(this, jSONNode2.Value, null);
				}
				else if (propertyInfo.PropertyType == typeof(int))
				{
					propertyInfo.SetValue(this, jSONNode2.AsInt, null);
				}
				else if (propertyInfo.PropertyType == typeof(float))
				{
					propertyInfo.SetValue(this, jSONNode2.AsFloat, null);
				}
				else if (propertyInfo.PropertyType == typeof(bool))
				{
					propertyInfo.SetValue(this, jSONNode2.AsBool, null);
				}
				else if (propertyInfo.PropertyType == typeof(Vector2))
				{
					Vector2 zero = Vector2.zero;
					zero = zero.FromString(jSONNode2.Value);
					propertyInfo.SetValue(this, zero, null);
				}
				else if (propertyInfo.PropertyType == typeof(Vector3))
				{
					Vector3 zero2 = Vector3.zero;
					zero2 = zero2.FromString(jSONNode2.Value);
					propertyInfo.SetValue(this, zero2, null);
				}
				else if (propertyInfo.PropertyType == typeof(Vector4))
				{
					Vector4 zero3 = Vector4.zero;
					zero3 = zero3.FromString(jSONNode2.Value);
					propertyInfo.SetValue(this, zero3, null);
				}
				else
				{
					if (!(propertyInfo.PropertyType == typeof(Transform)) || !(_Parent != null))
					{
						continue;
					}
					if (jSONNode2.Value == ".")
					{
						propertyInfo.SetValue(this, _Parent, null);
						continue;
					}
					Transform transform = _Parent.Find(jSONNode2.Value);
					if (transform != null)
					{
						propertyInfo.SetValue(this, transform, null);
					}
				}
			}
		}
	}
}
