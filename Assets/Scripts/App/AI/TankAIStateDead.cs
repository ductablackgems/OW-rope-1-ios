namespace App.AI
{
	public class TankAIStateDead : AIState
	{
		public TankAIStateDead(IAIEntity entity)
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
			return this;
		}
	}
}
