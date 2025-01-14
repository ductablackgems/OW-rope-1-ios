namespace App.AI
{
	public class DogAIStateIdle : DogAIState
	{
		public DogAIStateIdle(IAIEntity entity)
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
		}

		public override AIState GetNextState()
		{
			if (base.AIEntity.Health == 0f)
			{
				return GetFollower<DogAIStateDead>();
			}
			DogAIStateAttack follower = GetFollower<DogAIStateAttack>();
			if (follower.IsValid())
			{
				return follower;
			}
			DogAIStateGoToAttackRange follower2 = GetFollower<DogAIStateGoToAttackRange>();
			if (follower2.IsValid())
			{
				return follower2;
			}
			DogAIStateFollow follower3 = GetFollower<DogAIStateFollow>();
			if (follower3.IsValid())
			{
				return follower3;
			}
			return null;
		}
	}
}
