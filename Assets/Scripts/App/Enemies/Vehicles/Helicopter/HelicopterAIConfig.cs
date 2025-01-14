using UnityEngine;

namespace App.Enemies.Vehicles.Helicopter
{
	public class HelicopterAIConfig : MonoBehaviour
	{
		public float maxAimDistance = 300f;

		public float maxShootingDuration = 3f;

		public float delayAfterShooting = 2f;

		public float rocketsPerMinute = 6f;
	}
}
