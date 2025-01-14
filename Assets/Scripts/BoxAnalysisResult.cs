using UnityEngine;

namespace App.Vehicles.Car.Navigation.Modes.Curve
{
	public struct BoxAnalysisResult
	{
		private Transform[] vehicles;

		private int vehicleCount;

		private bool hitUnknown;

		public bool HitVehicle(Transform vehicle)
		{
			if (vehicleCount > 0)
			{
				for (int i = 0; i < vehicleCount; i++)
				{
					if (vehicles[i].Equals(vehicle))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HitImobileObstacle(Transform thisVehicle)
		{
			if (hitUnknown)
			{
				return true;
			}
			if (vehicleCount > 0)
			{
				for (int i = 0; i < vehicleCount; i++)
				{
					VehicleComponents component = vehicles[i].GetComponent<VehicleComponents>();
					if (component == null || component.driver == null || component.driver.CompareTag("Player") || (component.vehicleStuckManager != null && (component.vehicleStuckManager.Imobile() || component.vehicleStuckManager.HitVehicle(thisVehicle))))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void AddVehicle(Transform vehicle)
		{
			if (vehicles == null)
			{
				vehicles = new Transform[4];
			}
			if (vehicleCount != vehicles.Length)
			{
				vehicles[vehicleCount] = vehicle;
				vehicleCount++;
			}
		}

		public void MarkUnknown()
		{
			hitUnknown = true;
		}
	}
}
