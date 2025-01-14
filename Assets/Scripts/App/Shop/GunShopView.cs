using App.Shop.Slider;

namespace App.Shop
{
	public class GunShopView : AbstractShopItemView
	{
		public UILabel gunNameLabel;

		private void UpdateStickyRocketView(StickyRocketShopItem item)
		{
			gunNameLabel.text = $"{item.count}x {item.itemLabel}";
		}

		public override void UpdateView()
		{
			base.UpdateView();
			StickyRocketShopItem focusedItem = GetFocusedItem<StickyRocketShopItem>();
			if (focusedItem != null)
			{
				UpdateStickyRocketView(focusedItem);
				return;
			}
			GunShopItem focusedItem2 = GetFocusedItem<GunShopItem>();
			if (!focusedItem2.isArmor && focusedItem2.IsBuyed())
			{
				if (focusedItem2.IsGun())
				{
					gunNameLabel.text = $"{focusedItem2.ammo} ammo for {focusedItem2.itemLabel}";
				}
				else
				{
					gunNameLabel.text = $"{focusedItem2.ammo}x {focusedItem2.itemLabel}";
				}
			}
			else
			{
				gunNameLabel.text = focusedItem2.name;
			}
		}
	}
}
