using com.ootii.Base;

namespace com.ootii.Cameras
{
	[BaseName("Basic Motor")]
	[BaseDescription("Basic Motor that moves and rotates based on the Camera Controller's transform. It does not use the Anchor.")]
	public class BasicMotor : CameraMotor
	{
		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			if (RigController == null)
			{
				return mRigTransform;
			}
			mRigTransform.Position = RigController._Transform.position;
			mRigTransform.Rotation = RigController._Transform.rotation;
			return mRigTransform;
		}
	}
}
