namespace App.AI
{
	public class DogAIStateDead : DogAIState
	{
		public DogAIStateDead(IAIEntity entity)
			: base(entity)
		{
		}

		public override AIState GetNextState()
		{
			if (IsValid())
			{
				return this;
			}
			return GetFollower<DogAIStateIdle>();
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			base.NavMeshAgent.isStopped = true;
			ServiceLocator.Messages.Send(MessageID.Dog.Died, this, base.Dog);
		}

		protected override void OnExit()
		{
			base.OnExit();
			base.NavMeshAgent.isStopped = false;
		}

		protected override bool OnIsValid()
		{
			return base.AIEntity.Health == 0f;
		}
	}
}
