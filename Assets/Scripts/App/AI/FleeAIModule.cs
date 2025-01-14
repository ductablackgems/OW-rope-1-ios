using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class FleeAIModule : AbstractAIScript
	{
		private NavmeshWalker walker;

		private NavMeshAgent agent;

		private RandomPositionFinder randomPositionFinder;

		private Transform player;

		private AudioSource audio;

		public AudioClip[] clip;

		private DurationTimer stuckTimer = new DurationTimer(useFixedTime: true);

		private int clipNum;

		private bool scream;

		private void Awake()
		{
			walker = this.GetComponentSafe<NavmeshWalker>();
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			randomPositionFinder = ServiceLocator.Get<RandomPositionFinder>();
			player = ServiceLocator.GetGameObject("Player").transform;
			audio = this.GetComponentSafe<AudioSource>();
			clipNum = Random.RandomRange(0, clip.Length);
		}

		private void OnEnable()
		{
			walker.Flee(this, player);
		}

		private void OnDisable()
		{
			walker.Stop();
			if (agent.enabled && agent.isOnNavMesh)
			{
				agent.GetComponent<NavMeshAgent>().SetAreaCost(3, 5f);
			}
			scream = false;
		}

		private void Update()
		{
			if (walker.ChaserTransform != player && agent.desiredVelocity.magnitude < 0.1f)
			{
				walker.GoRandom(this, NavmeshWalkerSpeed.RunFast);
			}
		}

		private void FixedUpdate()
		{
			if (!(walker.ChaserTransform != null))
			{
				return;
			}
			if (walker.GetLastVelocity().magnitude < 4f)
			{
				if (!stuckTimer.Running())
				{
					stuckTimer.Run(1f);
					if (!scream && !audio.isPlaying)
					{
						scream = true;
						audio.clip = clip[clipNum];
						audio.Play();
					}
				}
			}
			else
			{
				stuckTimer.Stop();
			}
			if (stuckTimer.Done())
			{
				stuckTimer.Stop();
				if (agent.enabled && agent.isOnNavMesh)
				{
					agent.SetAreaCost(3, 0f);
				}
				scream = false;
				walker.GoRandom(this, NavmeshWalkerSpeed.RunFast);
			}
		}
	}
}
