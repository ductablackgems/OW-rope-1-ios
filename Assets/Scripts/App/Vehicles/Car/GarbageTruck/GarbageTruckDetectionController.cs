using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class GarbageTruckDetectionController : MonoBehaviour
	{
		private GarbageTruckManager garbagetruckManager;

		private ContainerParameters containerParameters;

		private void Awake()
		{
			containerParameters = GetComponentInParent<ContainerParameters>();
		}

		private void OnTriggerEnter(Collider coll)
		{
			if ((bool)containerParameters && coll.gameObject.CompareTag("Vehicle") && (bool)coll.gameObject.GetComponent<GarbageTruckManager>())
			{
				VehicleModesHandler component = coll.GetComponent<VehicleModesHandler>();
				GarbageTruckAI component2 = coll.GetComponent<GarbageTruckAI>();
				if ((bool)component && (bool)component2 && component.mode == VehicleMode.AI)
				{
					component2.CheckDistanceAndRotation();
				}
				GarbageTruckManager exists = coll.GetComponent<GarbageTruckManager>();
				if (!exists)
				{
					exists = coll.GetComponentInParent<GarbageTruckManager>();
				}
				if ((bool)exists && containerParameters.IsFull && !containerParameters.PhysicActivated)
				{
					garbagetruckManager = exists;
					garbagetruckManager.InDumpsterArea = true;
				}
			}
		}

		private void OnTriggerExit(Collider coll)
		{
			if (coll.gameObject.CompareTag("Vehicle") && (bool)coll.gameObject.GetComponent<GarbageTruckManager>())
			{
				VehicleComponents component = coll.GetComponent<VehicleComponents>();
				GarbageTruckAI component2 = coll.GetComponent<GarbageTruckAI>();
				GarbageTruckManager garbageTruckManager = coll.GetComponent<GarbageTruckManager>();
				if (component != null && component.vehicleModesHandler.mode == VehicleMode.AI && (bool)component2)
				{
					component2.StopCollect();
				}
				if (!garbageTruckManager)
				{
					garbageTruckManager = coll.GetComponentInParent<GarbageTruckManager>();
				}
				if ((bool)garbageTruckManager && (bool)garbagetruckManager && garbagetruckManager.gameObject.GetInstanceID() == garbageTruckManager.gameObject.GetInstanceID())
				{
					garbagetruckManager.InDumpsterArea = false;
					garbagetruckManager = null;
				}
			}
		}
	}
}
