using UnityEngine;

namespace App.Vehicles.Motorbike
{
	public class MotorbikeAnimator : MonoBehaviour
	{
		public Transform tiltObject;

		public float maxTiltAngle = 30f;

		public float tiltCoeff = 10f;

		public float tiltSpeed = 100f;

		[Space(10f)]
		public WheelCollider backWheelCollider;

		public WheelCollider frontWheelCollider;

		public Transform frontWheel;

		public Transform backWheel;

		public Transform handles;

		public Transform frontWheelContainer;

		public Transform frontWheelHelper;

		public Transform backWheelSuspension;

		private float lastTiltAngle;

		private bool initialized;

		public void SetSpeed(float speed, float steerAngle, float deltaTime, bool force = false)
		{
			initialized = true;
			tiltObject.localRotation = Quaternion.identity;
			handles.localRotation = Quaternion.identity;
			backWheelCollider.GetWorldPose(out Vector3 pos, out Quaternion quat);
			backWheel.position = pos;
			backWheel.rotation = quat;
			if (backWheelSuspension != null)
			{
				backWheelSuspension.LookAt(backWheel);
			}
			frontWheelCollider.GetWorldPose(out pos, out quat);
			frontWheel.position = pos;
			frontWheelHelper.rotation = quat;
			Vector3 localEulerAngles = frontWheelHelper.localEulerAngles;
			frontWheel.localRotation = Quaternion.Euler(localEulerAngles.x, 0f, 0f);
			float num = 0f - Mathf.Clamp(steerAngle * speed * speed / tiltCoeff, 0f - maxTiltAngle, maxTiltAngle);
			float num2 = deltaTime * tiltSpeed;
			if (!force)
			{
				num = Mathf.Clamp(num, lastTiltAngle - num2, lastTiltAngle + num2);
			}
			lastTiltAngle = num;
			tiltObject.localRotation = Quaternion.Euler(0f, 0f, num);
			if (Mathf.Abs(localEulerAngles.y) > 90f && Mathf.Abs(localEulerAngles.y) < 270f)
			{
				handles.localRotation = Quaternion.Euler(0f, 180f + localEulerAngles.y, 0f);
			}
			else
			{
				handles.localRotation = Quaternion.Euler(0f, localEulerAngles.y, 0f);
			}
		}

		private void Awake()
		{
			if (!initialized)
			{
				SetSpeed(0f, 0f, Time.deltaTime);
			}
		}
	}
}
