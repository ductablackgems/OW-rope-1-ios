using com.ootii.Geometry;
using com.ootii.Input;
using UnityEngine;

namespace com.ootii.Cameras
{
	public abstract class YawPitchMotor : CameraMotor
	{
		public const float EPSILON = 0.0001f;

		public float _MaxDistance = 3f;

		protected float mDistance = 3f;

		public bool _IsYawEnabled = true;

		public float _MinYaw = -180f;

		public float _MaxYaw = 180f;

		public float _YawSpeed = 120f;

		public float _TargetRotationMultiplier = 3f;

		public bool _IsPitchEnabled = true;

		public bool _InvertPitch = true;

		public float _MinPitch = -60f;

		public float _MaxPitch = 60f;

		public float _PitchSpeed = 45f;

		public float _Smoothing = 0.05f;

		protected Vector3 _Euler = Vector3.zero;

		protected Vector3 _EulerTarget = Vector3.zero;

		protected float mDegreesYPer60FPSTick = 1f;

		protected float mDegreesXPer60FPSTick = 1f;

		protected Vector3 mFrameEuler = Vector3.zero;

		protected Vector3 mAnchorLastPosition = Vector3.zero;

		protected Quaternion mAnchorLastRotation = Quaternion.identity;

		protected Vector3 mFocusLastPosition = Vector3.zero;

		protected bool mWasAnchorRotating;

		protected float mTargetYaw = float.MaxValue;

		protected float mTargetPitch = float.MaxValue;

		protected Transform mTarget;

		protected Vector3 mTargetForward = Vector3.zero;

		protected float mTargetYawSpeed;

		protected float mTargetPitchSpeed;

		protected bool mAutoClearTarget = true;

		protected float mViewVelocityY;

		protected float mViewVelocityX;

		public override float MaxDistance
		{
			get
			{
				return _MaxDistance;
			}
			set
			{
				_MaxDistance = value;
			}
		}

		public override float Distance
		{
			get
			{
				return mDistance;
			}
			set
			{
				mDistance = value;
			}
		}

		public virtual bool IsYawEnabled
		{
			get
			{
				return _IsYawEnabled;
			}
			set
			{
				_IsYawEnabled = value;
			}
		}

		public virtual float LocalYaw
		{
			get
			{
				float num = 0f;
				Transform anchor = Anchor;
				num = ((!(anchor == null)) ? (Quaternion.Inverse(anchor.rotation) * RigController._Transform.rotation).eulerAngles.y : RigController._Transform.rotation.eulerAngles.y);
				if (num > 180f)
				{
					num -= 360f;
				}
				else if (num < -180f)
				{
					num += 360f;
				}
				return num;
			}
		}

		public virtual float MinYaw
		{
			get
			{
				return _MinYaw;
			}
			set
			{
				_MinYaw = value;
			}
		}

		public virtual float MaxYaw
		{
			get
			{
				return _MaxYaw;
			}
			set
			{
				_MaxYaw = value;
			}
		}

		public virtual float YawSpeed
		{
			get
			{
				return _YawSpeed;
			}
			set
			{
				_YawSpeed = value;
				mDegreesYPer60FPSTick = _YawSpeed / 60f;
			}
		}

		public float TargetRotationMultiplier
		{
			get
			{
				return _TargetRotationMultiplier;
			}
			set
			{
				_TargetRotationMultiplier = value;
			}
		}

		public virtual bool IsPitchEnabled
		{
			get
			{
				return _IsPitchEnabled;
			}
			set
			{
				_IsPitchEnabled = value;
			}
		}

		public virtual bool InvertPitch
		{
			get
			{
				return _InvertPitch;
			}
			set
			{
				_InvertPitch = value;
			}
		}

		public virtual float LocalPitch
		{
			get
			{
				float num = 0f;
				Transform anchor = Anchor;
				num = ((!(anchor == null)) ? (Quaternion.Inverse(anchor.rotation) * RigController._Transform.rotation).eulerAngles.x : RigController._Transform.rotation.eulerAngles.x);
				if (num > 180f)
				{
					num -= 360f;
				}
				else if (num < -180f)
				{
					num += 360f;
				}
				return num;
			}
		}

