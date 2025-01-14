using App.Vehicles;
using UnityEngine;

namespace App.Player
{
	public class VehicleSensor : MonoBehaviour
	{
		private TriggerMonitor vehicleTrigger = new TriggerMonitor();

		public bool Triggered => vehicleTrigger.IsTriggered();

		public VehicleComponents Components => (VehicleComponents)vehicleTrigger.GetTrigger();

		private void FixedUpdate()
		{
			vehicleTrigger.FixedUpdate();
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Vehicle"))
			{
				VehicleComponents component = other.GetComponent<VehicleComponents>();
				if (!(component == null) && component.type == VehicleType.Car)
				{
					vehicleTrigger.MarkTrigger(component);
				}
			}
		}
	}
}
