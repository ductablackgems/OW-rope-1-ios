using UnityEngine;

namespace Forge3D
{
	public class F3DAudioController : MonoBehaviour
	{
		public static F3DAudioController instance;

		private float timer_01;

		private float timer_02;

		public Transform audioSource;

		[Header("Vulcan")]
		public AudioClip[] vulcanHit;

		public AudioClip vulcanShot;

		public float vulcanDelay;

		public float vulcanHitDelay;

		[Header("Solo gun")]
		public AudioClip[] soloGunHit;

		public AudioClip soloGunShot;

		public float soloGunDelay;

		public float soloGunHitDelay;

		[Header("Sniper")]
		public AudioClip[] sniperHit;

		public AudioClip sniperShot;

		public float sniperDelay;

		public float sniperHitDelay;

		[Header("Shot gun")]
		public AudioClip[] shotGunHit;

		public AudioClip shotGunShot;

		public float shotGunDelay;

		public float shotGunHitDelay;

		[Header("Seeker")]
		public AudioClip[] seekerHit;

		public AudioClip seekerShot;

		public float seekerDelay;

		public float seekerHitDelay;

		[Header("Rail gun")]
		public AudioClip[] railgunHit;

		public AudioClip railgunShot;

		public float railgunDelay;

		public float railgunHitDelay;

		[Header("Plasma gun")]
		public AudioClip[] plasmagunHit;

		public AudioClip plasmagunShot;

		public float plasmagunDelay;

		public float plasmagunHitDelay;

		[Header("Plasma beam")]
		public AudioClip plasmabeamOpen;

		public AudioClip plasmabeamLoop;

		public AudioClip plasmabeamClose;

		[Header("Plasma beam heavy")]
		public AudioClip plasmabeamHeavyOpen;

		public AudioClip plasmabeamHeavyLoop;

		public AudioClip plasmabeamHeavyClose;

		[Header("Lightning gun")]
		public AudioClip lightningGunOpen;

		public AudioClip lightningGunLoop;

		public AudioClip lightningGunClose;

		[Header("Flame gun")]
		public AudioClip flameGunOpen;

		public AudioClip flameGunLoop;

		public AudioClip flameGunClose;

		[Header("Laser impulse")]
		public AudioClip[] laserImpulseHit;

		public AudioClip laserImpulseShot;

		public float laserImpulseDelay;

		public float laserImpulseHitDelay;

		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
			timer_01 += Time.deltaTime;
			timer_02 += Time.deltaTime;
		}

