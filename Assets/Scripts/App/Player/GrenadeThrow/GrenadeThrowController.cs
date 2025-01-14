using App.Player.Definition;
using App.SaveSystem;
using App.Spawn.Pooling;
using App.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player.GrenadeThrow
{
	public class GrenadeThrowController : MonoBehaviour, ICharacterModule
	{
		public GunType grenadeType;

		public float throwForce = 100f;

		private PlayerAnimatorHandler animatorHandler;

		private RagdollHelper ragdollHelper;

		private GrenadeMesh[] grenadeMeshes;

		private SmartPooler smartPooler;

		private Transform cameraTransform;

		private Dictionary<GunType, WeaponInfo> grenadePrefabs;

		private PlayerSaveEntity playerSave;

		private GunSaveEntity grenadeSave;

		private GrenadeMesh activeGrenadeMesh;

		private bool active;

		private GunType thrownGrenadeType;

		public bool Runnable()
		{
			if (active || ragdollHelper.Ragdolled)
			{
				return false;
			}
			GunSaveEntity gunSaveEntity = GetGrenadeSave();
			if (gunSaveEntity != null)
			{
				return gunSaveEntity.ammo > 0;
			}
			return false;
		}

		public void Run()
		{
			if (!Runnable())
			{
				return;
			}
			active = true;
			thrownGrenadeType = grenadeType;
			GrenadeMesh[] array = grenadeMeshes;
			foreach (GrenadeMesh grenadeMesh in array)
			{
				if (grenadeMesh.grenadeType == grenadeType)
				{
					activeGrenadeMesh = grenadeMesh;
					grenadeMesh.gameObject.SetActive(value: true);
					break;
				}
			}
			animatorHandler.TriggerThrowGrenade();
		}

		public bool Running()
		{
			return active;
		}

		public void Stop()
		{
			HideActiveGrenadeMesh();
			active = false;
			thrownGrenadeType = GunType.Unknown;
		}

		public GunSaveEntity GetGrenadeSave()
		{
			if (grenadeType == GunType.Unknown)
			{
				return null;
			}
			if (grenadeSave == null || grenadeSave.GunType != grenadeType)
			{
				grenadeSave = playerSave.GetGunSave(grenadeType);
			}
			return grenadeSave;
		}

		public void OnThrowGrenade()
		{
			if (!Running())
			{
				return;
			}
			GunSaveEntity gunSaveEntity = GetGrenadeSave();
			if (gunSaveEntity == null || gunSaveEntity.GunType != thrownGrenadeType || gunSaveEntity.ammo == 0)
			{
				Stop();
				return;
			}
			gunSaveEntity.ammo--;
			gunSaveEntity.Save();
			base.transform.rotation = Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f);
			if (grenadePrefabs.TryGetValue(GetGrenadeSave().GunType, out WeaponInfo value))
			{
				GameObject gameObject = smartPooler.Pop(value.gameObject, activeGrenadeMesh.transform.position, activeGrenadeMesh.transform.rotation);
				gameObject.GetComponentSafe<Rigidbody>().AddForce(cameraTransform.forward * throwForce);
				if (GetGrenadeSave().GunType == GunType.Molotov)
				{
					gameObject.GetComponent<MolotovExplosion>().owner = base.gameObject;
				}
			}
			HideActiveGrenadeMesh();
			Stop();
		}

		private void Awake()
		{
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			ragdollHelper = this.GetComponentSafe<RagdollHelper>();
			grenadeMeshes = GetComponentsInChildren<GrenadeMesh>();
			smartPooler = ServiceLocator.Get<SmartPooler>();
			cameraTransform = ServiceLocator.GetGameObject("MainCamera").transform;
			grenadePrefabs = ServiceLocator.Get<PrefabsContainer>().gunPrefabs.GetSortedGrenadePrefabs();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			GunType[] grenadeTypes = playerSave.GrenadeTypes;
			foreach (GunType gunType in grenadeTypes)
			{
				GunSaveEntity gunSave = playerSave.GetGunSave(gunType);
				if (gunSave.ammo > 0)
				{
					grenadeType = gunType;
					grenadeSave = gunSave;
				}
			}
			GrenadeMesh[] array = grenadeMeshes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}

		private void Update()
		{
			if (Running())
			{
				if (ragdollHelper.Ragdolled)
				{
					Stop();
					return;
				}
				animatorHandler.GrenadeLayerWeight = Mathf.Clamp01(animatorHandler.GrenadeLayerWeight + Time.deltaTime * 7f);
				if (animatorHandler.GrenadeThrowState.Running)
				{
					base.transform.rotation = Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f);
				}
			}
			else
			{
				animatorHandler.GrenadeLayerWeight = Mathf.Clamp01(animatorHandler.GrenadeLayerWeight - Time.deltaTime * 7f);
			}
		}

		private void HideActiveGrenadeMesh()
		{
			if (activeGrenadeMesh != null)
			{
				activeGrenadeMesh.gameObject.SetActive(value: false);
				activeGrenadeMesh = null;
			}
		}
	}
}
