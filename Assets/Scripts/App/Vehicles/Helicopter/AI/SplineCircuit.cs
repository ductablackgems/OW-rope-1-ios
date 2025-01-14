using App.Player;
using UnityEngine;

namespace App.Vehicles.Helicopter.AI
{
	public class SplineCircuit : MonoBehaviour
	{
		public GameObject spline;

		public GameObject vehicle;

		private CrimeManager crimeManager;

		private Transform playerTransform;

		private DurationTimer checkTimer = new DurationTimer();

		private void Awake()
		{
			crimeManager = ServiceLocator.Get<CrimeManager>();
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
			checkTimer.FakeDone(1f);
		}

		private void Update()
		{
			if (checkTimer.Done())
			{
				checkTimer.Run(8f);
				bool active = (new Vector3(base.transform.position.x, playerTransform.position.y, base.transform.position.z) - playerTransform.position).magnitude < 100f && crimeManager.StarCount > 2;
				spline.SetActive(active);
				if (vehicle != null)
				{
					vehicle.SetActive(active);
				}
			}
		}
	}
}
