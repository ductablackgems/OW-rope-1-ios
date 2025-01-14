using App.GUI;
using App.Util;
using App.Vehicles;
using System;

namespace App.Player
{
	public class HelicopterDriver : AbstractVehicleDriver
	{
		public bool CanLand()
		{
			if (vehicleComponents != null)
			{
				return vehicleComponents.helicopterManager.CanLand();
			}
			return false;
		}

		public override void Stop()
		{
			if (CanLand())
			{
				vehicleComponents.helicopterManager.Land();
			}
		}

		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.SittingInHelicopterState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			throw new NotImplementedException();
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseHelicopter;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Helicopter;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseHelicopter = useVehicle;
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
				animatorHandler.UseHelicopter = true;
				base.transform.position = vehicleComponents.sitPoint.position;
				base.transform.rotation = vehicleComponents.sitPoint.rotation;
				vehicleComponents.driver = base.transform;
				vehicleComponents.helicopterManager.active = true;
				cameraManager.SetHelicopterCamera(vehicleComponents.cameraTarget);
				panelsManager.ShowPanel(PanelType.Helicopter);
			}
			else if (!vehicleComponents.helicopterManager.active)
			{
				SetVehicled(vehicled: false);
				if (isPlayer)
				{
					cameraManager.SetPlayerCamera();
					panelsManager.ShowPanel(PanelType.Game);
				}
			}
		}
	}
}
