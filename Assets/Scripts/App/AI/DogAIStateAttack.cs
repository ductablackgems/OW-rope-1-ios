using UnityEngine;

namespace App.AI
{
	public class DogAIStateAttack : DogAIState
	{
		private GameObject target;

		public DogAIStateAttack(IAIEntity entity)
			: base(entity)
		{
			base.TimerInterval = 1f;
		}

		public override AIState GetNextState()
		{
			if (IsValid())
			{
				return this;
			}
			if (base.AIEntity.Health == 0f)
			{
				return GetFollower<DogAIStateDead>();
			}
			DogAIStateGoToAttackRange follower = GetFollower<DogAIStateGoToAttackRange>();
			if (follower.IsValid())
			{
				return follower;
			}
			return GetFollower<DogAIStateFollow>();
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			target = null;
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
			target = base.AIEntity.TargetManager.GetTarget();
			if (!(target == null))
			{
				Vector3 position = target.transform.position;
				Vector3 vector = (position - base.Position).RemoveVertical();
				if (Vector3.Dot(rhs: base.AIEntity.Owner.transform.forward.RemoveVertical().normalized, lhs: vector.normalized) < 0.8f)
				{
					base.AIEntity.RotateTo(position);
					target = null;
				}
			}
		}

		protected override void OnTimerTick()
		{
			base.OnTimerTick();
			if (!(target == null))
			{
				base.AIEntity.Attack(target);
			}
		}

		protected override bool OnIsValid()
		{
			if (base.AIEntity.Health == 0f)
			{
				return false;
			}
			GameObject gameObject = base.AIEntity.TargetManager.GetTarget();
			if (gameObject == null)
			{
				return false;
			}
			if (Vector3.Distance(base.Position, gameObject.transform.position) > base.AIEntity.GetWeaponRange())
			{
				return false;
			}
			return true;
		}
	}
}
