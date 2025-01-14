using App.Player;
using App.Prefabs;
using App.SaveSystem;
using App.Spawn;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles
{
	public class EmptyCarSpawner : MonoBehaviour
	{
		public enum CarType
		{
			Common,
			Taxi,
			Sport
		}

		[Serializable]
		private class ProbabilityData
		{
			public CarType Type;

			public float Probability;
		}

		[SerializeField]
		private float spawnDistance = 200f;

		[SerializeField]
		private float despawnTime = 60f;

		[SerializeField]
		private float scanInterval = 2f;

		[Header("Probability")]
		[SerializeField]
		private ProbabilityData[] probability;

		private DurationTimer scanTimer = new DurationTimer();

		private PlayerModel player;

		private int layerMask;

		private VehicleSpawner spawner;

		private List<CarSpawnPoint> spawnPoints = new List<CarSpawnPoint>(16);

		private Collider[] colliders = new Collider[10];

		private List<ProbabilityData> probabilityList = new List<ProbabilityData>(4);

		private VehiclePrefabsScriptableObject prefabs;

		private void Awake()
		{
			Initialize();
		}

		private void Update()
		{
			if (scanTimer.Done())
			{
				UpdateSpawnpoints();
				scanTimer.Run(scanInterval);
			}
		}

		private void Initialize()
		{
			layerMask = LayerMask.GetMask("Impact");
			player = ServiceLocator.GetPlayerModel();
			spawner = ServiceLocator.Get<VehicleSpawner>();
			GetComponentsInChildren(spawnPoints);
			probabilityList.AddRange(probability);
			SettingsSaveEntity settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
			PrefabsContainer prefabsContainer = ServiceLocator.Get<PrefabsContainer>();
			prefabs = ((settingsSave.graphicQuality <= 1f) ? prefabsContainer.vehiclePrefabs : prefabsContainer.vehiclePrefabs2);
			scanTimer.Run(scanInterval);
		}

		private void Shuffle()
		{
			probabilityList.Shuffle();
			probabilityList.Sort((ProbabilityData obj1, ProbabilityData obj2) => obj1.Probability.CompareTo(obj2.Probability));
		}

		private void UpdateSpawnpoints()
		{
			Vector3 position = player.Transform.position;
			foreach (CarSpawnPoint spawnPoint in spawnPoints)
			{
				float distance = Vector3.Distance(position, spawnPoint.Position);
				if (spawnPoint.Vehicle == null)
				{
					TrySpawn(spawnPoint, distance);
				}
				else
				{
					TryDespawn(spawnPoint, distance);
				}
			}
		}

		private void TrySpawn(CarSpawnPoint spawnPoint, float distance)
		{
			if (!(distance > spawnDistance) && CheckCollisions(spawnPoint))
			{
				VehiclePrefabId vehiclePrefabId = null;
				vehiclePrefabId = ((!string.IsNullOrEmpty(spawnPoint.VehicleID)) ? prefabs.Find(spawnPoint.VehicleID) : GetRandomPrefab());
				if (!(vehiclePrefabId == null))
				{
					GameObject gameObject = spawner.SpawnVehicle(spawnPoint.Position, vehiclePrefabId.gameObject);
					gameObject.transform.rotation = spawnPoint.transform.rotation;
					spawnPoint.Vehicle = gameObject;
					EnableDestroyer(gameObject, enable: false);
				}
			}
		}

		private void TryDespawn(CarSpawnPoint spawnPoint, float distance)
		{
			if (distance < spawnDistance)
			{
				spawnPoint.DespawnTimer.Run(despawnTime);
			}
			else if (spawnPoint.DespawnTimer.Done())
			{
				spawnPoint.DespawnTimer.Stop();
				EnableDestroyer(spawnPoint.Vehicle.gameObject, enable: true);
				spawnPoint.Vehicle = null;
			}
		}

		private VehiclePrefabId GetRandomPrefab()
		{
			CarType carType = CarType.Common;
			float num = UnityEngine.Random.Range(0f, 1f);
			Shuffle();
			foreach (ProbabilityData probability2 in probabilityList)
			{
				if (probability2.Probability >= num)
				{
					carType = probability2.Type;
					break;
				}
			}
			switch (carType)
			{
			case CarType.Common:
				return prefabs.prefabs.GetRandomElement();
			case CarType.Taxi:
				return prefabs.taxiPrefabs.GetRandomElement();
			case CarType.Sport:
				return prefabs.superSportPrefabs.GetRandomElement();
			default:
				return null;
			}
		}

		private void EnableDestroyer(GameObject vehicle, bool enable)
		{
			vehicle.GetComponent<DestroyGameObject>().enabled = enable;
		}

		private bool CheckCollisions(CarSpawnPoint spawnPoint)
		{
			return Physics.OverlapBoxNonAlloc(spawnPoint.Position + spawnPoint.Bounds.center, spawnPoint.Bounds.size, colliders, base.transform.rotation, layerMask, QueryTriggerInteraction.Ignore) == 0;
		}
	}
}
