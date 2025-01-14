using App.Player;
using App.Player.SkeletonEffect;
using App.Util;
using App.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace App.Vehicles.Car.Firetruck
{
	public class Cannon : MonoBehaviour, IWeapon
	{
		public Transform waterFlowObject;

		public ParticleSystem waterFlowParticles;

		public TriggerStaySensor triggerStaySensor;

		public float MaxDistance = 10f;

		public bool throwingAcid;

		public string gunName = "";

		public int commonAmmoReserves = 10;

		public LayerMask layerMask;

		public bool ignorePlayerRagdoll;

		private float Distance;

		private bool isShooting;

		private int ammo;

		private int ammoReserves;

		private GameObject owner;

		private HashSet<Transform> checkedCollisionTransforms = new HashSet<Transform>();

		private float fireCheckTimer = -1f;

		float IWeapon.Range => MaxDistance;

		private event Action reloadCallback;

		private event Action shotCallback;

		public void Clear()
		{
			waterFlowObject.gameObject.SetActive(value: false);
			waterFlowParticles.Stop(withChildren: true);
			Distance = 0.5f;
			waterFlowObject.localScale = new Vector3(waterFlowObject.localScale.x, waterFlowObject.localScale.y, Distance);
		}

		private void Awake()
		{
			triggerStaySensor = this.GetComponentInChildrenSafe<TriggerStaySensor>();
			triggerStaySensor._OnTriggerStay += _OnTriggerStay;
		}

		private void OnDestroy()
		{
			triggerStaySensor._OnTriggerStay -= _OnTriggerStay;
		}

		private void Start()
		{
			Clear();
		}

		public void Control(bool attackPressed)
		{
			if (!attackPressed)
			{
				Clear();
				Distance = 0.5f;
				return;
			}
			checkedCollisionTransforms.Clear();
			Distance += 1f * Time.deltaTime * 8f;
			if (Distance >= MaxDistance)
			{
				Distance = MaxDistance;
			}
			waterFlowObject.gameObject.SetActive(value: true);
			waterFlowParticles.Play(withChildren: true);
			waterFlowObject.localScale = new Vector3(waterFlowObject.localScale.x, waterFlowObject.localScale.y, Distance);
		}

		private void _OnTriggerStay(Collider other)
		{
			WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Masks.FiretruckWater, checkedCollisionTransforms);
			if (whoIsResult.IsEmpty)
			{
				return;
			}
			if (whoIsResult.Compare(WhoIs.Entities.Vehicle))
			{
				if (!throwingAcid)
				{
					FireManager component = whoIsResult.transform.GetComponent<FireManager>();
					if (component != null)
					{
						component.CoolDown(Time.fixedDeltaTime);
					}
				}
			}
			else if (whoIsResult.Compare(WhoIs.Masks.AllRagdollableHumans))
			{
				bool num = other.CompareTag("RagdollPart");
				if (throwingAcid)
				{
					whoIsResult.transform.GetComponentSafe<HumanFireManager>().StartFire(null, acidType: true);
				}
				if (num)
				{
					other.GetComponentSafe<Rigidbody>().AddForce(base.transform.forward * 150f);
				}
				else if (!ignorePlayerRagdoll || !whoIsResult.transform.CompareTag("Player"))
				{
					RagdollHelper componentSafe = whoIsResult.transform.GetComponentSafe<RagdollHelper>();
					componentSafe.Ragdolled = true;
					componentSafe.SetRagdollVelocity(base.transform.forward * 4f);
					if (!throwingAcid)
					{
						componentSafe.GetComponentSafe<Health>().ApplyDamage(5f, 8);
					}
				}
			}
			else
			{
				if (!whoIsResult.Compare(WhoIs.Masks.AllStreetVehicles))
				{
					return;
				}
				VehicleComponents componentSafe2 = whoIsResult.transform.GetComponentSafe<VehicleComponents>();
				if (componentSafe2.driver != null)
				{
					if (throwingAcid)
					{
						componentSafe2.driver.GetComponentSafe<HumanFireManager>().StartFire(null, acidType: true);
					}
					RagdollHelper componentSafe3 = componentSafe2.driver.GetComponentSafe<RagdollHelper>();
					componentSafe2.KickOffCurrentDriver(relocateCharacter: false);
					componentSafe3.SetRagdollVelocity(base.transform.forward * 4f);
					componentSafe3.GetComponentSafe<Health>().ApplyDamage(5f, 8);
				}
			}
		}

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
			return ammoReserves;
		}

		int IWeapon.GetAmmoReserve()
		{
			return commonAmmoReserves;
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
			return GunType.AcidCannon;
		}

		Transform IWeapon.GetLeftHandPosition()
		{
			return base.transform;
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
			isShooting = true;
			fireCheckTimer = 0.1f;
			Control(attackPressed: true);
		}

		bool IWeapon.Shooting()
		{
			return isShooting;
		}

		void IWeapon.UnregisterOnReload(Action OnReload)
		{
			reloadCallback -= OnReload;
		}

		void IWeapon.UnregisterOnShot(Action OnShot)
		{
			shotCallback -= OnShot;
		}

		private void Update()
		{
			if (!(fireCheckTimer < 0f))
			{
				fireCheckTimer = Mathf.Max(fireCheckTimer - Time.deltaTime, 0f);
				if (!(fireCheckTimer > 0f))
				{
					isShooting = false;
					Control(attackPressed: false);
				}
			}
		}
	}
}
