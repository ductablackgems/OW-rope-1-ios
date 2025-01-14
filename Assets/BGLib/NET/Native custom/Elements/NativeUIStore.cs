using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIStore : NativeUIBase
    {
        [SerializeField] TMP_Text storeText;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            var storeT = n.GetStore();
            if (string.IsNullOrEmpty(storeT))
            {
                storeText.text = nativeManager.defaultStore;
            }
            else
            {
                storeText.text = storeT;
                n.RegisterStoreGameObject(gameObject);
            }
        }
    }
}