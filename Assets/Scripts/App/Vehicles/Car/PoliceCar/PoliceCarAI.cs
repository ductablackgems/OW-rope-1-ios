using App.AI;
using App.AI.Monitor;
using App.AI.Scanner;
using App.Player;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Sirene;
using UnityEngine;

namespace App.Vehicles.Car.PoliceCar
{
	public class PoliceCarAI : MonoBehaviour
	{
		public SireneController sirene;

		private VehicleComponents vehicleComponents;

		private VehicleModesHandler vehicleModesHandler;

		private AIVehicleModesHandler aiVehicleModesHandler;

		private TargettingVehicleController targettingVehicleController;

		private SkidBrake skidBrake;

		private DeathEnemiesMonitor deathEnemiesMonitor;

		private AIScanner scanner;

		private PlayerModel player;

		private CrimeManager crimeManager;

		private DurationTimer getOutOffCarTimer = new DurationTimer();

		private DurationTimer checkDeathsTimer = new DurationTimer();

		private DurationTimer checkPlayerTimer = new DurationTimer();

		private DurationTimer brakeTimer = new DurationTimer();

		private bool targettingActivated;

		private bool driverIsGettingOut;

		public DeathTarget DeathTarget
		{
			get;
			private set;
		}

		public bool TargetingToPlayer
		{
			get;
			private set;
		}

		private void Awake()
		{
			vehicleComponents = this.GetComponentSafe<VehicleComponents>();
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			aiVehicleModesHandler = this.GetComponentInChildrenSafe<AIVehicleModesHandler>();
			targettingVehicleController = this.GetComponentInChildrenSafe<TargettingVehicleController>();
			skidBrake = this.GetComponentInChildrenSafe<SkidBrake>();
			deathEnemiesMonitor = ServiceLocator.Get<DeathEnemiesMonitor>();
			scanner = this.GetComponentInChildrenSafe<AIScanner>();
			player = ServiceLocator.GetPlayerModel();
			crimeManager = ServiceLocator.Get<CrimeManager>();
			checkDeathsTimer.Run(2f);
			checkPlayerTimer.Run(1.3f);
		}

		private void Update()
		{
			if (vehicleModesHandler.mode != VehicleMode.AI)
			{
				if (sirene != null && sirene.Running())
				{
					sirene.Stop();
				}
				if (driverIsGettingOut)
				{
					driverIsGettingOut = false;
				}
			}
			else
			{
				if (driverIsGettingOut)
				{
					return;
				}
				if (checkDeathsTimer.Done())
				{
					checkDeathsTimer.Run(5f);
					CheckDeaths();
				}
				if (checkPlayerTimer.Done())
				{
					checkPlayerTimer.Run(1.3f);
				}
				if ((DeathTarget.IsValid() && (brakeTimer.Done() || (!getOutOffCarTimer.Running() && (DeathTarget.TargetTransform.position - base.transform.position).magnitude < 17f))) || (crimeManager.StarCount > 2 && scanner.HasTrackToPlayer && !getOutOffCarTimer.Running()))
				{
					brakeTimer.Stop();
					skidBrake.ForceBrake(5f);
					getOutOffCarTimer.Run(3f);
					if (sirene != null)
					{
						sirene.Run();
					}
				}
				if (!getOutOffCarTimer.Done())
				{
					return;
				}
				getOutOffCarTimer.Stop();
				if (vehicleComponents.driver != null && (DeathTarget.IsValid() || (crimeManager.StarCount > 2 && scanner.HasTrackToPlayer)))
				{
					driverIsGettingOut = true;
					if (DeathTarget.IsValid())
					{
						vehicleComponents.driver.GetComponentInChildrenSafe<PoliceAIModule>(includeInactive: true).DeathTarget = DeathTarget;
					}
					DeathTarget.Clear();
					targettingVehicleController.ClearTarget();
					if (vehicleComponents.type == VehicleType.Bike)
					{
						vehicleComponents.driver.GetComponentSafe<BikeDriver>().Stop();
					}
					else
					{
						vehicleComponents.driver.GetComponentSafe<CarDriver>().Stop();
					}
					if (scanner.HasTrackToPlayer)
					{
						vehicleComponents.driver.GetComponentInChildrenSafe<AIScanner>().UpdateByOtherScanner(scanner);
					}
				}
			}
		}

		private void CheckDeaths()
		{
			if (DeathTarget.IsValid())
			{
				return;
			}
			DeathTarget = deathEnemiesMonitor.FindAnyTarget(base.transform.position, 50f);
			if (!DeathTarget.IsValid())
			{
				if (!targettingActivated)
				{
					return;
				}
				aiVehicleModesHandler.SetMode(AIVehicleMode.Common);
				if (crimeManager.StarCount < 3 || !scanner.HasTrackToPlayer)
				{
					brakeTimer.Stop();
					getOutOffCarTimer.Stop();
					if (sirene != null)
					{
						sirene.Stop();
					}
				}
			}
			else
			{
				targettingActivated = true;
				targettingVehicleController.SetTarget(DeathTarget.TargetTransform);
				aiVehicleModesHandler.SetMode(AIVehicleMode.Target);
				brakeTimer.Run(12f);
				getOutOffCarTimer.Stop();
				if (sirene != null)
				{
					sirene.Run();
				}
			}
		}
	}
}
