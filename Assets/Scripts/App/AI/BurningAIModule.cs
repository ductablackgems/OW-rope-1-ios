using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class BurningAIModule : AbstractAIScript
	{
		private NavMeshAgent agent;

		private NavmeshWalker walker;

		private DurationTimer findPositionTimer = new DurationTimer();

		private void Awake()
		{
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			walker = this.GetComponentSafe<NavmeshWalker>();
		}

		private void OnEnable()
		{
			walker.GoRandom(this, NavmeshWalkerSpeed.RunFast);
			findPositionTimer.Run(Random.Range(3, 6));
		}

		private void OnDisable()
		{
			walker.Stop();
		}

		private void Update()
		{
			if (agent.enabled && findPositionTimer.Done())
			{
				walker.GoRandom(this, NavmeshWalkerSpeed.RunFast);
				findPositionTimer.Run(Random.Range(3, 6));
			}
		}
	}
}
