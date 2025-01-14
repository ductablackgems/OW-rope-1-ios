using App.Player;
using App.SaveSystem;
using App.Shop.Slider;
using System;

namespace App.Shop
{
	public class StickyRocketShopItem : AbstractShopItem
	{
		public int count = 5;

		private PlayerSaveEntity playerSave;

		private GunSaveEntity save;

		protected override void Awake()
		{
			base.Awake();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			save = playerSave.GetGunSave(GunType.StickyRocket);
		}

		public override bool IsBuyed()
		{
			return save.buyed;
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
			return null;
		}

		protected override bool SelectItem()
		{
			return false;
		}

		protected override bool BuyItem()
		{
			return false;
		}

		protected override bool UpgradeItem()
		{
			if (!save.buyed || playerSave.score < upgradePrices[0])
			{
				return false;
			}
			playerSave.score -= upgradePrices[0];
			playerSave.Save();
			save.ammo += count;
			save.Save();
			ServiceLocator.GetPlayerModel().GameObject.GetComponent<StickyRocketController>().RefreshRocketCount();
			return true;
		}
	}
}
