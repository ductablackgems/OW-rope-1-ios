using App.Player.Definition;
using App.Player.FightSystem.Definition;
using App.Util;
using UnityEngine;

namespace App.Player.FightSystem
{
	public class HitHandler : MonoBehaviour
	{
		private const float RagdollSpeed = 1f;

		private MagicShield magicShield;

		private Animator animator;

		private Rigidbody _rigidbody;

		private RigidbodyHelper rigidbodyHelper;

		private PlayerAnimatorHandler animatorHandler;

		private Health health;

		private RagdollHelper ragdollHelper;

		private CharacterControl characterControl;

		private GlideController glideController;

		private ClimbController climbController;

		private RopeController ropeController;

		private FlyController flyController;

		private FightMovementHandler movementHandler;

		private HitResolver hitResolver;

		private AnimationSimulator animationSimulator;

		private FightMovementDefinition hitMovementDefinition;

		private bool handleHit;

		private Vector3 ragdollDirection;

		public bool Running()
		{
			if (movementHandler.Definition == null)
			{
				return handleHit;
			}
			return true;
		}

		public bool RunningKinematic()
		{
			if (movementHandler.AnimationDefinition != null)
			{
				return movementHandler.AnimationDefinition.kinematic;
			}
			return false;
		}

		public bool HandleHit(FightHitDefinition hitDefinition, Transform agressor)
		{
			if (WillRagdoll() || ragdollHelper.Ragdolled)
			{
				return false;
			}
			if ((glideController != null && glideController.Running()) || (climbController != null && climbController.Running()) || (flyController != null && flyController.Running()))
			{
				ragdollHelper.Ragdolled = true;
				return true;
			}
			if (ropeController != null && ropeController.Running())
			{
				if (animatorHandler.PullRope)
				{
					ropeController.Stop();
				}
				else
				{
					ragdollHelper.Ragdolled = true;
				}
				return true;
			}
			if (IsInjuryBlockedByAbility())
			{
				return false;
			}
			hitMovementDefinition = hitResolver.ResolveHit(hitDefinition, agressor, base.transform);
			handleHit = true;
			if (hitDefinition.power == FightHitPower.Stagger)
			{
				Vector3 eulerAngles = agressor.rotation.eulerAngles;
				eulerAngles.y += hitDefinition.yAngleOffset;
				ragdollDirection = Quaternion.Euler(eulerAngles) * Vector3.forward;
				characterControl.ExtraVelocity = ragdollDirection * 1f;
			}
			else
			{
				characterControl.ExtraVelocity = Vector3.zero;
			}
			return true;
		}

		public void RunCustomMovement(string tid)
		{
			hitMovementDefinition = hitResolver.ResolveCustomMovementDefinition(tid);
			movementHandler.Run(hitMovementDefinition);
			handleHit = false;
			hitMovementDefinition = null;
		}

		public void Interrupt()
		{
			if (Running())
			{
				movementHandler.Clear();
				animatorHandler.GroundedState.RunCrossFixed(0.25f);
			}
		}

		public void Clear()
		{
			movementHandler.Clear();
			hitMovementDefinition = null;
			handleHit = false;
			characterControl.ExtraVelocity = Vector3.zero;
		}

		public bool MustExit()
		{
			if (!handleHit && movementHandler.Definition != null)
			{
				return movementHandler.MustExit();
			}
			return false;
		}

		public bool WillRagdoll()
		{
			if (movementHandler.Definition == null || !movementHandler.WillRagdollize())
			{
				if (handleHit)
				{
					return movementHandler.WillRagdollize(hitMovementDefinition);
				}
				return false;
			}
			return true;
		}

		private void Awake()
		{
			magicShield = GetComponent<MagicShield>();
			animator = this.GetComponentSafe<Animator>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			rigidbodyHelper = this.GetComponentSafe<RigidbodyHelper>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			health = this.GetComponentSafe<Health>();
			ragdollHelper = this.GetComponentSafe<RagdollHelper>();
			characterControl = this.GetComponentSafe<CharacterControl>();
			glideController = GetComponent<GlideController>();
			climbController = GetComponent<ClimbController>();
			ropeController = GetComponent<RopeController>();
			flyController = GetComponent<FlyController>();
			movementHandler = new FightMovementHandler(animator, health);
			hitResolver = ServiceLocator.Get<HitResolver>();
			animationSimulator = ServiceLocator.Get<AnimationSimulator>(showError: false);
			movementHandler.OnAnimationChange += OnAnimationChange;
		}

		private void OnDestroy()
		{
			movementHandler.OnAnimationChange -= OnAnimationChange;
		}

		private void Update()
		{
			if (ragdollHelper.VeryRagdolled)
			{
				Clear();
				return;
			}
			if (movementHandler.Definition != null)
			{
				movementHandler.UpdateStates();
			}
			if (handleHit)
			{
				movementHandler.Run(hitMovementDefinition);
				handleHit = false;
				hitMovementDefinition = null;
			}
			else if (movementHandler.Definition != null && movementHandler.MustExit())
			{
				movementHandler.Clear();
				animatorHandler.GroundedState.RunCrossFixed(0.25f);
			}
			else if (movementHandler.Definition != null && movementHandler.MustRagdollize())
			{
				ragdollHelper.Ragdolled = true;
				if (movementHandler.Definition.useCustomRagdollSpeed)
				{
					ragdollHelper.SetRagdollVelocity(base.transform.TransformDirection(movementHandler.Definition.ragdollSpeed));
				}
				else
				{
					Vector3 b = animator.deltaPosition / Time.deltaTime;
					ragdollHelper.SetRagdollVelocity(ragdollDirection * 1f + b);
				}
				movementHandler.Clear();
			}
		}

		private void LateUpdate()
		{
			if (movementHandler.AnimationDefinition != null && movementHandler.AnimationDefinition.kinematic)
			{
				base.transform.position = animationSimulator.victimAnimator.transform.position;
				base.transform.rotation = animationSimulator.victimAnimator.transform.rotation;
			}
		}

		private void OnAnimationChange(AnimationDefinition oldAnimation, AnimationDefinition newAnimation)
		{
			if (oldAnimation != null && oldAnimation.kinematic)
			{
				animationSimulator.StopVictimSimulation();
			}
			if (newAnimation != null)
			{
				rigidbodyHelper.SetKinematic(newAnimation.kinematic, this);
				if (newAnimation.kinematic)
				{
					animationSimulator.RunVictimSimulation(newAnimation.hash);
				}
			}
			else
			{
				rigidbodyHelper.SetKinematic(kinematic: false, this);
			}
		}

		private bool IsInjuryBlockedByAbility()
		{
			if (magicShield != null)
			{
				return magicShield.IsInjuryBlockRequest;
			}
			return false;
		}
	}
}
