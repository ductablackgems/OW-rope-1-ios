using App.Spawn;
using UnityEngine;

namespace App.AI
{
	public class RandomPositionFinder : MonoBehaviour
	{
		private HumanSpawner humanSpawner;

		private bool initialized;

		public Vector3 GetRandomTargetPoint(Transform origin = null)
		{
			Init();
			if (!humanSpawner.TryFindPosition(out Vector3 position, origin, 0, 35f, useMinimalDistance: false))
			{
				humanSpawner.TryFindPosition(out position, origin, -1, 35f, useMinimalDistance: false, skipCollisionTest: true);
			}
			return position;
		}

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			if (!initialized)
			{
				initialized = true;
				humanSpawner = ServiceLocator.Get<HumanSpawner>();
			}
		}
	}
}
