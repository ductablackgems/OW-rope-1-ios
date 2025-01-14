using BG_Library.NET.Native_custom.Elements;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BG_Library.NET.Native_custom
{
    public class NativeUIManager : MonoBehaviour
    {
        [BoxGroup("UI"), SerializeField] private NativeUIBase[] elements;

        [BoxGroup("SetDefault value")] public string defaultPrice = "FREE";

        [BoxGroup("SetDefault value")] public string defaultStore = "STORE";

        [BoxGroup("SetDefault value")] public float defaultStar = 4.5f;

        [BoxGroup("SetDefault value")] public string defaultAdBody = "We apologize for any inconvenience the advertisement may cause!";

        [BoxGroup("SetDefault value")] public string defaultAdHeadLine = "ADVERTISEMENT";

        [BoxGroup("SetDefault value")] public Texture defaultIcon;

        [BoxGroup("SetDefault value")] public Texture defaultImage;

        [BoxGroup("SetDefault value")] public string defaultButtonText = "GET IT!";

        [BoxGroup("SetDefault value")] public string defaultAdvertiserText = "Google";

        public void Init(NativeAd n)
        {
            if (n == null)
            {
                Deactive();
                return;
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].Setup(n, this);
            }
        }

        public void SetDefaultWhenDestroy()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].SetDefaultWhenDestroy();
            }
        }

        public void Deactive()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        [BoxGroup("UI"), Button]
        void GetElements()
        {
            elements = GetComponentsInChildren<NativeUIBase>();
        }
    }
}