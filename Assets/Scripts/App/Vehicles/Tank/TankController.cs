using System;
using UnityEngine;

namespace App.Vehicles.Tank
{
	public class TankController : MonoBehaviour
	{
		public Transform leftWheelCollidersParent;

		public Transform rightWheelCollidersParent;

		public WheelCollider leftIdlerWheel;

		public WheelCollider rightIdlerWheel;

		public float maxMotorTorque = 1000f;

		public float maxBrakeTorque = 500f;

		public float maxSpeed = 80f;

		public float maxBackwardSpeed = 20f;

		public float autoBrakeSpeed = 5f;

		public WheelCollider referenceWheel;

		public float wheelRotationRatio = 1f;

		public float asymetricSpeedDistance = 15f;

		[Space]
		public float steerSpeed = 0.7f;

		public float minimalSteerSpeed = 0.2f;

		[Space]
		public Transform centerOfMass;

		public Vector3 innertiaTensor;

		private Rigidbody _rigidbody;

		private WheelCollider[] leftWheelColliders;

		private WheelCollider[] rightWheelColliders;

		private float wheelPerimeter;

		public AudioSource m_MovementAudio;

		public AudioClip m_EngineIdling;

		public AudioClip m_EngineDriving;

		public float m_PitchRange = 0.2f;

		private float m_OriginalPitch = 1.6f;

		private TankManager tankManager;

		private bool _idle = true;

		public Quaternion LeftWheelRotation
		{
			get;
			private set;
		}

		public float LeftWheelRotationSpeed
		{
			get;
			private set;
		}

		public Quaternion RightWheelRotation
		{
			get;
			private set;
		}

		public float RightWheelRotationSpeed
		{
			get;
			private set;
		}

		private void Awake()
		{
			tankManager = GetComponent<TankManager>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			leftWheelColliders = leftWheelCollidersParent.GetComponentsInChildren<WheelCollider>();
			rightWheelColliders = rightWheelCollidersParent.GetComponentsInChildren<WheelCollider>();
			wheelPerimeter = (float)Math.PI * 2f * referenceWheel.radius * referenceWheel.transform.lossyScale.x;
			_rigidbody.centerOfMass = centerOfMass.localPosition;
			_rigidbody.inertiaTensor = innertiaTensor;
			LeftWheelRotation = Quaternion.identity;
			RightWheelRotation = Quaternion.identity;
			BrakaAllWheels(maxBrakeTorque);
		}

		private void OnEnable()
		{
			BrakaAllWheels(0f);
		}

		private void OnDisable()
		{
			BrakaAllWheels(maxBrakeTorque);
			m_MovementAudio.Stop();
		}

		private void BrakaAllWheels(float torque)
		{
			WheelCollider[] array = leftWheelColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].brakeTorque = torque;
			}
		}

		public void MoveAI(float v, float h)
		{
			EngineAudio(v, h);
			float num = v * 3.6f;
			float num2 = Mathf.Clamp01((maxSpeed - Mathf.Abs(num)) / maxSpeed);
			LeftWheelRotationSpeed = (num + h * asymetricSpeedDistance * num2) * wheelRotationRatio;
			LeftWheelRotation *= Quaternion.Euler(Vector3.right * Time.fixedDeltaTime * LeftWheelRotationSpeed);
			RightWheelRotationSpeed = (num - h * asymetricSpeedDistance * num2) * wheelRotationRatio;
			RightWheelRotation *= Quaternion.Euler(Vector3.right * Time.fixedDeltaTime * RightWheelRotationSpeed);
		}

		public void Move(float v, float h)
		{
			EngineAudio(v, h);
			float num = base.transform.InverseTransformDirection(_rigidbody.velocity).z * 3.6f;
			float expectedRpm = num / wheelPerimeter * 60f + 500f;
			int num2 = 0;
			WheelCollider[] array = leftWheelColliders;
			foreach (WheelCollider wheelCollider in array)
			{
				if (wheelCollider.isGrounded)
				{
					num2++;
				}
				ProccessWheel(wheelCollider, v, num, expectedRpm);
			}
			int num3 = 0;
			array = rightWheelColliders;
			foreach (WheelCollider wheelCollider2 in array)
			{
				if (wheelCollider2.isGrounded)
				{
					num3++;
				}
				ProccessWheel(wheelCollider2, v, num, expectedRpm);
			}
			ProccessWheel(leftIdlerWheel, v, num, expectedRpm);
			ProccessWheel(rightIdlerWheel, v, num, expectedRpm);
			float num4 = Mathf.Clamp01((maxSpeed - Mathf.Abs(num)) / maxSpeed);
			if (num2 > 0 && num3 > 0 && (num2 > 2 || num3 > 2))
			{
				float d = (((num2 < num3) ? num2 : num3) > 2) ? 1f : 0.5f;
				float d2 = Mathf.Lerp(minimalSteerSpeed, steerSpeed, num4);
				_rigidbody.AddRelativeTorque(Vector3.up * d2 * h * d, ForceMode.VelocityChange);
			}
			LeftWheelRotationSpeed = (num + h * asymetricSpeedDistance * num4) * wheelRotationRatio;
			LeftWheelRotation *= Quaternion.Euler(Vector3.right * Time.fixedDeltaTime * LeftWheelRotationSpeed);
			RightWheelRotationSpeed = (num - h * asymetricSpeedDistance * num4) * wheelRotationRatio;
			RightWheelRotation *= Quaternion.Euler(Vector3.right * Time.fixedDeltaTime * RightWheelRotationSpeed);
		}

		private void ProccessWheel(WheelCollider wheelCollider, float v, float speed, float expectedRpm)
		{
			float num = Mathf.Abs(speed);
			wheelCollider.brakeTorque = ((v == 0f && num < autoBrakeSpeed) ? maxBrakeTorque : 0f);
			if (v == 0f || (v > 0f && speed > maxSpeed) || (v < 0f && speed < 0f - maxBackwardSpeed))
			{
				wheelCollider.motorTorque = 0f;
			}
			else if (v > 0f && wheelCollider.rpm > expectedRpm)
			{
				wheelCollider.motorTorque = Mathf.Lerp(v * maxMotorTorque, 0f, (wheelCollider.rpm - expectedRpm) / 3000f);
			}
			else if (v < 0f && wheelCollider.rpm < expectedRpm)
			{
				wheelCollider.motorTorque = Mathf.Lerp(v * maxMotorTorque, 0f, (wheelCollider.rpm - expectedRpm) / 3000f);
			}
			else
			{
				wheelCollider.motorTorque = v * maxMotorTorque;
			}
		}

		private void EngineAudio(float v, float h)
		{
			if (!tankManager.Active)
			{
				m_MovementAudio.Stop();
				return;
			}
			if (!m_MovementAudio.isPlaying)
			{
				m_MovementAudio.Play();
			}
			if (Mathf.Abs(v) < 0.1f && Mathf.Abs(h) < 0.1f)
			{
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Mathf.Lerp(m_MovementAudio.pitch - Mathf.Abs(Mathf.Max(v, h)), m_OriginalPitch, m_PitchRange);
			}
			else
			{
				_idle = false;
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Mathf.Lerp(m_MovementAudio.pitch, m_OriginalPitch + Mathf.Abs(Mathf.Max(v, h)), m_PitchRange);
			}
		}
	}
}
