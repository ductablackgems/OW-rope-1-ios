using App.Shop.GunSlider;
using App.Shop.Slider;

namespace App.Shop.SkateSlider
{
	public class SkateSliderControl : GunSliderControl
	{
		protected override AbstractShopItemView GetView()
		{
			return ServiceLocator.Get<SkateShopView>();
		}
	}
}
