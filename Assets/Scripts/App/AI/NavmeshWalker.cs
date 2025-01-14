using App.Player;
using App.Spawn;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class NavmeshWalker : AbstractAIScript, IResetable
	{
		private NavMeshAgent agent;

		private CharacterControl characterControl;

		private bool isPlayer;

		private Vector3 lastPosition;

		private Vector3 lastVelocity;

		private RandomPositionFinder randomPositionFinder;

		private MonoBehaviour commander;

		private Vector3 target;

		private DurationTimer timeOutTimer = new DurationTimer();

		private bool agentDestinationSolved;

		private bool blockRotationUpdate;

		private int frames;

		private GameObject hrac;

		private float distance;

		private int omezeni = 30;

		private int kooficient = 1;

		public NavmeshWalkerState State
		{
			get;
			private set;
		}

		public NavmeshWalkerMode Mode
		{
			get;
			private set;
		}

		public NavmeshWalkerSpeed Speed
		{
			get;
			private set;
		}

		public Transform TargetTransform
		{
			get;
			private set;
		}

		public Transform ChaserTransform
		{
			get;
			private set;
		}

		public void BlockRotationUpdate(bool blockRotationUpdate)
		{
			if (!isPlayer)
			{
				this.blockRotationUpdate = blockRotationUpdate;
				if (blockRotationUpdate)
				{
					agent.updateRotation = false;
					characterControl.ApplyRotationForAI = false;
				}
				else
				{
					agent.updateRotation = (Mode == NavmeshWalkerMode.Navmesh);
					characterControl.ApplyRotationForAI = (Mode == NavmeshWalkerMode.Free);
				}
			}
		}

		public void ResetStates()
		{
			State = NavmeshWalkerState.Stay;
			Speed = NavmeshWalkerSpeed.Stand;
			TargetTransform = null;
			ChaserTransform = null;
			commander = null;
			agentDestinationSolved = false;
			blockRotationUpdate = false;
			if (agent.isOnNavMesh)
			{
				agent.Stop();
			}
		}

		public void FollowTransform(MonoBehaviour commander, Transform target, NavmeshWalkerSpeed speed, float timeOut = -1f)
		{
			State = NavmeshWalkerState.FollowTarget;
			Speed = speed;
			TargetTransform = target;
			ChaserTransform = null;
			this.target = target.position;
			this.commander = commander;
			SetMode(NavmeshWalkerMode.Navmesh);
			TrySolveAgentDestination();
		}

		public void GoRandom(MonoBehaviour commander, NavmeshWalkerSpeed speed)
		{
			State = NavmeshWalkerState.RandomWalk;
			Speed = speed;
			TargetTransform = null;
			ChaserTransform = null;
			target = randomPositionFinder.GetRandomTargetPoint(base.ComponentsRoot);
			this.commander = commander;
			SetMode(NavmeshWalkerMode.Navmesh);
			TrySolveAgentDestination();
		}

		public void Flee(MonoBehaviour commander, Transform chaser, float timeOut = -1f)
		{
			State = NavmeshWalkerState.Flee;
			Speed = NavmeshWalkerSpeed.RunFast;
			TargetTransform = null;
			ChaserTransform = chaser;
			this.commander = commander;
			SetMode(NavmeshWalkerMode.Free);
		}

		public void Stop()
		{
			if (State != 0)
			{
				State = NavmeshWalkerState.Stay;
				Speed = NavmeshWalkerSpeed.Stand;
				commander = null;
				TargetTransform = null;
				ChaserTransform = null;
				agentDestinationSolved = false;
				if (agent.isOnNavMesh)
				{
					agent.Stop();
				}
				characterControl.Move(Vector3.zero, runFast: false, crouch: false, jump: false);
			}
		}

		public bool CompareCommand(MonoBehaviour commander, NavmeshWalkerState state)
		{
			if (this.commander == commander)
			{
				return State == state;
			}
			return false;
		}

		public bool ReachingTarget(float minDistance = -1f)
		{
			return false;
		}

		public Vector3 GetLastVelocity()
		{
			return lastVelocity;
		}

		private void Awake()
		{
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			characterControl = base.ComponentsRoot.GetComponentSafe<CharacterControl>();
			randomPositionFinder = ServiceLocator.Get<RandomPositionFinder>(showError: false);
			isPlayer = base.ComponentsRoot.CompareTag("Player");
			agent.updatePosition = false;
		}

		private void Update()
		{
			if (isPlayer)
			{
				FrameOptimalUpdate();
				return;
			}
			frames++;
			if (frames % omezeni == 0)
			{
				FrameOptimalUpdate();
				if (hrac != null)
				{
					distance = Vector3.Distance(base.transform.position, hrac.transform.position);
				}
				if (distance < 40f)
				{
					omezeni = 1;
				}
				else
				{
					omezeni = 30 / kooficient;
				}
			}
		}

		private void FixedUpdate()
		{
			lastVelocity = (base.ComponentsRoot.position - lastPosition) / Time.fixedDeltaTime;
			lastPosition = base.ComponentsRoot.position;
		}

		private void FrameOptimalUpdate()
		{
			if (!agent.enabled)
			{
				return;
			}
			if (agent.enabled && agent.hasPath)
			{
				agent.nextPosition = base.ComponentsRoot.position;
			}
			if (State == NavmeshWalkerState.Stay)
			{
				characterControl.Move(Vector3.zero, runFast: false, crouch: false, jump: false);
				return;
			}
			if (State != NavmeshWalkerState.Flee)
			{
				bool flag = IsOnNavmesh();
				if (Mode == NavmeshWalkerMode.Navmesh != flag)
				{
					SetMode((!flag) ? NavmeshWalkerMode.Free : NavmeshWalkerMode.Navmesh);
				}
			}
			float num = CalculateAgentSpeed();
			if (State == NavmeshWalkerState.FollowTarget && TargetTransform != null && (target - TargetTransform.position).magnitude > 0.2f)
			{
				target = TargetTransform.position;
				agentDestinationSolved = false;
			}
			if (Mode == NavmeshWalkerMode.Navmesh)
			{
				if ((State == NavmeshWalkerState.RandomWalk || State == NavmeshWalkerState.FollowTarget) && !agentDestinationSolved)
				{
					TrySolveAgentDestination();
				}
				if (agent.isOnNavMesh && agent.hasPath)
				{
					agent.Resume();
					agent.speed = num;
				}
				characterControl.Move(agent.desiredVelocity, runFast: false, crouch: false, jump: false);
				if (agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
				{
					Stop();
				}
			}
			else if (State == NavmeshWalkerState.Flee)
			{
				Vector3 vector = (base.ComponentsRoot.position - ChaserTransform.position).normalized * num;
				characterControl.Move(new Vector3(vector.x, 0f, vector.z), runFast: false, crouch: false, jump: false);
			}
			else
			{
				Vector3 vector2 = (target - base.ComponentsRoot.position).normalized * num;
				characterControl.Move(new Vector3(vector2.x, 0f, vector2.z), runFast: false, crouch: false, jump: false);
			}
		}

		private void SetMode(NavmeshWalkerMode mode)
		{
			Mode = mode;
			if (mode == NavmeshWalkerMode.Navmesh)
			{
				agentDestinationSolved = false;
				if (!isPlayer && !blockRotationUpdate)
				{
					if (!blockRotationUpdate)
					{
						agent.updateRotation = true;
					}
					characterControl.ApplyRotationForAI = false;
				}
				return;
			}
			agentDestinationSolved = false;
			if (agent.isOnNavMesh)
			{
				agent.Stop();
			}
			if (!isPlayer)
			{
				agent.updateRotation = false;
				if (!blockRotationUpdate)
				{
					characterControl.ApplyRotationForAI = true;
				}
			}
		}

		private void TrySolveAgentDestination()
		{
			if (agent.isOnNavMesh)
			{
				agent.SetDestination(target);
				agentDestinationSolved = true;
			}
		}

		private float CalculateAgentSpeed()
		{
			float result = 0f;
			if (Speed == NavmeshWalkerSpeed.Walk)
			{
				result = 0.5f;
			}
			else if (Speed == NavmeshWalkerSpeed.Run)
			{
				result = 0.7692308f;
			}
			else if (Speed == NavmeshWalkerSpeed.RunFast)
			{
				result = 1f;
			}
			return result;
		}

		private bool IsOnNavmesh()
		{
			NavMeshHit hit;
			return NavMesh.SamplePosition(base.ComponentsRoot.position, out hit, 1f, -1);
		}
	}
}
