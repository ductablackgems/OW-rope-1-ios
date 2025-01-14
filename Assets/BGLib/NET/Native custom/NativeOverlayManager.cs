using GoogleMobileAds.Api;
using UnityEngine;

namespace BG_Library.NET.Native_custom
{
    public class NativeOverlayManager : MonoBehaviour
    {
        NativeOverlayAd _nativeOverlayAd;
        string _unitId = "ca-app-pub-5754423296608748/9005525738";

        // private void Start()
        // {
        //     MobileAds.Initialize(initStatus =>
        //     {
            
        //     });
        // }

        float timer = 0;
        bool isRender;
        private void Update()
        {
            if (isRender)
            {
                timer += Time.deltaTime;
                if (timer >= 5)
                {
                    isRender = false;
                    timer = 0;
                    DestroyAd();
                }
            }
        }

        /// <summary>
        /// Destroys the native overlay ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Destroying native overlay ad.");
                _nativeOverlayAd.Destroy();
                _nativeOverlayAd = null;
            }
        }

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void RequestNative()
        {
            // Clean up the old ad before loading a new one.
            if (_nativeOverlayAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading native overlay ad.");

            // Create a request used to load the ad.
            var adRequest = new AdRequest();

            // Optional: Define native ad options.
            // NativeAdOptions
            var options = new NativeAdOptions
            {
                AdChoicesPlacement = AdChoicesPlacement.BottomLeftCorner,
                MediaAspectRatio = MediaAspectRatio.Landscape,
                //VideoOptions = VideoOptions.
            };

            // Send the request to load the ad.
            NativeOverlayAd.Load(_unitId, adRequest, options,
                (NativeOverlayAd ad, LoadAdError error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Native Overlay ad failed to load an ad " +
                                       " with error: " + error);
                        return;
                    }

                    // The ad should always be non-null if the error is null, but
                    // double-check to avoid a crash.
                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Native Overlay ad load event " +
                                       " fired with null ad and null error.");
                        return;
                    }

                    // The operation completed successfully.
                    Debug.Log("Native Overlay ad loaded with response : " +
                              ad.GetResponseInfo());
                    _nativeOverlayAd = ad;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers(ad);
                });
        }

        void RegisterEventHandlers(NativeOverlayAd ad)
        {

        }

        /// <summary>
        /// Renders the ad.
        /// </summary>
        public void RenderAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Rendering Native Overlay ad.");

                // Define a native template style with a custom style.
                //NativeTemplateTextStyle
                var style = new NativeTemplateStyle
                {
                    TemplateId = NativeTemplateId.Small,
                    MainBackgroundColor = Color.black,
                    CallToActionText = new NativeTemplateTextStyle
                    {
                        BackgroundColor = Color.green,
                        TextColor = Color.white,
                        FontSize = 5,
                        Style = NativeTemplateFontStyle.Bold
                    }
                };
            
                // Renders a native overlay ad at the default size
                // and anchored to the bottom of the screne.
                _nativeOverlayAd.RenderTemplate(style, AdPosition.Bottom);
                isRender = true;
            }
        }
    }
}
