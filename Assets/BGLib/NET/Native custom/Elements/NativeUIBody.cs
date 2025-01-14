using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIBody : NativeUIBase
    {
        [SerializeField] Text bodyText;
        private string _defaultText;
        
        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            _defaultText = nativeManager.defaultAdBody;
            
            var bodyT = n.GetBodyText();
            if (string.IsNullOrEmpty(bodyT))
            {
                bodyText.text = _defaultText;
            }
            else
            {
                bodyText.text = bodyT;
                n.RegisterBodyTextGameObject(gameObject);
            }
        }

        public override void SetDefaultWhenDestroy()
        {
            bodyText.text = _defaultText;
        }
    }
}
