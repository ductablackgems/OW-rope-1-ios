using com.ootii.Base;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("Free Flight")]
	[BaseDescription("Editor style motor that allows the player to move the camera around the scene without an anchor.")]
	public class FreeFlightMotor : YawPitchMotor
	{
		public float _Speed = 5f;

		public float _VerticalSpeed = 5f;

		public string _UpActionAlias = "";

		public string _DownActionAlias = "";

		public float Speed
		{
			get
			{
				return _Speed;
			}
			set
			{
				_Speed = value;
			}
		}

		public float VerticalSpeed
		{
			get
			{
				return _VerticalSpeed;
			}
			set
			{
				_VerticalSpeed = value;
			}
		}

		public string UpActionAlias
		{
			get
			{
				return _UpActionAlias;
			}
			set
			{
				_UpActionAlias = value;
			}
		}

		public string DownActionAlias
		{
			get
			{
				return _DownActionAlias;
			}
			set
			{
				_DownActionAlias = value;
			}
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			if (RigController == null)
			{
				return mRigTransform;
			}
			if (RigController.InputSource == null)
			{
				return mRigTransform;
			}
			Transform transform = RigController._Transform;
			mFrameEuler = GetFrameEuler(rUseYawLimits: false);
			Quaternion lhs = Quaternion.AngleAxis(mFrameEuler.y, Vector3.up) * transform.rotation;
			lhs *= Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
			Vector3 a = new Vector3(RigController.InputSource.MovementX, 0f, RigController.InputSource.MovementY);
			Vector3 vector = transform.position + lhs * (a * _Speed * rDeltaTime);
			if (_UpActionAlias.Length > 0 && RigController.InputSource.IsPressed(_UpActionAlias))
			{
				vector += Vector3.up * (_VerticalSpeed * rDeltaTime);
			}
			if (_DownActionAlias.Length > 0 && RigController.InputSource.IsPressed(_DownActionAlias))
			{
				vector += Vector3.down * (_VerticalSpeed * rDeltaTime);
			}
			mRigTransform.Position = vector;
			mRigTransform.Rotation = lhs;
			return mRigTransform;
		}
	}
}
