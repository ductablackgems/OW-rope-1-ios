using com.ootii.Base;
using com.ootii.Geometry;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("3rd Person Follow")]
	[BaseDescription("Motor that allows the rig to orbit the anchor and anchor offset. This rig drags behind the anchor as if attached by a rope.")]
	public class OrbitFollowMotor : YawPitchMotor
	{
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

		public override void Awake()
		{
			base.Awake();
			if (Application.isPlaying && Anchor == null)
			{
				mRigTransform.Position = RigController._Transform.position;
				mRigTransform.Rotation = RigController._Transform.rotation;
			}
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
			if (RigController.FrameForceToFollowAnchor || Mathf.Abs(rTiltAngle) >= 2f)
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
				}
			}
			else if (mTargetForward.sqrMagnitude > 0f)
			{
				quaternion *= Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
				vector = focusPosition - quaternion.Forward() * mDistance;
			}
			else
			{
				quaternion *= Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
				Vector3 b = mFocusLastPosition - quaternion.Forward() * mDistance;
				forward = focusPosition - b;
				if (forward.sqrMagnitude < 0.0001f)
				{
					forward = transform.forward;
				}
				quaternion = Quaternion.LookRotation(forward.normalized, RigController.RotateAnchorOffset ? anchor.up : Vector3.up);
				focusPosition = GetFocusPosition(quaternion);
				Vector3 zero = Vector3.zero;
				zero = ((!RigController.RotateAnchorOffset) ? (anchor.position + AnchorOffset + Vector3.up * _Offset.y) : (anchor.position + anchor.rotation * AnchorOffset + anchor.up * _Offset.y));
				forward = focusPosition - zero;
				forward = focusPosition - b;
				if (forward.sqrMagnitude < 0.0001f)
				{
					forward = transform.forward;
				}
				Vector3 b2 = Vector3.Project(forward, RigController.RotateAnchorOffset ? RigController.Tilt.Up() : Vector3.up);
				Vector3 forward2 = forward - b2;
				quaternion = transform.rotation * Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
				Vector3 eulerAngles = (RigController.RotateAnchorOffset ? (Quaternion.Inverse(RigController.Tilt) * quaternion) : quaternion).eulerAngles;
				Quaternion rThis = Quaternion.LookRotation(forward2, RigController.RotateAnchorOffset ? RigController.Tilt.Up() : Vector3.up) * Quaternion.Euler(eulerAngles.x, 0f, 0f);
				vector = focusPosition - rThis.Forward() * mDistance;
			}
			forward = focusPosition - vector;
			if (forward.sqrMagnitude < 0.0001f)
			{
				forward = transform.forward;
			}
			quaternion = Quaternion.LookRotation(forward.normalized, RigController.RotateAnchorOffset ? anchor.up : Vector3.up);
			Vector3 eulerAngles2 = ((RigController.RotateAnchorOffset ? Quaternion.Inverse(Anchor.transform.rotation) : Quaternion.identity) * quaternion).eulerAngles;
			if (eulerAngles2.y > 180f)
			{
				eulerAngles2.y -= 360f;
			}
			else if (eulerAngles2.y < -180f)
			{
				eulerAngles2.y += 360f;
			}
			if (eulerAngles2.x > 180f)
			{
				eulerAngles2.x -= 360f;
			}
			else if (eulerAngles2.x < -180f)
			{
				eulerAngles2.x += 360f;
			}
			float num3 = (_MinYaw > -180f || _MaxYaw < 180f) ? Mathf.Clamp(eulerAngles2.y, _MinYaw, _MaxYaw) : eulerAngles2.y;
			float num4 = Mathf.Clamp(eulerAngles2.x, _MinPitch, _MaxPitch);
			if (num3 != eulerAngles2.y || num4 != eulerAngles2.x)
			{
				quaternion = (RigController.RotateAnchorOffset ? Anchor.transform.rotation : Quaternion.identity) * Quaternion.Euler(num4, num3, 0f);
				vector = focusPosition - quaternion.Forward() * mDistance;
			}
			mRigTransform.Position = vector;
			mRigTransform.Rotation = quaternion;
			return mRigTransform;
		}
	}
}
