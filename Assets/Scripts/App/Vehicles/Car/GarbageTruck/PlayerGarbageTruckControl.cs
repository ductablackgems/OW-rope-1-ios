using App.Util;
using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class PlayerGarbageTruckControl : MonoBehaviour
	{
		private VehicleModesHandler vehicleModesHandler;

		private GarbageTruckManager garbageTruckManager;

		private void Awake()
		{
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			garbageTruckManager = GetComponent<GarbageTruckManager>();
		}

		private void FixedUpdate()
		{
			if (vehicleModesHandler.mode == VehicleMode.Player)
			{
				bool isPressed = InputUtils.DumpStart.IsPressed;
				bool isPressed2 = InputUtils.DumpEnd.IsPressed;
				bool isPressed3 = InputUtils.PitchforkUp.IsPressed;
				bool isPressed4 = InputUtils.PitchforkDown.IsPressed;
				bool isPressed5 = InputUtils.LiftContainer.IsPressed;
				if (isPressed)
				{
					garbageTruckManager.StartDump();
				}
				if (isPressed2)
				{
					garbageTruckManager.EndDump(true);
				}
				if (isPressed3)
				{
					garbageTruckManager.LiftUp(true);
				}
				if (isPressed4)
				{
					garbageTruckManager.LiftDown(true);
				}
				if (isPressed5)
				{
					garbageTruckManager.Collect(true);
				}
			}
		}

		public bool CanLift()
		{
			return garbageTruckManager.CanLift();
		}

		public bool CanPitchforkUp()
		{
			return garbageTruckManager.CanPitchforkUp();
		}

		public bool CanPitchforkDown()
		{
			return garbageTruckManager.CanPitchforkDown();
		}

		public bool CanDumpStart()
		{
			return garbageTruckManager.CanDumpStart();
		}

		public bool CanDumpEnd()
		{
			return garbageTruckManager.CanDumpEnd();
		}
	}
}
