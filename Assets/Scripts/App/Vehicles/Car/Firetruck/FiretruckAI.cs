using App.AI;
using App.Util;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Sirene;
using App.Vehicles.Tank;
using UnityEngine;

namespace App.Vehicles.Car.Firetruck
{
	public class FiretruckAI : MonoBehaviour
	{
		public SireneController sirene;

		private VehicleModesHandler vehicleModesHandler;

		private AIVehicleModesHandler aiVehicleModesHandler;

		private TargettingVehicleController targettingVehicleController;

		private SkidBrake skidBrake;

		private TankTowerControl towerControl;

		private Cannon cannon;

		private FiresMonitor firesMonitor;

		private DurationTimer checkFiresTimer = new DurationTimer();

		private bool targettingActivated;

		public FireManager TargetFire
		{
			get;
			private set;
		}

		private void Awake()
		{
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			aiVehicleModesHandler = this.GetComponentInChildrenSafe<AIVehicleModesHandler>();
			targettingVehicleController = this.GetComponentInChildrenSafe<TargettingVehicleController>();
			skidBrake = this.GetComponentInChildrenSafe<SkidBrake>();
			towerControl = this.GetComponentInChildrenSafe<TankTowerControl>();
			cannon = this.GetComponentInChildrenSafe<Cannon>();
			firesMonitor = ServiceLocator.Get<FiresMonitor>();
			checkFiresTimer.Run(2f);
		}

		private void FixedUpdate()
		{
			if (vehicleModesHandler.mode != VehicleMode.AI)
			{
				if (towerControl.currentState == TankTowerState.Target)
				{
					towerControl.currentState = TankTowerState.Home;
					cannon.Clear();
				}
				if (sirene.Running())
				{
					sirene.Stop();
				}
				return;
			}
			if (checkFiresTimer.Done())
			{
				checkFiresTimer.Run(2f);
				if (TargetFire == null || !TargetFire.IsInFire())
				{
					TargetFire = firesMonitor.FindFireWithinDistance(base.transform.position, 50f);
					if (TargetFire == null)
					{
						if (targettingActivated)
						{
							targettingActivated = false;
							targettingVehicleController.ClearTarget();
							aiVehicleModesHandler.SetMode(AIVehicleMode.Common);
							sirene.Stop();
						}
					}
					else
					{
						targettingActivated = true;
						targettingVehicleController.SetTarget(TargetFire.transform);
						aiVehicleModesHandler.SetMode(AIVehicleMode.Target);
						sirene.Run();
					}
				}
			}
			if (TargetFire == null)
			{
				cannon.Clear();
				return;
			}
			float magnitude = (TargetFire.transform.position - base.transform.position).magnitude;
			if (magnitude < 15f)
			{
				skidBrake.ForceBrake(5f);
			}
			cannon.Control(magnitude < 15f);
			towerControl.targetTransform = TargetFire.transform;
			towerControl.currentState = TankTowerState.Target;
		}
	}
}
