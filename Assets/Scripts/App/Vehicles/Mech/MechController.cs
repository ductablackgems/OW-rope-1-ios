using App.Abilities;
using App.Util;
using UnityEngine;
using UnityEngine.AI;

namespace App.Vehicles.Mech
{
	public class MechController : MonoBehaviour, IDamageBlocker
	{
		public float RunThreshold = 0.5f;

		public float MaxForwardSpeed = 7f;

		public float MaxBackwardSpeed = 4f;

		public float RotationSpeed = 80f;

		public float WalkSoundInterval = 0.5f;

		public float RunSoundInteval = 0.8f;

		public AbilityShockWave AbilityShockWavePrefab;

		public AbilityAbsorbShield AbilityShieldPrefab;

		public Transform ShieldPosition;

		public GameObject FallingEffect;

		[SerializeField]
		private Bounds BoxCast = new Bounds(Vector3.zero, new Vector3(1.5f, 0.1f, 1.5f));

		private readonly int AnimSpeedID = Animator.StringToHash("speed");

		private readonly int AnimTurnID = Animator.StringToHash("turn");

		private Health health;

		private float nextStepTime;

		private bool isActive;

		private Vector2 move;

		private AbilityShockWave abilityShockWave;

		private AbilityAbsorbShield abilityShield;

		private MechTechnology technology;

		private NavMeshAgent navAgent;

		private RaycastHit[] hits = new RaycastHit[128];

		private int layerMask;

		private float cleanTimer;

		public Animator Animator
		{
			get;
			private set;
		}

		public MechSounds Sounds
		{
			get;
			private set;
		}

