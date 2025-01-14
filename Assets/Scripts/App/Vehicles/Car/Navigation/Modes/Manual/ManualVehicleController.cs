using App.AI;

namespace App.Vehicles.Car.Navigation.Modes.Manual
{
	public class ManualVehicleController : AbstractAIScript
	{
		private IVehicleController vehicleController;

		private SkidBrake skidBrake;

		public float VerticalInput
		{
			get;
			set;
		}

		public float HorizontalInput
		{
			get;
			set;
		}

		private void Awake()
		{
			vehicleController = base.ComponentsRoot.GetComponentSafe<IVehicleController>();
			skidBrake = this.GetComponentSafe<SkidBrake>();
		}

		private void FixedUpdate()
		{
			if (skidBrake.IsBreaking())
			{
				vehicleController.Move(HorizontalInput, 0f, -1f, 1f);
			}
			else
			{
				vehicleController.Move(HorizontalInput, VerticalInput, 0f, 0f);
			}
		}
	}
}
