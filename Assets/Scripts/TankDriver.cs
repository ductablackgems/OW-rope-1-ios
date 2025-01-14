using App.GUI;
using App.Player;
using App.Util;
using App.Vehicles;
using System;

public class TankDriver : AbstractVehicleDriver
{
	public bool HasMinigun()
	{
		return vehicleComponents.tankManager.HasMinigun;
	}

	public bool HasRockets()
	{
		return vehicleComponents.tankManager.HasRockets;
	}

	protected override VehicleType GetVehicleType()
	{
		return VehicleType.Tank;
	}

	public override void Stop()
	{
		vehicleComponents.tankManager.Active = false;
		SetUseVehicleParameter(useVehicle: false);
	}

	protected override AnimatorState GetSittingState()
	{
		return animatorHandler.SittingInTankState;
	}

	protected override AnimatorState GetThrowOutDriverState()
	{
		throw new NotImplementedException();
	}

	protected override bool GetUseVehicleParameter()
	{
		return animatorHandler.UseTank;
	}

	protected override void SetUseVehicleParameter(bool useVehicle)
	{
		animatorHandler.UseTank = useVehicle;
	}

	private void Update()
	{
		if (!running)
		{
			return;
		}
		if (moveTowardVehicleHandle)
		{
			moveTowardVehicleHandle = false;
			animatorHandler.UseTank = true;
			base.transform.position = vehicleComponents.sitPoint.position;
			base.transform.rotation = vehicleComponents.sitPoint.rotation;
			vehicleComponents.driver = base.transform;
			vehicleComponents.tankManager.Activate();
			vehicleComponents.tankManager.Owner = base.gameObject;
			cameraManager.SetVehicleCamera(vehicleComponents);
			panelsManager.ShowPanel(PanelType.Tank);
		}
		else if (!vehicleComponents.tankManager.Active)
		{
			base.transform.position = vehicleComponents.handleTrigger.position;
			SetVehicled(vehicled: false);
			vehicleComponents.tankManager.Deactivate();
			if (isPlayer)
			{
				cameraManager.SetPlayerCamera();
				panelsManager.ShowPanel(PanelType.Game);
			}
		}
	}
}
