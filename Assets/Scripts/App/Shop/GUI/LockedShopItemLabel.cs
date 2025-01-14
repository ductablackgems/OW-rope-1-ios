using App.Shop.Slider;
using UnityEngine;

namespace App.Shop.GUI
{
	public class LockedShopItemLabel : MonoBehaviour
	{
		private UILabel label;

		public void SetItem(AbstractShopItem item)
		{
			label.text = "Available at level " + item.minLevel.ToString("N0");
		}

		protected void Awake()
		{
			label = this.GetComponentSafe<UILabel>();
		}
	}
}
