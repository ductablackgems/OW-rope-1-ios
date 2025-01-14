using App.Shop.Slider;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveBuy : GameplayObjective
	{
		[Header("Objective Buy")]
		[SerializeField]
		private AbstractShopItem itemToBuy;

		protected override void OnActivated()
		{
			base.OnActivated();
			itemToBuy.OnBuy += OnItemBought;
			itemToBuy.OnUpgrade += OnItemBought;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			UnregisterListeners();
		}

		private void OnItemBought(AbstractShopItem shopItem)
		{
			Finish();
			UnregisterListeners();
		}

		private void UnregisterListeners()
		{
			itemToBuy.OnBuy -= OnItemBought;
			itemToBuy.OnUpgrade -= OnItemBought;
		}
	}
}
