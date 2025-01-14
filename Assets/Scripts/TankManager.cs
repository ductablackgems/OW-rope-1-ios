using App.Vehicles.Car.Navigation;
using App.Vehicles.Tank;
using UnityEngine;

public class TankManager : MonoBehaviour
{
	public bool HasMinigun;

	public bool HasRockets;

	public bool HasTires;

	public bool Active;

	public GameObject Owner;

	private TankController tankController;

	private ITankTowerControl towerControl;

	private void Awake()
	{
		if (!HasTires)
		{
			tankController = this.GetComponentSafe<TankController>();
		}
		towerControl = this.GetComponentInChildrenSafe<ITankTowerControl>();
	}

	public void Deactivate()
	{
		Active = false;
		if (!HasTires)
		{
			tankController.enabled = false;
		}
		towerControl.SetState(TankTowerState.Stay);
	}

	public void Activate()
	{
		Active = true;
		if (!HasTires)
		{
			tankController.enabled = true;
		}
		else
		{
			this.GetComponentSafe<VehicleModesHandler>().SetMode(VehicleMode.Player);
		}
		towerControl.SetState(TankTowerState.PlayerControl);
	}
}
