using App.AI;
using App.AI.Scanner;
using App.Player;
using App.Prefabs;
using App.SaveSystem;
using App.Settings;
using App.Spawn.Pooling;
using App.Util;
using App.Vehicles;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Car.Navigation.Modes.Curve;
using App.Vehicles.Car.Navigation.Roads;
using App.Vehicles.Car.PoliceCar;
using App.Vehicles.Motorbike;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace App.Spawn
{
	public class VehicleSpawner : MonoBehaviour
	{
		public const float MinCheckInterval = 0.7f;

		public const float MaxCheckInterval = 2f;

		public const float VehicleCountInterval = 2.6f;

		public const float NormalCarProbability = 60f;

		public const float SuperSportProbability = 5f;

		public const float PoliceProbability = 20f;

		public const float TaxiProbability = 15f;

		public const float RescuerProbability = 20f;

		public const float FiretruckProbability = 20f;

		public const float GarbageTruckProbability = 15f;

		public const float BikeProbability = 30f;

		public const float PoliceBikeProbability = 10f;

		private TrafficManager trafficManager;

		private RoadsContainer roadsContainer;

		private EnemyPooler enemyPooler;

		private Transform playerTransform;

		private VehiclePrefabsScriptableObject vehiclePrefabs;

		private VehicleSpawnScheme scheme;

		private Transform inactiveParent;

		private DurationTimer checkTimer = new DurationTimer();

		private DurationTimer vehicleCountTimer = new DurationTimer();

		private DurationTimer delayFiretruckTimer = new DurationTimer();

		private int vehicleCount;

		private int bikeCount;

		private VehiclePrefabId prefab;

		private VehicleSpawnType vehicleType;

		private bool isPolice;

		private int optimalNum;

		private List<VehiclePrefabId> randomVehicles = new List<VehiclePrefabId>(16);

		private List<Resetor> resetors = new List<Resetor>(32);

		private Collider[] colliders = new Collider[32];

		private SettingsSaveEntity settingsSave;

		public GameObject SpawnCarInFire(Vector3 origin, float minimalDistance)
		{
			if (roadsContainer.GetCarInFirePosition(origin, minimalDistance, out RoadSegment segment, out TrafficRoute route, out Vector3 position, out Quaternion rotation))
			{
				VehiclePrefabId vehiclePrefabId = UnityEngine.Object.Instantiate(vehiclePrefabs.prefabs[Random.Range(0, vehiclePrefabs.prefabs.Length)], position, rotation, inactiveParent);
				vehiclePrefabId.GetComponentInChildrenSafe<RoadSeeker>(includeInactive: true).Connect(segment, route);
				vehiclePrefabId.GetComponentSafe<DestroyGameObject>().enabled = false;
				Health componentSafe = vehiclePrefabId.GetComponentSafe<Health>();
				componentSafe.ApplyDamage(componentSafe.maxHealth / 2f);
				vehicleCount++;
				vehiclePrefabId.transform.parent = null;
				return vehiclePrefabId.gameObject;
			}
			return null;
		}

		public GameObject GetRandomSportCarPrefab()
		{
			if (randomVehicles.Count == 0)
			{
				randomVehicles.AddRange(vehiclePrefabs.superSportPrefabs);
			}
			int num = UnityEngine.Random.Range(0, randomVehicles.Count);
			if (num < 0)
			{
				return null;
			}
			GameObject gameObject = randomVehicles[num].gameObject;
			randomVehicles.RemoveAt(num);
			return gameObject;
		}

		public GameObject SpawnVehicle(Vector3 position, GameObject prefab)
		{
			if (prefab == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, null, worldPositionStays: false);
			gameObject.transform.position = position;
			gameObject.gameObject.SetActive(value: true);
			return gameObject;
		}

		public VehiclePrefabId SpawnAIVehicle(VehiclePrefabId prefabID, Vector3 position, Quaternion rotation)
		{
			int num = Physics.OverlapSphereNonAlloc(position, scheme.maxDistance, colliders, 256);
			RoadSegment roadSegment = null;
			float num2 = float.MaxValue;
			TrafficRoute route = null;
			for (int i = 0; i < num; i++)
			{
				RoadSegment component = colliders[i].GetComponent<RoadSegment>();
				Road road = (component != null) ? component.road : null;
				if (road == null)
				{
					continue;
				}
				for (int j = 0; j < road.routes.Length; j++)
				{
					TrafficRoute trafficRoute = road.routes[j];
					RoadSegment segment = null;
					Vector3 closestPosition = trafficRoute.GetClosestPosition(position, out segment);
					float num3 = Vector3.Distance(position, closestPosition);
					if (!(num3 > num2))
					{
						roadSegment = segment;
						num2 = num3;
						route = trafficRoute;
					}
				}
			}
			return SpawnVehicle_Internal(prefabID, position, rotation, roadSegment, route, isPoliceVehicle: false);
		}

		private void Awake()
		{
			settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
			if (settingsSave.graphicQuality == 0f || settingsSave.graphicQuality == 1f)
			{
				vehiclePrefabs = ServiceLocator.Get<PrefabsContainer>().vehiclePrefabs2;
			}
			else
			{
				vehiclePrefabs = ServiceLocator.Get<PrefabsContainer>().vehiclePrefabs;
			}
			trafficManager = ServiceLocator.Get<TrafficManager>();
			roadsContainer = ServiceLocator.Get<RoadsContainer>();
			enemyPooler = ServiceLocator.Get<EnemyPooler>();
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
			scheme = ServiceLocator.Get<QualitySchemeManager>().GetScheme().vehicleSpawn;
			trafficManager.OnPositionFound += OnSpawnPositionFound;
			inactiveParent = new GameObject("~VehicleSpawnerHelper").transform;
			inactiveParent.gameObject.SetActive(value: false);
		}

		private void OnDestroy()
		{
			trafficManager.OnPositionFound -= OnSpawnPositionFound;
		}

		private void Start()
		{
			checkTimer.Run(5f);
			delayFiretruckTimer.Run(10f);
			vehicleCountTimer.Run(2.6f);
		}

		private void Update()
		{
			if (checkTimer.Done())
			{
				checkTimer.Run(0.7f);
				if (vehicleCount < scheme.maxVehicleCount1 && !trafficManager.LookingForSpawnPosition())
				{
					float num = delayFiretruckTimer.InProgress() ? 0f : 20f;
					float num2 = 120f + num + 15f;
					if (bikeCount < scheme.maxBikeCount)
					{
						num2 += 40f;
					}
					float num3 = UnityEngine.Random.Range(0f, num2);
					vehicleType = VehicleSpawnType.Default;
					float num4 = 0f;
					if (num3 <= (num4 += 60f))
					{
						prefab = vehiclePrefabs.prefabs[Random.Range(0, vehiclePrefabs.prefabs.Length)];
					}
					else if (num3 <= (num4 += 5f))
					{
						prefab = vehiclePrefabs.superSportPrefabs[Random.Range(0, vehiclePrefabs.superSportPrefabs.Length)];
					}
					else if (num3 <= (num4 += 20f))
					{
						prefab = vehiclePrefabs.policePrefabs[Random.Range(0, vehiclePrefabs.policePrefabs.Length)];
						vehicleType = VehicleSpawnType.Police;
					}
					else if (num3 <= (num4 += 15f))
					{
						prefab = vehiclePrefabs.taxiPrefabs[Random.Range(0, vehiclePrefabs.taxiPrefabs.Length)];
					}
					else if (num3 <= (num4 += 20f))
					{
						prefab = vehiclePrefabs.ambulancePrefabs[Random.Range(0, vehiclePrefabs.ambulancePrefabs.Length)];
						vehicleType = VehicleSpawnType.Ambulance;
					}
					else if (num3 <= (num4 += num))
					{
						prefab = vehiclePrefabs.firetruckPrefabs[Random.Range(0, vehiclePrefabs.firetruckPrefabs.Length)];
						vehicleType = VehicleSpawnType.Firetruck;
					}
					else if (num3 <= (num4 += 15f))
					{
						prefab = vehiclePrefabs.garbageTruckPrefabs[Random.Range(0, vehiclePrefabs.garbageTruckPrefabs.Length)];
						vehicleType = VehicleSpawnType.GarbageTruck;
					}
					else if (num3 <= (num4 += 30f))
					{
						prefab = vehiclePrefabs.bikePrefabs[Random.Range(0, vehiclePrefabs.bikePrefabs.Length)];
					}
					else
					{
						prefab = vehiclePrefabs.bikePrefabs[Random.Range(0, vehiclePrefabs.bikePrefabs.Length)];
						vehicleType = VehicleSpawnType.Police;
					}
					isPolice = (vehicleType == VehicleSpawnType.Police);
					bool flag = vehicleType == VehicleSpawnType.Ambulance;
					if ((isPolice && enemyPooler.HasPolices(includeReserved: true)) || (flag && enemyPooler.HasRescuers(includeReserved: true)) || (!isPolice && !flag && enemyPooler.HasCiviles(includeReserved: true)))
					{
						EnemyType type = (vehicleType == VehicleSpawnType.Police) ? EnemyType.Police : ((vehicleType == VehicleSpawnType.Ambulance) ? EnemyType.Rescuer : EnemyType.Civil);
						int minRouteCount = scheme.minRouteCount1;
						if (vehicleCount < scheme.maxVehicleCount3)
						{
							minRouteCount = 0;
						}
						else if (vehicleCount < scheme.maxVehicleCount2)
						{
							minRouteCount = scheme.minRouteCount2;
						}
						if (trafficManager.FindSpawnPosition(minRouteCount))
						{
							enemyPooler.ReserveItem(type);
						}
						else
						{
							checkTimer.Run(2f);
						}
					}
				}
			}
			if (!vehicleCountTimer.Done())
			{
				return;
			}
			vehicleCountTimer.Run(2.6f);
			GameObject[] gameObjects = ServiceLocator.GetGameObjects("Vehicle");
			vehicleCount = gameObjects.Length;
			bikeCount = 0;
			GameObject[] array = gameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				if ((bool)array[i].GetComponent<MotorbikeControl>())
				{
					bikeCount++;
				}
			}
		}

		private VehiclePrefabId SpawnVehicle_Internal(VehiclePrefabId prefabID, Vector3 position, Quaternion rotation, RoadSegment roadSegment, TrafficRoute route, bool isPoliceVehicle)
		{
			VehiclePrefabId vehiclePrefabId = UnityEngine.Object.Instantiate(prefabID, position, rotation, inactiveParent);
			vehiclePrefabId.GetComponentInChildrenSafe<RoadSeeker>(includeInactive: true).Connect(roadSegment, route);
			VehicleModesHandler componentSafe = vehiclePrefabId.GetComponentSafe<VehicleModesHandler>();
			AIVehicleModesHandler componentInChildrenSafe = vehiclePrefabId.GetComponentInChildrenSafe<AIVehicleModesHandler>(includeInactive: true);
			componentSafe.SetMode(VehicleMode.AI);
			componentInChildrenSafe.SetMode(AIVehicleMode.Common);
			vehiclePrefabId.transform.parent = null;
			enemyPooler.ClearReservation();
			Resetor resetor = (vehicleType == VehicleSpawnType.Police) ? enemyPooler.Pop(EnemyType.Police) : ((vehicleType != VehicleSpawnType.Ambulance) ? enemyPooler.Pop() : enemyPooler.Pop(EnemyType.Rescuer));
			if (resetor == null)
			{
				resetors.Clear();
				enemyPooler.civilPooler.GetSpawned(resetors);
				resetor = resetors[Random.Range(0, resetors.Count)];
				resetor.ResetStates();
			}
			resetor.GetComponentInChildrenSafe<FreeWalkAIModule>(includeInactive: true).enabled = false;
			resetor.GetComponentSafe<NavMeshAgent>().enabled = false;
			VehicleComponents componentSafe2 = vehiclePrefabId.GetComponentSafe<VehicleComponents>();
			if (vehicleType == VehicleSpawnType.Ambulance)
			{
				resetor.GetComponentInChildrenSafe<RescuerAIModule>(includeInactive: true).myVehicle = componentSafe2;
			}
			else if (vehicleType == VehicleSpawnType.Police)
			{
				resetor.GetComponentInChildrenSafe<PoliceAIModule>(includeInactive: true).myVehicle = componentSafe2;
			}
			bool num = componentSafe2.type == VehicleType.Bike;
			if (num)
			{
				resetor.GetComponentSafe<BikeDriver>().RunDirectlySitting(componentSafe2);
			}
			else
			{
				resetor.GetComponentSafe<CarDriver>().RunDirectlySitting(componentSafe2);
			}
			if (num)
			{
				bikeCount++;
			}
			if (num && !isPoliceVehicle)
			{
				vehiclePrefabId.GetComponentSafe<PoliceCarAI>().enabled = false;
				vehiclePrefabId.GetComponentInChildrenSafe<AIScanner>().enabled = false;
			}
			vehicleCount++;
			return vehiclePrefabId;
		}

		private void OnSpawnPositionFound(Vector3 position, Quaternion rotation, RoadSegment roadSegment, TrafficRoute route)
		{
			SpawnVehicle_Internal(prefab, position, rotation, roadSegment, route, isPolice);
		}
	}
}