		public virtual float MinPitch
		{
			get
			{
				return _MinPitch;
			}
			set
			{
				_MinPitch = Mathf.Max(value, -87.4f);
			}
		}

		public virtual float MaxPitch
		{
			get
			{
				return _MaxPitch;
			}
			set
			{
				_MaxPitch = Mathf.Min(value, 87.4f);
			}
		}

		public virtual float PitchSpeed
		{
			get
			{
				return _PitchSpeed;
			}
			set
			{
				_PitchSpeed = value;
				mDegreesXPer60FPSTick = _PitchSpeed / 60f;
			}
		}

		public float Smoothing
		{
			get
			{
				return _Smoothing;
			}
			set
			{
				_Smoothing = value;
			}
		}

		public Vector3 Euler => _Euler;

		public Vector3 EulerTarget => _EulerTarget;

		public bool IsTargetSet
		{
			get
			{
				if (mTargetYaw != float.MaxValue)
				{
					return true;
				}
				if (mTargetPitch != float.MaxValue)
				{
					return true;
				}
				if (mTargetForward.sqrMagnitude != 0f)
				{
					return true;
				}
				return false;
			}
		}

		public override void Awake()
		{
			base.Awake();
			mDistance = _MaxDistance;
			mDegreesYPer60FPSTick = _YawSpeed / 60f;
			mDegreesXPer60FPSTick = _PitchSpeed / 60f;
			if (Application.isPlaying)
			{
				Transform anchor = Anchor;
				Transform transform = RigController.Transform;
				if (anchor != null)
				{
					mAnchorLastPosition = anchor.position;
					mAnchorLastRotation = anchor.rotation;
				}
				_Euler = transform.eulerAngles;
				NormalizeEuler(ref _Euler);
				_EulerTarget = _Euler;
				mFocusLastPosition = GetFocusPosition(transform.rotation);
			}
		}

		public override bool Initialize()
		{
			Transform anchor = Anchor;
			Transform transform = RigController.transform;
			if (anchor != null)
			{
				mAnchorLastPosition = anchor.position;
				mAnchorLastRotation = anchor.rotation;
			}
			Quaternion quaternion = transform.rotation;
			if (RigController.Anchor != null)
			{
				quaternion = RigController.Anchor.rotation.RotationTo(transform.rotation);
			}
			_Euler = quaternion.eulerAngles;
			NormalizeEuler(ref _Euler);
			_EulerTarget = _Euler;
			mFocusLastPosition = GetFocusPosition(transform.rotation);
			return base.Initialize();
		}

		public void ClearTargetYawPitch()
		{
			mTargetYaw = float.MaxValue;
			mTargetPitch = float.MaxValue;
		}

		public void SetTargetYawPitch(float rYaw, float rPitch, float rSpeed = -1f, bool rAutoClearTarget = true)
		{
			ClearTargetForward();
			mTargetYaw = rYaw;
			mTargetPitch = rPitch;
			mAutoClearTarget = rAutoClearTarget;
			float num = Mathf.Abs(mTargetYaw - LocalYaw);
			float num2 = Mathf.Abs(mTargetPitch - LocalPitch);
			if (rSpeed > 0f)
			{
				float num3 = (num >= num2) ? (num / rSpeed) : (num2 / rSpeed);
				if (num3 > 0f)
				{
					mTargetYawSpeed = num / num3;
					mTargetPitchSpeed = num2 / num3;
				}
				return;
			}
			if (rSpeed == 0f)
			{
				mTargetYawSpeed = 0f;
				mTargetPitchSpeed = 0f;
				return;
			}
			float num4 = 1f;
			if (mWasAnchorRotating)
			{
				num4 = _TargetRotationMultiplier;
			}
			else if (Anchor != null && !QuaternionExt.IsEqual(mAnchorLastRotation, Anchor.rotation))
			{
				num4 = _TargetRotationMultiplier;
			}
			mTargetYawSpeed = _YawSpeed * num4;
			mTargetPitchSpeed = _PitchSpeed * num4;
		}

		public void ClearTargetForward()
		{
			mTargetForward = Vector3.zero;
		}

