using App.AI;
using App.Vehicles.Car.Navigation.Modes.Curve;
using App.Vehicles.Car.Navigation.Modes.Manual;

namespace App.Vehicles.Car.Navigation
{
	public class AIVehicleModesHandler : AbstractAIScript
	{
		public AIVehicleMode mode;

		private CurveVehicleController curveVehicleController;

		private TargettingVehicleController targettingVehicleController;

		private ManualVehicleController manualVehicleController;

		private IVehicleController vehicleController;

		private RoadSeeker roadSeeker;

		private SkidBrake skidBrake;

		private bool initialized;

		public void SetMode(AIVehicleMode mode)
		{
			if (!initialized)
			{
				Init();
			}
			this.mode = mode;
			if (!base.enabled)
			{
				return;
			}
			switch (mode)
			{
			case AIVehicleMode.Common:
				targettingVehicleController.enabled = false;
				if (manualVehicleController != null)
				{
					manualVehicleController.enabled = false;
				}
				curveVehicleController.enabled = true;
				roadSeeker.enabled = true;
				vehicleController.SetOptimizedForAI(value: true);
				break;
			case AIVehicleMode.Target:
				curveVehicleController.enabled = false;
				if (manualVehicleController != null)
				{
					manualVehicleController.enabled = false;
				}
				targettingVehicleController.enabled = true;
				roadSeeker.enabled = false;
				vehicleController.SetOptimizedForAI(value: true);
				break;
			case AIVehicleMode.Manual:
				curveVehicleController.enabled = false;
				targettingVehicleController.enabled = false;
				if (manualVehicleController != null)
				{
					manualVehicleController.enabled = true;
				}
				roadSeeker.enabled = false;
				vehicleController.SetOptimizedForAI(value: false);
				break;
			}
		}

		private void Awake()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void OnEnable()
		{
			SetMode(mode);
			skidBrake.enabled = true;
		}

		private void OnDisable()
		{
			curveVehicleController.enabled = false;
			targettingVehicleController.enabled = false;
			roadSeeker.enabled = false;
			skidBrake.enabled = false;
			vehicleController.SetOptimizedForAI(value: false);
		}

		private void Init()
		{
			curveVehicleController = this.GetComponentSafe<CurveVehicleController>();
			targettingVehicleController = this.GetComponentSafe<TargettingVehicleController>();
			manualVehicleController = GetComponent<ManualVehicleController>();
			vehicleController = base.ComponentsRoot.GetComponentSafe<IVehicleController>();
			roadSeeker = this.GetComponentSafe<RoadSeeker>();
			skidBrake = this.GetComponentSafe<SkidBrake>();
		}
	}
}
