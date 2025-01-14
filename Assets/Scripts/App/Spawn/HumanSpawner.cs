using App.AI;
using App.Player;
using App.Prefabs;
using App.SaveSystem;
using App.Spawn.Pooling;
using App.Vehicles;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace App.Spawn
{
	public class HumanSpawner : MonoBehaviour
	{
		public const float CheckInterval = 0.4f;

		public const float HumanCountInterval = 2.3f;

		public const float MaxSamplingCount = 8f;

		public int maxHumanCount = 10;

		private Transform playerTransform;

		private EnemyPooler enemyPooler;

		private VehiclePrefabsScriptableObject vehiclePrefabs;

		private DurationTimer checkTimer = new DurationTimer();

		private DurationTimer humanCountTimer = new DurationTimer();

		private int humanCount;

		private float optimalNum;

		private int maxDistance;

		private SettingsSaveEntity settingsSave;

		private bool initialized;

		private List<Resetor> spawned = new List<Resetor>(64);

		public Resetor Spawn(EnemyType type, Vector3 position, bool isWalking, bool force = false, Func<List<Resetor>, Resetor> selectHumanTospawn = null)
		{
			Resetor resetor = Spawn_Internal(type, position, isCyclist: false, isGyroboard: false, isSkateboard: false);
			if (resetor != null)
			{
				resetor.GetComponentInChildrenSafe<FreeWalkAIModule>().enabled = isWalking;
				return resetor;
			}
			if (!force)
			{
				return null;
			}
			HumanPooler pooler = enemyPooler.GetPooler(type);
			if (pooler == null)
			{
				return null;
			}
			spawned.Clear();
			pooler.GetSpawned(spawned);
			if (spawned.Count == 0)
			{
				return null;
			}
			resetor = ((selectHumanTospawn != null) ? selectHumanTospawn(spawned) : spawned[UnityEngine.Random.Range(0, spawned.Count)]);
			resetor.ResetStates();
			resetor.transform.position = position;
			resetor.transform.rotation = Quaternion.identity;
			CleanRunningVehicle(resetor.gameObject);
			resetor.GetComponentInChildrenSafe<FreeWalkAIModule>().enabled = isWalking;
			NavMeshAgent componentSafe = resetor.GetComponentSafe<NavMeshAgent>();
			componentSafe.enabled = true;
			componentSafe.Warp(position);
			return resetor;
		}

		public void Despawn(GameObject human)
		{
			if (!(human == null))
			{
				CleanRunningVehicle(human);
				Resetor component = human.GetComponent<Resetor>();
				component.ResetStates();
				enemyPooler.Push(component);
				human.SetActive(value: false);
			}
		}

		private void CleanRunningVehicle(GameObject human)
		{
			AbstractVehicleDriver[] components = human.GetComponents<AbstractVehicleDriver>();
			foreach (AbstractVehicleDriver abstractVehicleDriver in components)
			{
				if (abstractVehicleDriver.Running())
				{
					VehicleComponents vehicle = abstractVehicleDriver.Vehicle;
					abstractVehicleDriver.Stop();
					if (vehicle.passenger != null)
					{
						Resetor componentSafe = vehicle.passenger.GetComponentSafe<Resetor>();
						enemyPooler.Push(componentSafe);
					}
					if (vehicle != null)
					{
						UnityEngine.Object.Destroy(vehicle.gameObject);
					}
				}
			}
		}

		private void Awake()
		{
			settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
			Init();
		}

		private void Start()
		{
			checkTimer.FakeDone(0.4f);
			humanCountTimer.Run(2.3f);
		}

		private void Update()
		{
			if (checkTimer.Done())
			{
				checkTimer.Run(0.4f);
				bool flag = UnityEngine.Random.Range(0f, 100f) < 17f;
				if ((flag ? enemyPooler.HasPolices() : enemyPooler.HasCiviles()) && humanCount < maxHumanCount && TryFindPosition(out Vector3 position))
				{
					EnemyType enemyType = flag ? EnemyType.Police : EnemyType.Civil;
					int num = UnityEngine.Random.Range(0, 100);
					bool flag2 = num < 17;
					bool flag3 = !flag2 && num < 34;
					bool isSkateboard = !flag2 && !flag3 && num < 51;
					Spawn_Internal(enemyType, position, flag2, flag3, isSkateboard);
				}
			}
			if (!humanCountTimer.Done())
			{
				return;
			}
			humanCountTimer.Run(2.3f);
			humanCount = 0;
			GameObject[] gameObjects = ServiceLocator.GetGameObjects("Enemy");
			foreach (GameObject gameObject in gameObjects)
			{
				if (gameObject.transform.parent == null || gameObject.transform.root.CompareTag("Bicycle") || gameObject.transform.root.CompareTag("Gyroboard") || gameObject.transform.root.CompareTag("Skateboard"))
				{
					humanCount++;
				}
			}
		}

		private Resetor Spawn_Internal(EnemyType enemyType, Vector3 position, bool isCyclist, bool isGyroboard, bool isSkateboard)
		{
			Resetor resetor = enemyPooler.Pop(enemyType);
			if (resetor == null)
			{
				return null;
			}
			resetor.transform.position = position;
			resetor.transform.rotation = Quaternion.identity;
			humanCount++;
			if (isCyclist | isGyroboard | isSkateboard)
			{
				GameObject original = isCyclist ? vehiclePrefabs.bicycles[UnityEngine.Random.Range(0, vehiclePrefabs.bicycles.Length)] : ((!isGyroboard) ? vehiclePrefabs.skateboardPrefabs[UnityEngine.Random.Range(0, vehiclePrefabs.skateboardPrefabs.Length)].gameObject : vehiclePrefabs.gyroboards[UnityEngine.Random.Range(0, vehiclePrefabs.gyroboards.Length)]);
				GameObject gameObject = UnityEngine.Object.Instantiate(original, position, Quaternion.identity);
				resetor.GetComponentInChildrenSafe<FreeWalkAIModule>().enabled = false;
				resetor.GetComponentSafe<NavMeshAgent>().enabled = false;
				StreetVehicleAIModule componentInChildrenSafe = resetor.GetComponentInChildrenSafe<StreetVehicleAIModule>();
				componentInChildrenSafe.myVehicle = gameObject.GetComponentSafe<VehicleComponents>();
				componentInChildrenSafe.myVehicle.streetVehicleModesHelper.SetAIState();
				componentInChildrenSafe.enabled = true;
				if (isCyclist)
				{
					resetor.GetComponentSafe<BicycleDriver>().RunDirectlySitting(componentInChildrenSafe.myVehicle);
				}
				else if (isGyroboard)
				{
					resetor.GetComponentSafe<GyroboardDriver>().RunDirectlySitting(componentInChildrenSafe.myVehicle);
				}
				else
				{
					resetor.GetComponentSafe<SkateboardDriver>().RunDirectlySitting(componentInChildrenSafe.myVehicle);
				}
			}
			else
			{
				resetor.GetComponentInChildrenSafe<FreeWalkAIModule>().enabled = true;
				resetor.GetComponentSafe<NavMeshAgent>().enabled = true;
			}
			return resetor;
		}

		private void Init()
		{
			if (!initialized)
			{
				initialized = true;
				playerTransform = ServiceLocator.GetGameObject("Player").transform;
				enemyPooler = ServiceLocator.Get<EnemyPooler>();
				vehiclePrefabs = ServiceLocator.Get<PrefabsContainer>().vehiclePrefabs;
				optimalNum = settingsSave.graphicQuality;
				if (optimalNum == 2f)
				{
					maxDistance = 50;
					maxHumanCount = 10;
				}
				else if (optimalNum == 1f)
				{
					maxDistance = 40;
					maxHumanCount = 7;
				}
				else if (optimalNum == 0f)
				{
					maxDistance = 25;
					maxHumanCount = 4;
				}
				else if (optimalNum == 3f)
				{
					maxDistance = 60;
					maxHumanCount = 15;
				}
			}
		}

		public bool TryFindPosition(out Vector3 position, Transform origin = null, int layerMask = 1, float maxDistanceExtra = 0f, bool useMinimalDistance = true, bool skipCollisionTest = false)
		{
			Init();
			position = Vector3.zero;
			float num = (float)maxDistance + maxDistanceExtra;
			if (!(origin == null))
			{
				Vector3 position2 = origin.position;
			}
			else
			{
				Vector3 position3 = playerTransform.position;
			}
			for (int i = 0; (float)i < 8f; i++)
			{
				if (!NavMesh.SamplePosition(UnityEngine.Random.insideUnitSphere * num + playerTransform.position, out NavMeshHit hit, num, layerMask))
				{
					continue;
				}
				float magnitude = (hit.position - playerTransform.position).magnitude;
				if (useMinimalDistance && magnitude < num * 0.7f)
				{
					continue;
				}
				if (skipCollisionTest)
				{
					position = hit.position;
					return true;
				}
				Collider[] array = Physics.OverlapSphere(hit.position, 10f);
				bool flag = false;
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					if (collider.CompareTag("Enemy") || collider.CompareTag("Vehicle"))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					position = hit.position;
					return true;
				}
			}
			return false;
		}
	}
}
