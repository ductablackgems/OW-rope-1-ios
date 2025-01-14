using App.Player;
using App.Util;
using App.Vehicles.Airplane;
using App.Vehicles.Bicycle;
using App.Vehicles.Car;
using App.Vehicles.Car.Firetruck;
using App.Vehicles.Car.GarbageTruck;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Car.PoliceCar;
using App.Vehicles.Gyroboard;
using App.Vehicles.Helicopter;
using App.Vehicles.Mech;
using App.Vehicles.Skateboard;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles
{
	public class VehicleComponents : MonoBehaviour
	{
		[Serializable]
		public class CameraViewSetting
		{
			public List<Transform> cameraViews;

			public float distance = 5f;

			public float height = 1f;

			public float angle = 20f;
		}

		public VehicleType type;

		public string displayName = string.Empty;

		public Transform handleTrigger;

		public Transform rightHandleTrigger;

		public Transform passengerHandleTrigger;

		public Transform door;

		public Transform passengerDoor;

		public Transform sitPoint;

		public Transform passengerSitPoint;

		public Transform driver;

		public Transform passenger;

		[Space(10f)]
		public Collider driverCollider;

		[Space(10f)]
		public DoorReservator doorReservator;

		public DoorReservator passengerDoorReservator;

		public VehicleModesHandler vehicleModesHandler;

		public VehicleStuckManager vehicleStuckManager;

		public Health health;

		public AmbulanceAI ambulanceAI;

		public PoliceCarAI policeCarAI;

		public PlayerFiretruckControl playerFiretruckControl;

		public PlayerGarbageTruckControl playerGarbageTruckControl;

		public CarSounds carSounds;

		public Transform cameraTarget;

		public HelicopterManager helicopterManager;

		public TankManager tankManager;

		public MechManager mechManager;

		public AirplaneManager airplaneManager;

		public Transform leftHandle;

		public Transform rightHandle;

		public Transform leftPedal;

		public Transform rightPedal;

		public AIBicycleController aIBicycleController;

		public StreetVehicleModesHelper streetVehicleModesHelper;

		public AIGyroboardController aiGyroboardController;

		public SkateboardAnimatorHandler skateboardAnimatorHandler;

		[Range(0f, 1f)]
		public float sittingBlend;

		public AudioClip[] deathSoundClips;

		public CameraViewSetting cameraViewSetting;

		private IFollowingVehicle followingVehicle;

		public AbstractVehicleDriver KickOffCurrentDriver(bool relocateCharacter = true, bool relocateForward = false)
		{
			if (driver != null)
			{
				AbstractVehicleDriver abstractVehicleDriver = (type == VehicleType.Bike) ? driver.GetComponentSafe<BikeDriver>() : ((type == VehicleType.Car) ? driver.GetComponentSafe<CarDriver>() : ((type == VehicleType.Bicycle) ? driver.GetComponentSafe<BicycleDriver>() : ((type == VehicleType.Gyroboard) ? driver.GetComponentSafe<GyroboardDriver>() : ((type == VehicleType.Skateboard) ? driver.GetComponentSafe<SkateboardDriver>() : ((type == VehicleType.Helicopter) ? driver.GetComponentSafe<HelicopterDriver>() : ((type == VehicleType.Mech) ? driver.GetComponentSafe<MechDriver>() : ((type != VehicleType.Airplane) ? ((AbstractVehicleDriver)driver.GetComponentSafe<TankDriver>()) : ((AbstractVehicleDriver)driver.GetComponentSafe<AirplaneDriver>()))))))));
				abstractVehicleDriver.HandleKickOutOffVehicle(relocateCharacter, relocateForward);
				return abstractVehicleDriver;
			}
			return null;
		}

		public AbstractVehicleDriver KickOffPassenger(bool relocateCharacter = true)
		{
			if (passenger != null)
			{
				CarDriver componentSafe = passenger.GetComponentSafe<CarDriver>();
				componentSafe.HandleKickOutOffVehicle(relocateCharacter);
				return componentSafe;
			}
			return null;
		}

		public bool OpenableVelocity()
		{
			return (double)this.GetComponentSafe<Rigidbody>().velocity.magnitude * 3.6 < 4.0;
		}

		public GameObject GetPersonByCollider(Collider collider)
		{
			if (collider.Equals(driverCollider) && driver != null)
			{
				return driver.gameObject;
			}
			return null;
		}

		public IFollowingVehicle GetFollowingVehicle()
		{
			if (followingVehicle == null)
			{
				if (type == VehicleType.Bicycle)
				{
					followingVehicle = aIBicycleController;
				}
				else if (type == VehicleType.Gyroboard || type == VehicleType.Skateboard)
				{
					followingVehicle = aiGyroboardController;
				}
			}
			return followingVehicle;
		}

		private void Reset()
		{
			if (doorReservator == null)
			{
				doorReservator = GetComponent<DoorReservator>();
			}
			if (vehicleModesHandler == null)
			{
				vehicleModesHandler = GetComponent<VehicleModesHandler>();
			}
			Transform[] componentsInChildren;
			if (door == null)
			{
				componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
				foreach (Transform transform in componentsInChildren)
				{
					if (transform.name == "LeftDoor")
					{
						door = transform;
						break;
					}
				}
			}
			if (handleTrigger == null)
			{
				componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
				foreach (Transform transform2 in componentsInChildren)
				{
					if (transform2.name == "Handle Trigger")
					{
						handleTrigger = transform2;
						break;
					}
				}
			}
			if (!(sitPoint == null))
			{
				return;
			}
			componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
			int i = 0;
			Transform transform3;
			while (true)
			{
				if (i < componentsInChildren.Length)
				{
					transform3 = componentsInChildren[i];
					if (transform3.name == "Sit Point")
					{
						break;
					}
					i++;
					continue;
				}
				return;
			}
			sitPoint = transform3;
		}
	}
}