		public MechCabinRotator Cabine
		{
			get;
			private set;
		}

		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				SetIsActive(value);
			}
		}

		public bool CanFly
		{
			get
			{
				if (technology != null)
				{
					return technology.CanFly;
				}
				return false;
			}
		}

		public bool IsFlying
		{
			get
			{
				if (technology != null)
				{
					return technology.IsFlying;
				}
				return false;
			}
		}

		public bool IsGrounded => GetIsGrounded();

		public Rigidbody Rigidbody
		{
			get;
			private set;
		}

		public MechTechnology Technology => technology;

		public bool IsAI => navAgent != null;

		private bool IsTechnologyActive
		{
			get
			{
				if (technology != null)
				{
					return technology.IsActive;
				}
				return false;
			}
		}

		private void Awake()
		{
			Animator = GetComponent<Animator>();
			Sounds = GetComponentInChildren<MechSounds>();
			health = GetComponent<Health>();
			Rigidbody = GetComponent<Rigidbody>();
			technology = GetComponent<MechTechnology>();
			navAgent = GetComponent<NavMeshAgent>();
			Cabine = GetComponentInChildren<MechCabinRotator>();
			layerMask = LayerMask.GetMask("Ground", "Climbable", "Impact", "Enemy");
			if (technology != null)
			{
				technology.Initialize(this);
			}
			InitializeAbility(AbilityShockWavePrefab, ref abilityShockWave);
			InitializeAbility(AbilityShieldPrefab, ref abilityShield);
		}

		public void ToggleTechnology()
		{
			if (technology == null)
			{
				return;
			}
			if (technology.CanActivate)
			{
				technology.Activate();
				if (!technology.IsAbilitySupported(abilityShield))
				{
					SetActiveShield(isActive: false);
				}
			}
			else
			{
				technology.Deactivate();
				SetActiveShield(isActive: true);
			}
		}

		public void Move(Vector2 move)
		{
			if (!(health.GetCurrentHealth() <= 0f))
			{
				this.move = ((move.magnitude > 1f) ? move.normalized : move);
				cleanTimer = 0.25f;
			}
		}

		public void ActivateShockwave(Vector3 position)
		{
			if (!(abilityShockWave == null))
			{
				abilityShockWave.Activate(position);
			}
		}

		private void FixedUpdate()
		{
			UpdateMovement(Time.fixedDeltaTime);
			cleanTimer = Mathf.Max(0f, cleanTimer - Time.fixedDeltaTime);
			if (cleanTimer == 0f)
			{
				move = Vector2.zero;
			}
		}

		public void MoveUp()
		{
			if (!(technology == null))
			{
				technology.MoveUp();
			}
		}

		public void MoveDown()
		{
			if (!(technology == null))
			{
				technology.MoveDown();
			}
		}

		public bool TryLand()
		{
			Vector3 groundHit = GetGroundHit(5f);
			if (groundHit == Vector3.zero)
			{
				return false;
			}
			if (base.transform.position.y - groundHit.y > 1f)
			{
				return false;
			}
			Vector3 position = base.transform.position;
			position.y = groundHit.y + 0.1f;
			ActivateShockwave(position);
			return true;
		}

		public void SetActiveFallingEffect(bool isActive)
		{
			if (!(FallingEffect == null))
			{
				FallingEffect.SetActive(isActive);
			}
		}

		public Vector3 MoveAccordingToCameraDiraction(float deltaTime)
		{
			Transform transform = Cabine.transform;
			Vector3 vector = move.y * transform.forward.normalized + move.x * transform.right.normalized;
			vector.y = 0f;
			if (vector != Vector3.zero)
			{
				Quaternion b = Quaternion.LookRotation(vector, Vector3.up);
				Quaternion rotation = Quaternion.Lerp(base.transform.rotation, b, deltaTime * 3f);
				Vector3 forward = Cabine.transform.forward;
				base.transform.rotation = rotation;
				Cabine.transform.forward = forward;
			}
			return vector;
		}

		bool IDamageBlocker.IsDamageBlocked(float damage)
		{
			if (abilityShield == null)
			{
				return false;
			}
			if (!abilityShield.IsRunning)
			{
				return false;
			}
			return abilityShield.AbsorbDamage(damage);
		}

		private void UpdateMovement(float deltaTime)
		{
			float magnitude = move.magnitude;
			float num = magnitude * MaxForwardSpeed;
			bool isRun = magnitude > RunThreshold;
			float turn = move.x * RotationSpeed * deltaTime;
			if (IsTechnologyActive)
			{
				technology.Move(move);
				technology.Turn(turn);
				return;
			}
			if (!IsAI)
			{
				Rigidbody.velocity = MoveAccordingToCameraDiraction(deltaTime) * num;
			}
			else
			{
				UpdateRotation(turn);
			}
			UpdateMoveAnimation(num, isRun);
			UpdateFootSteps(num, isRun);
		}

		private void UpdateMoveAnimation(float speed, bool isRun)
		{
			float value = 0f;
			if (speed != 0f)
			{
				value = (isRun ? 2f : 1f);
			}
			Animator.SetFloat(AnimSpeedID, value);
		}

		private void UpdateRotation(float turn)
		{
			Animator.SetFloat(AnimTurnID, turn);
			if (turn != 0f)
			{
				Vector3 forward = Cabine.transform.forward;
				base.transform.Rotate(Vector3.up, turn);
				Cabine.transform.forward = forward;
			}
		}

		private void UpdateFootSteps(float speed, bool isRun)
		{
			if (IsTechnologyActive)
			{
				return;
			}
			nextStepTime -= Time.deltaTime;
			if (!(nextStepTime > 0f) && Mathf.Abs(speed) != 0f)
			{
				AudioClip nextSound = GetNextSound(Sounds.FootStepClips);
				if (!(nextSound == null))
				{
					nextStepTime = (isRun ? RunSoundInteval : WalkSoundInterval);
					Sounds.FootSteps.PlayOneShot(nextSound);
					nextSound = GetNextSound(Sounds.LegsMoveClips);
					Sounds.LegMoves.PlayOneShot(nextSound);
				}
			}
		}

		private AudioClip GetNextSound(AudioClip[] clips)
		{
			int num = clips.Length;
			switch (num)
			{
			case 0:
				return null;
			case 1:
				return clips[0];
			default:
			{
				int num2 = UnityEngine.Random.Range(1, num);
				AudioClip audioClip = clips[num2];
				clips[num2] = clips[0];
				clips[0] = audioClip;
				return audioClip;
			}
			}
		}

		private void SetIsActive(bool isActive)
		{
			this.isActive = isActive;
			if (isActive)
			{
				Sounds.Engine.Play();
				SetActiveShield(isActive: true);
				return;
			}
			Sounds.Engine.Stop();
			SetActiveShield(isActive: false);
			if (IsTechnologyActive)
			{
				technology.Deactivate();
			}
		}

		private void SetActiveShield(bool isActive)
		{
			if (abilityShield == null)
			{
				return;
			}
			if (isActive)
			{
				if (!(ShieldPosition != null))
				{
					Vector3 position = base.transform.position;
				}
				else
				{
					Vector3 position2 = ShieldPosition.position;
				}
				abilityShield.Activate(ShieldPosition.position);
			}
			else
			{
				abilityShield.Deactivate();
			}
		}

		private Vector3 GetGroundHit(float raycastDistance)
		{
			float num = float.MaxValue;
			float d = 1f;
			int num2 = Physics.BoxCastNonAlloc(base.transform.position + Vector3.up * d, BoxCast.size, Vector3.down, hits, base.transform.rotation, raycastDistance, layerMask, QueryTriggerInteraction.Ignore);
			if (num2 == 0)
			{
				return Vector3.zero;
			}
			RaycastHit raycastHit = default(RaycastHit);
			for (int i = 0; i < num2; i++)
			{
				RaycastHit raycastHit2 = hits[i];
				if (!(raycastHit2.collider.gameObject == base.gameObject) && raycastHit2.distance < num)
				{
					raycastHit = raycastHit2;
					num = raycastHit2.distance;
				}
			}
			return raycastHit.point;
		}

		private bool GetIsGrounded()
		{
			Vector3 groundHit = GetGroundHit(3f);
			if (groundHit == Vector3.zero)
			{
				return false;
			}
			if (base.transform.position.y - groundHit.y > 1f)
			{
				return false;
			}
			return true;
		}

		private void InitializeAbility<T>(Ability prefab, ref T ability) where T : Ability
		{
			if (!(prefab == null))
			{
				ability = (UnityEngine.Object.Instantiate(prefab, base.transform) as T);
				ability.Initialize(base.gameObject);
			}
		}

		private void OnDrawGizmos()
		{
			Color color = Gizmos.color;
			Color magenta = Color.magenta;
			magenta.a = 0.5f;
			Gizmos.color = magenta;
			Gizmos.DrawCube(base.transform.position, BoxCast.size);
			Gizmos.color = color;
		}
	}
}
