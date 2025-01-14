using App.Shop.Slider;
using UnityEngine;

namespace App.Shop.GUI
{
	public class BuyShopItemButton : MonoBehaviour
	{
		public BuyShopItemButtonType type = BuyShopItemButtonType.Upgrading;

		public UILabel label;

		private AbstractShopItem item;

		public void SetItem(AbstractShopItem item)
		{
			this.item = item;
			if (type == BuyShopItemButtonType.Upgrading)
			{
				if (item.IsBuyed())
				{
					label.text = "Upgrade $" + item.GetUpgradePrice().ToString("N0");
				}
				else
				{
					label.text = "Buy $" + item.price.ToString("N0");
				}
			}
		}

		protected void OnClick()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.character_buy, () =>
			{
				if (item.IsBuyed())
				{
					item.Upgrade();
				}
				else
				{
					item.Buy();
				}
			});
		}
	}
}
