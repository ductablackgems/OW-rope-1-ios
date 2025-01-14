using App.Player;
using App.Spawn;
using App.Util;
using App.Vehicles;
using UnityEngine;

namespace App.AI
{
	public class AggressionTracker : AbstractAIScript, IResetable
	{
		private Health health;

		private BikeDriver bikeDriver;

		private CarDriver carDriver;

		private BicycleDriver bicycleDriver;

		private CrimeManager crimeManager;

		private float lastAgressionTime = -1f;

		private float lastShotTime = -1f;

		private float lastPunchTime = -1f;

		private float lastVehicleHitTime = -1f;

		private float lastKickedOutOffVehicleTime = -1f;

		public bool Ganster;

		public void ResetStates()
		{
			lastAgressionTime = -1f;
			lastShotTime = -1f;
			lastPunchTime = -1f;
			lastVehicleHitTime = -1f;
			lastKickedOutOffVehicleTime = -1f;
		}

		public bool AttackedRecently(float interval)
		{
			if (lastAgressionTime != -1f)
			{
				return Time.time - interval < lastAgressionTime;
			}
			return false;
		}

		public bool ShotRecently(float interval)
		{
			if (lastShotTime != -1f)
			{
				return Time.time - interval < lastShotTime;
			}
			return false;
		}

		public bool PunchedRecently(float interval)
		{
			if (lastPunchTime != -1f)
			{
				return Time.time - interval < lastPunchTime;
			}
			return false;
		}

		public bool HitByVehicleRecently(float interval)
		{
			if (lastVehicleHitTime != -1f)
			{
				return Time.time - interval < lastVehicleHitTime;
			}
			return false;
		}

		public bool KickedOutOffVehicleRecently(float interval)
		{
			if (lastKickedOutOffVehicleTime != -1f)
			{
				return Time.time - interval < lastKickedOutOffVehicleTime;
			}
			return false;
		}

		private void Awake()
		{
			health = base.ComponentsRoot.GetComponentSafe<Health>();
			bikeDriver = base.ComponentsRoot.GetComponentSafe<BikeDriver>();
			carDriver = base.ComponentsRoot.GetComponentSafe<CarDriver>();
			bicycleDriver = base.ComponentsRoot.GetComponentSafe<BicycleDriver>();
			crimeManager = ServiceLocator.Get<CrimeManager>();
			health.OnDamage += OnDamage;
			health.OnDie += OnDie;
			bikeDriver.OnKickedOutOffVehicle += OnKickedOutOffVehicle;
			carDriver.OnKickedOutOffVehicle += OnKickedOutOffVehicle;
			bicycleDriver.OnKickedOutOffVehicle += OnKickedOutOffVehicle;
		}

		private void OnDestroy()
		{
			health.OnDamage -= OnDamage;
			bikeDriver.OnKickedOutOffVehicle -= OnKickedOutOffVehicle;
			carDriver.OnKickedOutOffVehicle -= OnKickedOutOffVehicle;
			bicycleDriver.OnKickedOutOffVehicle -= OnKickedOutOffVehicle;
		}

		private void OnDamage(float damage, int damageType, GameObject agressor)
		{
			switch (damageType)
			{
			case 2:
			case 4:
				lastAgressionTime = Time.time;
				lastShotTime = Time.time;
				crimeManager.MarkCrime(CrimeType.ShotPerson);
				break;
			case 1:
				lastAgressionTime = Time.time;
				lastPunchTime = Time.time;
				crimeManager.MarkCrime(CrimeType.PunchPerson);
				break;
			case 3:
			{
				VehicleComponents component = agressor.GetComponent<VehicleComponents>();
				if (component != null && component.driver != null && component.driver.CompareTag("Player"))
				{
					lastAgressionTime = Time.time;
					lastVehicleHitTime = Time.time;
					crimeManager.MarkCrime(CrimeType.HitPersonByVehicle);
				}
				break;
			}
			case 5:
			case 7:
				if (agressor.CompareTagSafe("Player"))
				{
					lastAgressionTime = Time.time;
				}
				break;
			case 6:
				lastAgressionTime = Time.time;
				break;
			case 11:
				if (agressor.CompareTagSafe("Dog"))
				{
					lastAgressionTime = Time.time;
				}
				break;
			}
		}

		private void OnDie()
		{
			if (!Ganster)
			{
				crimeManager.MarkCrime(CrimeType.KillPerson);
			}
			else
			{
				crimeManager.MarkCrime(CrimeType.KillPersonGanster);
			}
		}

		private void OnKickedOutOffVehicle()
		{
			lastAgressionTime = Time.time;
			lastKickedOutOffVehicleTime = Time.time;
			crimeManager.MarkCrime(CrimeType.StealVehicle);
		}
	}
}
