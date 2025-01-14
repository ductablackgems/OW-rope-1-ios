using App.Camera;
using App.GUI;
using App.Player;
using App.Player.Clothes;
using App.Player.Clothes.GUI;
using UnityEngine;

namespace App.Shop
{
    public class ExitClothesShoppingButton : MonoBehaviour
    {
        public PanelType exitPanel = PanelType.Game;

        private CameraManager cameraManager;

        private PanelsManager panelsManager;

        private PlayerController playerController;

        private ClothesPanel clothesPanel;

        private void Awake()
        {
            cameraManager = ServiceLocator.Get<CameraManager>();
            panelsManager = ServiceLocator.Get<PanelsManager>();
            playerController = ServiceLocator.GetGameObject("Player").GetComponentSafe<PlayerController>();
            clothesPanel = ServiceLocator.Get<ClothesPanel>();
        }

        private void OnClick()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.character_back, () =>
            {
                if (exitPanel == PanelType.MainMenu)
                {
                    cameraManager.SetMenuCamera();
                }
                else
                {
                    cameraManager.SetPlayerCamera();
                }
                panelsManager.ShowPanel(exitPanel);
                clothesPanel.EditedClothesKind = ClothesKind.Unsorted;
                if (exitPanel == PanelType.Game)
                {
                    playerController.controlled = true;
                }
            });
        }
    }
}
