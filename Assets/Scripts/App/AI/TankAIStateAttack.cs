using App.Util;
using UnityEngine;

namespace App.AI
{
	public class TankAIStateAttack : AIState
	{
		public TankAIStateAttack(IAIEntity entity)
			: base(entity)
		{
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			base.NavMeshAgent.isStopped = true;
		}

		protected override void OnExit()
		{
			base.OnExit();
			base.NavMeshAgent.isStopped = false;
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			GameObject visibleTargetInRange = base.AIEntity.TargetManager.GetVisibleTargetInRange();
			if (!(visibleTargetInRange == null))
			{
				base.AIEntity.RotateTo(visibleTargetInRange.transform.position);
				base.AIEntity.Attack(visibleTargetInRange);
			}
		}

		public override AIState GetNextState()
		{
			if (base.AIEntity.Health == 0f)
			{
				return GetFollower<TankAIStateDead>();
			}
			GameObject visibleTargetInRange = base.AIEntity.TargetManager.GetVisibleTargetInRange();
			if (visibleTargetInRange == null)
			{
				if (base.AIEntity.TargetManager.GetTarget() == null)
				{
					return GetFollower<TankAIStateIdle>();
				}
				return GetFollower<TankAIStateGoToWeaponRange>();
			}
			Health component = visibleTargetInRange.GetComponent<Health>();
			if (component == null || component.GetCurrentHealth() == 0f)
			{
				return GetFollower<TankAIStateIdle>();
			}
			return this;
		}
	}
}
