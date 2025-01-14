using App.Util;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Tank;
using UnityEngine;

namespace App.Vehicles.Car.Firetruck
{
	public class PlayerFiretruckControl : MonoBehaviour
	{
		private VehicleModesHandler vehicleModesHandler;

		private TankTowerControl towerControl;

		private Cannon cannon;

		private void Awake()
		{
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			towerControl = this.GetComponentInChildrenSafe<TankTowerControl>();
			cannon = this.GetComponentInChildrenSafe<Cannon>();
		}

		private void FixedUpdate()
		{
			if (vehicleModesHandler.mode != VehicleMode.Player)
			{
				if (towerControl.currentState == TankTowerState.PlayerControl)
				{
					towerControl.currentState = TankTowerState.Home;
					cannon.Clear();
				}
			}
			else
			{
				towerControl.currentState = TankTowerState.PlayerControl;
				cannon.Control(InputUtils.WaterShoot.IsPressed);
			}
		}
	}
}
