using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIPrice : NativeUIBase
    {
        [SerializeField] Text priceText;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            string priceT = n.GetPrice();
            if (string.IsNullOrEmpty(priceT))
            {
                priceText.text = nativeManager.defaultPrice;
            }
            else
            {
                priceText.text = priceT;
                n.RegisterPriceGameObject(gameObject);
            }
        }
    }
}
