using App.Audio;
using App.Particles;
using App.Player.Clothes;
using App.Spawn;
using App.Util;
using System.Collections;
using UnityEngine;

namespace App.Player.SkeletonEffect
{
	public class HumanFireManager : MonoBehaviour, IResetable
	{
		private const float YellDelay = 1.5f;

		private const float HeatDamage = 2f;

		private const float PlayerHeatDamage = 1f;

		private const float HitDamage = 25f;

		private const float PlayerHitDamage = 12.5f;

		public AudioClipsScriptableObject yellClips;

		public AudioSource audioSource;

		[Space]
		public PSMeshRendererUpdater rendererUpdater;

		public ParticlesGroup particlesGroup;

		[Space]
		public PSMeshRendererUpdater acidRendererUpdater;

		public ParticlesGroup acidParticlesGroup;

		[Space]
		public PSMeshRendererUpdater electricRendererUpdater;

		public ParticlesGroup electricParticlesGroup;

		[Space]
		public GameObject mesh;

		public GameObject mesh2;

		public ClothesManager clothesManager;

		private Health health;

		private DurationTimer stopTimer = new DurationTimer();

		private DurationTimer hitTimer = new DurationTimer();

		private DurationTimer yellDelayTimer = new DurationTimer();

		private DurationTimer electricityTimer = new DurationTimer();

		private GameObject agressor;

		private bool isPlayer;

		private float heat;

		private bool typeIsAcid;

		public bool IsBurning()
		{
			return stopTimer.InProgress();
		}

		public bool IsBurningByAcid()
		{
			if (stopTimer.InProgress())
			{
				return typeIsAcid;
			}
			return false;
		}

		public bool IsElectrified()
		{
			return electricityTimer.InProgress();
		}

		public void ResetStates()
		{
			stopTimer.Stop();
			hitTimer.Stop();
			yellDelayTimer.Stop();
			electricityTimer.Stop();
			agressor = null;
			particlesGroup.Stop();
			particlesGroup.Clear();
			rendererUpdater.Clear();
			rendererUpdater.enabled = false;
			rendererUpdater.gameObject.SetActive(value: false);
			acidParticlesGroup.Stop();
			acidParticlesGroup.Clear();
			acidRendererUpdater.Clear();
			acidRendererUpdater.enabled = false;
			acidRendererUpdater.gameObject.SetActive(value: false);
			if (HasElectricEffects())
			{
				electricParticlesGroup.Stop();
				electricParticlesGroup.Clear();
				electricRendererUpdater.Clear();
				electricRendererUpdater.enabled = false;
				electricRendererUpdater.gameObject.SetActive(value: false);
			}
		}

		public void Heat(float amount, GameObject agressor)
		{
			heat += amount;
			if (heat >= 1f)
			{
				StartFire(agressor);
			}
			float num = isPlayer ? 1f : 2f;
			heat = Mathf.Clamp01(heat);
			health.ApplyDamage(amount * num, 5, agressor);
		}

		public void StartElectricity(GameObject agressor, float damage)
		{
			if (HasElectricEffects())
			{
				GameObject go = (clothesManager == null) ? mesh : clothesManager.GetWearedItem(ClothesKind.Shirt).gameObject;
				electricRendererUpdater.gameObject.SetActive(value: true);
				electricRendererUpdater.UpdateMeshEffect(go);
				electricRendererUpdater.enabled = true;
				electricParticlesGroup.Play();
				electricityTimer.Run(2f);
				health.ApplyDamage(damage, 10, agressor);
			}
		}

		public void StopElectricity()
		{
			if (HasElectricEffects())
			{
				electricParticlesGroup.Stop();
				electricParticlesGroup.Clear();
				electricRendererUpdater.Clear();
				electricRendererUpdater.gameObject.SetActive(value: false);
				electricRendererUpdater.enabled = false;
				electricityTimer.Stop();
			}
		}

