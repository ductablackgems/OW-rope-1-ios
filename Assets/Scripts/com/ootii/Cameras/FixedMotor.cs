using com.ootii.Base;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("Fixed Motor")]
	[BaseDescription("Rig Motor that keeps the camera at the fixed position and rotation from the anchor.")]
	public class FixedMotor : CameraMotor
	{
		public Quaternion _RotationOffset = Quaternion.identity;

		public Quaternion RotationOffset
		{
			get
			{
				return _RotationOffset;
			}
			set
			{
				_RotationOffset = value;
			}
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			if (RigController == null)
			{
				return mRigTransform;
			}
			if (Anchor != null)
			{
				mRigTransform.Position = AnchorPosition + Anchor.rotation * _Offset;
			}
			else
			{
				mRigTransform.Position = AnchorOffset + _Offset;
			}
			if (Anchor != null)
			{
				mRigTransform.Rotation = Anchor.rotation * _RotationOffset;
			}
			else
			{
				mRigTransform.Rotation = RigController._Transform.rotation * _RotationOffset;
			}
			return mRigTransform;
		}
	}
}