		public void SetTargetForward(Vector3 rForward, float rSpeed = -1f, bool rAutoClearTarget = true)
		{
			if (rForward.sqrMagnitude == 0f || Anchor == null)
			{
				return;
			}
			ClearTargetYawPitch();
			mTargetForward = rForward;
			mAutoClearTarget = rAutoClearTarget;
			Quaternion rhs = Quaternion.LookRotation(mTargetForward, Anchor.up);
			Quaternion quaternion = Quaternion.Inverse(Anchor.rotation) * RigController._Transform.rotation;
			Quaternion quaternion2 = Quaternion.Inverse(Anchor.rotation) * rhs;
			float num = Mathf.Abs(quaternion2.eulerAngles.y - quaternion.eulerAngles.y);
			float num2 = Mathf.Abs(quaternion2.eulerAngles.x - quaternion.eulerAngles.x);
			if (rSpeed > 0f)
			{
				float num3 = (num >= num2) ? (num / rSpeed) : (num2 / rSpeed);
				if (num3 > 0f)
				{
					mTargetYawSpeed = num / num3;
					mTargetPitchSpeed = num2 / num3;
				}
			}
			else if (rSpeed == 0f)
			{
				mTargetYawSpeed = 0f;
				mTargetPitchSpeed = 0f;
			}
			else
			{
				mTargetYawSpeed = _YawSpeed;
				mTargetPitchSpeed = _PitchSpeed;
			}
		}

		public void ClearTarget()
		{
			mTarget = null;
		}

		public void SetTarget(Transform rTarget)
		{
			mTarget = rTarget;
			mTargetYawSpeed = 0f;
			mTargetPitchSpeed = 0f;
		}

