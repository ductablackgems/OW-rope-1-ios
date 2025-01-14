using App.GUI;
using App.Util;
using App.Vehicles;
using System;

namespace App.Player
{
	public class MechDriver : AbstractVehicleDriver
	{
		public override void Stop()
		{
			vehicleComponents.mechManager.Deactivate();
			SetUseVehicleParameter(useVehicle: false);
		}

		public override bool Runnable(bool useTargetTrigger = false)
		{
			if (!base.Runnable(useTargetTrigger))
			{
				return false;
			}
			VehicleComponents triggeredVehicle = GetTriggeredVehicle();
			if (triggeredVehicle != null)
			{
				return triggeredVehicle.type == VehicleType.Mech;
			}
			return false;
		}

		public override bool Running()
		{
			if (!base.Running())
			{
				return false;
			}
			return vehicleComponents.type == VehicleType.Mech;
		}

		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.SittingInTankState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			throw new NotImplementedException("Player can not be throwed from a Mech");
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseTank;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Mech;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseTank = useVehicle;
		}

		protected override void OnHandleKickOutOffVehicle(bool relocateCharacter, bool relocateForward)
		{
			base.OnHandleKickOutOffVehicle(relocateCharacter, relocateForward);
			vehicleComponents.mechManager.ExitVehicle();
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
				vehicleComponents.mechManager.Activate(base.gameObject);
				cameraManager.SetVehicleCamera(vehicleComponents);
				panelsManager.ShowPanel(PanelType.Mech);
			}
			else if (!vehicleComponents.mechManager.IsActive)
			{
				base.transform.position = vehicleComponents.handleTrigger.position;
				SetVehicled(vehicled: false);
				vehicleComponents.mechManager.Deactivate();
				if (isPlayer)
				{
					cameraManager.SetPlayerCamera();
					panelsManager.ShowPanel(PanelType.Game);
				}
			}
		}
	}
}
