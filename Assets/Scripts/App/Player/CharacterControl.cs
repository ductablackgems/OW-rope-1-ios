using App.Player.Definition;
using App.Util;
using UnityEngine;

namespace App.Player
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class CharacterControl : MonoBehaviour
	{
		public bool optimizedForAI = true;

		public LayerMask groundRaycastMask;

		public float maxAirborneMoveForce = 300f;

		[SerializeField]
		private float m_MovingTurnSpeed = 360f;

		[SerializeField]
		private float m_StationaryTurnSpeed = 180f;

		[SerializeField]
		private float m_JumpPower = 12f;

		[Range(1f, 4f)]
		[SerializeField]
		private float m_GravityMultiplier = 2f;

		[SerializeField]
		private float m_RunCycleLegOffset = 0.2f;

		[SerializeField]
		private float m_MoveSpeedMultiplier = 1f;

		[SerializeField]
		private float m_AnimSpeedMultiplier = 1f;

		[SerializeField]
		private float m_GroundCheckDistance = 0.1f;

		private Rigidbody m_Rigidbody;

		private Animator m_Animator;

		private bool m_IsGrounded;

		private float m_OrigGroundCheckDistance;

		private const float k_Half = 0.5f;

		private float m_TurnAmount;

		private float m_ForwardAmount;

		private Vector3 m_GroundNormal;

		private float m_CapsuleHeight;

		private Vector3 m_CapsuleCenter;

		private CapsuleCollider m_Capsule;

		private bool m_Crouching;

		[SerializeField]
		private float m_StepInterval;

		[SerializeField]
		private bool footsSoundActive;

		[SerializeField]
		private AudioClip[] m_FootstepSounds;

		private float m_StepCycle;

		private float m_NextStep;

		private bool m_Jumping;

		private AudioSource m_AudioSource;

		private PlayerAnimatorHandler animatorHandler;

		private EnergyScript energy;

		private bool preventFastRun;

		private bool m_GateOpen;

		private float duration = 1.5f;

		public bool Grounded => m_IsGrounded;

		public bool RunningFast => animatorHandler.Forward > 1.5f;

		public bool ApplyRotationForAI
		{
			get;
			set;
		}

		public bool BlockRotation
		{
			get;
			set;
		}

		public Vector3 ExtraVelocity
		{
			get;
			set;
		}

		private void ProgressStepCycle(float speed, float deltaTime)
		{
			if (m_Rigidbody.velocity.sqrMagnitude > 0f)
			{
				m_StepCycle += speed * deltaTime;
			}
			if (m_StepCycle > m_NextStep)
			{
				m_NextStep = m_StepCycle + m_StepInterval;
				if (footsSoundActive)
				{
					PlayFootStepAudio();
				}
			}
		}

		private void PlayFootStepAudio()
		{
			if (m_IsGrounded && !animatorHandler.FightMachine.Running && !m_GateOpen && !m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("AdvancedFight"))
			{
				int num = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
				m_AudioSource.clip = m_FootstepSounds[num];
				m_AudioSource.PlayOneShot(m_AudioSource.clip);
				m_FootstepSounds[num] = m_FootstepSounds[0];
				m_FootstepSounds[0] = m_AudioSource.clip;
			}
		}

		private void Awake()
		{
			m_AudioSource = GetComponent<AudioSource>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			energy = GetComponent<EnergyScript>();
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;
			m_StepCycle = 0f;
			m_NextStep = m_StepCycle / 2f;
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}

		public void Move(Vector3 move, bool runFast, bool crouch, bool jump, Vector3 cameraForward = default(Vector3), float deltaTime = -1f)
		{
			if (deltaTime == -1f)
			{
				deltaTime = Time.deltaTime;
			}
			if (runFast && animatorHandler.WalkTreeBlend.TargetValue == 0f && animatorHandler.GroundedState.Running)
			{
				base.transform.LookAt(base.transform.position + cameraForward);
				move = base.transform.forward;
				if (preventFastRun)
				{
					runFast = false;
				}
				else
				{
					energy.ConsumeFastRunEnergy(deltaTime);
				}
				if (energy.GetCurrentEnergy() < 0.05f)
				{
					preventFastRun = true;
				}
			}
			if (preventFastRun && energy.GetCurrentEnergy() > 0.15f)
			{
				preventFastRun = false;
			}
			if (move.magnitude > 1f)
			{
				move.Normalize();
			}
			move = base.transform.InverseTransformDirection(move);
			if (m_Rigidbody.isKinematic)
			{
				m_Animator.applyRootMotion = false;
			}
			else
			{
				CheckGroundStatus();
			}
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump, runFast);
			}
			else
			{
				HandleAirborneMovement();
			}
			bool optimizedForAI2 = optimizedForAI;
			UpdateAnimator(move, runFast, deltaTime);
		}

		public bool IsGroundedRaw(out RaycastHit hitInfo)
		{
			return Physics.Raycast(base.transform.position + base.transform.up * 0.3f, Vector3.down, out hitInfo, 0.5f, groundRaycastMask, QueryTriggerInteraction.Ignore);
		}

		private void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (!m_Crouching)
				{
					m_Capsule.height /= 2f;
					m_Capsule.center /= 2f;
					m_Crouching = true;
				}
			}
			else if (Physics.SphereCast(new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * 0.5f, Vector3.up), maxDistance: m_CapsuleHeight - m_Capsule.radius * 0.5f, radius: m_Capsule.radius * 0.5f, layerMask: -1, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
			}
			else
			{
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		private void PreventStandingInLowHeadroom()
		{
			if (!m_Crouching && Physics.SphereCast(new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * 0.5f, Vector3.up), maxDistance: m_CapsuleHeight - m_Capsule.radius * 0.5f, radius: m_Capsule.radius * 0.5f, layerMask: -1, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
			}
		}

		private void Gate(float deltaTime)
		{
			if (duration > 0f)
			{
				duration -= deltaTime;
				if (duration <= 0f)
				{
					m_GateOpen = false;
					duration = 0.7f;
				}
			}
		}

		private void UpdateAnimator(Vector3 move, bool runFast, float deltaTime)
		{
			m_Animator.SetFloat("Forward", runFast ? 2f : m_ForwardAmount, 0.1f, deltaTime);
			m_Animator.SetFloat("Turn", runFast ? 0f : m_TurnAmount, 0.1f, deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded || base.transform.parent != null);
			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}
			if (animatorHandler.Kick && m_IsGrounded)
			{
				m_GateOpen = true;
				duration = 0.7f;
			}
			if (m_GateOpen)
			{
				Gate(deltaTime);
			}
			if (!optimizedForAI)
			{
				float value = (float)((Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1f) < 0.5f) ? 1 : (-1)) * m_ForwardAmount;
				if (m_IsGrounded)
				{
					m_Animator.SetFloat("JumpLeg", value);
				}
				if (!m_Crouching)
				{
					ProgressStepCycle(move.magnitude, deltaTime);
				}
			}
			if (m_IsGrounded && move.magnitude > 0f)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				m_Animator.speed = 1f;
			}
		}

		private void HandleAirborneMovement()
		{
			Vector3 vector = Physics.gravity * m_GravityMultiplier - Physics.gravity;
			if (base.transform.InverseTransformDirection(m_Rigidbody.velocity).z < 1f)
			{
				vector += base.transform.forward * maxAirborneMoveForce * m_ForwardAmount;
			}
			m_Rigidbody.AddForce(vector);
			m_GroundCheckDistance = ((m_Rigidbody.velocity.y < 0f) ? m_OrigGroundCheckDistance : 0.15f);
		}

		private void HandleGroundedMovement(bool crouch, bool jump, bool runFast)
		{
			if (jump && animatorHandler.GroundedState.Running && !crouch)
			{
				if (runFast)
				{
					animatorHandler.TriggerFrontFlip();
					m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower * 1.55f, m_Rigidbody.velocity.z);
				}
				else
				{
					m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				}
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}

		private void ApplyExtraTurnRotation()
		{
			float num = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			base.transform.Rotate(0f, m_TurnAmount * num * Time.deltaTime, 0f);
		}

		public void OnAnimatorMove()
		{
			if (m_IsGrounded && Time.deltaTime > 0f)
			{
				Vector3 velocity = m_Animator.deltaPosition * m_MoveSpeedMultiplier / Time.deltaTime + ExtraVelocity;
				velocity.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = velocity;
			}
			if (!BlockRotation && ((!optimizedForAI && animatorHandler.WalkTreeBlend.TargetValue == 0f) || ApplyRotationForAI) && Time.deltaTime > 0f)
			{
				ApplyExtraTurnRotation();
			}
		}

		private void CheckGroundStatus()
		{
			RaycastHit hitInfo;
			if (optimizedForAI)
			{
				m_IsGrounded = true;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = true;
			}
			else if (IsGroundedRaw(out hitInfo))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
