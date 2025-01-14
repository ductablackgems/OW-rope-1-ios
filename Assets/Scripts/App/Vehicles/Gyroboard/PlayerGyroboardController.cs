using UnityEngine;

namespace App.Vehicles.Gyroboard
{
	public class PlayerGyroboardController : MonoBehaviour
	{
		public WheelCollider wheel;

		public float motorPower = 60f;

		public float rotationSpeed = 30f;

		private float horizontalInput;

		private void OnEnable()
		{
			horizontalInput = 0f;
		}

		private void FixedUpdate()
		{
			horizontalInput = VehicleInputsHelper.GetHorizonatalInput(horizontalInput);
			float verticalInput = VehicleInputsHelper.GetVerticalInput();
			base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
			wheel.motorTorque = verticalInput * motorPower;
			if (horizontalInput != 0f)
			{
				base.transform.Rotate(new Vector3(0f, rotationSpeed * Time.fixedDeltaTime * horizontalInput, 0f));
			}
		}
	}
}
