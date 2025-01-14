using UnityEngine.AI;

namespace App.AI
{
	public class FreeWalkAIModule : AbstractAIScript
	{
		private NavMeshAgent agent;

		private NavmeshWalker walker;

		private RandomPositionFinder randomPositionFinder;

		private DurationTimer findPositionDelayTimer = new DurationTimer();

		protected void Awake()
		{
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			randomPositionFinder = ServiceLocator.Get<RandomPositionFinder>();
		}

		private void OnDisable()
		{
			walker.Stop();
		}

		protected void Update()
		{
			if (!walker.CompareCommand(this, NavmeshWalkerState.RandomWalk))
			{
				walker.GoRandom(this, NavmeshWalkerSpeed.Walk);
			}
		}
	}
}
