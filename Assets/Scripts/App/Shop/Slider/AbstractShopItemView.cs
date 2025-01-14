using App.Shop.GUI;
using UnityEngine;

namespace App.Shop.Slider
{
	public abstract class AbstractShopItemView : MonoBehaviour
	{
		public GameObject selectedLabelGO;

		public UILabel nameLabel;

		public UILabel priceLabel;

		public GameObject[] RewardUI;

		public GameObject shopItemsParent;

		private AbstractShopItem[] shopItems;

		private AbstractShopItem focusedItem;

		private SelectShopItemButton selectButton;

		private BuyShopItemButton buyButton;

		private LockedShopItemLabel lockedLabel;

		private FulfillManager fulfillManager;

		public virtual void UpdateView()
		{
			if (nameLabel != null)
			{
				nameLabel.text = focusedItem.itemLabel;
			}
			buyButton.gameObject.SetActive(!focusedItem.IsLocked() && (!focusedItem.IsBuyed() || focusedItem.IsUpgradable()));
			lockedLabel.gameObject.SetActive(focusedItem.IsLocked());
			if (selectButton != null)
			{
				selectButton.gameObject.SetActive(focusedItem.IsBuyed() && !focusedItem.IsSelected());
				selectButton.SetItem(focusedItem);
			}
			if (selectedLabelGO != null)
			{
				selectedLabelGO.SetActive(focusedItem.IsSelected());
			}
			if (priceLabel != null)
			{
				if (!focusedItem.IsBuyed())
				{
					priceLabel.text = "$" + focusedItem.price.ToString("N0");
					priceLabel.gameObject.SetActive(value: true);
				}
				else if (focusedItem.IsUpgradable())
				{
					priceLabel.text = "$" + focusedItem.GetUpgradePrice().ToString("N0");
					priceLabel.gameObject.SetActive(value: true);
				}
				else
				{
					priceLabel.gameObject.SetActive(value: false);
				}
			}
			buyButton.SetItem(focusedItem);
			lockedLabel.SetItem(focusedItem);
		}

		public void Focus(AbstractShopItem shopItem)
		{
			focusedItem = shopItem;
			UpdateView();
		}

		protected T GetFocusedItem<T>() where T : AbstractShopItem
		{
			return focusedItem as T;
		}

		protected virtual void Awake()
		{
			shopItems = shopItemsParent.GetComponentsInChildren<AbstractShopItem>();
			selectButton = GetComponentInChildren<SelectShopItemButton>();
			buyButton = this.GetComponentInChildrenSafe<BuyShopItemButton>();
			lockedLabel = this.GetComponentInChildrenSafe<LockedShopItemLabel>();
			fulfillManager = ServiceLocator.Get<FulfillManager>(showError: false);
			if (!(fulfillManager == null))
			{
				return;
			}
			for (int i = 0; i < RewardUI.Length; i++)
			{
				if (RewardUI[i] != null)
				{
					RewardUI[i].SetActive(value: false);
					RewardUI = new GameObject[0];
				}
			}
		}

		protected virtual void OnEnable()
		{
			AbstractShopItem[] array = shopItems;
			foreach (AbstractShopItem obj in array)
			{
				obj.OnBuy += Focus;
				obj.OnSelect += Focus;
				obj.OnUpgrade += Focus;
			}
		}

		protected virtual void OnDisable()
		{
			AbstractShopItem[] array = shopItems;
			foreach (AbstractShopItem obj in array)
			{
				obj.OnBuy -= Focus;
				obj.OnSelect -= Focus;
				obj.OnUpgrade -= Focus;
			}
		}
	}
}
