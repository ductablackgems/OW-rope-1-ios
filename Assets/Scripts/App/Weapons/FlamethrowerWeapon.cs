using App.Particles;
using App.Player.SkeletonEffect;
using App.Util;
using App.Vehicles;
using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace App.Weapons
{
	public class FlamethrowerWeapon : MonoBehaviour, IWeapon
	{
		private const float AmmoDecreaseSpeed = 16f;

		private const float ReloadDuration = 2.5f;

		private const float HumanHeatSpeed = 1.4f;

		private const float VehicleDamagePerSec = 100f;

		public bool infiniteAmmo;

		public bool testRaycast = true;

		public ParticlesGroup flameParticles;

		public Transform flameOuter;

		public TriggerStaySensor flameCollider;

		public LayerMask layerMask;

		public float maxDistance = 15f;

		public Transform leftHandPosition;

		[Space(10f)]
		public AudioClip fireClip;

		public AudioClip reloadClip;

		private AudioSource audioSource;

		private GameObject owner;

		private float ammo;

		private int ammoReserve;

		private float lastShootTime = -1f;

		private HashSet<Transform> checkedFlameTransforms = new HashSet<Transform>();

		private DurationTimer reloadTimer = new DurationTimer();

		public float Range => maxDistance;

		private event Action OnShot;

		private event Action OnReload;

		public void FocusMissileOuterTo(Vector3 target)
		{
			flameOuter.LookAt(target);
		}

		public int GetAmmo()
		{
			return Mathf.RoundToInt(ammo);
		}

		public int GetAmmoReserve()
		{
			return ammoReserve;
		}

		public int GetAmmoCommonReserve()
		{
			return 10;
		}

		public GameObject GetGameObject()
		{
			return base.gameObject;
		}

		public string GetGunName()
		{
			return "Flamethrower";
		}

		public GunType GetGunType()
		{
			return GunType.Flamethrower;
		}

		public Transform GetLeftHandPosition()
		{
			return leftHandPosition;
		}

		public bool IsAutomat()
		{
			return true;
		}

		public void SetAmmo(int ammo)
		{
			this.ammo = ammo;
		}

		public void SetAmmoReserve(int ammoReserve)
		{
			this.ammoReserve = ammoReserve;
		}

		public void Shoot()
		{
			lastShootTime = Time.time;
		}

		public void RegisterOnReload(Action OnReload)
		{
			this.OnReload += OnReload;
		}

		public void RegisterOnShot(Action OnShot)
		{
			this.OnShot += OnShot;
		}

		public void UnregisterOnReload(Action OnReload)
		{
			this.OnReload -= OnReload;
		}

		public void UnregisterOnShot(Action OnShot)
		{
			this.OnShot -= OnShot;
		}

		public bool Shooting()
		{
			if (lastShootTime >= Time.time - Time.deltaTime)
			{
				return ammo > 0f;
			}
			return false;
		}

		public void SetOwner(GameObject owner)
		{
			this.owner = owner;
		}

		private void Awake()
		{
			audioSource = this.GetComponentSafe<AudioSource>();
			flameCollider._OnTriggerStay += OnFlameTriggerStay;
		}

		private void OnDestroy()
		{
			flameCollider._OnTriggerStay -= OnFlameTriggerStay;
		}

		private void Update()
		{
			if (infiniteAmmo)
			{
				ammo = 100f;
			}
			if (ammo == 0f && !reloadTimer.Running() && ammoReserve > 0)
			{
				reloadTimer.Run(2.5f);
				if (reloadClip != null)
				{
					audioSource.Stop();
					audioSource.PlayOneShot(reloadClip);
				}
				if (this.OnReload != null)
				{
					this.OnReload();
				}
			}
			if (reloadTimer.Done())
			{
				reloadTimer.Stop();
				if (ammoReserve > 0)
				{
					ammo = 100f;
					ammoReserve--;
				}
			}
			if (!Shooting())
			{
				flameParticles.Stop();
				flameCollider.gameObject.SetActive(value: false);
				if (!reloadTimer.Running())
				{
					audioSource.Stop();
				}
				return;
			}
			ammo -= 16f * Time.deltaTime;
			if (ammo <= 0f)
			{
				ammo = 0f;
				return;
			}
			if (fireClip != null && !audioSource.isPlaying)
			{
				audioSource.loop = true;
				audioSource.clip = fireClip;
				audioSource.Play();
			}
			flameCollider.gameObject.SetActive(value: true);
			flameParticles.Play();
			Ray ray = new Ray(flameOuter.position, flameOuter.forward);
			if (testRaycast && Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance - 0.2f, layerMask))
			{
				float magnitude = (flameOuter.position - hitInfo.point).magnitude;
				flameOuter.localScale = new Vector3(flameOuter.localScale.x, flameOuter.localScale.y, magnitude + 0.2f);
			}
			else
			{
				flameOuter.localScale = new Vector3(flameOuter.localScale.x, flameOuter.localScale.y, maxDistance);
			}
		}

		private void FixedUpdate()
		{
			checkedFlameTransforms.Clear();
		}

		private void OnFlameTriggerStay(Collider other)
		{
			WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Masks.Flamethrower, checkedFlameTransforms);
			if (whoIsResult.IsEmpty)
			{
				return;
			}
			if (whoIsResult.Compare(WhoIs.Masks.AllRagdollableHumans))
			{
				HumanFireManager component = whoIsResult.gameObject.GetComponent<HumanFireManager>();
				if (component != null)
				{
					component.Heat(Time.fixedDeltaTime * 1.4f, owner);
				}
			}
			else
			{
				if (!whoIsResult.Compare(WhoIs.Masks.AllVehicles))
				{
					return;
				}
				Health component2 = whoIsResult.gameObject.GetComponent<Health>();
				if (component2 != null)
				{
					if (!whoIsResult.Compare(WhoIs.Entities.Vehicle))
					{
						float fixedDeltaTime = Time.fixedDeltaTime;
					}
					else
					{
						float fixedDeltaTime2 = Time.fixedDeltaTime;
					}
					component2.ApplyDamage(Time.fixedDeltaTime * 100f, 0, owner);
				}
				VehicleComponents component3 = whoIsResult.gameObject.GetComponent<VehicleComponents>();
				if (component3 == null)
				{
					return;
				}
				GameObject personByCollider = component3.GetPersonByCollider(other);
				if (personByCollider != null)
				{
					HumanFireManager component4 = personByCollider.GetComponent<HumanFireManager>();
					if (component4 != null)
					{
						component4.Heat(Time.fixedDeltaTime * 1.4f, owner);
					}
				}
			}
		}
	}
}
