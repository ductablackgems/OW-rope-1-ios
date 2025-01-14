using App.Vehicles.Car.Navigation;
using App.Vehicles.Skid;
using UnityEngine;

namespace App.Vehicles.Car
{
	public class CarController : MonoBehaviour, ISoundableCar, IHandbrakeVehicle, IVehicleController, IVehicleAnimator
	{
		[SerializeField]
		private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;

		[SerializeField]
		public WheelCollider[] m_WheelColliders = new WheelCollider[6];

		[SerializeField]
		private GameObject[] m_WheelMeshes = new GameObject[6];

		[SerializeField]
		private WheelEffects[] m_WheelEffects = new WheelEffects[6];

		[SerializeField]
		private Vector3 m_CentreOfMassOffset;

		[SerializeField]
		private float m_MaximumSteerAngle;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_SteerHelper;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_TractionControl;

		[SerializeField]
		private float m_FullTorqueOverAllWheels;

		[SerializeField]
		private float m_ReverseTorque;

		[SerializeField]
		private float m_MaxHandbrakeTorque;

		[SerializeField]
		private float m_Downforce = 100f;

		[SerializeField]
		private SpeedType m_SpeedType;

		[SerializeField]
		private float m_Topspeed = 200f;

		[SerializeField]
		private int NoOfGears = 5;

		[SerializeField]
		private float m_RevRangeBoundary = 1f;

		[SerializeField]
		private float m_SlipLimit;

		[SerializeField]
		private float m_BrakeTorque;

		private Vector3 m_Prevpos;

		private Vector3 m_Pos;

		private float m_SteerAngle;

		private int m_GearNum;

		private float m_GearFactor;

		private float m_OldRotation;

		private float m_CurrentTorque;

		private Rigidbody m_Rigidbody;

		private const float k_ReversingThreshold = 0.01f;

		private bool animated = true;

		private Vector3[] initialWheelPositions;

		private Quaternion[] initialWheelRotations;

		public WheelEffects[] WheelEffects => m_WheelEffects;

		public WheelCollider[] WheelColliders => m_WheelColliders;

		public bool Skidding
		{
			get;
			private set;
		}

		public float BrakeInput
		{
			get;
			private set;
		}

		public float HandbreakInput
		{
			get;
			private set;
		}

		public float CurrentSteerAngle => m_SteerAngle;

		public float CurrentSpeed => m_Rigidbody.velocity.magnitude * 2.23693633f;

		public float MaxSpeed => m_Topspeed;

		public float MaxSteer => m_MaximumSteerAngle;

		public float Revs
		{
			get;
			private set;
		}

		public float AccelInput
		{
			get;
			private set;
		}

		public bool OptimizedForAI
		{
			get;
			set;
		}

		public float GetRevs()
		{
			return Revs;
		}

		public float GetAccelerationInput()
		{
			return AccelInput;
		}

		public void SetOptimizedForAI(bool value)
		{
			OptimizedForAI = value;
		}

		public void Animate(float deltaTime)
		{
			if (Animated())
			{
				for (int i = 0; i < m_WheelColliders.Length; i++)
				{
					m_WheelColliders[i].GetWorldPose(out Vector3 pos, out Quaternion quat);
					m_WheelMeshes[i].transform.position = pos;
					m_WheelMeshes[i].transform.rotation = quat;
				}
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
				for (int i = 0; i < m_WheelMeshes.Length; i++)
				{
					GameObject obj = m_WheelMeshes[i];
					obj.transform.localPosition = initialWheelPositions[i];
					obj.transform.localRotation = initialWheelRotations[i];
				}
			}
			this.animated = animated;
		}

		private void Awake()
		{
			m_Rigidbody = GetComponent<Rigidbody>();
			initialWheelPositions = new Vector3[m_WheelMeshes.Length];
			initialWheelRotations = new Quaternion[m_WheelMeshes.Length];
			for (int i = 0; i < m_WheelMeshes.Length; i++)
			{
				GameObject gameObject = m_WheelMeshes[i];
				initialWheelPositions[i] = gameObject.transform.localPosition;
				initialWheelRotations[i] = gameObject.transform.localRotation;
			}
		}

