using App.Util;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class TrashController : MonoBehaviour
	{
		private Health playerHealth;

		private void Awake()
		{
			playerHealth = ServiceLocator.GetGameObject("Player").GetComponentSafe<Health>();
		}

		private void Update()
		{
			if ((bool)playerHealth && (playerHealth.transform.position - base.transform.position).magnitude > 50f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
