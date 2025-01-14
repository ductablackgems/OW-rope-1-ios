namespace App.AI
{
	public class TankAIStateIdle : AIState
	{
		public TankAIStateIdle(IAIEntity entity)
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
				return GetFollower<TankAIStateDead>();
			}
			if (base.AIEntity.TargetManager.GetVisibleTargetInRange() != null)
			{
				return GetFollower<TankAIStateAttack>();
			}
			if (base.AIEntity.TargetManager.GetTarget() != null)
			{
				TankAIStateGoToWeaponRange follower = GetFollower<TankAIStateGoToWeaponRange>();
				if (follower.IsValid())
				{
					return follower;
				}
			}
			return this;
		}
	}
}
