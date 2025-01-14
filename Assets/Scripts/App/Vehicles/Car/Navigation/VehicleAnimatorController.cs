using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class VehicleAnimatorController : MonoBehaviour
	{
		public float minimalDistance = 20f;

		private IVehicleAnimator vehicleAnimator;

		private Transform mainCamera;

		private void Awake()
		{
			vehicleAnimator = this.GetComponentSafe<IVehicleAnimator>();
			mainCamera = ServiceLocator.GetGameObject("MainCamera").transform;
		}

		private void LateUpdate()
		{
			float num = Vector3.Distance(mainCamera.position, base.transform.position);
			vehicleAnimator.SetAnimated(num < minimalDistance);
			vehicleAnimator.Animate(Time.deltaTime);
		}
	}
}
