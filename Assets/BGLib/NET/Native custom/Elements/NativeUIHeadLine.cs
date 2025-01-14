using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIHeadLine : NativeUIBase
    {
        [SerializeField] Text headlineText;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            var headLineT = n.GetHeadlineText();
            if (string.IsNullOrEmpty(headLineT))
            {
                headlineText.text = nativeManager.defaultAdHeadLine;
            }
            else
            {
                headlineText.text = headLineT;
                n.RegisterHeadlineTextGameObject(gameObject);
            }
        }
    }
}
