using UnityEngine;

namespace App.Vehicles.Bicycle
{
	public class PlayerBicycleController : MonoBehaviour
	{
		public WheelCollider backWheel;

		public WheelCollider frontWheel;

		public float motorPower = 60f;

		public float breakTorque = 0.1f;

		public float tresholdStopSpeed = 0.8f;

		public float maxBackSpeed = 2f;

		public float maxSteerAngle = 30f;

		private Rigidbody _rigidbody;

		private BicycleAnimator animator;

		private float horizontalInput;

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			animator = this.GetComponentSafe<BicycleAnimator>();
		}

		private void OnEnable()
		{
			horizontalInput = 0f;
		}

		private void FixedUpdate()
		{
			horizontalInput = VehicleInputsHelper.GetHorizonatalInput(horizontalInput);
			float verticalInput = VehicleInputsHelper.GetVerticalInput();
			base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y, 0f);
			float num = verticalInput * motorPower;
			Vector3 vector = base.transform.InverseTransformDirection(_rigidbody.velocity);
			if (verticalInput < 0f && vector.z < 0f - maxBackSpeed)
			{
				num = 0f;
			}
			float num4 = frontWheel.motorTorque = (backWheel.motorTorque = num);
			frontWheel.steerAngle = maxSteerAngle * horizontalInput;
			if (verticalInput != 0f || !(Mathf.Abs(vector.z) < tresholdStopSpeed))
			{
				num4 = (frontWheel.brakeTorque = (backWheel.brakeTorque = 0f));
			}
			else
			{
				num4 = (frontWheel.brakeTorque = (backWheel.brakeTorque = breakTorque));
			}
			animator.SetSpeed(vector.z, frontWheel.steerAngle);
		}
	}
}
