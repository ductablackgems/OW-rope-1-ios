using UnityEngine;

namespace App.Vehicles.Airplane
{
	public class AirplaneJetEffect : MonoBehaviour
	{
		public Color minColour;

		private IAirplaneController controller;

		private ParticleSystem particles;

		private float orginalStartSize;

		private float originalLifeTime;

		private Color originalColor;

		private void Start()
		{
			controller = GetComponentInParent<IAirplaneController>();
			particles = GetComponent<ParticleSystem>();
			originalLifeTime = particles.main.startLifetime.constant;
			orginalStartSize = particles.main.startSize.constant;
			originalColor = particles.main.startColor.color;
			SetJets(0f);
		}

		private void Update()
		{
			if (controller != null && controller.IsActive)
			{
				SetJets(controller.Throttle);
			}
		}

		private void SetJets(float throttle)
		{
			ParticleSystem.MainModule main = particles.main;
			main.startLifetime = Mathf.Lerp(0f, originalLifeTime, throttle);
			main.startSize = Mathf.Lerp(orginalStartSize * 0.3f, orginalStartSize, throttle);
			main.startColor = Color.Lerp(minColour, originalColor, throttle);
		}
	}
}
