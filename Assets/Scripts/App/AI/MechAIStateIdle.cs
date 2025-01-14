namespace App.AI
{
	public class MechAIStateIdle : AIState
	{
		public MechAIStateIdle(IAIEntity entity)
			: base(entity)
		{
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			base.AIEntity.NavMeshAgent.isStopped = true;
		}

		protected override void OnExit()
		{
			base.OnExit();
			base.AIEntity.NavMeshAgent.isStopped = false;
		}

		public override AIState GetNextState()
		{
			if (base.AIEntity.Health == 0f)
			{
				return GetFollower<MechAIStateDead>();
			}
			if (base.AIEntity.TargetManager.GetVisibleTargetInRange() != null)
			{
				return GetFollower<MechAIStateAttack>();
			}
			if (base.AIEntity.TargetManager.GetTarget() != null)
			{
				MechAIStateGoToWeaponRange follower = GetFollower<MechAIStateGoToWeaponRange>();
				if (follower.IsValid())
				{
					return follower;
				}
			}
			return this;
		}
	}
}
