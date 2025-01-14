using App.AI;
using App.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Spawn
{
	public abstract class AIVehicleSpawner : MonoBehaviour
	{
		[Serializable]
		public class SpawnConfig
		{
			public float MinSpawnDistance = 30f;

			public float MaxSpawnDistance = 40f;
		}

		public SpawnConfig Config;

		public AIController[] Prefabs;

		private PlayerModel player;

		private List<AIController> spawnedAI = new List<AIController>(8);

		private List<AIController> aiPrefabs = new List<AIController>(8);

		private NavMeshUtils.ScanInput scanInput;

		public int SpawnedAICount => spawnedAI.Count;

		public event Action Spawned;

		public event Action Despawned;


		protected virtual void OnInitialized()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		public AIController Spawn(AIController prefab, Vector3 position = default(Vector3))
		{
			if (prefab == null)
			{
				return null;
			}
			scanInput.StartDirection = GetSpawnDirection();
			scanInput.Pivot = player.Transform.position;
			position = ((position != Vector3.zero) ? position : FindPosition(prefab, scanInput));
			if (position == Vector3.zero)
			{
				return null;
			}
			AIController aIController = UnityEngine.Object.Instantiate(prefab);
			aIController.Initialize();
			aIController.Spawn(position);
			Vector3 forward = player.Transform.position - aIController.transform.position;
			forward.y = 0f;
			aIController.transform.forward = forward;
			AIController component = aIController.GetComponent<AIController>();
			component.ControllerDestroy += OnAIDestroyed;
			spawnedAI.Add(component);
			if (this.Spawned != null)
			{
				this.Spawned();
			}
			return aIController;
		}

		public AIController SpawnRandomAI()
		{
			return Spawn(GetRandomPrefab());
		}

		public void DespawnAll()
		{
			foreach (AIController item in spawnedAI)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Update()
		{
			OnUpdate();
		}

		private void OnAIDestroyed(AIController controller)
		{
			int num = spawnedAI.Count;
			while (true)
			{
				if (num-- > 0)
				{
					AIController x = spawnedAI[num];
					if (x == null)
					{
						spawnedAI.RemoveAt(num);
					}
					else if (!(x != controller))
					{
						break;
					}
					continue;
				}
				return;
			}
			spawnedAI.RemoveAt(num);
			if (this.Despawned != null)
			{
				this.Despawned();
			}
		}

		protected virtual Vector3 FindPosition(AIController controller, NavMeshUtils.ScanInput input)
		{
			return NavMeshUtils.FindValidNavMeshPosition(input);
		}

		private void Initialize()
		{
			player = ServiceLocator.GetPlayerModel();
			scanInput = new NavMeshUtils.ScanInput
			{
				MinScanDistance = Config.MinSpawnDistance,
				MaxScanDistance = Config.MaxSpawnDistance,
				ScanAngle = 10f,
				ScanStep = 3f
			};
			OnInitialized();
		}

		private Vector3 GetSpawnDirection()
		{
			Vector3 forward = UnityEngine.Camera.main.transform.forward;
			forward.y = 0f;
			return forward;
		}

		private AIController GetRandomPrefab()
		{
			if (Prefabs.Length == 0)
			{
				return null;
			}
			if (aiPrefabs.Count == 0)
			{
				aiPrefabs.AddRange(Prefabs);
			}
			int index = UnityEngine.Random.Range(0, aiPrefabs.Count);
			AIController result = aiPrefabs[index];
			aiPrefabs.RemoveAt(index);
			return result;
		}
	}
}
