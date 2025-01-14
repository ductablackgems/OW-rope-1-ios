using App.Prefabs;
using App.Spawn;
using App.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Airplane
{
	public class AirplaneController : MonoBehaviour, IAirplaneController
	{
		public const float MIN_VELOCITY = 0.1f;

		public const float CRASH_DAMAGE_INTERVAL = 1f;

		public const float IS_STUCKED_CHECK_INTERVAL = 0.5f;

		[SerializeField]
		private float m_MaxEnginePower = 40f;

		[SerializeField]
		private float m_Lift = 0.002f;

		[SerializeField]
		private float m_ZeroLiftSpeed = 300f;

		[SerializeField]
		private float m_RollEffect = 1f;

		[SerializeField]
		private float m_PitchEffect = 1f;

		[SerializeField]
		private float m_YawEffect = 0.2f;

		[SerializeField]
		private float m_BankedTurnEffect = 0.5f;

		[SerializeField]
		private float m_AerodynamicEffect = 0.02f;

		[SerializeField]
		private float m_AutoTurnPitch = 0.5f;

		[SerializeField]
		private float m_AutoRollLevel = 0.2f;

		[SerializeField]
		private float m_AutoPitchLevel = 0.2f;

		[SerializeField]
		private float m_AirBrakesEffect = 3f;

		[SerializeField]
		private float m_ThrottleChangeSpeed = 0.3f;

		[SerializeField]
		private float m_DragIncreaseFactor = 0.001f;

		[Space]
		[SerializeField]
		private Transform m_CenterOfMass;

		[Range(5f, 100f)]
		[SerializeField]
		private float minCrashSpeed = 20f;

		[Range(0f, 1f)]
		[SerializeField]
		private float maxCrashDamage = 0.25f;

		[SerializeField]
		private Bounds airplaneBounds;

		[Header("Wheels")]
		[SerializeField]
		private float groundRotationSpeed = 5f;

		private float m_OriginalDrag;

		private float m_OriginalAngularDrag;

		private float m_AeroFactor;

		private bool m_IsActive;

		private float m_BankedTurnAmount;

		private Rigidbody m_Rigidbody;

		private float m_Mass;

		private Health m_Health;

		private int m_LayerMask;

		private bool m_IsInitialized;

		private AirplaneAudio m_Audio;

		private DurationTimer m_StuckCheckTimer = new DurationTimer(useFixedTime: true);

		private DurationTimer m_CrashDamageTimer = new DurationTimer();

		private List<AirplaneWheel> m_AeroplaneWheels = new List<AirplaneWheel>(4);

		private RaycastHit[] m_Hits = new RaycastHit[8];

		private Collider[] m_Colliders = new Collider[8];

		private Vector3 m_SpawnPosition;

		private Quaternion m_SpawnRotation;

		private VehiclePrefabId m_ID;

		private static WhoIsItemDefinition[] m_IgnoredDefinitions;

		public float Altitude
		{
			get;
			private set;
		}

		public float Throttle
		{
			get;
			private set;
		}

		public bool AirBrakes
		{
			get;
			private set;
		}

		public float ForwardSpeed
		{
			get;
			private set;
		}

		public float EnginePower
		{
			get;
			private set;
		}

		public float RollAngle
		{
			get;
			private set;
		}

		public float PitchAngle
		{
			get;
			private set;
		}

		public float RollInput
		{
			get;
			private set;
		}

		public float PitchInput
		{
			get;
			private set;
		}

		public float YawInput
		{
			get;
			private set;
		}

		public float ThrottleInput
		{
			get;
			private set;
		}

		public bool IsActive
		{
			get
			{
				return m_IsActive;
			}
			set
			{
				SetIsActive(value);
			}
		}

		public bool IsStucked
		{
			get;
			private set;
		}

		public float MaxEnginePower => m_MaxEnginePower * m_Mass;

		public Bounds Bounds => airplaneBounds;

		public VehiclePrefabId ID => GetID();

		public bool EnableDistanceDestroy
		{
			get;
			set;
		}

		private float Lift => m_Lift * m_Mass;

		private float RollEffect => m_RollEffect * m_Mass;

		private float PitchEffect => m_PitchEffect * m_Mass;

		private float YawEffect => m_YawEffect * m_Mass;

		private float BankedTurnEffect => m_BankedTurnEffect * m_Mass;

		public event Action<AirplaneController> Destroyed;


		public void Initialize(bool enableDistanceDestroy = true)
		{
			if (!m_IsInitialized)
			{
				EnableDistanceDestroy = enableDistanceDestroy;
				m_SpawnPosition = base.transform.position;
				m_SpawnRotation = base.transform.rotation;
				m_Health = GetComponent<Health>();
				m_Rigidbody = GetComponent<Rigidbody>();
				m_Audio = GetComponent<AirplaneAudio>();
				GetComponentsInChildren(m_AeroplaneWheels);
				m_LayerMask = LayerMask.GetMask("Ground");
				m_Mass = m_Rigidbody.mass;
				m_OriginalDrag = m_Rigidbody.drag;
				m_OriginalAngularDrag = m_Rigidbody.angularDrag;
				if (m_CenterOfMass != null)
				{
					m_Rigidbody.centerOfMass = m_CenterOfMass.localPosition;
				}
				m_IsInitialized = true;
			}
		}

		public void Spawn(SpawnPoint spawnPoint)
		{
			base.transform.position = spawnPoint.Position;
			base.transform.rotation = spawnPoint.transform.rotation;
			m_SpawnPosition = base.transform.position;
			m_SpawnRotation = base.transform.rotation;
		}

		public void Move(float rollInput, float pitchInput, float yawInput, float throttleInput, bool airBrakes)
		{
			RollInput = rollInput;
			PitchInput = pitchInput;
			YawInput = yawInput;
			ThrottleInput = throttleInput;
			AirBrakes = airBrakes;
			ClampInputs();
			CalculateRollAndPitchAngles();
			AutoLevel();
			CalculateForwardSpeed();
			ControlThrottle();
			CalculateDrag();
			CaluclateAerodynamicEffect();
			CalculateLinearForces();
			CalculateTorque();
			if (CheckGroundDistance(8f, -base.transform.up) && IsOnWheels())
			{
				float angle = yawInput * groundRotationSpeed * Time.deltaTime;
				base.transform.Rotate(base.transform.up, angle);
				if (throttleInput < 0f)
				{
					base.transform.position = base.transform.position + -base.transform.forward.normalized * Time.deltaTime;
				}
			}
		}

		public void Respawn(bool isRepair = false)
		{
			base.transform.position = m_SpawnPosition;
			base.transform.rotation = m_SpawnRotation;
			m_Rigidbody.velocity = Vector3.zero;
			m_Rigidbody.drag = m_OriginalDrag;
			m_Rigidbody.angularDrag = m_OriginalAngularDrag;
			Throttle = 0f;
			EnginePower = 0f;
			if (isRepair)
			{
				m_Health.Heal(m_Health.maxHealth);
			}
			IsStucked = false;
		}

		private void Start()
		{
			Initialize();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!IsActive || ForwardSpeed <= minCrashSpeed || !m_CrashDamageTimer.Done())
			{
				return;
			}
			m_CrashDamageTimer.Run(1f);
			if (!WhoIs.Resolve(collision.collider, GetDefinitions()).IsEmpty)
			{
				return;
			}
			bool flag = false;
			ContactPoint[] contacts = collision.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				if (flag)
				{
					break;
				}
				Collider thisCollider = contacts[i].thisCollider;
				flag = !IsWheelCollision(thisCollider);
			}
			if (flag)
			{
				float num = m_Health.maxHealth * maxCrashDamage;
				float damage = ForwardSpeed / 100f * num;
				m_Health.ApplyDamage(damage);
				m_Audio.PlayCrashSound();
			}
		}

		private void FixedUpdate()
		{
			if (IsActive)
			{
				CheckIsStucked();
			}
		}

		private void OnDestroy()
		{
			if (this.Destroyed != null)
			{
				this.Destroyed(this);
			}
		}

		private void ClampInputs()
		{
			RollInput = Mathf.Clamp(RollInput, -1f, 1f);
			PitchInput = Mathf.Clamp(PitchInput, -1f, 1f);
			YawInput = Mathf.Clamp(YawInput, -1f, 1f);
			ThrottleInput = Mathf.Clamp(ThrottleInput, -1f, 1f);
		}

		private void CalculateRollAndPitchAngles()
		{
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			if (forward.sqrMagnitude > 0f)
			{
				forward.Normalize();
				Vector3 vector = base.transform.InverseTransformDirection(forward);
				PitchAngle = Mathf.Atan2(vector.y, vector.z);
				Vector3 direction = Vector3.Cross(Vector3.up, forward);
				Vector3 vector2 = base.transform.InverseTransformDirection(direction);
				RollAngle = Mathf.Atan2(vector2.y, vector2.x);
			}
		}

		private void AutoLevel()
		{
			m_BankedTurnAmount = Mathf.Sin(RollAngle);
			if (RollInput == 0f)
			{
				RollInput = (0f - RollAngle) * m_AutoRollLevel;
			}
			if (PitchInput == 0f)
			{
				PitchInput = (0f - PitchAngle) * m_AutoPitchLevel;
				PitchInput -= Mathf.Abs(m_BankedTurnAmount * m_BankedTurnAmount * m_AutoTurnPitch);
			}
		}

		private void CalculateForwardSpeed()
		{
			Vector3 vector = base.transform.InverseTransformDirection(m_Rigidbody.velocity);
			ForwardSpeed = Mathf.Max(0f, vector.z);
		}

		private void ControlThrottle()
		{
			Throttle = Mathf.Clamp01(Throttle + ThrottleInput * Time.deltaTime * m_ThrottleChangeSpeed);
			EnginePower = Throttle * MaxEnginePower;
		}

		private void CalculateDrag()
		{
			float magnitude = m_Rigidbody.velocity.magnitude;
			if (magnitude < 0.1f)
			{
				m_Rigidbody.drag = m_OriginalDrag;
				m_Rigidbody.angularDrag = m_OriginalAngularDrag;
			}
			else
			{
				float num = magnitude * m_DragIncreaseFactor;
				m_Rigidbody.drag = (AirBrakes ? ((m_OriginalDrag + num) * m_AirBrakesEffect) : (m_OriginalDrag + num));
				m_Rigidbody.angularDrag = m_OriginalAngularDrag * ForwardSpeed;
			}
		}

		private void CaluclateAerodynamicEffect()
		{
			if (!(m_Rigidbody.velocity.magnitude < 0.1f))
			{
				m_AeroFactor = Vector3.Dot(base.transform.forward, m_Rigidbody.velocity.normalized);
				m_AeroFactor *= m_AeroFactor;
				Vector3 velocity = Vector3.Lerp(m_Rigidbody.velocity, base.transform.forward * ForwardSpeed, m_AeroFactor * ForwardSpeed * m_AerodynamicEffect * Time.deltaTime);
				m_Rigidbody.velocity = velocity;
				Quaternion b = Quaternion.LookRotation(m_Rigidbody.velocity, base.transform.up);
				m_Rigidbody.rotation = Quaternion.Slerp(m_Rigidbody.rotation, b, m_AerodynamicEffect * Time.deltaTime);
			}
		}

		private void CalculateLinearForces()
		{
			Vector3 zero = Vector3.zero;
			zero += EnginePower * base.transform.forward;
			Vector3 normalized = Vector3.Cross(m_Rigidbody.velocity, base.transform.right).normalized;
			float num = Mathf.InverseLerp(m_ZeroLiftSpeed, 0f, ForwardSpeed);
			float d = ForwardSpeed * ForwardSpeed * Lift * num * m_AeroFactor;
			zero += d * normalized;
			m_Rigidbody.AddForce(zero);
		}

		private void CalculateTorque()
		{
			Vector3 zero = Vector3.zero;
			zero += PitchInput * PitchEffect * base.transform.right;
			zero += YawInput * YawEffect * base.transform.up;
			zero += (0f - RollInput) * RollEffect * base.transform.forward;
			zero += m_BankedTurnAmount * BankedTurnEffect * base.transform.up;
			m_Rigidbody.AddTorque(zero * ForwardSpeed * m_AeroFactor);
		}

		private bool IsWheelCollision(Collider collider)
		{
			for (int i = 0; i < m_AeroplaneWheels.Count; i++)
			{
				if (m_AeroplaneWheels[i].Collider == collider)
				{
					return true;
				}
			}
			return false;
		}

		private void BrakeWheels(bool isBrake)
		{
			for (int i = 0; i < m_AeroplaneWheels.Count; i++)
			{
				m_AeroplaneWheels[i].WheelCollider.brakeTorque = (isBrake ? 1f : 0f);
			}
		}

		private bool IsOnWheels()
		{
			for (int i = 0; i < m_AeroplaneWheels.Count; i++)
			{
				if (!m_AeroplaneWheels[i].IsGrounded())
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckGroundDistance(float distance, Vector3 direction)
		{
			return Physics.SphereCastNonAlloc(base.transform.position, 0.1f, direction, m_Hits, distance, m_LayerMask, QueryTriggerInteraction.Ignore) > 0;
		}

		private bool GetIsStucked()
		{
			if (ForwardSpeed > 1f)
			{
				return false;
			}
			if (Physics.OverlapBoxNonAlloc(base.transform.position, airplaneBounds.size, m_Colliders, base.transform.rotation, m_LayerMask, QueryTriggerInteraction.Ignore) == 0)
			{
				return false;
			}
			if (IsOnWheels())
			{
				return false;
			}
			return true;
		}

		private void CheckIsStucked()
		{
			if (ForwardSpeed > 1f)
			{
				IsStucked = false;
			}
			else if (m_StuckCheckTimer.Done())
			{
				IsStucked = GetIsStucked();
				m_StuckCheckTimer.Run(0.5f);
			}
		}

		private void SetIsActive(bool isActive)
		{
			m_IsActive = isActive;
			if (m_IsActive)
			{
				m_StuckCheckTimer.Run(0.5f);
				m_CrashDamageTimer.Run(1f);
				return;
			}
			m_StuckCheckTimer.Stop();
			m_CrashDamageTimer.Stop();
			Throttle = 0f;
			EnginePower = 0f;
		}

		private VehiclePrefabId GetID()
		{
			if (m_ID == null)
			{
				m_ID = GetComponent<VehiclePrefabId>();
			}
			return m_ID;
		}

		private WhoIsItemDefinition[] GetDefinitions()
		{
			if (m_IgnoredDefinitions != null)
			{
				return m_IgnoredDefinitions;
			}
			List<WhoIsItemDefinition> list = new List<WhoIsItemDefinition>(16);
			list.AddRange(WhoIs.Masks.AllRagdollableHumans);
			list.AddRange(WhoIs.Masks.AllStreetVehicles);
			m_IgnoredDefinitions = list.ToArray();
			return m_IgnoredDefinitions;
		}

		private void OnDrawGizmos()
		{
			Rigidbody component = GetComponent<Rigidbody>();
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere((m_CenterOfMass != null) ? m_CenterOfMass.position : ((component != null) ? (base.transform.position + component.centerOfMass) : base.transform.position), 1f);
			DrawAirplaneBounds();
		}

		private void DrawAirplaneBounds()
		{
			Color color = Gizmos.color;
			Color magenta = Color.magenta;
			magenta.a = 0.2f;
			Gizmos.color = magenta;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
			Gizmos.DrawCube(airplaneBounds.center, airplaneBounds.size);
			Gizmos.color = color;
		}
	}
}
