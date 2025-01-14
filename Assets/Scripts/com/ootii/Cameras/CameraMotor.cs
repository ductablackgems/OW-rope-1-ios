using com.ootii.Base;
using com.ootii.Data.Serializers;
using com.ootii.Geometry;
using com.ootii.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace com.ootii.Cameras
{
	public abstract class CameraMotor : BaseObject
	{
		public string _Type = "";

		public bool _IsEnabled = true;

		public float _Priority;

		public string _ActionAlias = "";

		[NonSerialized]
		public bool _IsActive;

		public bool _UseRigAnchor = true;

		public int _AnchorIndex = -1;

		[NonSerialized]
		public Transform _Anchor;

		public Vector3 _AnchorOffset = Vector3.zero;

		public Vector3 _Offset = Vector3.zero;

		public bool _IsCollisionEnabled = true;

		public bool _IsFadingEnabled = true;

		public bool _SpecifyFadeRenderers;

		public List<int> _FadeTransformIndexes = new List<int>();

		[NonSerialized]
		public CameraController RigController;

		[NonSerialized]
		protected CameraTransform mRigTransform;

		protected float mAnchorOffsetDistance;

		public virtual string Type
		{
			get
			{
				return _Type;
			}
			set
			{
				_Type = value;
			}
		}

		public virtual bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
			}
		}

		public float Priority
		{
			get
			{
				return _Priority;
			}
			set
			{
				_Priority = value;
			}
		}

		public string ActionAlias
		{
			get
			{
				return _ActionAlias;
			}
			set
			{
				_ActionAlias = value;
			}
		}

		public virtual bool IsActive => _IsActive;

		public bool UseRigAnchor
		{
			get
			{
				return _UseRigAnchor;
			}
			set
			{
				_UseRigAnchor = value;
			}
		}

		public virtual int AnchorIndex
		{
			get
			{
				return _AnchorIndex;
			}
			set
			{
				_AnchorIndex = value;
				if (_AnchorIndex >= 0 && RigController != null && _AnchorIndex < RigController.StoredTransforms.Count)
				{
					_Anchor = RigController.StoredTransforms[_AnchorIndex];
				}
			}
		}

		public virtual Transform Anchor
		{
			get
			{
				if (_UseRigAnchor)
				{
					if (RigController != null)
					{
						return RigController._Anchor;
					}
					return null;
				}
				return _Anchor;
			}
			set
			{
				_Anchor = value;
			}
		}

		public virtual Transform AnchorRoot
		{
			get
			{
				Transform transform = null;
				if (_UseRigAnchor)
				{
					if (RigController != null)
					{
						transform = RigController._Anchor;
					}
				}
				else
				{
					transform = _Anchor;
				}
				if (transform != null)
				{
					IBaseCameraAnchor component = transform.gameObject.GetComponent<IBaseCameraAnchor>();
					if (component != null)
					{
						transform = component.Root;
					}
				}
				return transform;
			}
		}

		public virtual Vector3 AnchorOffset
		{
			get
			{
				if (_UseRigAnchor)
				{
					if (RigController != null)
					{
						return RigController._AnchorOffset;
					}
					return Vector3.zero;
				}
				return _AnchorOffset;
			}
			set
			{
				_AnchorOffset = value;
			}
		}

		public virtual Vector3 AnchorPosition
		{
			get
			{
				Transform anchor = Anchor;
				if (anchor == null)
				{
					return AnchorOffset;
				}
				if (RigController.RotateAnchorOffset)
				{
					return anchor.position + anchor.rotation * AnchorOffset;
				}
				return anchor.position + AnchorOffset;
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

		public virtual float MaxDistance
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public virtual float Distance
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public virtual bool IsCollisionEnabled
		{
			get
			{
				return _IsCollisionEnabled;
			}
			set
			{
				_IsCollisionEnabled = value;
			}
		}

		public virtual bool IsFadingEnabled
		{
			get
			{
				return _IsFadingEnabled;
			}
			set
			{
				_IsFadingEnabled = value;
			}
		}

		public bool SpecifyFadeRenderers
		{
			get
			{
				return _SpecifyFadeRenderers;
			}
			set
			{
				_SpecifyFadeRenderers = value;
			}
		}

		public List<int> FadeTransformIndexes
		{
			get
			{
				return _FadeTransformIndexes;
			}
			set
			{
				_FadeTransformIndexes = value;
			}
		}

		public virtual void Awake()
		{
			if (RigController != null && RigController.Camera != null)
			{
				mRigTransform.FieldOfView = RigController.Camera.fieldOfView;
			}
			mAnchorOffsetDistance = AnchorOffset.magnitude;
		}

		public virtual bool Initialize()
		{
			mAnchorOffsetDistance = AnchorOffset.magnitude;
			return true;
		}

		public virtual bool TestActivate(CameraMotor rActiveMotor)
		{
			if (!_IsEnabled)
			{
				return false;
			}
			if (_ActionAlias.Length > 0 && RigController.InputSource != null && RigController.InputSource.IsJustPressed(_ActionAlias))
			{
				return true;
			}
			return false;
		}

		public virtual void Activate(CameraMotor rOldMotor)
		{
			if (Initialize())
			{
				_IsActive = true;
			}
		}

		public virtual void Deactivate(CameraMotor rNewMotor)
		{
			_IsActive = false;
		}

		public virtual void Clear()
		{
			Anchor = null;
			_FadeTransformIndexes.Clear();
		}

		public virtual CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			return mRigTransform;
		}

		public virtual void PostRigLateUpdate()
		{
			mAnchorOffsetDistance = Mathf.Min(mAnchorOffsetDistance + 1f * Time.deltaTime, AnchorOffset.magnitude);
		}

		public virtual Vector3 GetFocusPosition(Quaternion rCameraRotation)
		{
			Transform anchor = Anchor;
			Vector3 anchorOffset = AnchorOffset;
			Vector3 anchorPosition = AnchorPosition;
			Vector3 vector = rCameraRotation.Right() * _Offset.x + rCameraRotation.Forward() * _Offset.z;
			if (anchor != null)
			{
				if (RigController.IsCollisionsEnabled && _IsCollisionEnabled && anchorOffset.sqrMagnitude > 0f)
				{
					Vector3 vector2 = anchorPosition - anchor.position;
					Vector3 normalized = vector2.normalized;
					float magnitude = vector2.magnitude;
					float num = magnitude * 0.5f;
					if (RaycastExt.SafeSphereCast(anchor.position + normalized * num, normalized, RigController.CollisionRadius * 1.1f, out RaycastHit rHitInfo, magnitude - num, RigController.CollisionLayers, AnchorRoot) && rHitInfo.distance > 0f)
					{
						mAnchorOffsetDistance = rHitInfo.distance + num;
					}
					anchorOffset = normalized * mAnchorOffsetDistance;
					vector = anchor.position + anchorOffset + vector;
				}
				else
				{
					vector = anchor.position + (RigController.RotateAnchorOffset ? anchor.rotation : Quaternion.identity) * anchorOffset + vector;
				}
				vector += (RigController.RotateAnchorOffset ? anchor.up : Vector3.up) * _Offset.y;
				if (RigController.IsCollisionsEnabled && _IsCollisionEnabled && Offset.sqrMagnitude > 0f)
				{
					Vector3 vector3 = vector - anchorPosition;
					Vector3 normalized2 = vector3.normalized;
					float magnitude2 = vector3.magnitude;
					if (RaycastExt.SafeSphereCast(anchorPosition, normalized2, RigController.CollisionRadius * 1.1f, out RaycastHit rHitInfo2, magnitude2, RigController.CollisionLayers, AnchorRoot))
					{
						vector = anchorPosition + normalized2 * rHitInfo2.distance;
					}
				}
			}
			return vector;
		}

		public void NormalizeEuler(ref Vector3 rEuler)
		{
			if (rEuler.x < -180f)
			{
				rEuler.x += 360f;
			}
			else if (rEuler.x > 180f)
			{
				rEuler.x -= 360f;
			}
			if (rEuler.y < -180f)
			{
				rEuler.y += 360f;
			}
			else if (rEuler.y > 180f)
			{
				rEuler.y -= 360f;
			}
		}

		public bool IsFadeRenderer(Transform rTransform)
		{
			if (!IsFadingEnabled)
			{
				return false;
			}
			if (!SpecifyFadeRenderers)
			{
				return false;
			}
			for (int i = 0; i < _FadeTransformIndexes.Count; i++)
			{
				int num = _FadeTransformIndexes[i];
				if (num >= 0 && num < RigController.StoredTransforms.Count && RigController.StoredTransforms[num] == rTransform)
				{
					return true;
				}
			}
			return false;
		}

		public Transform GetFadeRenderer(int rIndex)
		{
			if (rIndex < 0 || rIndex >= _FadeTransformIndexes.Count)
			{
				return null;
			}
			int num = _FadeTransformIndexes[rIndex];
			if (RigController.StoredTransforms == null)
			{
				return null;
			}
			if (num < 0 || num >= RigController.StoredTransforms.Count)
			{
				return null;
			}
			return RigController.StoredTransforms[num];
		}

		public void AddFadeRenderer(Transform rTransform)
		{
			int num = -1;
			for (int i = 0; i < RigController.StoredTransforms.Count; i++)
			{
				if (RigController.StoredTransforms[i] == rTransform)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				RigController.StoredTransforms.Add(rTransform);
				num = RigController.StoredTransforms.Count - 1;
			}
			if (!_FadeTransformIndexes.Contains(num))
			{
				_FadeTransformIndexes.Add(num);
			}
		}

		public void SetFadeRenderer(int rIndex, Transform rTransform)
		{
			int num = -1;
			while (rIndex >= _FadeTransformIndexes.Count)
			{
				_FadeTransformIndexes.Add(-1);
			}
			for (int i = 0; i < RigController.StoredTransforms.Count; i++)
			{
				if (RigController.StoredTransforms[i] == rTransform)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				RigController.StoredTransforms.Add(rTransform);
				num = RigController.StoredTransforms.Count - 1;
			}
			_FadeTransformIndexes[rIndex] = num;
		}

		public void RemoveFadeRenderer(Transform rTransform)
		{
			int num = -1;
			for (int i = 0; i < RigController.StoredTransforms.Count; i++)
			{
				if (RigController.StoredTransforms[i] == rTransform)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				_FadeTransformIndexes.Remove(num);
			}
		}

		public void RemoveFadeRenderer(int rIndex)
		{
			if (rIndex >= 0 && rIndex < _FadeTransformIndexes.Count)
			{
				_FadeTransformIndexes.RemoveAt(rIndex);
			}
		}

		public virtual string SerializeMotor()
		{
			if (_Type.Length == 0)
			{
				_Type = GetType().AssemblyQualifiedName;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(", \"Type\" : \"" + _Type + "\"");
			stringBuilder.Append(", \"Name\" : \"" + _Name + "\"");
			stringBuilder.Append(", \"IsEnabled\" : \"" + _IsEnabled.ToString() + "\"");
			stringBuilder.Append(", \"ActionAlias\" : \"" + _ActionAlias.ToString() + "\"");
			stringBuilder.Append(", \"UseRigAnchor\" : \"" + _UseRigAnchor.ToString() + "\"");
			stringBuilder.Append(", \"AnchorIndex\" : \"" + _AnchorIndex.ToString() + "\"");
			stringBuilder.Append(", \"AnchorOffset\" : \"" + _AnchorOffset.Serialize() + "\"");
			stringBuilder.Append(", \"Offset\" : \"" + _Offset.Serialize() + "\"");
			stringBuilder.Append(", \"Distance\" : \"" + Distance.ToString("f5", CultureInfo.InvariantCulture) + "\"");
			stringBuilder.Append(", \"MaxDistance\" : \"" + MaxDistance.ToString("f5", CultureInfo.InvariantCulture) + "\"");
			stringBuilder.Append(", \"IsCollisionEnabled\" : \"" + _IsCollisionEnabled.ToString() + "\"");
			stringBuilder.Append(", \"IsFadingEnabled\" : \"" + _IsFadingEnabled.ToString() + "\"");
			stringBuilder.Append(", \"SpecifyFadeRenderers\" : \"" + _SpecifyFadeRenderers.ToString() + "\"");
			stringBuilder.Append(", \"FadeTransformIndexes\" : \"" + string.Join(",", (from n in _FadeTransformIndexes
				select n.ToString()).ToArray()) + "\"");
			PropertyInfo[] properties = typeof(CameraMotor).GetProperties();
			PropertyInfo[] properties2 = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties2)
			{
				if (!propertyInfo.CanWrite)
				{
					continue;
				}
				bool flag = true;
				for (int j = 0; j < properties.Length; j++)
				{
					if (propertyInfo.Name == properties[j].Name)
					{
						flag = false;
						break;
					}
				}
				if (!flag || propertyInfo.GetValue(this, null) == null)
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
				else if (propertyInfo.PropertyType == typeof(Quaternion))
				{
					stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + ((Quaternion)value).Serialize() + "\"");
				}
				else
				{
					if (propertyInfo.PropertyType == typeof(Transform))
					{
						continue;
					}
					if (propertyInfo.PropertyType == typeof(List<int>))
					{
						List<int> source = value as List<int>;
						stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + string.Join(",", (from n in source
							select n.ToString(CultureInfo.InvariantCulture)).ToArray()) + "\"");
					}
					else if (propertyInfo.PropertyType == typeof(AnimationCurve))
					{
						stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"");
						AnimationCurve animationCurve = value as AnimationCurve;
						for (int k = 0; k < animationCurve.keys.Length; k++)
						{
							Keyframe keyframe = animationCurve.keys[k];
							stringBuilder.Append(keyframe.time.ToString("f5", CultureInfo.InvariantCulture) + "|" + keyframe.value.ToString("f5", CultureInfo.InvariantCulture) + "|" + keyframe.tangentMode.ToString(CultureInfo.InvariantCulture) + "|" + keyframe.inTangent.ToString("f5", CultureInfo.InvariantCulture) + "|" + keyframe.outTangent.ToString("f5", CultureInfo.InvariantCulture));
							if (k < animationCurve.keys.Length - 1)
							{
								stringBuilder.Append(";");
							}
						}
						stringBuilder.Append("\"");
					}
					else
					{
						stringBuilder.Append(", \"" + propertyInfo.Name + "\" : \"" + value.ToString() + "\"");
					}
				}
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public virtual void DeserializeMotor(string rDefinition)
		{
			JSONNode jSONNode = JSONNode.Parse(rDefinition);
			if (jSONNode == null)
			{
				return;
			}
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.CanWrite || propertyInfo.GetValue(this, null) == null)
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
				else if (propertyInfo.PropertyType == typeof(Quaternion))
				{
					Quaternion identity = Quaternion.identity;
					identity = identity.FromString(jSONNode2.Value);
					propertyInfo.SetValue(this, identity, null);
				}
				else
				{
					if (propertyInfo.PropertyType == typeof(Transform))
					{
						continue;
					}
					if (propertyInfo.PropertyType == typeof(List<int>))
					{
						if (jSONNode2.Value.Length > 0)
						{
							List<int> value = (from x in jSONNode2.Value.Split(',')
								select int.Parse(x, NumberStyles.Integer, CultureInfo.InvariantCulture)).ToList();
							propertyInfo.SetValue(this, value, null);
						}
					}
					else
					{
						if (!(propertyInfo.PropertyType == typeof(AnimationCurve)) || jSONNode2.Value.Length <= 0)
						{
							continue;
						}
						AnimationCurve animationCurve = new AnimationCurve();
						string[] array = jSONNode2.Value.Split(';');
						for (int j = 0; j < array.Length; j++)
						{
							string[] array2 = array[j].Split('|');
							if (array2.Length == 5)
							{
								int result = 0;
								float result2 = 0f;
								Keyframe key = default(Keyframe);
								if (float.TryParse(array2[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
								{
									key.time = result2;
								}
								if (float.TryParse(array2[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
								{
									key.value = result2;
								}
								if (int.TryParse(array2[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
								{
									key.tangentMode = result;
								}
								if (float.TryParse(array2[3], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
								{
									key.inTangent = result2;
								}
								if (float.TryParse(array2[4], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
								{
									key.outTangent = result2;
								}
								animationCurve.AddKey(key);
							}
						}
						propertyInfo.SetValue(this, animationCurve, null);
					}
				}
			}
			if (_AnchorIndex >= 0)
			{
				if (_AnchorIndex < RigController.StoredTransforms.Count)
				{
					_Anchor = RigController.StoredTransforms[_AnchorIndex];
					return;
				}
				_Anchor = null;
				_AnchorIndex = -1;
			}
		}
	}
}
