using UnityEngine;

namespace App.Player.Clothes.GUI
{
    public class WearClothesButton : MonoBehaviour
    {
        public UILabel label;

        public GameObject buttonVideo;

        private ClothesPanel clothesPanel;

        private ClothesManager clothesManager;

        private ClothesItem clothesItem;

        public void SetItem(ClothesItem clothesItem)
        {
            this.clothesItem = clothesItem;
            if (clothesItem == null || clothesItem.buyed)
            {
                buttonVideo.gameObject.SetActive(value: false);
                label.text = LocalizationManager.Instance.GetText(110);
                transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
            }
            else
            {
                buttonVideo.gameObject.SetActive(value: true);
                label.text = LocalizationManager.Instance.GetText(105) + " $" + clothesItem.price.ToString("N0");
                transform.localPosition = new Vector3(-125, transform.localPosition.y, transform.localPosition.z);
            }
        }

        private void Awake()
        {
            clothesPanel = ServiceLocator.Get<ClothesPanel>();
            clothesManager = ServiceLocator.Get<ClothesManager>();
        }

        private void OnClick()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.character_buy, () =>
            {
                if (clothesItem != null && !clothesItem.buyed)
                {
                    clothesManager.TryBuyCurrentItem(clothesPanel.EditedClothesKind);
                }
                clothesManager.TryWearCurrentItem(clothesPanel.EditedClothesKind);
            });
        }
    }
}
