using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIActionToCall : NativeUIBase
    {
        [SerializeField] Text btnText;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            string c = n.GetCallToActionText();
            if (string.IsNullOrEmpty(c))
            {
                btnText.text = nativeManager.defaultButtonText;
            }
            else
            {
                btnText.text = c;
                n.RegisterCallToActionGameObject(gameObject);
            }
        }
    }
}
