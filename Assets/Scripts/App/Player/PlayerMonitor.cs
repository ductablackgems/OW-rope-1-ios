using App.Interaction;
using App.Vehicles;
using UnityEngine;

namespace App.Player
{
	public class PlayerMonitor : MonoBehaviour
	{
		private CarDriver carDriver;

		private BikeDriver bikeDriver;

		private BicycleDriver bicycleDriver;

		private GyroboardDriver gyroboardDriver;

		private SkateboardDriver skateboardDriver;

		private InteractiveObjectSensor interactiveObjectSensor = new InteractiveObjectSensor();

		public InteractiveObjectSensor InteractiveObjectSensor => interactiveObjectSensor;

		public float GetDistance(Vector3 positionFrom)
		{
			return (base.transform.position - positionFrom).magnitude;
		}

		public bool SittingInVehicle()
		{
			if (carDriver != null)
			{
				if (!carDriver.SittingInVehicle && !bikeDriver.SittingInVehicle && !bicycleDriver.SittingInVehicle && !gyroboardDriver.SittingInVehicle)
				{
					return skateboardDriver.SittingInVehicle;
				}
				return true;
			}
			return false;
		}

		public VehicleComponents GetVehicle()
		{
			if (carDriver.Vehicle != null)
			{
				return carDriver.Vehicle;
			}
			if (bikeDriver.Vehicle != null)
			{
				return bikeDriver.Vehicle;
			}
			if (bicycleDriver.Vehicle != null)
			{
				return bicycleDriver.Vehicle;
			}
			if (gyroboardDriver.Vehicle != null)
			{
				return gyroboardDriver.Vehicle;
			}
			if (skateboardDriver.Vehicle != null)
			{
				return skateboardDriver.Vehicle;
			}
			return null;
		}

		public bool UsingFiretruck()
		{
			if (carDriver.Vehicle != null)
			{
				return carDriver.Vehicle.playerFiretruckControl != null;
			}
			return false;
		}

		public bool UsingGarbageTruck()
		{
			if (SittingInVehicle() && carDriver.Vehicle != null)
			{
				return carDriver.Vehicle.playerGarbageTruckControl != null;
			}
			return false;
		}

		public bool canPitchforkUp()
		{
			if (carDriver.Vehicle != null && carDriver.Vehicle.playerGarbageTruckControl != null)
			{
				return carDriver.Vehicle.playerGarbageTruckControl.CanPitchforkUp();
			}
			return false;
		}

		public bool canPitchforkDown()
		{
			if (carDriver.Vehicle != null && carDriver.Vehicle.playerGarbageTruckControl != null)
			{
				return carDriver.Vehicle.playerGarbageTruckControl.CanPitchforkDown();
			}
			return false;
		}

		public bool canStartDump()
		{
			if (carDriver.Vehicle != null && carDriver.Vehicle.playerGarbageTruckControl != null)
			{
				return carDriver.Vehicle.playerGarbageTruckControl.CanDumpStart();
			}
			return false;
		}

		public bool canEndDump()
		{
			if (carDriver.Vehicle != null && carDriver.Vehicle.playerGarbageTruckControl != null)
			{
				return carDriver.Vehicle.playerGarbageTruckControl.CanDumpEnd();
			}
			return false;
		}

		public bool canCollectContainer()
		{
			if (carDriver.Vehicle != null && carDriver.Vehicle.playerGarbageTruckControl != null)
			{
				return carDriver.Vehicle.playerGarbageTruckControl.CanLift();
			}
			return false;
		}

		private void Awake()
		{
			carDriver = this.GetComponentSafe<CarDriver>();
			bikeDriver = this.GetComponentSafe<BikeDriver>();
			bicycleDriver = this.GetComponentSafe<BicycleDriver>();
			gyroboardDriver = this.GetComponentSafe<GyroboardDriver>();
			skateboardDriver = this.GetComponentSafe<SkateboardDriver>();
			interactiveObjectSensor.Initialize();
		}

		private void Update()
		{
			interactiveObjectSensor.Update();
		}
	}
}
