using UnityEngine;

namespace App.Player.Clothes.GUI
{
    public class MoveClothesButton : MonoBehaviour
    {
        public MoveClothesButtonDirection direction;

        private ClothesPanel clothesPanel;

        private ClothesManager clothesManager;

        private void Awake()
        {
            clothesPanel = ServiceLocator.Get<ClothesPanel>();
            clothesManager = ServiceLocator.Get<ClothesManager>();
        }

        private void OnClick()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.character_next, () =>
            {
                if (direction == MoveClothesButtonDirection.Left)
                {
                    clothesManager.MoveLeft(clothesPanel.EditedClothesKind);
                }
                else
                {
                    clothesManager.MoveRight(clothesPanel.EditedClothesKind);
                }
            });
        }
    }
}
