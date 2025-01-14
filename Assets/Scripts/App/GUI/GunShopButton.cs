using App.Camera;
using App.Shop.GunSlider;
using UnityEngine;

namespace App.GUI
{
	public class GunShopButton : MonoBehaviour
	{
		private GunSliderControl sliderControl;

		private CameraManager cameraManager;

		private PanelsManager panelsManager;

		private void Awake()
		{
			sliderControl = ServiceLocator.Get<GunSliderControl>();
			cameraManager = ServiceLocator.Get<CameraManager>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}

		private void OnClick()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.menu_weapon, () =>
			{
				sliderControl.Run();
				cameraManager.SetGunShopCamera();
				panelsManager.ShowPanel(PanelType.GunShop);
			});
		}
	}
}
