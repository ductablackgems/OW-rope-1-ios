using App.Util;
using UnityEngine;

namespace App.Vehicles.Deformations
{
	public class CharactersInVehicle : MonoBehaviour
	{
		private VehicleComponents vehicleComponents;

		private void Start()
		{
			vehicleComponents = base.gameObject.GetComponent<VehicleComponents>();
		}

		public void Kill()
		{
			KickOffDriver();
			for (int i = 0; i < vehicleComponents.sitPoint.childCount; i++)
			{
				Transform child = vehicleComponents.sitPoint.GetChild(i);
				if (child.CompareTag("Player") || child.CompareTag("Enemy"))
				{
					vehicleComponents.driver = child;
					KickOffDriver();
					break;
				}
			}
			KickOffPassenger();
			if (!(vehicleComponents.passengerSitPoint != null))
			{
				return;
			}
			int num = 0;
			Transform child2;
			while (true)
			{
				if (num < vehicleComponents.passengerSitPoint.childCount)
				{
					child2 = vehicleComponents.passengerSitPoint.GetChild(num);
					if (child2.CompareTag("Player") || child2.CompareTag("Enemy"))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			vehicleComponents.passenger = child2;
			KickOffPassenger();
		}

		private void KickOffDriver()
		{
			Transform driver = vehicleComponents.driver;
			vehicleComponents.KickOffCurrentDriver();
			if (driver != null)
			{
				driver.GetComponent<Health>().Kill();
			}
		}

		private void KickOffPassenger()
		{
			Transform passenger = vehicleComponents.passenger;
			vehicleComponents.KickOffPassenger();
			if (passenger != null)
			{
				passenger.GetComponent<Health>().Kill();
			}
		}
	}
}
