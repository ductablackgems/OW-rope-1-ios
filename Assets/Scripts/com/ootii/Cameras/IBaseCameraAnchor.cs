using com.ootii.Actors;
using UnityEngine;

namespace com.ootii.Cameras
{
	public interface IBaseCameraAnchor
	{
		bool IsFollowingEnabled
		{
			get;
			set;
		}

		Transform Transform
		{
			get;
		}

		Transform Root
		{
			get;
			set;
		}

		Vector3 RootOffset
		{
			get;
			set;
		}

		ControllerLateUpdateDelegate OnAnchorPostLateUpdate
		{
			get;
			set;
		}

		void ClearTarget(bool rFollowRoot = false);

		void ClearTarget(float rSpeed = 0f, float rLerp = 0f);

		void SetTargetPosition(Transform rTarget, Vector3 rPosition, float rSpeed, float rLerp = 0f, bool rClear = true);
	}
}
