using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player.Clothes.GUI
{
    public class VideoButtonClothes : MonoBehaviour
    {
        private ClothesPanel clothesPanel;

        private ClothesItem clothesItem;

        private ClothesManager clothesManager;

        private void Awake()
        {
            clothesPanel = ServiceLocator.Get<ClothesPanel>();
            clothesManager = ServiceLocator.Get<ClothesManager>();
            GetComponentInChildren<UILabel>().text = "Free";
        }

        private void OnClick()
        {
            CallAdsManager.ShowRewardVideo(() =>
            {
                if (clothesItem != null && !clothesItem.buyed)
                {
                    clothesManager.TryUnlockBuyVideoCurrentItem(clothesPanel.EditedClothesKind, gameObject);
                }
                clothesManager.TryUnlockBuyVideoCurrentItem(clothesPanel.EditedClothesKind, gameObject);
            });
        }
    }
}
