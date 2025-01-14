using App.AI;
using App.GUI;
using App.Spawn;
using App.Util;
using UnityEngine;

namespace App.Player
{
	public class CrimeManager : MonoBehaviour
	{
		public float decreaseCriminalitySpeed = 2f;

		public float maxCrime = 1000f;

		public float[] criminalityTresholds = new float[4];

		public float extraForcesSpawnInterval = 120f;

		public float killPersonWeight = 110f;

		public float killPersonGansterWeight = 35f;

		public float punchPersonWeight = 5f;

		public float shotPersonWeight = 25f;

		public float hitPersonByVehicleWeight = 5f;

		public float stealVehicleWeight = 35f;

		[Header("Sounds")]
		public AudioClip Alarm;

		private PanelsManager panelsManager;

		private RagdollInteractor playerRagdollInteractor;

		private float criminalityValue;

		private int starCount;

		private DurationTimer vehicleSpawnTimer = new DurationTimer();

		private DurationTimer alarmTimer = new DurationTimer();

		private CameraSounds sounds;

		private int spawnedVehicles;

		private AIVehicleSpawner[] vehicleSpawners;

		private AIController lastSpawnedVehicle;

		public int StarCount => starCount;

		public bool Busted
		{
			get;
			private set;
		}

		public float CriminalityValue => criminalityValue;

		public void ResetStates()
		{
			criminalityValue = 0f;
			starCount = 0;
			AIVehicleSpawner[] array = vehicleSpawners;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DespawnAll();
			}
		}

		public void SetCrime(float crime)
		{
			criminalityValue = crime;
		}

		public void MarkCrime(CrimeType type)
		{
			switch (type)
			{
			case CrimeType.KillPerson:
				criminalityValue += killPersonWeight;
				break;
			case CrimeType.PunchPerson:
				criminalityValue += punchPersonWeight;
				break;
			case CrimeType.ShotPerson:
				criminalityValue += shotPersonWeight;
				break;
			case CrimeType.HitPersonByVehicle:
				criminalityValue += hitPersonByVehicleWeight;
				break;
			case CrimeType.StealVehicle:
				criminalityValue += stealVehicleWeight;
				break;
			case CrimeType.KillPersonGanster:
				criminalityValue += killPersonGansterWeight;
				break;
			}
			if (criminalityValue > maxCrime)
			{
				criminalityValue = maxCrime;
			}
		}

		public void SetBusted(bool busted)
		{
			Busted = busted;
			playerRagdollInteractor.standUpBlocked = busted;
			if (busted)
			{
				panelsManager.ShowPanel(PanelType.Busted);
			}
		}

		private void Awake()
		{
			panelsManager = ServiceLocator.Get<PanelsManager>();
			playerRagdollInteractor = ServiceLocator.GetGameObject("Player").GetComponentSafe<RagdollInteractor>();
			sounds = ServiceLocator.Get<CameraSounds>();
			vehicleSpawners = new AIVehicleSpawner[2]
			{
				ServiceLocator.Get<AIMechSpawner>(),
				ServiceLocator.Get<AITankSpawner>()
			};
			AIVehicleSpawner[] array = vehicleSpawners;
			foreach (AIVehicleSpawner obj in array)
			{
				obj.Spawned += OnVehicleSpawned;
				obj.Despawned += OnVehicleDespawned;
			}
		}

		private void Update()
		{
			criminalityValue -= decreaseCriminalitySpeed * Time.deltaTime;
			if (criminalityValue < 0f)
			{
				criminalityValue = 0f;
			}
			starCount = criminalityTresholds.Length;
			for (int i = 0; i < criminalityTresholds.Length; i++)
			{
				if (criminalityTresholds[i] > criminalityValue)
				{
					starCount = i;
					break;
				}
			}
			TrySpawnExtraForces();
			UpdateAlarm();
		}

		private void OnVehicleSpawned()
		{
			spawnedVehicles++;
		}

		private void OnVehicleDespawned()
		{
			vehicleSpawnTimer.Run(extraForcesSpawnInterval);
			spawnedVehicles = Mathf.Max(0, spawnedVehicles - 1);
			lastSpawnedVehicle = null;
		}

		private void TrySpawnExtraForces()
		{
			if (starCount != criminalityTresholds.Length || vehicleSpawnTimer.InProgress() || spawnedVehicles > 0)
			{
				return;
			}
			AIController x = SpawnRandomAIVehicle();
			if (x == null)
			{
				vehicleSpawnTimer.Run(5f);
				return;
			}
			vehicleSpawnTimer.Stop();
			if (CanPlayAlarm())
			{
				PlaySound(Alarm);
			}
			lastSpawnedVehicle = x;
		}

		private void UpdateAlarm()
		{
			if (starCount != criminalityTresholds.Length)
			{
				alarmTimer.Stop();
			}
			else if (!CanPlayAlarm())
			{
				alarmTimer.Stop();
			}
			else if (alarmTimer.Done() || !alarmTimer.Running())
			{
				alarmTimer.Run(UnityEngine.Random.Range(10f, 15f));
				PlaySound(Alarm);
			}
		}

		private void PlaySound(AudioClip clip)
		{
			if (!(sounds == null))
			{
				sounds.PlayOneShot(clip);
			}
		}

		private AIController SpawnRandomAIVehicle()
		{
			int num = UnityEngine.Random.Range(0, vehicleSpawners.Length);
			return vehicleSpawners[num].SpawnRandomAI();
		}

		private bool CanPlayAlarm()
		{
			if (lastSpawnedVehicle == null)
			{
				return false;
			}
			return lastSpawnedVehicle is MechAIController;
		}
	}
}
