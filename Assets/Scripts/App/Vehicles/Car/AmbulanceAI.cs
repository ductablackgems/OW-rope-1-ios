using App.AI;
using App.AI.Monitor;
using App.Player;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Sirene;
using UnityEngine;

namespace App.Vehicles.Car
{
	public class AmbulanceAI : MonoBehaviour
	{
		public SireneController sirene;

		private VehicleComponents vehicleComponents;

		private VehicleModesHandler vehicleModesHandler;

		private AIVehicleModesHandler aiVehicleModesHandler;

		private TargettingVehicleController targettingVehicleController;

		private SkidBrake skidBrake;

		private DeathEnemiesMonitor deathEnemiesMonitor;

		private DurationTimer getOutOffCarTimer = new DurationTimer();

		private DurationTimer checkDeathsTimer = new DurationTimer();

		private DurationTimer brakeTimer = new DurationTimer();

		private bool targettingActivated;

		private bool driverIsGettingOut;

		public DeathTarget DeathTarget
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
			checkDeathsTimer.Run(6f);
		}

		private void Update()
		{
			if (vehicleModesHandler.mode != VehicleMode.AI)
			{
				if (sirene.Running())
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
				if (!DeathTarget.IsValid())
				{
					DeathTarget.Clear();
				}
				if (checkDeathsTimer.Done())
				{
					checkDeathsTimer.Run(6f);
					if (!DeathTarget.IsValid())
					{
						DeathTarget = deathEnemiesMonitor.FindHealthTarget(base.transform.position, 50f);
						if (!DeathTarget.IsValid())
						{
							if (targettingActivated)
							{
								targettingActivated = false;
								targettingVehicleController.ClearTarget();
								aiVehicleModesHandler.SetMode(AIVehicleMode.Common);
								brakeTimer.Stop();
								getOutOffCarTimer.Stop();
								sirene.Stop();
							}
						}
						else
						{
							targettingActivated = true;
							targettingVehicleController.SetTarget(DeathTarget.TargetTransform);
							aiVehicleModesHandler.SetMode(AIVehicleMode.Target);
							brakeTimer.Run(12f);
							getOutOffCarTimer.Stop();
							sirene.Run();
						}
					}
				}
				if (DeathTarget.IsValid() && (brakeTimer.Done() || (!getOutOffCarTimer.Running() && (DeathTarget.TargetTransform.position - base.transform.position).magnitude < 18.5f)))
				{
					brakeTimer.Stop();
					skidBrake.ForceBrake(5f);
					getOutOffCarTimer.Run(3f);
				}
				if (getOutOffCarTimer.Done())
				{
					getOutOffCarTimer.Stop();
					if (vehicleComponents.driver != null && DeathTarget.IsValid())
					{
						vehicleComponents.driver.GetComponentInChildrenSafe<RescuerAIModule>(includeInactive: true).DeathTarget = DeathTarget;
						driverIsGettingOut = true;
						DeathTarget.Clear();
						targettingVehicleController.ClearTarget();
						vehicleComponents.driver.GetComponentSafe<CarDriver>().Stop();
					}
				}
			}
		}
	}
}
