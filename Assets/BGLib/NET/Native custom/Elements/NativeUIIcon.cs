using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIIcon : NativeUIBase
    {
        [SerializeField] RawImage img;
        private Texture _defaultIcon;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            _defaultIcon = nativeManager.defaultIcon;
            
            var ic = n.GetIconTexture();
            if (ic)
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
                img.texture = ic;

                n.RegisterIconImageGameObject(gameObject);
            }
            else
            {
                if (!_defaultIcon)
                {
                    if (gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                    }

                    return;
                }
                img.texture = _defaultIcon;
            }
        }

        public override void SetDefaultWhenDestroy()
        {
            if (!_defaultIcon)
            {
                return;
            }
            img.texture = _defaultIcon;
        }
    }
}
