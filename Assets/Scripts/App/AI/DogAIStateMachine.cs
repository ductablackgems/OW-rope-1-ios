using System.Collections.Generic;

namespace App.AI
{
	public class DogAIStateMachine : AIStateMachine
	{
		private DogAIStateIdle defaultState;

		protected override AIState DefaultState => defaultState;

		public DogAIStateMachine(IAIEntity entity)
			: base(entity)
		{
		}

		protected override void GenerateStates(List<AIState> states)
		{
			DogAIStateIdle item = new DogAIStateIdle(base.AIEntity);
			states.Add(item);
			states.Add(new DogAIStateFollow(base.AIEntity));
			states.Add(new DogAIStateGoToAttackRange(base.AIEntity));
			states.Add(new DogAIStateAttack(base.AIEntity));
			states.Add(new DogAIStateDead(base.AIEntity));
			defaultState = item;
		}
	}
}
