using App.Player;
using App.Vehicles.Airplane;
using UnityEngine;

namespace App.Vehicles
{
	public class AirplaneCrimeReporter : MonoBehaviour
	{
		[SerializeField]
		private int numberOfStars = 3;

		private CrimeManager crimeManager;

		private void Awake()
		{
			crimeManager = ServiceLocator.Get<CrimeManager>();
			ServiceLocator.Messages.Subscribe(MessageID.Airplane.Enter, this, OnAirplaneEnter);
		}

		private void OnAirplaneEnter(object sender, object data)
		{
			if (!((data as VehicleComponents).gameObject != base.gameObject))
			{
				ServiceLocator.Messages.Unsubscribe(MessageID.Airplane.Enter, this, OnAirplaneEnter);
				int num = numberOfStars - 1;
				if (num >= 0)
				{
					float num2 = crimeManager.criminalityTresholds[num];
					num2 += num2 * 0.2f;
					crimeManager.SetCrime(crimeManager.CriminalityValue + num2);
					GetComponent<AirplaneController>().EnableDistanceDestroy = true;
				}
			}
		}
	}
}
