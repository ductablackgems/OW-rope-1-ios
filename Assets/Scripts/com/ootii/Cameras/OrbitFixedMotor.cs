using com.ootii.Actors;
using com.ootii.Base;
using com.ootii.Geometry;
using com.ootii.Helpers;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("3rd Person Fixed")]
	[BaseDescription("Motor that allows the rig to orbit the anchor and anchor offset. This rig follows the anchor as if attached by a hard pole.")]
	public class OrbitFixedMotor : YawPitchMotor
	{
		public bool _RotateWithAnchor;

		public bool _RotateAnchor;

		public string _RotateAnchorAlias = "Camera Rotate Character";

		protected ICharacterController mCharacterController;

		public override Transform Anchor
		{
			get
			{
				return base.Anchor;
			}
			set
			{
				base.Anchor = value;
				if (Anchor != null)
				{
					mCharacterController = InterfaceHelper.GetComponent<ICharacterController>(Anchor.gameObject);
				}
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
				_MaxDistance = value;
			}
		}

		public bool RotateWithAnchor
		{
			get
			{
				return _RotateWithAnchor;
			}
			set
			{
				_RotateWithAnchor = value;
			}
		}

		public bool RotateAnchor
		{
			get
			{
				return _RotateAnchor;
			}
			set
			{
				_RotateAnchor = value;
			}
		}

		public string RotateAnchorAlias
		{
			get
			{
				return _RotateAnchorAlias;
			}
			set
			{
				_RotateAnchorAlias = value;
			}
		}

		public override void Awake()
		{
			base.Awake();
			if (Anchor != null)
			{
				mCharacterController = InterfaceHelper.GetComponent<ICharacterController>(Anchor.gameObject);
			}
			if (Application.isPlaying && Anchor == null)
			{
				mRigTransform.Position = RigController._Transform.position;
				mRigTransform.Rotation = RigController._Transform.rotation;
			}
		}

		public override void Clear()
		{
			mCharacterController = null;
			base.Clear();
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			Transform anchor = Anchor;
			if (anchor == null)
			{
				return mRigTransform;
			}
			if (RigController == null)
			{
				return mRigTransform;
			}
			Transform transform = RigController._Transform;
			float num = mAnchorLastRotation.Forward().HorizontalAngleTo(anchor.forward, anchor.up);
			float num2 = (Mathf.Abs(rTiltAngle) >= 2f) ? num : 0f;
			if (!RigController.RotateAnchorOffset)
			{
				num = 0f;
				num2 = 0f;
			}
			mFrameEuler = GetFrameEuler(rUseYawLimits: true);
			Quaternion quaternion = Quaternion.AngleAxis(mFrameEuler.y + num2, RigController.RotateAnchorOffset ? RigController.Tilt.Up() : Vector3.up) * transform.rotation;
			Vector3 focusPosition = GetFocusPosition(quaternion);
			Vector3 vector = transform.position;
			Vector3 forward = transform.forward;
			if (_RotateWithAnchor || RigController.FrameForceToFollowAnchor || Mathf.Abs(rTiltAngle) >= 2f)
			{
				Matrix4x4 lhs = Matrix4x4.TRS(mFocusLastPosition, RigController.RotateAnchorOffset ? mAnchorLastRotation : Quaternion.identity, Vector3.one);
				Matrix4x4 rhs = Matrix4x4.TRS(focusPosition, RigController.RotateAnchorOffset ? anchor.rotation : Quaternion.identity, Vector3.one);
				if (mTargetYaw < float.MaxValue)
				{
					mFrameEuler.y -= num;
				}
				if (mFrameEuler.sqrMagnitude != 0f || lhs != rhs)
				{
					Vector3 point = lhs.inverse.MultiplyPoint(transform.position);
					Vector3 axis = lhs.inverse.MultiplyVector(transform.right);
					point = Quaternion.AngleAxis(mFrameEuler.y, Vector3.up) * Quaternion.AngleAxis(mFrameEuler.x, axis) * point;
					vector = rhs.MultiplyPoint(point);
					Vector3 normalized = (vector - focusPosition).normalized;
					vector = focusPosition + normalized * mDistance;
				}
				float num3 = Vector3.Distance(AnchorPosition, vector);
				float num4 = Vector3.Distance(AnchorPosition, focusPosition);
				if (num3 < num4)
				{
					vector = focusPosition - quaternion.Forward() * mDistance;
				}
			}
			else
			{
				quaternion *= Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
				vector = focusPosition - quaternion.Forward() * mDistance;
			}
			forward = focusPosition - vector;
			if (forward.sqrMagnitude < 0.0001f)
			{
				forward = transform.forward;
			}
			quaternion = Quaternion.LookRotation(forward.normalized, RigController.RotateAnchorOffset ? anchor.up : Vector3.up);
			if (forward.magnitude != mDistance)
			{
				vector = focusPosition - forward.normalized * mDistance;
			}
			mRigTransform.Position = vector;
			mRigTransform.Rotation = quaternion;
			return mRigTransform;
		}

		public override void PostRigLateUpdate()
		{
			base.PostRigLateUpdate();
			if (_RotateAnchor && Anchor != null && (_RotateAnchorAlias.Length == 0 || RigController.InputSource == null || RigController.InputSource.IsPressed(_RotateAnchorAlias)))
			{
				if (mCharacterController != null)
				{
					Quaternion rhs = Quaternion.AngleAxis(Anchor.forward.HorizontalAngleTo(RigController.Transform.forward, Anchor.up), Vector3.up);
					mCharacterController.Yaw *= rhs;
					Anchor.rotation = mCharacterController.Tilt * mCharacterController.Yaw;
				}
				else
				{
					Quaternion rhs2 = Quaternion.AngleAxis(Anchor.forward.HorizontalAngleTo(RigController.Transform.forward, Anchor.up), Vector3.up);
					Anchor.rotation *= rhs2;
				}
			}
		}

		public override void DeserializeMotor(string rDefinition)
		{
			base.DeserializeMotor(rDefinition);
			if (Anchor != null)
			{
				mCharacterController = InterfaceHelper.GetComponent<ICharacterController>(Anchor.gameObject);
			}
		}
	}
}
