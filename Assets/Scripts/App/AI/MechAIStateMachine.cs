using System.Collections.Generic;

namespace App.AI
{
	public class MechAIStateMachine : AIStateMachine
	{
		private AIState defaultState;

		private bool isAirborne;

		protected override AIState DefaultState => defaultState;

		public MechAIStateMachine(IAIEntity entity, bool isAirborne = false)
			: base(entity)
		{
			this.isAirborne = isAirborne;
		}

		protected override void GenerateStates(List<AIState> states)
		{
			defaultState = new MechAIStateIdle(base.AIEntity);
			states.Add(defaultState);
			states.Add(new MechAIStateGoToWeaponRange(base.AIEntity));
			states.Add(new MechAIStateAttack(base.AIEntity));
			states.Add(new MechAIStateDead(base.AIEntity));
			if (isAirborne)
			{
				MechAIStateDeploy item = new MechAIStateDeploy(base.AIEntity);
				states.Insert(0, item);
				defaultState = item;
			}
		}
	}
}
