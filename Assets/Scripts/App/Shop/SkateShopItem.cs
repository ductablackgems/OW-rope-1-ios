using App.Shop.Slider;
using System;
using UnityEngine;

namespace App.Shop
{
	public sealed class SkateShopItem : AbstractShopItem
	{
		[SerializeField]
		private string tid;

		private SkateShoppingZone zone;

		public string ID => tid;

		public void Initialize(SkateShoppingZone zone)
		{
			this.zone = zone;
		}

		public override int GetUpgradeLevel()
		{
			return 0;
		}

		public override bool IsBuyed()
		{
			return false;
		}

		public override bool IsSelected()
		{
			return false;
		}

		public override bool IsLocked()
		{
			return false;
		}

		protected override bool SelectItem()
		{
			return true;
		}

		protected override bool UpgradeItem()
		{
			return true;
		}

		protected override bool BuyItem()
		{
			return zone.BuyItem(this);
		}

		public override Type GetViewType()
		{
			return typeof(SkateShopView);
		}
	}
}
