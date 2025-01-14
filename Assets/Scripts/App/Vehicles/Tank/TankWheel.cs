using UnityEngine;

namespace App.Vehicles.Tank
{
	public class TankWheel : MonoBehaviour
	{
		public bool isLeft;

		public TankController tankController;

		public WheelCollider wheelCollider;

		public Vector3 offset;

		private void FixedUpdate()
		{
			if (wheelCollider != null)
			{
				wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion _);
				base.transform.position = pos + base.transform.parent.TransformDirection(offset);
			}
			if (isLeft)
			{
				base.transform.localRotation = tankController.LeftWheelRotation;
			}
			else
			{
				base.transform.localRotation = tankController.RightWheelRotation;
			}
		}
	}
}
