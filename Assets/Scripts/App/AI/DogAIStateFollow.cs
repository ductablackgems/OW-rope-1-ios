using App.Util;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class DogAIStateFollow : DogAIState
	{
		private const float MinDistanceToRepath = 0.25f;

		private const float MinDistanceToRun = 5f;

		private readonly FloatRange FollowRange = new FloatRange(1.5f, 3f);

		private Vector3 lastDestination;

		private float followRange;

		private NavMeshPath path = new NavMeshPath();

		private bool isValid = true;

		public DogAIStateFollow(IAIEntity entity)
			: base(entity)
		{
			base.TimerInterval = 0.25f;
			followRange = FollowRange.GetRandomValue();
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			lastDestination = Vector3.zero;
			isValid = true;
			base.NavMeshAgent.autoRepath = false;
			SetMaxSpeed(isRunning: false);
		}

		protected override void OnExit()
		{
			base.OnExit();
			followRange = FollowRange.GetRandomValue();
			isValid = true;
			base.NavMeshAgent.autoRepath = true;
			SetMaxSpeed(isRunning: false);
		}

		protected override void OnTimerTick()
		{
			base.OnTimerTick();
			Vector3 position = base.Dog.Spot.Position;
			if (!(Vector3.Distance(position, lastDestination) < 0.25f))
			{
				lastDestination = position;
				NavMeshHit hit = default(NavMeshHit);
				if (!NavMesh.SamplePosition(position, out hit, 2f, base.NavMeshAgent.areaMask))
				{
					position = base.Dog.Owner.transform.position;
				}
				if (!base.NavMeshAgent.CalculatePath(position, path))
				{
					isValid = false;
					return;
				}
				if (path.status != 0)
				{
					isValid = false;
					return;
				}
				isValid = base.NavMeshAgent.SetPath(path);
				SetMaxSpeed(CanRun());
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
			if (base.AIEntity.TargetManager.GetTarget() != null)
			{
				return false;
			}
			return CanFollow();
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
			DogAIStateGoToAttackRange follower2 = GetFollower<DogAIStateGoToAttackRange>();
			if (follower2.IsValid())
			{
				return follower2;
			}
			return GetFollower<DogAIStateIdle>();
		}

		private bool CanFollow()
		{
			return Vector3.Distance(base.Position, base.Dog.Spot.Position) > followRange;
		}

		private bool CanRun()
		{
			return Vector3.Distance(base.Position, base.Dog.Spot.Position) > 5f;
		}

		private void SetMaxSpeed(bool isRunning)
		{
			base.NavMeshAgent.speed = (isRunning ? base.Settings.RunSpeed : base.Settings.WalkSpeed);
		}
	}
}
