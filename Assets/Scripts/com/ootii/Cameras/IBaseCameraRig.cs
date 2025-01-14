using UnityEngine;

namespace com.ootii.Cameras
{
	public interface IBaseCameraRig
	{
		Transform Transform
		{
			get;
		}

		Transform Anchor
		{
			get;
			set;
		}

		int Mode
		{
			get;
			set;
		}

		bool LockMode
		{
			get;
			set;
		}

		bool IsInternalUpdateEnabled
		{
			get;
			set;
		}

		void EnableMode(int rMode, bool rEnable);

		void ClearTargetYawPitch();

		void SetTargetYawPitch(float rYaw, float rPitch, float rSpeed = -1f, bool rAutoClearTarget = true);

		void ClearTargetForward();

		void SetTargetForward(Vector3 rForward, float rSpeed = -1f, bool rAutoClearTarget = true);

		void ExtrapolateAnchorPosition(out Vector3 rPosition, out Quaternion rRotation);

		void RigLateUpdate(float rDeltaTime, int rUpdateIndex);
	}
}
