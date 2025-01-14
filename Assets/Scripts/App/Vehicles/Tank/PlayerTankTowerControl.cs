using App.Util;
using App.Vehicles.Car.Firetruck;
using UnityEngine;

namespace App.Vehicles.Tank
{
	public class PlayerTankTowerControl : MonoBehaviour
	{
		private TankManager tankManager;

		private TankTowerControl towerControl;

		private Cannon cannon;

		private void Awake()
		{
			tankManager = this.GetComponentSafe<TankManager>();
			towerControl = this.GetComponentInChildrenSafe<TankTowerControl>();
			cannon = this.GetComponentInChildrenSafe<Cannon>();
		}

		private void FixedUpdate()
		{
			if (!tankManager.Active)
			{
				if (towerControl.currentState == TankTowerState.PlayerControl)
				{
					towerControl.currentState = TankTowerState.Stay;
					cannon.Clear();
				}
			}
			else
			{
				towerControl.currentState = TankTowerState.PlayerControl;
				cannon.Control(InputUtils.TankMinigun.IsPressed);
			}
		}
	}
}
