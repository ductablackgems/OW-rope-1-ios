using UnityEngine;

namespace com.ootii.Actors
{
	public interface ICharacterController
	{
		GameObject gameObject
		{
			get;
		}

		Quaternion Yaw
		{
			get;
			set;
		}

		Quaternion Tilt
		{
			get;
			set;
		}

		ControllerLateUpdateDelegate OnControllerPreLateUpdate
		{
			get;
			set;
		}

		ControllerLateUpdateDelegate OnControllerPostLateUpdate
		{
			get;
			set;
		}

		ControllerMoveDelegate OnPreControllerMove
		{
			get;
			set;
		}

		void SetRotation(Quaternion rRotation);

		void SetRotation(Quaternion rYaw, Quaternion rTilt);

		void SetPosition(Vector3 rPosition);

		bool IsIgnoringCollision(Collider rCollider);

		void ClearIgnoreCollisions();

		void IgnoreCollision(Collider rCollider, bool rIgnore = true);
	}
}
