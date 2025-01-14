using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class DogAIStateGoToAttackRange : DogAIState
	{
		private const float MinDistanceToRepath = 0.25f;

		private Vector3 lastTargetPos;

		private NavMeshPath path = new NavMeshPath();

		private bool isValid = true;

		public DogAIStateGoToAttackRange(IAIEntity entity)
			: base(entity)
		{
			base.TimerInterval = 0.25f;
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			lastTargetPos = Vector3.zero;
			isValid = true;
			SetMaxSpeed(isRunning: true);
			base.NavMeshAgent.autoRepath = false;
		}

		protected override void OnExit()
		{
			base.OnExit();
			isValid = true;
			SetMaxSpeed(isRunning: false);
			base.NavMeshAgent.autoRepath = true;
		}

		protected override void OnTimerTick()
		{
			base.OnTimerTick();
			GameObject target = base.AIEntity.TargetManager.GetTarget();
			if (target == null)
			{
				return;
			}
			Vector3 position = target.transform.position;
			if (!((position - base.Position).magnitude < 0.25f))
			{
				base.NavMeshAgent.ResetPath();
				lastTargetPos = position;
				if (!base.NavMeshAgent.CalculatePath(position, path))
				{
					isValid = false;
				}
				else if (path.status != 0)
				{
					isValid = false;
				}
				else
				{
					isValid = base.NavMeshAgent.SetPath(path);
				}
			}
		}

		protected override bool OnIsValid()
		{
			if (!isValid)
			{
				return false;
			}
			if (base.AIEntity.Health == 0f)
			{
				return false;
			}
			GameObject target = base.AIEntity.TargetManager.GetTarget();
			if (target == null)
			{
				return false;
			}
			return Vector3.Distance(target.transform.position, base.Position) > base.AIEntity.GetWeaponRange();
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
			DogAIStateAttack follower = GetFollower<DogAIStateAttack>();
			if (follower.IsValid())
			{
				return follower;
			}
			return GetFollower<DogAIStateFollow>();
		}

		private void SetMaxSpeed(bool isRunning)
		{
			base.NavMeshAgent.speed = (isRunning ? base.Settings.RunSpeed : base.Settings.WalkSpeed);
		}
	}
}
