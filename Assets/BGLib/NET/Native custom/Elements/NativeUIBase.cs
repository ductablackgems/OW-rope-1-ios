using GoogleMobileAds.Api;
using UnityEngine;

namespace BG_Library.NET.Native_custom.Elements
{
    public abstract class NativeUIBase : MonoBehaviour
    {
        public abstract void Setup(NativeAd n, NativeUIManager nativeManager);
        public virtual void SetDefaultWhenDestroy()
        {

        }
    }
}
