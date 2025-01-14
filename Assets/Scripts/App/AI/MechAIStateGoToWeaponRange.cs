using UnityEngine;

namespace App.AI
{
	public class MechAIStateGoToWeaponRange : AIState
	{
		private const float maxDistance = 5f;

		private bool isMoving;

		private float goToRange;

		public MechAIStateGoToWeaponRange(IAIEntity entity)
			: base(entity)
		{
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			base.NavMeshAgent.isStopped = false;
			float weaponRange = base.AIEntity.GetWeaponRange();
			goToRange = Random.Range(weaponRange * 0.75f, weaponRange);
			isMoving = true;
		}

		protected override void OnExit()
		{
			base.OnExit();
			base.NavMeshAgent.isStopped = true;
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			Vector3 targetPosition = GetTargetPosition();
			if (targetPosition == Vector3.zero)
			{
				isMoving = false;
				return;
			}
			GameObject visibleTargetInRange = base.AIEntity.TargetManager.GetVisibleTargetInRange();
			if (visibleTargetInRange != null)
			{
				base.AIEntity.Attack(visibleTargetInRange);
			}
			base.AIEntity.NavMeshAgent.SetDestination(targetPosition);
			base.AIEntity.Move(base.AIEntity.NavMeshAgent.velocity);
			isMoving = (Vector3.Distance(targetPosition, base.Position) > goToRange);
		}

		protected override bool OnIsValid()
		{
			Vector3 targetPosition = GetTargetPosition();
			if (targetPosition == Vector3.zero)
			{
				return false;
			}
			return Vector3.Distance(base.Position, targetPosition) > 5f;
		}

		public override AIState GetNextState()
		{
			if (base.AIEntity.Health == 0f)
			{
				return GetFollower<MechAIStateDead>();
			}
			if (!isMoving)
			{
				return GetFollower<MechAIStateIdle>();
			}
			return this;
		}

		private Vector3 GetTargetPosition()
		{
			GameObject target = base.AIEntity.TargetManager.GetTarget();
			if (target == null)
			{
				return Vector3.zero;
			}
			Vector3 result = Vector3.zero;
			Vector3 position = target.transform.position;
			if (position.y - base.Position.y > 5f)
			{
				position.y = base.Position.y;
			}
			if (!AIUtils.GetSafeNavmeshPosition(base.Position, position, -1, out result, 5f))
			{
				return Vector3.zero;
			}
			return result;
		}
	}
}
