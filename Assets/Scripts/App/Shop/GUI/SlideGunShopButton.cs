using App.Shop.GunSlider;
using UnityEngine;

namespace App.Shop.GUI
{
    public class SlideGunShopButton : MonoBehaviour
    {
        public SlideGunShopDirection direction;

        public GunSliderControl gunSlider;

        private void Awake()
        {
            gunSlider = ServiceLocator.Get<GunSliderControl>();
        }

        private void OnClick()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.weapon_next, () =>
            {
                if (direction == SlideGunShopDirection.Left)
                {
                    gunSlider.MoveLeft();
                }
                else
                {
                    gunSlider.MoveRight();
                }
            });
        }
    }
}
