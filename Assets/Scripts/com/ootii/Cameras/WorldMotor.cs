using com.ootii.Base;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Input;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("Top-Down View Motor")]
	[BaseDescription("Camera Motor for strategy games and MOBAs that allow for a top-down view.")]
	public class WorldMotor : YawPitchMotor
	{
		public Vector3 _MinBounds = new Vector3(-100f, -100f, -100f);

		public Vector3 _MaxBounds = new Vector3(100f, 100f, 100f);

		public bool _FollowGround;

		public float _GroundDistance;

		public float _GroundSmoothing = 0.05f;

		public int _GroundLayers = 1;

		public bool _FollowAnchor;

		public string _FollowAlias = "Camera Follow";

		public bool _FollowElevation = true;

		public bool _FollowFromView = true;

		public bool _AllowDisconnect = true;

		public bool _GripPan = true;

		public string _GripAlias = "Camera Grip";

		public float _GripPanSpeed = 50f;

		public bool _EdgePan = true;

		public float _EdgePanBorder = 20f;

		public float _EdgePanSpeed = 10f;

		public bool _InputPan = true;

		public bool _UseViewInput;

		public string _ForwardAlias = "Camera Forward";

		public string _BackAlias = "Camera Back";

		public string _LeftAlias = "Camera Left";

		public string _RightAlias = "Camera Right";

		public float _InputPanSpeed = 10f;

		protected bool mIsFollowConnected = true;

		public float mGripUnitsPerTick;

		public float mEdgeUnitsPerTick;

		public float mInputUnitsPerTick;

		protected Vector3 mPositionTarget = Vector3.zero;

		protected Vector3 mPositionVelocity = Vector3.zero;

		public Vector3 MinBounds
		{
			get
			{
				return _MinBounds;
			}
			set
			{
				_MinBounds = value;
			}
		}

		public Vector3 MaxBounds
		{
			get
			{
				return _MaxBounds;
			}
			set
			{
				_MaxBounds = value;
			}
		}

		public bool FollowGround
		{
			get
			{
				return _FollowGround;
			}
			set
			{
				_FollowGround = value;
			}
		}

		public float GroundDistance
		{
			get
			{
				return _GroundDistance;
			}
			set
			{
				_GroundDistance = value;
			}
		}

		public float GroundSmoothing
		{
			get
			{
				return _GroundSmoothing;
			}
			set
			{
				_GroundSmoothing = Mathf.Clamp(value, 0f, 0.8f);
			}
		}

		public int GroundLayers
		{
			get
			{
				return _GroundLayers;
			}
			set
			{
				_GroundLayers = value;
			}
		}

		public bool FollowAnchor
		{
			get
			{
				return _FollowAnchor;
			}
			set
			{
				_FollowAnchor = value;
			}
		}

		public string FollowAlias
		{
			get
			{
				return _FollowAlias;
			}
			set
			{
				_FollowAlias = value;
			}
		}

		public bool FollowElevation
		{
			get
			{
				return _FollowElevation;
			}
			set
			{
				_FollowElevation = value;
			}
		}

		public bool FollowFromView
		{
			get
			{
				return _FollowFromView;
			}
			set
			{
				_FollowFromView = value;
			}
		}

		public bool AllowDisconnect
		{
			get
			{
				return _AllowDisconnect;
			}
			set
			{
				_AllowDisconnect = value;
			}
		}

		public bool GripPan
		{
			get
			{
				return _GripPan;
			}
			set
			{
				_GripPan = value;
			}
		}

		public string GripAlias
		{
			get
			{
				return _GripAlias;
			}
			set
			{
				_GripAlias = value;
			}
		}

		public float GripPanSpeed
		{
			get
			{
				return _GripPanSpeed;
			}
			set
			{
				_GripPanSpeed = value;
			}
		}

		public bool EdgePan
		{
			get
			{
				return _EdgePan;
			}
			set
			{
				_EdgePan = value;
			}
		}

		public float EdgePanBorder
		{
			get
			{
				return _EdgePanBorder;
			}
			set
			{
				_EdgePanBorder = value;
			}
		}

		public float EdgePanSpeed
		{
			get
			{
				return _EdgePanSpeed;
			}
			set
			{
				_EdgePanSpeed = value;
			}
		}

		public bool InputPan
		{
			get
			{
				return _InputPan;
			}
			set
			{
				_InputPan = value;
			}
		}

		public bool UseViewInput
		{
			get
			{
				return _UseViewInput;
			}
			set
			{
				_UseViewInput = value;
			}
		}

		public string ForwardAlias
		{
			get
			{
				return _ForwardAlias;
			}
			set
			{
				_ForwardAlias = value;
			}
		}

		public string BackAlias
		{
			get
			{
				return _BackAlias;
			}
			set
			{
				_BackAlias = value;
			}
		}

		public string LeftAlias
		{
			get
			{
				return _LeftAlias;
			}
			set
			{
				_LeftAlias = value;
			}
		}

		public string RightAlias
		{
			get
			{
				return _RightAlias;
			}
			set
			{
				_RightAlias = value;
			}
		}

		public float InputPanSpeed
		{
			get
			{
				return _InputPanSpeed;
			}
			set
			{
				_InputPanSpeed = value;
			}
		}

		public bool IsFollowConnected
		{
			get
			{
				return mIsFollowConnected;
			}
			set
			{
				mIsFollowConnected = value;
			}
		}

		public WorldMotor()
		{
			_MaxDistance = 8f;
			mDistance = 8f;
		}

		public override bool Initialize()
		{
			base.Initialize();
			mGripUnitsPerTick = _GripPanSpeed * Time.deltaTime;
			mEdgeUnitsPerTick = _EdgePanSpeed * Time.deltaTime;
			mInputUnitsPerTick = _InputPanSpeed * Time.deltaTime;
			mRigTransform.Position = RigController.Transform.position;
			mRigTransform.Rotation = RigController.Transform.rotation;
			mPositionTarget = mRigTransform.Position;
			if (_MaxDistance == 0f)
			{
				MaxDistance = RigController._Transform.position.y - AnchorPosition.y;
			}
			if (_GroundDistance == 0f)
			{
				GroundDistance = GetGroundDistance();
			}
			return true;
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			bool flag = true;
			bool flag2 = true;
			Vector3 vector = Vector3.zero;
			IInputSource inputSource = RigController.InputSource;
			Quaternion rotation = Quaternion.Euler(0f, RigController._Transform.rotation.eulerAngles.y, 0f);
			if (_FollowAnchor && Anchor != null && vector.sqrMagnitude == 0f)
			{
				if (!_AllowDisconnect)
				{
					mIsFollowConnected = true;
				}
				else if (_FollowAlias.Length == 0)
				{
					mIsFollowConnected = true;
				}
				else if (_FollowAlias.Length > 0 && inputSource != null && inputSource.IsPressed(_FollowAlias))
				{
					mIsFollowConnected = true;
				}
				if (mIsFollowConnected)
				{
					flag = false;
					flag2 = false;
					Vector3 anchorPosition = AnchorPosition;
					if (_FollowFromView)
					{
						Vector3 vector2 = mPositionTarget;
						mPositionTarget = anchorPosition - RigController._Transform.forward * _MaxDistance;
						if (!_FollowElevation)
						{
							mPositionTarget.y = vector2.y;
						}
					}
					else
					{
						vector.x = anchorPosition.x - RigController._Transform.position.x;
						vector.y = (_FollowElevation ? (anchorPosition.y - RigController._Transform.position.y) : 0f);
						vector.z = anchorPosition.z - RigController._Transform.position.z;
					}
				}
			}
			else
			{
				mIsFollowConnected = false;
				mPositionTarget = RigController._Transform.position;
			}
			if (_GripPan && inputSource != null && vector.sqrMagnitude == 0f && (_GripAlias.Length == 0 || inputSource.IsPressed(_GripAlias)) && (inputSource.ViewX != 0f || inputSource.ViewY != 0f))
			{
				mIsFollowConnected = false;
				if (flag2)
				{
					flag2 = false;
					vector.x = inputSource.ViewX * (0f - mGripUnitsPerTick);
					vector.z = inputSource.ViewY * (0f - mGripUnitsPerTick);
					vector = rotation * vector;
				}
			}
			if (_InputPan && inputSource != null && vector.sqrMagnitude == 0f)
			{
				float num = (_ForwardAlias.Length > 0 && inputSource.IsPressed(_ForwardAlias)) ? 1f : 0f;
				num -= ((_BackAlias.Length > 0 && inputSource.IsPressed(_BackAlias)) ? 1f : 0f);
				float num2 = (_RightAlias.Length > 0 && inputSource.IsPressed(_RightAlias)) ? 1f : 0f;
				num2 -= ((_LeftAlias.Length > 0 && inputSource.IsPressed(_LeftAlias)) ? 1f : 0f);
				if (num2 != 0f || num != 0f)
				{
					mIsFollowConnected = false;
					if (flag2)
					{
						float rMagnitude = 1f;
						InputManagerHelper.ConvertToRadialInput(ref num2, ref num, ref rMagnitude);
						flag2 = false;
						vector.x = num2;
						vector.z = num;
						vector = rotation * vector * mInputUnitsPerTick;
					}
				}
			}
			if (_EdgePan && vector.sqrMagnitude == 0f)
			{
				Vector3 zero = Vector3.zero;
				float num3 = Screen.width;
				float num4 = Screen.height;
				Vector3 mousePosition = UnityEngine.Input.mousePosition;
				mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, num3 - 1f);
				mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, num4 - 1f);
				float num5 = (_EdgePanBorder > 1f) ? _EdgePanBorder : 1f;
				if (mousePosition.x < num5)
				{
					float num6 = (num5 - mousePosition.x) / num5;
					zero.x = (0f - mEdgeUnitsPerTick) * num6;
				}
				else if (mousePosition.x >= num3 - _EdgePanBorder)
				{
					float num7 = (num5 - (num3 - 1f - mousePosition.x)) / num5;
					zero.x = mEdgeUnitsPerTick * num7;
				}
				if (mousePosition.y < _EdgePanBorder)
				{
					float num8 = (num5 - mousePosition.y) / num5;
					zero.z = (0f - mEdgeUnitsPerTick) * num8;
				}
				else if (mousePosition.y >= num4 - _EdgePanBorder)
				{
					float num9 = (num5 - (num4 - 1f - mousePosition.y)) / num5;
					zero.z = mEdgeUnitsPerTick * num9;
				}
				if (zero.sqrMagnitude > 0f)
				{
					mIsFollowConnected = false;
					if (flag2)
					{
						flag2 = false;
						vector.x = zero.x;
						vector.z = zero.z;
						vector = rotation * vector;
					}
				}
			}
			mPositionTarget += vector;
			if (!mIsFollowConnected || !_FollowElevation)
			{
				Vector3 groundTarget = GetGroundTarget();
				if (groundTarget != Vector3Ext.Null)
				{
					mPositionTarget.y = SmoothDamp(mPositionTarget.y, groundTarget.y + _GroundDistance, _GroundSmoothing, rDeltaTime);
				}
			}
			if (_MinBounds.x != 0f || _MaxBounds.x != 0f)
			{
				mPositionTarget.x = Mathf.Clamp(mPositionTarget.x, _MinBounds.x, _MaxBounds.x);
			}
			if (_MinBounds.y != 0f || _MaxBounds.y != 0f)
			{
				mPositionTarget.y = Mathf.Clamp(mPositionTarget.y, _MinBounds.y, _MaxBounds.y);
			}
			if (_MinBounds.z != 0f || _MaxBounds.z != 0f)
			{
				mPositionTarget.z = Mathf.Clamp(mPositionTarget.z, _MinBounds.z, _MaxBounds.z);
			}
			vector = ((!flag) ? mPositionTarget : Vector3.SmoothDamp(RigController._Transform.position, mPositionTarget, ref mPositionVelocity, rDeltaTime)) - RigController._Transform.position;
			mRigTransform.Position = RigController._Transform.position + vector;
			mRigTransform.Rotation = RigController._Transform.rotation;
			return mRigTransform;
		}

		public override void PostRigLateUpdate()
		{
			if (Anchor != null)
			{
				mAnchorLastPosition = Anchor.position;
				mAnchorLastRotation = Anchor.rotation;
			}
			if (Vector3.Distance(mPositionTarget, mRigTransform.Position) < 0.0001f)
			{
				mPositionTarget = mRigTransform.Position;
				mPositionVelocity.x = 0f;
				mPositionVelocity.y = 0f;
				mPositionVelocity.z = 0f;
			}
			mAnchorOffsetDistance = Mathf.Min(mAnchorOffsetDistance + 1f * Time.deltaTime, AnchorOffset.magnitude);
		}

		public Vector3 GetGroundTarget()
		{
			if (RaycastExt.SafeRaycast(RigController._Transform.position, rDistance: (_GroundDistance > 0f) ? (_GroundDistance * 20f) : 100f, rRayDirection: Vector3.down, rHitInfo: out RaycastHit rHitInfo, rLayerMask: _GroundLayers, rIgnore: _Anchor))
			{
				return rHitInfo.point;
			}
			return Vector3Ext.Null;
		}

		public float GetGroundDistance()
		{
			if (RaycastExt.SafeRaycast(RigController._Transform.position, rDistance: (_GroundDistance > 0f) ? (_GroundDistance * 20f) : 100f, rRayDirection: Vector3.down, rHitInfo: out RaycastHit rHitInfo, rLayerMask: _GroundLayers, rIgnore: _Anchor))
			{
				return rHitInfo.distance;
			}
			return 0f;
		}
	}
}
