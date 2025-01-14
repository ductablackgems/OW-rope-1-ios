using App.Spawn;
using App.Spawn.Pooling;
using App.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Airplane.AI
{
	public class LandingAirTraffic : AirTraffic
	{
		[Header("Passengers")]
		[SerializeField]
		private bool spawnPassengers = true;

		[SerializeField]
		private float maxPassengerSpawnDistance = 100f;

		[SerializeField]
		private FloatRange passengerSpawnDelay = new FloatRange(0.5f, 2f);

		private HumanSpawner humanSpawner;

		private Coroutine spawnCoroutine;

		protected override void OnInitialize()
		{
			base.OnInitialize();
			humanSpawner = ServiceLocator.Get<HumanSpawner>();
		}

		protected override void OnAirplanePathFinished()
		{
			if (!CanSpawnPassengers())
			{
				base.IsRunning = false;
				return;
			}
			StopSpawnCoroutine();
			spawnCoroutine = StartCoroutine(SpawnLeavingPassengers_Coroutine(airplane));
		}

		protected override void OnStop()
		{
			base.OnStop();
			StopSpawnCoroutine();
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

		private IEnumerator SpawnLeavingPassengers_Coroutine(AirplaneAIController airplane)
		{
			int amount = humanSpawner.maxHumanCount;
			for (int idx = 0; idx < amount; idx++)
			{
				humanSpawner.Spawn(EnemyType.Civil, airplane.transform.position, isWalking: true, force: true, SelectBestHumanToSpawn);
				yield return new WaitForSeconds(passengerSpawnDelay.GetRandomValue());
			}
			base.IsRunning = false;
		}

		private void StopSpawnCoroutine()
		{
			if (spawnCoroutine != null)
			{
				StopCoroutine(spawnCoroutine);
				spawnCoroutine = null;
			}
		}

		private bool CanSpawnPassengers()
		{
			if (!spawnPassengers)
			{
				return false;
			}
			if (maxPassengerSpawnDistance <= 0f)
			{
				return true;
			}
			return Vector3.Distance(player.Transform.position, airplane.transform.position) <= maxPassengerSpawnDistance;
		}
	}
}
