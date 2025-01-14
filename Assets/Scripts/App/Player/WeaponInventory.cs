using App.Player.Definition;
using App.Player.FightSystem;
using App.Player.GrenadeThrow;
using App.SaveSystem;
using App.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player
{
	[RequireComponent(typeof(ShotController))]
	[RequireComponent(typeof(AdvancedFightController))]
	public class WeaponInventory : MonoBehaviour
	{
		public delegate void GunLoadEventHandler(GunLoad gunLoad, IWeapon weapon);

		public const float SaveSynchronizeInterval = 2.4f;

		public AudioSource audioSource;

		private MagicShield magicShield;

		private MagicController magicController;

		private ShotController shotController;

		private AdvancedFightController advancedFightController;

		private GrenadeThrowController grenadeThrowController;

		private PlayerAnimatorHandler animatorHandler;

		private CharacterModesHandler characterModesHandler;

		private PlayerSaveEntity playerSave;

		private DurationTimer saveSynchronizeTimer = new DurationTimer();

		private GunType gunBeforeMagicShield;

		private GunType gunBeforeMagic;

		private bool magicShieldActive;

		private bool magicRun;

		public event GunLoadEventHandler OnCollectAmmo;

		public event GunLoadEventHandler OnCollectWeapon;

		public void AddLoad(GunLoad gunLoad)
		{
			IWeapon weapon = shotController.GetWeapon(gunLoad.gunType);
			GunSaveEntity gunSave = playerSave.GetGunSave(gunLoad.gunType);
			if (gunSave.buyed)
			{
				weapon.SetAmmoReserve(weapon.GetAmmoReserve() + gunLoad.GetTotalAmmo());
				if (this.OnCollectAmmo != null)
				{
					this.OnCollectAmmo(gunLoad, weapon);
				}
			}
			else
			{
				gunSave.buyed = true;
				weapon.SetAmmoReserve(gunLoad.reserveAmmo);
				weapon.SetAmmo(gunLoad.ammo);
				if (this.OnCollectWeapon != null)
				{
					this.OnCollectWeapon(gunLoad, weapon);
				}
			}
			if (gunSave.IsGun)
			{
				SynchronizeGunSave(gunSave, weapon);
			}
		}

		public void SwitchGun()
		{
			if (!(magicShield != null) || !magicShield.IsMovementBlocked)
			{
				GunType gunType = GunType.Unknown;
				bool flag = shotController.gunType == GunType.Unknown;
				foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
				{
					if (gun.Value.buyed && gun.Value.IsGun)
					{
						if (flag)
						{
							gunType = gun.Key;
							break;
						}
						if (gun.Key == shotController.gunType)
						{
							flag = true;
						}
					}
				}
				shotController.gunType = gunType;
			}
		}

		public void SwitchToGun(GunType gunType)
		{
			GunType gunType2 = GunType.Unknown;
			bool flag = shotController.gunType == GunType.Unknown;
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
			{
				if (gun.Value.buyed && gun.Value.IsGun)
				{
					if (flag)
					{
						gunType2 = gun.Key;
						break;
					}
					if (gun.Key == gunType)
					{
						flag = true;
					}
				}
			}
			shotController.gunType = gunType2;
		}

		public void SwitchGunBack()
		{
			GunType gunType = GunType.Unknown;
			GunType gunType2 = shotController.gunType;
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
			{
				if (gun.Value.buyed && gun.Value.IsGun)
				{
					if (gun.Key == shotController.gunType)
					{
						break;
					}
					gunType = gun.Key;
				}
			}
			shotController.gunType = gunType;
		}

		public void SynchronizeGunSave(GunSaveEntity gunSave)
		{
			if (gunSave.IsGun)
			{
				SynchronizeGunSave(gunSave, shotController.GetWeapon(gunSave.GunType));
			}
		}

		public void SwitchGrenade()
		{
			GunType gunType = GunType.Unknown;
			bool flag = grenadeThrowController.grenadeType == GunType.Unknown;
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
			{
				if (gun.Value.IsGrenade && gun.Value.ammo != 0)
				{
					if (gunType == GunType.Unknown)
					{
						gunType = gun.Key;
					}
					if (flag)
					{
						gunType = gun.Key;
						break;
					}
					if (gun.Key == grenadeThrowController.grenadeType)
					{
						flag = true;
					}
				}
			}
			grenadeThrowController.grenadeType = gunType;
		}

		protected void Awake()
		{
			magicShield = GetComponent<MagicShield>();
			magicController = GetComponent<MagicController>();
			shotController = this.GetComponentSafe<ShotController>();
			advancedFightController = this.GetComponentSafe<AdvancedFightController>();
			grenadeThrowController = this.GetComponentSafe<GrenadeThrowController>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			characterModesHandler = this.GetComponentSafe<CharacterModesHandler>();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
			{
				gun.Value.OnSave += OnGunSave;
			}
		}

		private void Start()
		{
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
			{
				if (gun.Value.IsGun)
				{
					SynchronizeWeaponLauncher(gun.Value, shotController.GetWeapon(gun.Value.GunType));
				}
			}
			saveSynchronizeTimer.Run(2.4f);
		}

		private void OnDestroy()
		{
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
			{
				gun.Value.OnSave -= OnGunSave;
			}
		}

		protected void Update()
		{
			if ((bool)magicShield && magicShield.ShieldActivated && !magicShieldActive)
			{
				gunBeforeMagicShield = shotController.gunType;
				if (shotController.gunType != 0)
				{
					SwitchToGun(GunType.Unknown);
				}
				magicShieldActive = true;
				return;
			}
			if ((bool)magicShield && !magicShield.ShieldActivated && magicShieldActive)
			{
				shotController.gunType = gunBeforeMagicShield;
				magicShieldActive = false;
			}
			if ((bool)magicController && magicController.MagicRun() && !magicRun)
			{
				gunBeforeMagic = shotController.gunType;
				if (shotController.gunType != 0)
				{
					SwitchToGun(GunType.Unknown);
				}
				magicRun = true;
				return;
			}
			if ((bool)magicController && !magicController.MagicRun() && magicRun)
			{
				shotController.gunType = gunBeforeMagic;
				magicRun = false;
			}
			if (animatorHandler.LaserHandLayerWeight > 0f)
			{
				if (shotController.Running())
				{
					shotController.Stop();
				}
				if (advancedFightController.Running())
				{
					advancedFightController.Stop();
				}
			}
			else if (!grenadeThrowController.Running())
			{
				if (shotController.gunType == GunType.Unknown)
				{
					if (shotController.Running())
					{
						shotController.Stop();
					}
				}
				else
				{
					if (advancedFightController.Running())
					{
						advancedFightController.Stop();
					}
					if (characterModesHandler.GetRunningMode() == CharacterMode.Default)
					{
						shotController.Run();
					}
				}
			}
			if (saveSynchronizeTimer.Done())
			{
				saveSynchronizeTimer.Run(2.4f);
				foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
				{
					if (gun.Value.IsGun)
					{
						SynchronizeGunSave(gun.Value, shotController.GetWeapon(gun.Value.GunType));
					}
				}
			}
			GunSaveEntity grenadeSave = grenadeThrowController.GetGrenadeSave();
			if (grenadeSave != null && grenadeSave.ammo == 0)
			{
				SwitchGrenade();
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("GunLoad"))
			{
				CollectableGunLoad componentSafe = other.GetComponentSafe<CollectableGunLoad>();
				componentSafe.Collect();
				AddLoad(componentSafe.gunLoad);
				UnityEngine.Object.Destroy(other.gameObject);
				audioSource.Play();
			}
		}

		private void OnGunSave(AbstractSaveEntity entity)
		{
			GunSaveEntity gunSaveEntity = (GunSaveEntity)entity;
			if (gunSaveEntity.IsGun)
			{
				IWeapon weapon = shotController.GetWeapon(gunSaveEntity.GunType);
				SynchronizeWeaponLauncher(gunSaveEntity, weapon);
			}
		}

		private void SynchronizeGunSave(GunSaveEntity gunSave, IWeapon launcher)
		{
			if (gunSave.buyed && (gunSave.ammo != launcher.GetAmmo() || gunSave.ammoReserve != launcher.GetAmmoReserve()))
			{
				gunSave.ammo = launcher.GetAmmo();
				gunSave.ammoReserve = launcher.GetAmmoReserve();
				gunSave.Save();
			}
		}

		private void SynchronizeWeaponLauncher(GunSaveEntity gunSave, IWeapon launcher)
		{
			launcher.SetAmmo(gunSave.ammo);
			launcher.SetAmmoReserve(gunSave.ammoReserve);
		}
	}
}
