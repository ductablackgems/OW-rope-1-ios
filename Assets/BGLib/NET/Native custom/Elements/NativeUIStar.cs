using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIStar : NativeUIBase
    {
        [SerializeField] Image[] starImgs;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            var numb = n.GetStarRating();
            if (numb < 0.5f)
            {
                numb = nativeManager.defaultStar;
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            var s1 = (int)numb;
            var s2 = numb - s1;

            for (var i = 0; i < 5; i++)
            {
                if (i < s1)
                {
                    starImgs[i].fillAmount = 1;
                }
                else if (i == s1)
                {
                    starImgs[i].fillAmount = (float)s2;
                }
                else
                {
                    starImgs[i].fillAmount = 0;
                }
            }
        }
    }
}
