using App.Player;
using App.Prefabs;
using App.SaveSystem;
using App.Shop.Slider;
using App.Weapons;
using System;
using UnityEngine;

namespace App.Shop
{
	public class GunShopItem : AbstractShopItem
	{
		public int ammo = 10;

		public GunType gunType;

		public bool isArmor;

		private PlayerSaveEntity playerSave;

		private GunSaveEntity gunSave;

		private IWeapon prefabWeaponLauncher;

		private PlayerModel player;

		public bool IsGun()
		{
			return gunSave.IsGun;
		}

		public override bool IsBuyed()
		{
			if (isArmor)
			{
				return playerSave.armor >= 1f;
			}
			return gunSave.buyed;
		}

		public override bool IsSelected()
		{
			return false;
		}

		public override bool IsLocked()
		{
			return false;
		}

		public override int GetUpgradeLevel()
		{
			return 0;
		}

		public override Type GetViewType()
		{
			return typeof(GunShopView);
		}

		protected override bool SelectItem()
		{
			throw new NotImplementedException();
		}

		protected override bool BuyItem()
		{
			if (isArmor)
			{
				if (playerSave.armor >= 1f || playerSave.score < price)
				{
					return false;
				}
				playerSave.armor = 1f;
				playerSave.score -= price;
				playerSave.Save();
				return true;
			}
			if (gunSave.buyed || playerSave.score < price)
			{
				return false;
			}
			gunSave.buyed = true;
			gunSave.ammo = prefabWeaponLauncher.GetAmmo();
			gunSave.ammoReserve = prefabWeaponLauncher.GetAmmoCommonReserve();
			gunSave.Save();
			playerSave.score -= price;
			playerSave.Save();
			player.ShotController.gunType = gunType;
			return true;
		}

		protected override bool UpgradeItem()
		{
			if (isArmor || !gunSave.buyed || playerSave.score < upgradePrices[0])
			{
				return false;
			}
			player.WeaponInventory.SynchronizeGunSave(gunSave);
			if (gunSave.IsGun)
			{
				gunSave.ammoReserve += ammo;
			}
			else
			{
				gunSave.ammo += ammo;
			}
			gunSave.Save();
			playerSave.score -= upgradePrices[0];
			playerSave.Save();
			if (gunSave.IsGrenade)
			{
				player.GrenadeThrowController.grenadeType = gunType;
			}
			else if (gunSave.IsGun)
			{
				player.ShotController.gunType = gunType;
			}
			return true;
		}

		protected override void Awake()
		{
			base.Awake();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			if (gunType != 0)
			{
				gunSave = playerSave.GetGunSave(gunType);
				GunPrefabsScriptableObject gunPrefabs = ServiceLocator.Get<PrefabsContainer>().gunPrefabs;
				player = ServiceLocator.GetPlayerModel();
				if (gunSave.IsGun)
				{
					gunPrefabs.GetSortedWeaponLauncherPrefabs().TryGetValue(gunType, out GameObject value);
					prefabWeaponLauncher = value.GetComponent<IWeapon>();
				}
			}
		}
	}
}
