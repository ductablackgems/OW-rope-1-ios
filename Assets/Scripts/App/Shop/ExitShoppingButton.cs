using App.Camera;
using App.GUI;
using App.Shop.GunSlider;
using UnityEngine;

namespace App.Shop
{
    public class ExitShoppingButton : MonoBehaviour
    {
        public PanelType exitPanel = PanelType.Game;

        public GunSliderControl sliderControl;

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
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.weapon_back, () =>
            {
                sliderControl.Stop();
                if (exitPanel == PanelType.MainMenu)
                {
                    cameraManager.SetMenuCamera();
                }
                else
                {
                    cameraManager.SetPlayerCamera();
                }
                panelsManager.ShowPanel(exitPanel);
            });
        }
    }
}
