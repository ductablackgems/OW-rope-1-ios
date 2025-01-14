using App.GUI;
using App.Util;
using App.Vehicles;
using System;

namespace App.Player
{
	public class AirplaneDriver : AbstractVehicleDriver
	{
		public override void Stop()
		{
			vehicleComponents.airplaneManager.Deactivate();
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
				return triggeredVehicle.type == VehicleType.Airplane;
			}
			return false;
		}

		public override bool Running()
		{
			if (!base.Running())
			{
				return false;
			}
			return vehicleComponents.type == VehicleType.Airplane;
		}

		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.SittingInTankState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			throw new NotImplementedException("Player can not be thrown out from an Aircraft");
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseTank;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Airplane;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseTank = useVehicle;
		}

		protected override void SetVehicled(bool vehicled, VehicleComponents vehicleComponents = null, bool fixRotation = true)
		{
			base.SetVehicled(vehicled, vehicleComponents, fixRotation);
			if (vehicled)
			{
				ServiceLocator.Messages.Send(MessageID.Airplane.Enter, this, vehicleComponents);
			}
			else
			{
				ServiceLocator.Messages.Send(MessageID.Airplane.Leave, this, vehicleComponents);
			}
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
				vehicleComponents.airplaneManager.Activate(base.gameObject);
				cameraManager.SetAircraftCamera(vehicleComponents.cameraTarget);
				panelsManager.ShowPanel(PanelType.Airplane);
			}
			else if (!vehicleComponents.airplaneManager.IsActive)
			{
				base.transform.position = vehicleComponents.handleTrigger.position;
				SetVehicled(vehicled: false, vehicleComponents);
				vehicleComponents.airplaneManager.Deactivate();
				if (isPlayer)
				{
					cameraManager.SetPlayerCamera();
					panelsManager.ShowPanel(PanelType.Game);
				}
			}
		}
	}
}
