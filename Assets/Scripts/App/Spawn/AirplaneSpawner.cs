using App.Player;
using App.Prefabs;
using App.Vehicles.Airplane;
using System.Collections.Generic;
using UnityEngine;

namespace App.Spawn
{
	public class AirplaneSpawner : MonoBehaviour
	{
		[SerializeField]
		private float deactivationDistance = 200f;

		[SerializeField]
		private float destroyDistance = 400f;

		[SerializeField]
		private float distanceCheckInterval = 5f;

		private SpawnPoint[] spawnPoints;

		private int layerMask;

		private PlayerModel player;

		private Collider[] colliders = new Collider[10];

		private List<AirplaneController> spawned = new List<AirplaneController>(4);

		private List<AirplaneController> airplanesInDock = new List<AirplaneController>(2);

		private DurationTimer distanceCheckTimer = new DurationTimer();

		public List<AirplaneController> Prefabs
		{
			get;
			private set;
		}

		public void Spawn(string vehicleID)
		{
			AirplaneController airplane = GetAirplane(vehicleID, Prefabs);
			SpawnPoint spawnPoint = FindSpawnPoistion(airplane);
			if (!(spawnPoint == null))
			{
				AirplaneController airplaneController = UnityEngine.Object.Instantiate(airplane, spawnPoint.Position, spawnPoint.transform.rotation);
				airplaneController.Initialize();
				airplaneController.Spawn(spawnPoint);
				airplaneController.Destroyed += OnAirplaneDestroyed;
				spawned.Add(airplaneController);
			}
		}

		public void Respawn(string id)
		{
			AirplaneController airplane = GetAirplane(id, spawned);
			if (airplane == null)
			{
				Spawn(id);
			}
			else
			{
				airplane.Respawn();
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Update()
		{
			Scan();
		}

		private void OnAirplaneDestroyed(AirplaneController airplane)
		{
			airplane.Destroyed -= OnAirplaneDestroyed;
			spawned.Remove(airplane);
		}

		private void Initialize()
		{
			layerMask = LayerMask.GetMask("Impact");
			player = ServiceLocator.GetPlayerModel();
			VehiclePrefabId[] airplanePrefabs = ServiceLocator.Get<PrefabsContainer>().vehiclePrefabs.airplanePrefabs;
			Prefabs = new List<AirplaneController>(airplanePrefabs.Length);
			VehiclePrefabId[] array = airplanePrefabs;
			foreach (VehiclePrefabId vehiclePrefabId in array)
			{
				Prefabs.Add(vehiclePrefabId.GetComponent<AirplaneController>());
			}
			spawnPoints = GetComponentsInChildren<SpawnPoint>();
			GetComponentsInChildren(airplanesInDock);
			foreach (AirplaneController item in airplanesInDock)
			{
				item.Initialize(enableDistanceDestroy: false);
				item.transform.SetParent(null);
			}
			distanceCheckTimer.Run(distanceCheckInterval);
		}

		private SpawnPoint FindSpawnPoistion(AirplaneController controller)
		{
			SpawnPoint[] array = spawnPoints;
			foreach (SpawnPoint spawnPoint in array)
			{
				if (IsValidPosition(spawnPoint, controller.Bounds))
				{
					return spawnPoint;
				}
			}
			return null;
		}

		private bool IsValidPosition(SpawnPoint spawnPoint, Bounds airplaneBounds)
		{
			return Physics.OverlapBoxNonAlloc(spawnPoint.Position + airplaneBounds.center, airplaneBounds.extents, colliders, spawnPoint.transform.rotation, layerMask) == 0;
		}

		private static AirplaneController GetAirplane(string id, List<AirplaneController> list)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			foreach (AirplaneController item in list)
			{
				if (!(item.ID.tid != id))
				{
					return item.GetComponent<AirplaneController>();
				}
			}
			return null;
		}

		private void Scan()
		{
			if (distanceCheckTimer.Done())
			{
				CheckDistances(spawned);
				CheckDistances(airplanesInDock);
				distanceCheckTimer.Run(distanceCheckInterval);
			}
		}

		private void CheckDistances(List<AirplaneController> airplanes)
		{
			if (airplanes.Count == 0)
			{
				return;
			}
			Vector3 position = player.Transform.position;
			int num = airplanes.Count;
			while (num-- > 0)
			{
				AirplaneController airplaneController = airplanes[num];
				if (!(airplaneController == null) && !airplaneController.IsActive)
				{
					float num3 = Vector3.Distance(position, airplaneController.transform.position);
					if (airplaneController.EnableDistanceDestroy && num3 > destroyDistance)
					{
						UnityEngine.Object.Destroy(airplaneController.gameObject);
						airplanes.RemoveAt(num);
					}
					else
					{
						bool active = num3 < deactivationDistance;
						airplaneController.gameObject.SetActive(active);
					}
				}
			}
		}
	}
}
