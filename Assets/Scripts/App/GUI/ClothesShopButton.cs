using App.Camera;
using App.Player.Clothes.GUI;
using UnityEngine;

namespace App.GUI
{
    public class ClothesShopButton : MonoBehaviour
    {
        private CameraManager cameraManager;

        private PanelsManager panelsManager;

        private ClothesPanel clothesPanel;

        private void Awake()
        {
            cameraManager = ServiceLocator.Get<CameraManager>();
            panelsManager = ServiceLocator.Get<PanelsManager>();
            clothesPanel = ServiceLocator.Get<ClothesPanel>();
        }

        private void OnClick()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.menu_character, () =>
            {
                clothesPanel.EditedClothesKind = clothesPanel.defaultClothesKind;
                cameraManager.SetClothesShopCamera();
                panelsManager.ShowPanel(PanelType.ClothesShop);
            });
        }
    }
}
