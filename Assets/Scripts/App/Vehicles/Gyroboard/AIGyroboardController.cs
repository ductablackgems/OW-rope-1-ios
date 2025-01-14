using App.AI;
using App.Player;
using UnityEngine;
using UnityEngine.AI;

namespace App.Vehicles.Gyroboard
{
	public class AIGyroboardController : MonoBehaviour, IFollowingVehicle
	{
		private NavMeshAgent agent;

		private RandomPositionFinder randomPositionFinder;

		private PlayerModel player;

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
			randomPositionFinder = ServiceLocator.Get<RandomPositionFinder>();
			player = ServiceLocator.GetPlayerModel();
		}

		private void OnEnable()
		{
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
			SetFollowingPlayer(followingPlayer: false);
		}

		private void Update()
		{
			if (agent.pathPending)
			{
				return;
			}
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
	}
}
