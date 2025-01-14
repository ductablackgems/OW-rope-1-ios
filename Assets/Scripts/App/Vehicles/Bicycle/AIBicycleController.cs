using App.AI;
using App.Player;
using UnityEngine;
using UnityEngine.AI;

namespace App.Vehicles.Bicycle
{
	public class AIBicycleController : MonoBehaviour, IFollowingVehicle
	{
		private const float SteerSpeed = 100f;

		private NavMeshAgent agent;

		private BicycleAnimator bicycleAnimator;

		private RandomPositionFinder randomPositionFinder;

		private PlayerModel player;

		private float lastYAngle;

		private float lastSteerAngle;

		public bool FollowingPlayer
		{
			get;
			private set;
		}

		public void SetFollowingPlayer(bool followingPlayer)
		{
			if (!followingPlayer && FollowingPlayer && base.enabled && !agent.pathPending)
			{
				agent.SetDestination(randomPositionFinder.GetRandomTargetPoint(base.transform));
			}
			FollowingPlayer = followingPlayer;
		}

		private void Awake()
		{
			agent = this.GetComponentSafe<NavMeshAgent>();
			bicycleAnimator = this.GetComponentSafe<BicycleAnimator>();
			randomPositionFinder = ServiceLocator.Get<RandomPositionFinder>();
			player = ServiceLocator.GetPlayerModel();
		}

		private void OnEnable()
		{
			lastYAngle = base.transform.rotation.eulerAngles.y;
			lastYAngle = 0f;
			agent.enabled = true;
			GetComponent<NavMeshObstacle>().enabled = false;
			if (!agent.pathPending)
			{
				agent.SetDestination(randomPositionFinder.GetRandomTargetPoint(base.transform));
			}
		}

		private void OnDisable()
		{
			agent.enabled = false;
			GetComponent<NavMeshObstacle>().enabled = true;
			bicycleAnimator.SetSpeed(0f, 0f);
			SetFollowingPlayer(followingPlayer: false);
		}

		private void Update()
		{
			if (!agent.pathPending)
			{
				if (FollowingPlayer)
				{
					if ((agent.destination - player.Transform.position).magnitude > 0.5f)
					{
						agent.SetDestination(player.Transform.position);
					}
				}
				else if (agent.desiredVelocity.magnitude < 0.1f)
				{
					agent.SetDestination(randomPositionFinder.GetRandomTargetPoint(base.transform));
				}
			}
			float value = Mathf.Clamp((base.transform.rotation.eulerAngles.y - lastYAngle) / Time.deltaTime * 0.5f, -30f, 30f);
			value = (lastSteerAngle = Mathf.Clamp(value, lastSteerAngle - 100f * Time.deltaTime, lastSteerAngle + 100f * Time.deltaTime));
			lastYAngle = base.transform.rotation.eulerAngles.y;
			bicycleAnimator.SetSpeed(agent.desiredVelocity.magnitude, value);
		}
	}
}
