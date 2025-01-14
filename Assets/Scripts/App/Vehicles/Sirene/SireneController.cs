using UnityEngine;

namespace App.Vehicles.Sirene
{
	public class SireneController : MonoBehaviour
	{
		public Light light1;

		public Light light2;

		public float speed = 1f;

		public float maxLightIntensity = 1f;

		private int direction = 1;

		private float current;

		public bool Running()
		{
			return base.gameObject.activeSelf;
		}

		public void Run()
		{
			base.gameObject.SetActive(value: true);
		}

		public void Stop()
		{
			base.gameObject.SetActive(value: false);
		}

		private void Update()
		{
			current += Time.deltaTime * speed * (float)direction;
			if (current >= 1f)
			{
				current = 1f;
				direction = -1;
			}
			else if (current <= 0f)
			{
				current = 0f;
				direction = 1;
			}
			light1.intensity = current * maxLightIntensity;
			light2.intensity = (1f - current) * maxLightIntensity;
		}
	}
}