		public void VulcanShot(Vector3 pos)
		{
			if (timer_01 >= vulcanDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, vulcanShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.95f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 5f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void VulcanHit(Vector3 pos)
		{
			if (timer_02 >= vulcanHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, vulcanHit[Random.Range(0, vulcanHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.95f, 1f);
					component.volume = UnityEngine.Random.Range(0.6f, 1f);
					component.minDistance = 7f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}

		public void SoloGunShot(Vector3 pos)
		{
			if (timer_01 >= soloGunDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, soloGunShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.95f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 30f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void SoloGunHit(Vector3 pos)
		{
			if (timer_02 >= soloGunHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, soloGunHit[Random.Range(0, soloGunHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.95f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 50f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}

		public void SniperShot(Vector3 pos)
		{
			if (timer_01 >= sniperDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, sniperShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.9f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 6f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void SniperHit(Vector3 pos)
		{
			if (timer_02 >= sniperHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, sniperHit[Random.Range(0, sniperHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.9f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 8f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}

		public void ShotGunShot(Vector3 pos)
		{
			if (timer_01 >= shotGunDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, shotGunShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.9f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 8f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void ShotGunHit(Vector3 pos)
		{
			if (timer_02 >= shotGunHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, shotGunHit[Random.Range(0, shotGunHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.9f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 7f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}

		public void SeekerShot(Vector3 pos)
		{
			if (timer_01 >= seekerDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, seekerShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.8f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 8f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void SeekerHit(Vector3 pos)
		{
			if (timer_02 >= seekerHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, seekerHit[Random.Range(0, seekerHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.8f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 25f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}

		public void RailGunShot(Vector3 pos)
		{
			if (timer_01 >= railgunDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, railgunShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.8f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 4f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void RailGunHit(Vector3 pos)
		{
			if (timer_02 >= railgunHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, railgunHit[Random.Range(0, railgunHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.8f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 20f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}

		public void PlasmaGunShot(Vector3 pos)
		{
			if (timer_01 >= plasmagunDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmagunShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.8f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 4f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void PlasmaGunHit(Vector3 pos)
		{
			if (timer_02 >= plasmagunHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmagunHit[Random.Range(0, plasmagunHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.8f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 50f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}

		public void PlasmaBeamLoop(Vector3 pos, Transform loopParent)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmabeamOpen, pos, null).gameObject.GetComponent<AudioSource>();
			AudioSource component2 = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmabeamLoop, pos, loopParent).gameObject.GetComponent<AudioSource>();
			if (component != null && component2 != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
				component2.pitch = UnityEngine.Random.Range(0.95f, 1f);
				component2.volume = UnityEngine.Random.Range(0.95f, 1f);
				component2.loop = true;
				component2.minDistance = 50f;
				component2.Play();
			}
		}

		public void PlasmaBeamClose(Vector3 pos)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmabeamClose, pos, null).gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
			}
		}

		public void PlasmaBeamHeavyLoop(Vector3 pos, Transform loopParent)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmabeamHeavyOpen, pos, null).gameObject.GetComponent<AudioSource>();
			AudioSource component2 = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmabeamHeavyLoop, pos, loopParent).gameObject.GetComponent<AudioSource>();
			if (component != null && component2 != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
				component2.pitch = UnityEngine.Random.Range(0.95f, 1f);
				component2.volume = UnityEngine.Random.Range(0.95f, 1f);
				component2.loop = true;
				component2.minDistance = 50f;
				component2.Play();
			}
		}

		public void PlasmaBeamHeavyClose(Vector3 pos)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, plasmabeamHeavyClose, pos, null).gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
			}
		}

		public void LightningGunLoop(Vector3 pos, Transform loopParent)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, lightningGunOpen, pos, null).gameObject.GetComponent<AudioSource>();
			AudioSource component2 = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, lightningGunLoop, pos, loopParent.parent).gameObject.GetComponent<AudioSource>();
			if (component != null && component2 != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
				component2.pitch = UnityEngine.Random.Range(0.95f, 1f);
				component2.volume = UnityEngine.Random.Range(0.95f, 1f);
				component2.loop = true;
				component2.minDistance = 50f;
				component2.Play();
			}
		}

		public void LightningGunClose(Vector3 pos)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, lightningGunClose, pos, null).gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
			}
		}

		public void FlameGunLoop(Vector3 pos, Transform loopParent)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, flameGunOpen, pos, null).gameObject.GetComponent<AudioSource>();
			AudioSource component2 = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, flameGunLoop, pos, loopParent.parent).gameObject.GetComponent<AudioSource>();
			if (component != null && component2 != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
				component2.pitch = UnityEngine.Random.Range(0.95f, 1f);
				component2.volume = UnityEngine.Random.Range(0.95f, 1f);
				component2.loop = true;
				component2.minDistance = 50f;
				component2.Play();
			}
		}

		public void FlameGunClose(Vector3 pos)
		{
			AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, flameGunClose, pos, null).gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.pitch = UnityEngine.Random.Range(0.8f, 1f);
				component.volume = UnityEngine.Random.Range(0.8f, 1f);
				component.minDistance = 50f;
				component.loop = false;
				component.Play();
			}
		}

		public void LaserImpulseShot(Vector3 pos)
		{
			if (timer_01 >= laserImpulseDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, laserImpulseShot, pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.9f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 20f;
					component.loop = false;
					component.Play();
					timer_01 = 0f;
				}
			}
		}

		public void LaserImpulseHit(Vector3 pos)
		{
			if (timer_02 >= laserImpulseHitDelay)
			{
				AudioSource component = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, laserImpulseHit[Random.Range(0, plasmagunHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.pitch = UnityEngine.Random.Range(0.8f, 1f);
					component.volume = UnityEngine.Random.Range(0.8f, 1f);
					component.minDistance = 20f;
					component.loop = false;
					component.Play();
					timer_02 = 0f;
				}
			}
		}
	}
}
