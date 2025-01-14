using App.AI;
using UnityEngine;

namespace App.Spawn
{
	public sealed class AITankSpawner : AIVehicleSpawner
	{
		private int layerMask;

		private Collider[] colliders = new Collider[32];

		protected override void OnInitialized()
		{
			base.OnInitialized();
			layerMask = LayerMask.GetMask("Climbable", "Impact", "Enemy", "Player");
		}

		protected override Vector3 FindPosition(AIController controller, NavMeshUtils.ScanInput input)
		{
			input.Validate = ((Vector3 position) => CheckSpawnPosition(position, controller));
			return NavMeshUtils.FindValidNavMeshPosition(input);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		private bool CheckSpawnPosition(Vector3 spawnPosition, AIController controller)
		{
			int num = Physics.OverlapSphereNonAlloc(spawnPosition, 3f, colliders, layerMask, QueryTriggerInteraction.Ignore);
			if (num == 0)
			{
				return true;
			}
			for (int i = 0; i < num; i++)
			{
				if (!(colliders[i].gameObject == controller.gameObject))
				{
					return false;
				}
			}
			return true;
		}
	}
}
