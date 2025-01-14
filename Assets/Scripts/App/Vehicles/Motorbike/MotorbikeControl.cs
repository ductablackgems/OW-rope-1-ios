using App.Vehicles.Car;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Skid;
using UnityEngine;

namespace App.Vehicles.Motorbike
{
	public class MotorbikeControl : MonoBehaviour, ISoundableCar, IHandbrakeVehicle, IVehicleController, IVehicleAnimator
	{
		public WheelCollider backWheel;

		public WheelCollider frontWheel;

		public float motorPower = 1800f;

		public float reverseMotorPower = 400f;

		public float breakTorque = 0.1f;

		public float tresholdStopSpeed = 0.8f;

		public float maxBackSpeed = 2f;

		[Space(10f)]
		public float minSteerAngle;

		public float maxSteerAngle = 30f;

		public float startLimitSteerSpeed;

		public float maxSteerLimitSpeed;

		[Space(10f)]
		public float maxFrontTiltAngle = 60f;

		public float frontTiltForce = 30f;

		[Space(10f)]
		[SerializeField]
		private float m_Topspeed = 200f;

		public float maxTorqueSpeed = 250f;

		[SerializeField]
		private int NoOfGears = 5;

		[SerializeField]
		private float m_RevRangeBoundary = 1f;

		private int m_GearNum;

		private float m_GearFactor;

		private Rigidbody _rigidbody;

		private MotorbikeAnimator animator;

		private float lastVerticalInput;

		private bool reverse;

		private bool animated = true;

		private DurationTimer finishAnimatorTimer = new DurationTimer();

		public bool AiOptimized
		{
			get;
			set;
		}

		public float HandbreakInput
		{
			get;
			private set;
		}

		public float Revs
		{
			get;
			private set;
		}

		public float CurrentSpeed => _rigidbody.velocity.magnitude * 3.6f;

		public float MaxSpeed => m_Topspeed;

		public float GetRevs()
		{
			if (!reverse)
			{
				return Revs;
			}
			return 0f;
		}

		public float GetAccelerationInput()
		{
			return lastVerticalInput;
		}

		public void Animate(float deltaTime)
		{
			if (Animated() || finishAnimatorTimer.InProgress())
			{
				if (finishAnimatorTimer.InProgress())
				{
					animator.SetSpeed(0f, 0f, Time.deltaTime);
					return;
				}
				finishAnimatorTimer.Stop();
				Vector3 vector = base.transform.InverseTransformDirection(_rigidbody.velocity);
				animator.SetSpeed(vector.z, frontWheel.steerAngle, deltaTime);
			}
		}

		public bool Animated()
		{
			return animated;
		}

		public void SetAnimated(bool animated)
		{
			if (!animated && this.animated)
			{
				finishAnimatorTimer.Run(1f);
			}
			else if (animated)
			{
				finishAnimatorTimer.Stop();
			}
			this.animated = animated;
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			animator = this.GetComponentSafe<MotorbikeAnimator>();
		}

		public void Move(float steering, float accel, float footbrake, float handbrake)
		{
			Control(steering, accel, 0f - footbrake, handbrake);
		}

		public void SetOptimizedForAI(bool value)
		{
			AiOptimized = value;
		}

		public void UpdateOldRotation()
		{
		}

		public void SteerHelper()
		{
		}

		public bool MoveEachFrame()
		{
			return true;
		}

		public void Control(float horizontalInput, float verticalInput, float brake = 0f, float handbrake = 0f)
		{
			lastVerticalInput = verticalInput;
			base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y, 0f);
			float num = (verticalInput >= 0f) ? (verticalInput * motorPower) : (verticalInput * reverseMotorPower);
			Vector3 vector = base.transform.InverseTransformDirection(_rigidbody.velocity);
			float z = vector.z;
			reverse = (vector.z < 0f);
			HandbreakInput = Mathf.Clamp(handbrake, 0f, 1f);
			bool flag = false;
			float num2 = 1f;
			if (brake > 0f)
			{
				flag = true;
				num2 = brake;
				num = 0f;
			}
			if (z < 0f - maxBackSpeed && verticalInput < 0f)
			{
				num = 0f;
			}
			backWheel.motorTorque = num;
			frontWheel.motorTorque = num / 4f;
			float num3 = maxSteerAngle * horizontalInput;
			float num4 = Mathf.Abs(vector.z) * 3.6f;
			if (num4 > maxSteerLimitSpeed)
			{
				num3 = Mathf.Clamp(num3, 0f - minSteerAngle, minSteerAngle);
			}
			else if (num4 > startLimitSteerSpeed)
			{
				float num5 = Mathf.Abs(num3);
				float num6 = maxSteerLimitSpeed - startLimitSteerSpeed;
				float num7 = num4 - startLimitSteerSpeed;
				float num8 = (num3 == 0f) ? 1f : (num3 / num5);
				num3 = Mathf.Clamp(num5, 0f, minSteerAngle + (maxSteerAngle - minSteerAngle) * (1f - num7 / num6)) * num8;
			}
			if (!flag && (verticalInput != 0f || !(Mathf.Abs(vector.z) < tresholdStopSpeed)))
			{
				float num11 = frontWheel.brakeTorque = (backWheel.brakeTorque = 0f);
			}
			else
			{
				float num11 = frontWheel.brakeTorque = (backWheel.brakeTorque = breakTorque * num2);
			}
			if (!AiOptimized && !backWheel.isGrounded && !frontWheel.isGrounded && (base.transform.rotation.eulerAngles.x > 270f || base.transform.rotation.eulerAngles.x < maxFrontTiltAngle))
			{
				num3 = 0f;
				_rigidbody.AddRelativeTorque(new Vector3(frontTiltForce, 0f, 0f));
			}
			frontWheel.steerAngle = num3;
			CalculateRevs();
			GearChanging();
		}

		private void GearChanging()
		{
			float num = Mathf.Abs(CurrentSpeed / MaxSpeed);
			float num2 = 1f / (float)NoOfGears * (float)(m_GearNum + 1);
			float num3 = 1f / (float)NoOfGears * (float)m_GearNum;
			if (m_GearNum > 0 && num < num3)
			{
				m_GearNum--;
			}
			if (num > num2 && m_GearNum < NoOfGears - 1)
			{
				m_GearNum++;
			}
		}

		private void CalculateGearFactor()
		{
			float num = 1f / (float)NoOfGears;
			float b = Mathf.InverseLerp(num * (float)m_GearNum, num * (float)(m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
			m_GearFactor = Mathf.Lerp(m_GearFactor, b, Time.deltaTime * 5f);
		}

		private void CalculateRevs()
		{
			CalculateGearFactor();
			float num = (float)m_GearNum / (float)NoOfGears;
			float from = ULerp(0f, m_RevRangeBoundary, CurveFactor(num));
			float to = ULerp(m_RevRangeBoundary, 1f, num);
			Revs = ULerp(from, to, m_GearFactor);
		}

		private static float CurveFactor(float factor)
		{
			return 1f - (1f - factor) * (1f - factor);
		}

		private static float ULerp(float from, float to, float value)
		{
			return (1f - value) * from + value * to;
		}
	}
}
