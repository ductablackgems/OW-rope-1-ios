using System;
using UnityEngine;

namespace App.Weapons
{
	public class BeamWeapon : MonoBehaviour, IWeapon
	{
		public string gunName;

		public int commonAmmoReserves = 10;

		[Header("Sounds")]
		public AudioClip ActivatedSound;

		public AudioClip DeactivatedSound;

		public Transform leftHandPosition;

		public GameObject beam;

		public float range;

		private int ammo;

		private int ammoReserves;

		private GameObject owner;

		private float timer;

		private AudioSource audioSource;

		private bool isActive;

		float IWeapon.Range => range;

		private event Action reloadCallback;

		private event Action shotCallback;

		void IWeapon.FocusMissileOuterTo(Vector3 target)
		{
			base.transform.LookAt(target);
		}

		int IWeapon.GetAmmo()
		{
			return ammo;
		}

		int IWeapon.GetAmmoCommonReserve()
		{
			return commonAmmoReserves;
		}

		int IWeapon.GetAmmoReserve()
		{
			return ammoReserves;
		}

		GameObject IWeapon.GetGameObject()
		{
			return base.gameObject;
		}

		string IWeapon.GetGunName()
		{
			return gunName;
		}

		GunType IWeapon.GetGunType()
		{
			return GunType.Beam;
		}

		Transform IWeapon.GetLeftHandPosition()
		{
			if (!(leftHandPosition != null))
			{
				return base.transform;
			}
			return leftHandPosition;
		}

		bool IWeapon.IsAutomat()
		{
			return true;
		}

		void IWeapon.RegisterOnReload(Action OnReload)
		{
			reloadCallback += OnReload;
		}

		void IWeapon.RegisterOnShot(Action OnShot)
		{
			shotCallback += OnShot;
		}

		void IWeapon.SetAmmo(int ammo)
		{
			this.ammo = ammo;
		}

		void IWeapon.SetAmmoReserve(int ammoReserve)
		{
			ammoReserves = ammoReserve;
		}

		void IWeapon.SetOwner(GameObject owner)
		{
			this.owner = owner;
		}

		void IWeapon.Shoot()
		{
			Attack();
		}

		bool IWeapon.Shooting()
		{
			return timer > 0f;
		}

		void IWeapon.UnregisterOnReload(Action OnReload)
		{
			reloadCallback -= OnReload;
		}

		void IWeapon.UnregisterOnShot(Action OnShot)
		{
			shotCallback -= OnShot;
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			beam.SetActive(value: false);
		}

		private void Update()
		{
			if (!(timer <= 0f))
			{
				timer -= Time.deltaTime;
				if (!(timer > 0f))
				{
					StopAttack();
				}
			}
		}

		private void Attack()
		{
			timer = 0.1f;
			if (!isActive)
			{
				beam.SetActive(value: true);
				audioSource.PlayOneShot(ActivatedSound);
				audioSource.Play();
				isActive = true;
			}
		}

		private void StopAttack()
		{
			beam.SetActive(value: false);
			audioSource.Stop();
			audioSource.PlayOneShot(DeactivatedSound);
			isActive = false;
		}
	}
}
