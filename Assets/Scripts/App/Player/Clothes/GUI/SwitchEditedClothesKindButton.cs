using UnityEngine;

namespace App.Player.Clothes.GUI
{
    public class SwitchEditedClothesKindButton : MonoBehaviour
    {
        public ClothesKind clothesKind;

        private ClothesPanel clothesPanel;

        private void Awake()
        {
            clothesPanel = ServiceLocator.Get<ClothesPanel>();
        }

        private void OnClick()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.character_skin, () =>
            {
                clothesPanel.EditedClothesKind = clothesKind;
            });
        }
    }
}
