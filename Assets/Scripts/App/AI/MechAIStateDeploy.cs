using App.Vehicles.Mech;

namespace App.AI
{
	public class MechAIStateDeploy : AIState
	{
		private MechController controller;

		private bool isLanded;

		public MechAIStateDeploy(IAIEntity entity)
			: base(entity)
		{
			controller = base.AIEntity.Owner.GetComponent<MechController>();
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			controller.Rigidbody.isKinematic = false;
			base.AIEntity.NavMeshAgent.enabled = false;
			controller.SetActiveFallingEffect(isActive: true);
			isLanded = false;
		}

		protected override void OnExit()
		{
			base.OnExit();
			base.AIEntity.NavMeshAgent.enabled = true;
			controller.Rigidbody.isKinematic = true;
			controller.SetActiveFallingEffect(isActive: false);
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (!isLanded)
			{
				isLanded = controller.TryLand();
			}
		}

		public override AIState GetNextState()
		{
			if (isLanded)
			{
				return GetFollower<MechAIStateIdle>();
			}
			return this;
		}
	}
}
