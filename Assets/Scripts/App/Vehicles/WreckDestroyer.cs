using UnityEngine;

namespace App.Vehicles
{
	public class WreckDestroyer : MonoBehaviour
	{
		private DurationTimer timer = new DurationTimer();

		private Transform player;

		private void Start()
		{
			timer.Run(45f);
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}

		private void Update()
		{
			if (timer.Done())
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void AddForceVehicle(float power)
		{
			GetComponent<Rigidbody>().AddForce(player.forward * power, ForceMode.Impulse);
		}
	}
}