		public virtual Vector3 GetFrameEuler(bool rUseYawLimits, bool rUsePitchLimits = true)
		{
			Vector3 zero = Vector3.zero;
			if (RigController.LastPosition != RigController._Transform.position)
			{
				mFocusLastPosition = GetFocusPosition(RigController._Transform.rotation);
			}
			if (mTarget != null)
			{
				mTargetForward = (mTarget.position - mRigTransform.Position).normalized;
			}
			if (mTargetForward.sqrMagnitude > 0f)
			{
				Quaternion rhs = Quaternion.LookRotation(mTargetForward, Anchor.up);
				Quaternion quaternion = Quaternion.Inverse(Anchor.rotation) * RigController._Transform.rotation;
				Quaternion quaternion2 = Quaternion.Inverse(Anchor.rotation) * rhs;
				float num = quaternion2.eulerAngles.y - quaternion.eulerAngles.y;
				if (num > 180f)
				{
					num -= 360f;
				}
				else if (num < -180f)
				{
					num += 360f;
				}
				float num2 = quaternion2.eulerAngles.x - quaternion.eulerAngles.x;
				if (num2 > 180f)
				{
					num2 -= 360f;
				}
				else if (num2 < -180f)
				{
					num2 += 360f;
				}
				if (mTargetYawSpeed <= 0f)
				{
					zero.y = num;
				}
				else
				{
					zero.y = Mathf.Sign(num) * Mathf.Min(mTargetYawSpeed * Time.deltaTime, Mathf.Abs(num));
				}
				if (mTargetPitchSpeed <= 0f)
				{
					zero.x = num2;
				}
				else
				{
					zero.x = Mathf.Sign(num2) * Mathf.Min(mTargetPitchSpeed * Time.deltaTime, Mathf.Abs(num2));
				}
				if (Mathf.Abs(zero.y) < 0.0001f && Mathf.Abs(zero.x) < 0.0001f)
				{
					_Euler.y = LocalYaw;
					_Euler.x = LocalPitch;
					_EulerTarget = _Euler;
					if (mAutoClearTarget)
					{
						mTargetForward = Vector3.zero;
					}
				}
			}
			else if (mTargetYaw < float.MaxValue || mTargetPitch < float.MaxValue)
			{
				if (mTargetYaw < float.MaxValue)
				{
					float num3 = mTargetYaw - LocalYaw;
					if (mTargetYawSpeed <= 0f)
					{
						zero.y = num3;
					}
					else
					{
						zero.y = Mathf.Sign(num3) * Mathf.Min(mTargetYawSpeed * Time.deltaTime, Mathf.Abs(num3));
					}
					Transform anchor = Anchor;
					float num4 = mAnchorLastRotation.Forward().HorizontalAngleTo(anchor.forward, anchor.up);
					if (Mathf.Abs(zero.y) - Mathf.Abs(num4 * 2f) < 0.0001f)
					{
						_Euler.y = mTargetYaw;
						_EulerTarget.y = mTargetYaw;
					}
					if (mAutoClearTarget && Mathf.Abs(num3) < 0.0001f)
					{
						mTargetYaw = float.MaxValue;
					}
				}
				if (mTargetPitch < float.MaxValue)
				{
					float num5 = mTargetPitch - LocalPitch;
					if (mTargetPitchSpeed <= 0f)
					{
						zero.x = num5;
					}
					else
					{
						zero.x = Mathf.Sign(num5) * Mathf.Min(mTargetPitchSpeed * Time.deltaTime, Mathf.Abs(num5));
					}
					if (Mathf.Abs(zero.x) < 0.0001f)
					{
						_Euler.x = mTargetPitch;
						_EulerTarget.x = mTargetPitch;
					}
					if (mAutoClearTarget && Mathf.Abs(num5) < 0.0001f)
					{
						mTargetPitch = float.MaxValue;
					}
				}
			}
			else
			{
				IInputSource inputSource = RigController.InputSource;
				if (inputSource.IsViewingActivated)
				{
					if (_IsYawEnabled && zero.y == 0f)
					{
						zero.y = inputSource.ViewX * mDegreesYPer60FPSTick;
					}
					if (_IsPitchEnabled && zero.x == 0f)
					{
						zero.x = ((RigController._InvertPitch || _InvertPitch) ? (-1f) : 1f) * inputSource.ViewY * mDegreesXPer60FPSTick;
					}
				}
				_EulerTarget.y = ((rUseYawLimits && (_MinYaw > -180f || _MaxYaw < 180f)) ? Mathf.Clamp(_EulerTarget.y + zero.y, _MinYaw, _MaxYaw) : (_EulerTarget.y + zero.y));
				zero.y = ((_Smoothing <= 0f) ? _EulerTarget.y : SmoothDamp(_Euler.y, _EulerTarget.y, _Smoothing * 0.001f, Time.deltaTime)) - _Euler.y;
				_Euler.y += zero.y;
				_EulerTarget.x = ((rUsePitchLimits && (_MinPitch > -180f || _MaxPitch < 180f)) ? Mathf.Clamp(_EulerTarget.x + zero.x, _MinPitch, _MaxPitch) : (_EulerTarget.x + zero.x));
				zero.x = ((_Smoothing <= 0f) ? _EulerTarget.x : SmoothDamp(_Euler.x, _EulerTarget.x, _Smoothing * 0.001f, Time.deltaTime)) - _Euler.x;
				_Euler.x += zero.x;
			}
			return zero;
		}

		public override void PostRigLateUpdate()
		{
			Transform anchor = Anchor;
			if (anchor != null)
			{
				mWasAnchorRotating = !QuaternionExt.IsEqual(mAnchorLastRotation, anchor.rotation);
				mAnchorLastPosition = anchor.position;
				mAnchorLastRotation = anchor.rotation;
			}
			mFocusLastPosition = GetFocusPosition(RigController._Transform.rotation);
			if (Mathf.Abs(_EulerTarget.y - _Euler.y) < 0.0001f)
			{
				_Euler.y = LocalYaw;
				_EulerTarget.y = _Euler.y;
				mViewVelocityY = 0f;
			}
			if (Mathf.Abs(_EulerTarget.x - _Euler.x) < 0.0001f)
			{
				_Euler.x = LocalPitch;
				_EulerTarget.x = _Euler.x;
				mViewVelocityX = 0f;
			}
			base.PostRigLateUpdate();
		}

		public float SmoothDamp(float rSource, float rTarget, float rSmoothing, float rDeltaTime)
		{
			return Mathf.Lerp(rSource, rTarget, 1f - Mathf.Pow(rSmoothing, rDeltaTime));
		}
	}
}