		private void Start()
		{
			m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
			m_MaxHandbrakeTorque = float.MaxValue;
			m_CurrentTorque = m_FullTorqueOverAllWheels - m_TractionControl * m_FullTorqueOverAllWheels;
			Move(0f, 0f, 0f, 0f);
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

		private static float CurveFactor(float factor)
		{
			return 1f - (1f - factor) * (1f - factor);
		}

		private static float ULerp(float from, float to, float value)
		{
			return (1f - value) * from + value * to;
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

		public void Move(float steering, float accel, float footbrake, float handbrake)
		{
			if (accel == 0f && footbrake == 0f && handbrake == 0f && Mathf.Abs(base.transform.InverseTransformDirection(m_Rigidbody.velocity).z) < 1.5f)
			{
				handbrake = 1f;
			}
			steering = Mathf.Clamp(steering, -1f, 1f);
			AccelInput = accel;
			BrakeInput = (footbrake = -1f * Mathf.Clamp(footbrake, -1f, 0f));
			HandbreakInput = (handbrake = Mathf.Clamp(handbrake, 0f, 1f));
			m_SteerAngle = steering * m_MaximumSteerAngle;
			m_WheelColliders[0].steerAngle = m_SteerAngle;
			m_WheelColliders[1].steerAngle = m_SteerAngle;
			if (handbrake == 0f)
			{
				for (int i = 2; i < m_WheelColliders.Length; i++)
				{
					m_WheelColliders[i].brakeTorque = 0f;
				}
			}
			if (!OptimizedForAI)
			{
				SteerHelper();
			}
			ApplyDrive(accel, footbrake);
			if (!OptimizedForAI)
			{
				CapSpeed();
			}
			if (handbrake > 0f)
			{
				float brakeTorque = handbrake * m_MaxHandbrakeTorque;
				m_WheelColliders[2].brakeTorque = brakeTorque;
				m_WheelColliders[3].brakeTorque = brakeTorque;
			}
			CalculateRevs();
			GearChanging();
			if (!OptimizedForAI)
			{
				AddDownForce();
			}
			TractionControl();
		}

		private void CapSpeed()
		{
			float magnitude = m_Rigidbody.velocity.magnitude;
			switch (m_SpeedType)
			{
			case SpeedType.MPH:
				magnitude *= 2.23693633f;
				if (magnitude > m_Topspeed)
				{
					m_Rigidbody.velocity = m_Topspeed / 2.23693633f * m_Rigidbody.velocity.normalized;
				}
				break;
			case SpeedType.KPH:
				magnitude *= 3.6f;
				if (magnitude > m_Topspeed)
				{
					m_Rigidbody.velocity = m_Topspeed / 3.6f * m_Rigidbody.velocity.normalized;
				}
				break;
			}
		}

		private void ApplyDrive(float accel, float footbrake)
		{
			switch (m_CarDriveType)
			{
			case CarDriveType.FourWheelDrive:
			{
				float num = (!(accel > 0f)) ? (m_ReverseTorque * accel) : (accel * (m_CurrentTorque / (float)m_WheelColliders.Length));
				for (int i = 0; i < m_WheelColliders.Length; i++)
				{
					m_WheelColliders[i].motorTorque = num;
				}
				break;
			}
			case CarDriveType.FrontWheelDrive:
			{
				float num = (!(accel > 0f)) ? (m_ReverseTorque * accel) : (accel * (m_CurrentTorque / 2f));
				float num4 = m_WheelColliders[0].motorTorque = (m_WheelColliders[1].motorTorque = num);
				break;
			}
			case CarDriveType.RearWheelDrive:
			{
				float num = (!(accel > 0f)) ? (m_ReverseTorque * accel) : (accel * (m_CurrentTorque / 2f));
				float num4 = m_WheelColliders[2].motorTorque = (m_WheelColliders[3].motorTorque = num);
				break;
			}
			}
			for (int j = 0; j < m_WheelColliders.Length; j++)
			{
				m_WheelColliders[j].brakeTorque = m_BrakeTorque * footbrake;
			}
		}

		public void UpdateOldRotation()
		{
			m_OldRotation = base.transform.eulerAngles.y;
		}

		public void SteerHelper()
		{
			if (OptimizedForAI)
			{
				m_WheelColliders[0].GetGroundHit(out WheelHit hit);
				if (hit.normal == Vector3.zero)
				{
					return;
				}
				m_WheelColliders[2].GetGroundHit(out hit);
				if (hit.normal == Vector3.zero)
				{
					return;
				}
			}
			else
			{
				for (int i = 0; i < m_WheelColliders.Length; i++)
				{
					m_WheelColliders[i].GetGroundHit(out WheelHit hit2);
					if (hit2.normal == Vector3.zero)
					{
						return;
					}
				}
			}
			if (Mathf.Abs(m_OldRotation - base.transform.eulerAngles.y) < 10f)
			{
				Quaternion rotation = Quaternion.AngleAxis((base.transform.eulerAngles.y - m_OldRotation) * m_SteerHelper, Vector3.up);
				m_Rigidbody.velocity = rotation * m_Rigidbody.velocity;
			}
			m_OldRotation = base.transform.eulerAngles.y;
		}

		public bool MoveEachFrame()
		{
			return false;
		}

		private void AddDownForce()
		{
			m_WheelColliders[0].attachedRigidbody.AddForce(-base.transform.up * m_Downforce * m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
		}

		private void CheckForWheelSpin()
		{
			for (int i = 0; i < m_WheelColliders.Length; i++)
			{
				m_WheelColliders[i].GetGroundHit(out WheelHit hit);
				if (Mathf.Abs(hit.forwardSlip) >= m_SlipLimit || Mathf.Abs(hit.sidewaysSlip) >= m_SlipLimit)
				{
					m_WheelEffects[i].EmitTyreSmoke();
				}
				else
				{
					m_WheelEffects[i].EndSkidTrail();
				}
			}
		}

		private void TractionControl()
		{
			WheelHit hit;
			switch (m_CarDriveType)
			{
			case CarDriveType.FourWheelDrive:
				for (int i = 0; i < m_WheelColliders.Length; i++)
				{
					m_WheelColliders[i].GetGroundHit(out hit);
					AdjustTorque(hit.forwardSlip);
				}
				break;
			case CarDriveType.RearWheelDrive:
				m_WheelColliders[2].GetGroundHit(out hit);
				AdjustTorque(hit.forwardSlip);
				m_WheelColliders[3].GetGroundHit(out hit);
				AdjustTorque(hit.forwardSlip);
				break;
			case CarDriveType.FrontWheelDrive:
				m_WheelColliders[0].GetGroundHit(out hit);
				AdjustTorque(hit.forwardSlip);
				m_WheelColliders[1].GetGroundHit(out hit);
				AdjustTorque(hit.forwardSlip);
				break;
			}
		}

		private void AdjustTorque(float forwardSlip)
		{
			if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0f)
			{
				m_CurrentTorque -= 10f * m_TractionControl;
				return;
			}
			m_CurrentTorque += 10f * m_TractionControl;
			if (m_CurrentTorque > m_FullTorqueOverAllWheels)
			{
				m_CurrentTorque = m_FullTorqueOverAllWheels;
			}
		}

		public void SetNewSpeed(float newSpeed)
		{
			m_Topspeed = newSpeed;
		}
	}
}
