using App.Player.FightSystem.Definition;
using App.Util;
using UnityEngine;

namespace App.Player.FightSystem
{
	public class FightMovementHandler
	{
		public delegate void MovementChangeEventHandler(FightMovementDefinition oldMovement, FightMovementDefinition newMovement);

		public delegate void AnimationChangeEventHandler(AnimationDefinition oldAnimation, AnimationDefinition newAnimation);

		public const string AnimationStatesPrefix = "Base Layer.AdvancedFightMachine.";

		private Animator animator;

		private Health health;

		private float exceedAnimationTime;

		private float remainingAnimationTime;

		private bool isInTransition;

		private int animationIndex;

		private float runTime;

		public FightMovementDefinition Definition
		{
			get;
			private set;
		}

		public FightMovementDefinition LastDefinition
		{
			get;
			private set;
		}

		public AnimationDefinition AnimationDefinition
		{
			get;
			private set;
		}

		public event MovementChangeEventHandler OnMovementChange;

		public event AnimationChangeEventHandler OnAnimationChange;

		public FightMovementHandler(Animator animator, Health health)
		{
			this.animator = animator;
			this.health = health;
		}

		public void UpdateStates()
		{
			if (Definition == null || runTime == Time.time)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			AnimatorStateInfo nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(0);
			bool flag = AnimationDefinition.hash == currentAnimatorStateInfo.fullPathHash;
			bool flag2 = AnimationDefinition.hash == nextAnimatorStateInfo.fullPathHash;
			if (!flag2 && TryUpdateAnimationDefinition(nextAnimatorStateInfo))
			{
				flag = false;
				flag2 = true;
			}
			if (!flag && !flag2)
			{
				if (!TryUpdateAnimationDefinition(currentAnimatorStateInfo))
				{
					Clear();
					return;
				}
				flag = true;
				flag2 = false;
			}
			AnimatorStateInfo animatorStateInfo = flag2 ? nextAnimatorStateInfo : currentAnimatorStateInfo;
			exceedAnimationTime = animatorStateInfo.length * animatorStateInfo.normalizedTime;
			remainingAnimationTime = animatorStateInfo.length * (1f - animatorStateInfo.normalizedTime);
			isInTransition = animator.IsInTransition(0);
		}

		public void Run(FightMovementDefinition definition)
		{
			FightMovementDefinition definition2 = Definition;
			AnimationDefinition animationDefinition = AnimationDefinition;
			runTime = Time.time;
			Definition = definition;
			LastDefinition = definition;
			AnimationDefinition = definition.animations[0];
			if (AnimationDefinition.hash == 0)
			{
				AnimationDefinition.hash = Animator.StringToHash("Base Layer.AdvancedFightMachine." + AnimationDefinition.name);
			}
			if (AnimationDefinition.useCustomTransition)
			{
				animator.CrossFade(AnimationDefinition.hash, AnimationDefinition.customTransitionDuration);
			}
			else
			{
				animator.CrossFade(AnimationDefinition.hash, 0.25f);
			}
			exceedAnimationTime = 0f;
			remainingAnimationTime = 999f;
			isInTransition = true;
			if (this.OnMovementChange != null)
			{
				this.OnMovementChange(definition2, definition);
			}
			if (this.OnAnimationChange != null)
			{
				this.OnAnimationChange(animationDefinition, AnimationDefinition);
			}
		}

		public void Clear()
		{
			FightMovementDefinition definition = Definition;
			AnimationDefinition animationDefinition = AnimationDefinition;
			Definition = null;
			AnimationDefinition = null;
			exceedAnimationTime = 0f;
			remainingAnimationTime = 0f;
			isInTransition = false;
			if (this.OnMovementChange != null && definition != null)
			{
				this.OnMovementChange(definition, null);
			}
			if (this.OnAnimationChange != null && animationDefinition != null)
			{
				this.OnAnimationChange(animationDefinition, null);
			}
		}

		public bool MustExit()
		{
			if (!isInTransition)
			{
				return remainingAnimationTime <= AnimationDefinition.maxRemainExitTime;
			}
			return false;
		}

		public bool MustRagdollize()
		{
			if (Definition.alwaysRagdollize || health.Dead())
			{
				return exceedAnimationTime >= Definition.ragdollTime;
			}
			return false;
		}

		public bool WillRagdollize(FightMovementDefinition definition = null)
		{
			if (definition == null)
			{
				definition = Definition;
			}
			if (!definition.alwaysRagdollize)
			{
				return health.Dead();
			}
			return true;
		}

		private bool TryUpdateAnimationDefinition(AnimatorStateInfo comparingStateInfo)
		{
			AnimationDefinition[] animations = Definition.animations;
			foreach (AnimationDefinition animationDefinition in animations)
			{
				if (animationDefinition.hash == 0)
				{
					animationDefinition.hash = Animator.StringToHash("Base Layer.AdvancedFightMachine." + animationDefinition.name);
				}
				if (animationDefinition != AnimationDefinition && animationDefinition.hash == comparingStateInfo.fullPathHash)
				{
					AnimationDefinition animationDefinition2 = AnimationDefinition;
					AnimationDefinition = animationDefinition;
					if (this.OnAnimationChange != null)
					{
						this.OnAnimationChange(animationDefinition2, animationDefinition);
					}
					return true;
				}
			}
			return false;
		}
	}
}
