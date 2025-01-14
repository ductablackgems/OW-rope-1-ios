using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIAdvertiser : NativeUIBase
    {
        [SerializeField] Text adText;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            var at = n.GetAdvertiserText();
            if (string.IsNullOrEmpty(at))
            {
                adText.text = nativeManager.defaultAdvertiserText;
            }
            else
            {
                adText.text = at;
                n.RegisterAdvertiserTextGameObject(gameObject);
            }
        }
    }
}
