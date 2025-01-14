using UnityEngine;

namespace com.ootii.Actors
{
	public delegate void ControllerMoveDelegate(ICharacterController rController, ref Vector3 rFinalPosition, ref Quaternion rFinalRotation);
}
