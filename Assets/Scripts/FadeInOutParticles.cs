using UnityEngine;

public class FadeInOutParticles : MonoBehaviour
{
	private EffectSettings effectSettings;

	private ParticleSystem[] particles;

	private bool oldVisibleStat;

	private void GetEffectSettingsComponent(Transform tr)
	{
		Transform parent = tr.parent;
		if (parent != null)
		{
			effectSettings = parent.GetComponentInChildren<EffectSettings>();
			if (effectSettings == null)
			{
				GetEffectSettingsComponent(parent.transform);
			}
		}
	}

	private void Start()
	{
		GetEffectSettingsComponent(base.transform);
		particles = effectSettings.GetComponentsInChildren<ParticleSystem>();
		oldVisibleStat = effectSettings.IsVisible;
	}

	private void Update()
	{
		if (effectSettings.IsVisible != oldVisibleStat)
		{
			if (effectSettings.IsVisible)
			{
				ParticleSystem[] array = particles;
				foreach (ParticleSystem particleSystem in array)
				{
					if (effectSettings.IsVisible)
					{
						particleSystem.Play();
						particleSystem.enableEmission = true;
					}
				}
			}
			else
			{
				ParticleSystem[] array = particles;
				foreach (ParticleSystem particleSystem2 in array)
				{
					if (!effectSettings.IsVisible)
					{
						particleSystem2.Stop();
						particleSystem2.enableEmission = false;
					}
				}
			}
		}
		oldVisibleStat = effectSettings.IsVisible;
	}
}
