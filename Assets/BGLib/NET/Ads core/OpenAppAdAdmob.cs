using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using System;
using BG_Library.DEBUG;
using BG_Library.Common;

namespace BG_Library.NET
{
    public class OpenAppAdAdmob : MonoBehaviour
    {
        public static OpenAppAdAdmob Ins { get; private set; }

        [BoxGroup("Detail"), ReadOnly] public bool isInitAdmobComplete;
        [BoxGroup("Detail"), ReadOnly] public bool isShowFirstTime;
        [BoxGroup("Detail"), ReadOnly] public float waitAdTimer;

        [BoxGroup("Config")] public bool debuggable;

        private bool isOpenAdsJustOpened;
        private bool isOpenAdsClose;
        private int retryAttempt;

        public bool IsOpenAdsJustClosed => isOpenAdsJustOpened;
        
        void Awake()
        {
            if (Ins == null)
            {
                Ins = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        IEnumerator Start()
        {
            // Bat buoc init admob de su dung cho cac Mediation khac
            MobileAds.Initialize(initStatus =>
            {
                isInitAdmobComplete = true;
                LoadAppOpenAd();
            });

            var firstTime = PlayerPrefs.GetInt("AppOpenFirstTime", 0); // Co phai lan dau khong
            if (firstTime == 0)
            {
                PlayerPrefs.SetInt("AppOpenFirstTime", 1);
            }
            
            var ignoreFirstTime = firstTime == 0  && NetConfigsSO.Ins.ignoreAppOpenFirstTime; // Neu la lan dau va setting ignore lan dau

            isShowFirstTime = false;
            if (NetConfigsSO.Ins.useAppOpenAdmob && !ignoreFirstTime)
            {
                while (true)
                {
                    waitAdTimer += Time.deltaTime;

                    if (waitAdTimer > NetConfigsSO.Ins.WaitAppOpenTimer)
                    {
                        if (debuggable) Debug.Log("App Open Admob timeout");
                        if (DebugManager.IsDebug)
                        {
                            DebugManager.Ins.appOpen.UpdateDebugText($"Time out, wait time : {NetConfigsSO.Ins.WaitAppOpenTimer}s");
                        }

                        break;
                    }
                    
                    if (isShowFirstTime)
                    {
                        DebugManager.Ins.appOpen.UpdateDebugText($"Show AO admob first time open app");
                        ShowAppOpenAd();
                        break;
                    }
                    yield return null;
                }
            }
            else
            {
                if (debuggable) Debug.Log("Ignore first time or not use");
                if (DebugManager.IsDebug)
                {
                    if (!NetConfigsSO.Ins.useAppOpenAdmob)
                    {
                        DebugManager.Ins.appOpen.UpdateDebugText($"Not use AO");
                    }
                    else if (ignoreFirstTime)
                    {
                        DebugManager.Ins.appOpen.UpdateDebugText($"Ignore first time AO");
                    }
                }
            }
        }

        #region OpenApp Admob
        private AppOpenAd appOpenAd;

        IEnumerator WaitOpenAdsClose()
        {
            yield return new WaitUntil(()=> isOpenAdsClose);
            Time.timeScale = 1;
        }

        public bool IsAppOpenReady => appOpenAd != null && appOpenAd.CanShowAd();

        /// <summary>
        /// Shows the app open ad.
        /// </summary>
        public void ShowAppOpenAd()
        {
            if (appOpenAd != null && appOpenAd.CanShowAd())
            {
                if (debuggable) Debug.Log("Showing app open Admob. " + waitAdTimer);
                
                Time.timeScale = 0f;
                isOpenAdsClose = false;
                isOpenAdsJustOpened = true;

                StartCoroutine(WaitOpenAdsClose());
                
                appOpenAd.Show();
            }
            else
            {
                if (debuggable) Debug.LogError("App Open Admob is not ready yet.");
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.appOpen.UpdateDebugText("App Open Admob is not ready yet.");
                }
            }
        }

        /// <summary>
        /// Loads the app open ad.
        /// </summary>
        private void LoadAppOpenAd()
        {
            if (AdsManager.IAP_RemoveAds)
            {
                return;
            }

            // Clean up the old ad before loading a new one.
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();
            // send the request to load the ad.
            AppOpenAd.Load(NetConfigsSO.Ins.AppOpenIdAdmob, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        if (debuggable) Debug.LogError("app open Admob failed to load an ad " +
                                       "with error : " + error);

                        AdsManager.AdsEnqueueCallback(()=>
                        {
                            retryAttempt++;
                            var retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
                            Invoke(nameof(LoadAppOpenAd), (float)retryDelay);

                            if (DebugManager.IsDebug)
                            {
                                DebugManager.Ins.appOpen.LoadFail(DebugFullScreenAdsManager.FORMAT.ADMOB
                                    , $"Load after {retryDelay}s", $"Load fail with ERROR:\n {error}");
                            }
                        });
                        return;
                    }

                    isShowFirstTime = true;
                    if (debuggable) Debug.Log("App open Admob loaded with response : " + ad.GetResponseInfo());
                    if (DebugManager.IsDebug)
                    {
                        AdsManager.AdsEnqueueCallback(() =>
                        {
                            DebugManager.Ins.appOpen.Loaded(DebugFullScreenAdsManager.FORMAT.ADMOB
                                , "", ad.GetResponseInfo().ToString());
                        });
                    }

                    appOpenAd = ad;
                    RegisterEventHandlers(ad);
                });

            if (debuggable) Debug.Log("Loading the app open Admob.");
            if (DebugManager.IsDebug)
            {
                AdsManager.AdsEnqueueCallback(() =>
                {
                    DebugManager.Ins.appOpen.Load(DebugFullScreenAdsManager.FORMAT.ADMOB, "");
                });
            }
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                var rev = adValue.Value / 1000000f;
                FirebaseEvent.LogEventAdImpression(
                    AdjustConst.admob_sdk,
                    string.Empty,
                    string.Empty,
                    "APPOPEN",
                    rev,
                    adValue.CurrencyCode,
                    "AdMob");

                if (debuggable) Debug.Log(String.Format("App open Admob paid {0} {1}.",
                    rev, adValue.CurrencyCode));
                if (DebugManager.IsDebug)
                {
                    AdsManager.AdsEnqueueCallback(() =>
                    {
                        DebugManager.Ins.appOpen.UpdateRev(DebugFullScreenAdsManager.FORMAT.ADMOB, rev, adValue.CurrencyCode);
                    });
                }
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                if (debuggable) Debug.Log("App open Admob recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                if (debuggable) Debug.Log("App open Admob was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                if (debuggable) Debug.Log("App open Admob full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                AdsManager.Ins.lastTime = AdsManager.PlayTime;
                isOpenAdsClose = true;

                AdsManager.AdsEnqueueCallback(()=>
                {
                    DOTweenManager.Ins.TweenDelay(5, ()=> isOpenAdsJustOpened = false);
                });

                LoadAppOpenAd();

                if (debuggable) Debug.Log("App open Admob full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                LoadAppOpenAd();

                if (debuggable) Debug.LogError("App open Admob failed to open full screen content " +
                    "with error : " + error);
                if (DebugManager.IsDebug)
                {
                    AdsManager.AdsEnqueueCallback(() =>
                    {
                        DebugManager.Ins.appOpen.DisplayFail(DebugFullScreenAdsManager.FORMAT.ADMOB, "", $"Display fail with error: {error}");                        
                    });
                }
            };
        }
        #endregion
    }
}