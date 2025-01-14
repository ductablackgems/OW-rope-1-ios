using App.Util;
using UnityEngine;

namespace App.Vehicles.Deformations
{
	public class VehicleDeformationBase : MonoBehaviour
	{
		protected VehicleComponents vehicleComponents;

		protected CharactersInVehicle charactersInVehicle;

		protected Health health;

		protected bool isTankCollision;

		private void Start()
		{
			Initialize();
		}

		protected virtual void Initialize()
		{
			vehicleComponents = GetComponent<VehicleComponents>();
			charactersInVehicle = base.gameObject.AddComponent<CharactersInVehicle>();
			health = GetComponent<Health>();
		}

		protected void DestroyDoors()
		{
			if (vehicleComponents.handleTrigger != null)
			{
				UnityEngine.Object.Destroy(vehicleComponents.handleTrigger.gameObject);
			}
			if (vehicleComponents.passengerHandleTrigger != null)
			{
				UnityEngine.Object.Destroy(vehicleComponents.passengerHandleTrigger.gameObject);
			}
		}

		protected void LockTheDoor()
		{
			GameObject gameObject = new GameObject("doorLock");
			gameObject.transform.SetParent(base.transform);
			vehicleComponents.doorReservator.Reserve(gameObject.transform);
		}
	}
}
