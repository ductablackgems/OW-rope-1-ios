using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIAdChoice : NativeUIBase
    {
        [SerializeField] RawImage img;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            Texture2D ac = n.GetAdChoicesLogoTexture();
            if (ac != null)
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                img.texture = ac;
                n.RegisterAdChoicesLogoGameObject(gameObject);
            }
            else
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
