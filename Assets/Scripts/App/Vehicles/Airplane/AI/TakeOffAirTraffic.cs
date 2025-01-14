using App.AI;
using App.Spawn;
using App.Spawn.Pooling;
using App.Util;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Airplane.AI
{
	public class TakeOffAirTraffic : AirTraffic
	{
		[Header("Passengers")]
		[SerializeField]
		private bool spawnPassengers = true;

		[SerializeField]
		private float waitForPassengers = 20f;

		[SerializeField]
		private float maxPassengerSpawnDistance = 100f;

		[SerializeField]
		private FloatRange passengerSpawnDelay = new FloatRange(0.5f, 1f);

		private SpawnPoint[] spawnPoints;

		private HumanSpawner spawner;

		private int agentsToSpawn;

		private DurationTimer takeOffTimer = new DurationTimer();

		private DurationTimer spawnTimer = new DurationTimer();

		private List<NavmeshWalker> passengers = new List<NavmeshWalker>(16);

		protected override void OnInitialize()
		{
			base.OnInitialize();
			spawner = ServiceLocator.Get<HumanSpawner>();
			spawnPoints = GetComponentsInChildren<SpawnPoint>();
		}

		protected override void OnRun()
		{
			base.OnRun();
			if (CanSpawnPassangers())
			{
				airplane.Pause(isPaused: true);
				takeOffTimer.Run(waitForPassengers);
				spawnTimer.Run(passengerSpawnDelay.GetRandomValue());
				agentsToSpawn = spawner.maxHumanCount / 2;
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (base.IsRunning && takeOffTimer.Running())
			{
				UpdatePassengerSpawn();
				UpdatePassengersDestination();
				UpdateTakeOffTime();
			}
		}

		private Resetor SelectBestHumanToSpawn(List<Resetor> candidates)
		{
			float num = 0f;
			Vector3 position = base.Airplane.transform.position;
			Resetor result = null;
			for (int i = 0; i < candidates.Count; i++)
			{
				Resetor resetor = candidates[i];
				if (!(resetor == null))
				{
					float num2 = Vector3.Distance(resetor.transform.position, position);
					if (!(num2 < num))
					{
						num = num2;
						result = resetor;
					}
				}
			}
			return result;
		}

		private void UpdatePassengersDestination()
		{
			if (passengers.Count == 0)
			{
				return;
			}
			int num = passengers.Count;
			while (num-- > 0)
			{
				NavmeshWalker navmeshWalker = passengers[num];
				if (!(Vector3.Distance(navmeshWalker.transform.position, airplane.transform.position) > 10f))
				{
					navmeshWalker.Stop();
					passengers.RemoveAt(num);
					spawner.Despawn(navmeshWalker.ComponentsRoot.gameObject);
				}
			}
		}

		private void SpawnPassenger()
		{
			SpawnPoint randomElement = spawnPoints.GetRandomElement(allowRepeat: false);
			Resetor resetor = spawner.Spawn(EnemyType.Civil, randomElement.transform.position, isWalking: false, force: true, SelectBestHumanToSpawn);
			if (!(resetor == null))
			{
				NavmeshWalker componentInChildren = resetor.GetComponentInChildren<NavmeshWalker>();
				componentInChildren.transform.rotation = randomElement.transform.rotation;
				componentInChildren.FollowTransform(this, airplane.transform, NavmeshWalkerSpeed.RunFast);
				passengers.Add(componentInChildren);
			}
		}

		private bool CanSpawnPassangers()
		{
			if (!spawnPassengers)
			{
				return false;
			}
			if (spawnPoints.Length == 0)
			{
				return false;
			}
			if (maxPassengerSpawnDistance <= 0f)
			{
				return true;
			}
			return Vector3.Distance(player.Transform.position, airplane.transform.position) < maxPassengerSpawnDistance;
		}

		private void UpdateTakeOffTime()
		{
			if (takeOffTimer.Done())
			{
				takeOffTimer.Stop();
				foreach (NavmeshWalker passenger in passengers)
				{
					passenger.Stop();
				}
				passengers.Clear();
				airplane.Pause(isPaused: false);
			}
		}

		private void UpdatePassengerSpawn()
		{
			if (spawnTimer.Done())
			{
				if (agentsToSpawn == 0)
				{
					spawnTimer.Stop();
					return;
				}
				agentsToSpawn--;
				SpawnPassenger();
				spawnTimer.Run(passengerSpawnDelay.GetRandomValue());
			}
		}
	}
}
