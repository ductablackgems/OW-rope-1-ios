using System;
using UnityEngine;

namespace App.Effects
{
	public class GameplayEffect : MonoBehaviour
	{
		private ParticleSystem[] particles;

		private Transform parentOrginal;

		private Action<GameplayEffect> returnToCache;

		public void Initialize(Action<GameplayEffect> returnToCacheCallback)
		{
			parentOrginal = base.transform.parent;
			particles = GetComponentsInChildren<ParticleSystem>();
			returnToCache = returnToCacheCallback;
		}

		public void Activate(Vector3 position, Transform parentOverride = null)
		{
			if (parentOverride != null)
			{
				base.transform.SetParent(parentOverride);
			}
			base.transform.position = position;
			SetActive(isActive: true);
			ActivateParticles(activate: true);
		}

		public void Deactivate()
		{
			if (base.transform.parent != parentOrginal)
			{
				base.transform.SetParent(parentOrginal);
			}
			ActivateParticles(activate: false);
			SetActive(isActive: false);
			if (returnToCache != null)
			{
				returnToCache(this);
			}
		}

		private void ActivateParticles(bool activate)
		{
			for (int i = 0; i < particles.Length; i++)
			{
				ParticleSystem particleSystem = particles[i];
				if (!(particleSystem == null))
				{
					if (activate)
					{
						particleSystem.Play();
					}
					else
					{
						particleSystem.Stop();
					}
				}
			}
		}

		private void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}
	}
}
