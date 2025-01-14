using App.Shop.Slider;
using UnityEngine;

namespace App.Shop.GUI
{
	public class SelectShopItemButton : MonoBehaviour
	{
		private AbstractShopItem item;

		public void SetItem(AbstractShopItem item)
		{
			this.item = item;
		}

		protected void OnClick()
		{
			item.Select();
		}
	}
}