		public void StartFire(GameObject agressor, bool acidType = false)
		{
			typeIsAcid = acidType;
			bool num = IsBurning();
			this.agressor = agressor;
			stopTimer.Run(11f);
			if (!num)
			{
				if (!hitTimer.Running() || hitTimer.GetProgress() > 0.3f)
				{
					hitTimer.FakeDone(1f);
				}
				particlesGroup.Play();
				GameObject go = (clothesManager == null) ? mesh : clothesManager.GetWearedItem(ClothesKind.Shirt).gameObject;
				if (typeIsAcid)
				{
					rendererUpdater.gameObject.SetActive(value: false);
					rendererUpdater.enabled = false;
					acidRendererUpdater.gameObject.SetActive(value: true);
					acidRendererUpdater.UpdateMeshEffect(go);
					acidRendererUpdater.enabled = true;
				}
				else
				{
					acidRendererUpdater.gameObject.SetActive(value: false);
					acidRendererUpdater.enabled = false;
					rendererUpdater.gameObject.SetActive(value: true);
					rendererUpdater.UpdateMeshEffect(go);
					rendererUpdater.enabled = true;
				}
				audioSource.PlayOneShot(yellClips.GetRandomClip());
			}
		}

		public void StopFire()
		{
			stopTimer.Stop();
			hitTimer.Stop();
			yellDelayTimer.Stop();
			agressor = null;
			particlesGroup.Stop();
			rendererUpdater.Clear();
			rendererUpdater.enabled = false;
			acidRendererUpdater.Clear();
			acidRendererUpdater.enabled = false;
			acidRendererUpdater.gameObject.SetActive(value: false);
		}

		private void Awake()
		{
			health = this.GetComponentSafe<Health>();
			particlesGroup.Stop();
			particlesGroup.Clear();
			rendererUpdater.enabled = false;
			isPlayer = CompareTag("Player");
			rendererUpdater.gameObject.SetActive(value: false);
			if (HasElectricEffects())
			{
				electricRendererUpdater.gameObject.SetActive(value: false);
			}
		}

		private IEnumerator Start()
		{
			if (CompareTag("Player"))
			{
				yield return new WaitForSeconds(0.3f);
				if (mesh != null && !mesh.activeSelf && mesh2 != null)
				{
					mesh = mesh2;
				}
			}
		}

		private void Update()
		{
			heat = Mathf.Clamp01(heat - Time.deltaTime * 0.2f);
			if ((particlesGroup.Awaken || acidParticlesGroup.Awaken) && !IsBurning())
			{
				particlesGroup.Stop();
				particlesGroup.Clear();
				acidParticlesGroup.Stop();
				acidParticlesGroup.Clear();
			}
			UpdateElectricEffects();
			if (!stopTimer.Running())
			{
				return;
			}
			if (!audioSource.isPlaying && !health.Dead() && stopTimer.GetProgress() < 0.8f)
			{
				if (isPlayer)
				{
					if (!yellDelayTimer.Running())
					{
						yellDelayTimer.Run(1.5f);
					}
					if (yellDelayTimer.Done())
					{
						yellDelayTimer.Stop();
						audioSource.PlayOneShot(yellClips.GetRandomClip());
					}
				}
				else
				{
					audioSource.PlayOneShot(yellClips.GetRandomClip());
				}
			}
			if (hitTimer.Done())
			{
				float damage = isPlayer ? 12.5f : 25f;
				hitTimer.Run(2f);
				health.ApplyDamage(damage, typeIsAcid ? 7 : 5, agressor);
			}
			if (stopTimer.Done())
			{
				StopFire();
			}
		}

		private bool HasElectricEffects()
		{
			if (electricParticlesGroup != null)
			{
				return electricRendererUpdater != null;
			}
			return false;
		}

		private void UpdateElectricEffects()
		{
			if (HasElectricEffects())
			{
				if (electricParticlesGroup.Awaken && !IsElectrified())
				{
					electricParticlesGroup.Stop();
					electricParticlesGroup.Clear();
				}
				if (electricityTimer.Done())
				{
					StopElectricity();
				}
			}
		}
	}
}
