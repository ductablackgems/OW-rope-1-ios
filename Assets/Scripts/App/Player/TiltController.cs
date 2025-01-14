using UnityEngine;

namespace App.Player
{
	public class TiltController : MonoBehaviour
	{
		public float maxHorizontalAngle = 20f;

		public float maxVerticalAngle = 20f;

		public float verticalAngleOffset = 10f;

		public float speed = 50f;

		private AccelerationJoystick accelerationJoystick;

		protected void Awake()
		{
			accelerationJoystick = ServiceLocator.Get<AccelerationJoystick>();
		}

		protected void FixedUpdate()
		{
			float axis = ETCInput.GetAxis("VerticalJoystick");
			float verticalAxis = accelerationJoystick.GetVerticalAxis();
			float num = 0f;
			num = ((!(axis > 0f)) ? (axis * verticalAngleOffset) : (axis * verticalAngleOffset + axis * verticalAxis * maxVerticalAngle));
			float z = (0f - ETCInput.GetAxis("HorizontalJoystick")) * maxHorizontalAngle;
			base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, Quaternion.Euler(num, 0f, z), speed * Time.fixedDeltaTime);
		}
	}
}
