using System.Collections.Generic;

namespace App.AI
{
	public class TankAIStateMachine : AIStateMachine
	{
		private AIState defaultState;

		protected override AIState DefaultState => defaultState;

		public TankAIStateMachine(IAIEntity entity)
			: base(entity)
		{
		}

		protected override void GenerateStates(List<AIState> states)
		{
			defaultState = new TankAIStateIdle(base.AIEntity);
			states.Add(defaultState);
			states.Add(new TankAIStateGoToWeaponRange(base.AIEntity));
			states.Add(new TankAIStateAttack(base.AIEntity));
			states.Add(new TankAIStateDead(base.AIEntity));
		}
	}
}
