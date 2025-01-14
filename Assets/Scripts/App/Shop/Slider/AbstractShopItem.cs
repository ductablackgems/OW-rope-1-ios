using System;
using UnityEngine;

namespace App.Shop.Slider
{
	public abstract class AbstractShopItem : MonoBehaviour
	{
		public delegate void ShopItemEventHandler(AbstractShopItem shopItem);

		public string itemName;

		public string itemLabel;

		public int price;

		public int minLevel;

		public int[] upgradePrices;

		private ShopSounds shopSounds;

		private AbstractShopItemView abstractShopItemView;

		public bool Focused
		{
			get;
			set;
		}

		public event ShopItemEventHandler OnSelect;

		public event ShopItemEventHandler OnBuy;

		public event ShopItemEventHandler OnUpgrade;

		public void Select()
		{
			SelectItem();
			if (SelectItem() && this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		public void Buy()
		{
			if (BuyItem())
			{
				shopSounds.Play(shopSounds.buySound);
				if (this.OnBuy != null)
				{
					this.OnBuy(this);
				}
				return;
			}
			shopSounds.Play(shopSounds.NoMoneySound);
			if (abstractShopItemView.RewardUI.Length == 0)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (abstractShopItemView.RewardUI[i] != null)
				{
					abstractShopItemView.RewardUI[i].SetActive(value: true);
				}
			}
		}

		public void Upgrade()
		{
			if (!IsUpgradable())
			{
				UnityEngine.Debug.LogWarning("Shop item is not upgradable.");
				return;
			}
			if (UpgradeItem())
			{
				shopSounds.Play(shopSounds.buySound);
				if (this.OnUpgrade != null)
				{
					this.OnUpgrade(this);
				}
				return;
			}
			shopSounds.Play(shopSounds.NoMoneySound);
			if (abstractShopItemView.RewardUI.Length == 0)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (abstractShopItemView.RewardUI[i] != null)
				{
					abstractShopItemView.RewardUI[i].SetActive(value: true);
				}
			}
		}

		public bool IsUpgradable()
		{
			if (GetUpgradeLevel() < upgradePrices.Length)
			{
				return IsBuyed();
			}
			return false;
		}

		public int GetUpgradePrice()
		{
			if (!IsUpgradable())
			{
				return 0;
			}
			return upgradePrices[GetUpgradeLevel()];
		}

		public abstract bool IsBuyed();

		public abstract bool IsSelected();

		public abstract bool IsLocked();

		public abstract int GetUpgradeLevel();

		public abstract Type GetViewType();

		protected abstract bool SelectItem();

		protected abstract bool BuyItem();

		protected abstract bool UpgradeItem();

		protected virtual void Awake()
		{
			shopSounds = ServiceLocator.Get<ShopSounds>();
			abstractShopItemView = ServiceLocator.Get<AbstractShopItemView>();
		}
	}
}
