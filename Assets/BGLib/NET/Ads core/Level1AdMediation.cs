using System;
using BG_Library.DEBUG;
using UnityEngine;
using Sirenix.OdinInspector;
using Firebase.Analytics;

namespace BG_Library.NET.Ads_core
{
    /// <summary>
    /// Max Mediation + Admob (Native overlay)
    /// </summary>
    public class Level1AdMediation : AdsCore
    {
        [BoxGroup("MAX MEDIATION"), SerializeField, ReadOnly]
        protected string maxSDK = "";

        [BoxGroup("MAX MEDIATION"), SerializeField, ReadOnly]
        protected string idFAMax = "";

        [BoxGroup("MAX MEDIATION"), SerializeField, ReadOnly]
        protected string idRWMax = "";

        [BoxGroup("MAX MEDIATION"), SerializeField, ReadOnly]
        protected string idBNMax = "";

        [BoxGroup("MAX MEDIATION"), SerializeField, ReadOnly]
        protected string idOpenAds = "";

        [BoxGroup("MAX MEDIATION"), SerializeField, ReadOnly]
        protected bool isInitMaxComplete;

        protected bool isTimerBlockActive;
		protected bool isBlockAppOpenAfterAds;
        protected bool isIgnoreBlockAOAfterAds;
        protected bool isIgnoreBlockAOAfterAO;

        protected float blockAppOpenAdsTimer;

        protected virtual void Update()
        {
            if (isTimerBlockActive)
            {
                blockAppOpenAdsTimer += Time.deltaTime;
                if (blockAppOpenAdsTimer >= 4)
                {
                    isBlockAppOpenAfterAds = false;
                    isTimerBlockActive = false;

                    blockAppOpenAdsTimer = 0;
                    if (debuggable) Debug.Log("Timeout block AO after fullscreen ads");
                }
            }
        }

        public override void Init(AdsManager.NetworkInfor netInfor)
        {
            maxSDK = netInfor.MaxInforIns.MaxSDKKey;
            idFAMax = netInfor.MaxInforIns.IdFA;
            idRWMax = netInfor.MaxInforIns.IdRW;
            idBNMax = netInfor.MaxInforIns.IdBN;
            idOpenAds = netInfor.MaxInforIns.IdOA;

            isIgnoreBlockAOAfterAds = AdsManager.Ins.AdsConfig.OpenApp.IsShowAfterFullScreenAds;
            isIgnoreBlockAOAfterAO = AdsManager.Ins.AdsConfig.OpenApp.IsShowAfterFullScreenAds2;

            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                isInitMaxComplete = true;

                InitOpenAds();
                InitializeInterstitialAds();
                InitializeRewardedAds();
                InitializeBannerAds();

                //ManagerExistingPrivacySettings();
            };

            MaxSdk.SetSdkKey(maxSDK);
            MaxSdk.InitializeSdk();
        }

        protected void SetTimerBlockActive()
        {
            isTimerBlockActive = true;
            blockAppOpenAdsTimer = 0;
        }

        #region Interstitial Ad Methods---------------------------------------------------------------------
        protected int interstitialRetryAttempt;

        public override void ShowForceAds()
        {
            if (MaxSdk.IsInterstitialReady(idFAMax))
            {
				BreakAdsInterstitial.Ins.WaitToShow();

				isBlockAppOpenAfterAds = true;
				MaxSdk.ShowInterstitial(idFAMax);
            }
        }

		public override bool IsFAReady()
		{
			return MaxSdk.IsInterstitialReady(idFAMax);
		}

		protected virtual void InitializeInterstitialAds()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
			MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

            LoadInterstitial();
        }

        protected virtual void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(idFAMax);

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.interstitial.Load(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, "");
            }
        }

        protected virtual void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            interstitialRetryAttempt = 0;

            if (debuggable) Debug.Log("Interstitial max loaded: " + adInfo.Placement);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.interstitial.Loaded(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                , $"eCPM: {adInfo.Revenue * 1000}$", adInfo.NetworkName);
            }
        }
		protected virtual void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
			FirebaseEvent.LogEvent(BgAdsConst.fa_show, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));
		}

        protected virtual void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
			FirebaseEvent.LogEvent(BgAdsConst.fa_click, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));
		}

		protected virtual void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            interstitialRetryAttempt++;
            var retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
            Invoke(nameof(LoadInterstitial), (float)retryDelay);
            
            if (debuggable) Debug.Log("Interstitial max failed to load with error code: " + errorInfo.Code);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.interstitial.LoadFail(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , $"Load after {retryDelay}s"
                    , "Load fail with ERROR: " + errorInfo.AdLoadFailureInfo + "\n" + errorInfo.Code);
            }
        }

        protected virtual void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
			FirebaseEvent.LogEvent(BgAdsConst.fa_failed, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));
			LoadInterstitial();

            if (debuggable) Debug.Log("Interstitial max failed to display with error code: " + errorInfo.Code);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.interstitial.DisplayFail(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, ""
                    , "Display fail with ERROR: " + errorInfo.AdLoadFailureInfo + "\n" + errorInfo.Code);
            }
        }

        protected virtual void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
			FirebaseEvent.LogEvent(BgAdsConst.fa_finish, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));

			AdsManager.Ins.lastTime = AdsManager.PlayTime;
            SetTimerBlockActive();

            LoadInterstitial();

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.interstitial.Dismissed(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , "", "Dismissed, last time: " + AdsManager.Ins.lastTime);
            }

            if (isIgnoreBlockAOAfterAds)
            {
                DebugManager.Ins.appOpen.UpdateDebugText($"Show AO Max after FA"); 
                MaxSdk.ShowAppOpenAd(idOpenAds);
            }
        }

        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
			AdsManager.AdsEnqueueCallback(() =>
			{
				AdsManager.SetInterstitialCount();
				FirebaseEvent.LogEventAdImpressionPos(
					AdjustConst.applovin_max_sdk,
					adInfo.AdUnitIdentifier,
					adInfo.Placement,
					adInfo.AdFormat,
					adInfo.Revenue,
					"USD",
					adInfo.NetworkName,
					AdsManager.LastInterstitialPos,
					AdsManager.GetInterstitialCount);

				if (debuggable) Debug.Log("Interstitial max paid eCPM: " + adInfo.Revenue * 1000);
				if (DebugManager.IsDebug)
				{
					DebugManager.Ins.interstitial.UpdateRev(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, adInfo.Revenue, "USD");
				}
			});
        }

        #endregion Interstitial Ad Methods---------------------------------------------------------------------

        #region Rewarded Ad Methods----------------------------------------------------------------
        protected int rewardedRetryAttempt;

        public override void ShowRewarded()
        {
            if (MaxSdk.IsRewardedAdReady(idRWMax))
            {
				BreakAdsInterstitial.Ins.WaitToShow();

				isBlockAppOpenAfterAds = true;
				MaxSdk.ShowRewardedAd(idRWMax);
            }
        }

        public override bool IsRewardedReady()
        {
            return !string.IsNullOrEmpty(idRWMax) && MaxSdk.IsRewardedAdReady(idRWMax);
        }

        protected virtual void InitializeRewardedAds()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
			MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
			MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

            LoadRewardedAd();
        }

        protected virtual void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(idRWMax);

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.reward.Load(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, "");
            }
        }

        protected virtual void OnRewardedLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            rewardedRetryAttempt = 0;

            if (debuggable) Debug.Log("Rewarded max loaded: " + adInfo.Placement);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.reward.Loaded(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , $"eCPM: {adInfo.Revenue * 1000}$", adInfo.NetworkName);
            }
        }

        protected virtual void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
			FirebaseEvent.LogEvent(BgAdsConst.rw_show, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));
		}

		protected virtual void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			FirebaseEvent.LogEvent(BgAdsConst.rw_click, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));
		}

		protected virtual void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
            Invoke(nameof(LoadRewardedAd), (float)retryDelay);

            if (debuggable) Debug.Log("Rewarded max failed to load with error code: " + errorInfo.Code);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.reward.LoadFail(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , $"Load after {retryDelay}s"
                    , "Load fail with Error: " + errorInfo.AdLoadFailureInfo + "\n" + errorInfo.Code);
            }
        }

        protected virtual void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
			FirebaseEvent.LogEvent(BgAdsConst.rw_failed, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));

			LoadRewardedAd();

            if (debuggable) Debug.Log("Rewarded max failed to display with error code: " + errorInfo.Code);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.reward.DisplayFail(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, "",
                    "Display fail with Error: " + errorInfo.AdLoadFailureInfo + "\n" + errorInfo.Code);
            }
        }

        protected virtual void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
			FirebaseEvent.LogEvent(BgAdsConst.rw_finish, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));

			AdsManager.Ins.lastTime = AdsManager.PlayTime;
            SetTimerBlockActive();

            LoadRewardedAd();

			if (debuggable) Debug.Log("Rewarded max dismissed " + AdsManager.Ins.lastTime);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.reward.Dismissed(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , "", "Dismissed, last time: " + AdsManager.Ins.lastTime);
            }

            if (isIgnoreBlockAOAfterAds)
            {
                DebugManager.Ins.appOpen.UpdateDebugText($"Show AO Max after RW"); 
                MaxSdk.ShowAppOpenAd(idOpenAds);
            }
        }

        protected virtual void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward,
            MaxSdkBase.AdInfo adInfo)
        {
            AdsManager.Ins.isGetReward = true;
        }

        protected virtual void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
			AdsManager.AdsEnqueueCallback(() =>
			{
				AdsManager.SetRewardCount();
				FirebaseEvent.LogEventAdImpressionPos(
					AdjustConst.applovin_max_sdk,
					adInfo.AdUnitIdentifier,
					adInfo.Placement,
					adInfo.AdFormat,
					adInfo.Revenue,
					"USD",
					adInfo.NetworkName,
					AdsManager.LastRewardedPos,
                    AdsManager.GetRewardCount);

				if (debuggable) Debug.Log("Rewarded max paid eCPM: " + adInfo.Revenue);
				if (DebugManager.IsDebug)
				{
					DebugManager.Ins.reward.UpdateRev(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, adInfo.Revenue, "USD");
				}
			});
        }

        #endregion Rewarded Ad Methods----------------------------------------------------------------

        #region Banner Ad Methods-------------------------------------------------------
        protected int bannerRetryAttempt;
        protected bool isLoadedBannerMax;
        protected bool isShowingBannerMax;

        public override void ShowBanner()
        {
            MaxSdk.ShowBanner(idBNMax);

            if (isLoadedBannerMax)
            {
                isShowingBannerMax = true;
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.banner.Show(DebugBannerManager.FORMAT.BN_MAX);
                }
            }
        }

        public override void HiddenBanner()
        {
            MaxSdk.HideBanner(idBNMax);

            if (isShowingBannerMax)
            {
                isShowingBannerMax = false;
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.banner.Hide(DebugBannerManager.FORMAT.BN_MAX);
                }
            }
        }

        public override void DestroyBanner()
        {
            MaxSdk.DestroyBanner(idBNMax);

            if (isShowingBannerMax)
            {
                isShowingBannerMax = false;
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.banner.Hide(DebugBannerManager.FORMAT.BN_MAX);
                }
            }
        }

        protected void InitializeBannerAds()
        {
            isLoadedBannerMax = false;
            if (!AdsManager.Ins.AdsConfig.Banner.IsShow)
            {
                return;
            }

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

            LoadMaxBanner();
        }

        protected void LoadMaxBanner()
        {
            var pos = AdsManager.Ins.AdsConfig.Banner.Pos ==
                      AdsManager.StructAdsConfig.BannerInfor.BannerPosition.TopCenter
                ? MaxSdkBase.BannerPosition.TopCenter
                : MaxSdkBase.BannerPosition.BottomCenter;
            MaxSdk.CreateBanner(idBNMax, pos);
            MaxSdk.SetBannerExtraParameter(idBNMax, "adaptive_baner", "true");

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.banner.Load(DebugBannerManager.FORMAT.BN_MAX);
            }
        }

        protected void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            isLoadedBannerMax = true;

            if (AdsManager.Ins.AdsConfig.Banner.IsAutoShow
                && !AdsManager.Ins.NetInfor.NativeBannerInforsIns.IsEnable 
                && !AdsManager.IAP_RemoveAds)
            {
                ShowBanner();
            }

            if (debuggable) Debug.Log("Banner max loaded: " + adInfo.Placement);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.banner.Loaded(DebugBannerManager.FORMAT.BN_MAX, $"eCPM: {adInfo.Revenue * 1000}$", adInfo.Placement);

                if (isShowingBannerMax)
                {
                    DebugManager.Ins.banner.Show(DebugBannerManager.FORMAT.BN_MAX);
                }
            }
        }

        protected void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            isLoadedBannerMax = false;

            if (debuggable) Debug.Log("Banner max failed to load with error code: " + errorInfo.Code);

            bannerRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, bannerRetryAttempt));
            Invoke(nameof(LoadMaxBanner), (float)retryDelay);

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.banner.LoadFail(DebugBannerManager.FORMAT.BN_MAX, $"Delay load {retryDelay}s", errorInfo.Code.ToString());
            }
        }

        protected void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
        }

        protected void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            FirebaseEvent.LogEventAdImpression(
                AdjustConst.applovin_max_sdk,
                adInfo.AdUnitIdentifier,
                adInfo.Placement,
                adInfo.AdFormat,
                adInfo.Revenue,
                "USD",
                adInfo.NetworkName);

            if (debuggable) Debug.Log("Banner max paid eCPM: " + adInfo.Revenue * 1000);
            if (DebugManager.IsDebug)
            {
                AdsManager.AdsEnqueueCallback(() =>
                {
                    DebugManager.Ins.banner.UpdateRev(DebugBannerManager.FORMAT.BN_MAX, adInfo.Revenue, "USD");
                });
            }
        }

        #endregion Banner Ad Methods-------------------------------------------------------

        #region Open app ----------------------------------------------------
        protected int openAdsRetryAttempt;
        bool isOpenAppLoaded;

        public override bool IsOpenAppReady()
        {
            if (!isOpenAppLoaded) return false;
            return MaxSdk.IsAppOpenAdReady(idOpenAds);
        }

        public override void ShowOpenApp()
        {
            // Khong show AO sau khi fullscreen Ads tat
            if (isBlockAppOpenAfterAds)
            {
                return;
            }

            // Khong show AO sau khi 1 AO vua tat
            if (OpenAppAdAdmob.Ins.IsOpenAdsJustClosed)
            {
                return;
            }

            DebugManager.Ins.appOpen.UpdateDebugText($"Show AO Max resume application");

            if (AdsManager.Ins.AdsConfig.OpenApp.IsUseAllAdmob && OpenAppAdAdmob.Ins.IsAppOpenReady)
            {
                OpenAppAdAdmob.Ins.ShowAppOpenAd();
                return;
            }
            
            MaxSdk.ShowAppOpenAd(idOpenAds);
        }

        protected void InitOpenAds()
        {
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += AppOpen_OnAdLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += AppOpen_OnAdRevenuePaidEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += AppOpen_OnAdDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += AppOpen_OnAdDisplayFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += AppOpen_OnAdLoadFailedEvent;
            
            LoadOpenAds();
        }

        protected void LoadOpenAds()
        {
            isOpenAppLoaded = false;
            MaxSdk.LoadAppOpenAd(idOpenAds);

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.appOpen.Load(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION);
            }
        }

        protected void AppOpen_OnAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            isOpenAppLoaded = true;
            openAdsRetryAttempt = 0;

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.appOpen.Loaded(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , $"eCPM: {arg2.Revenue * 1000}$", arg2.NetworkName);
            }
        }

        protected void AppOpen_OnAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
        {
            openAdsRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, openAdsRetryAttempt));

            Invoke(nameof(LoadOpenAds), (float)retryDelay);

            if (debuggable) Debug.Log("App open max failed to load with error code: " + arg2.Code);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.appOpen.LoadFail(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , $"Load after {retryDelay}s"
                    , "Load fail with ERROR: " + arg2.AdLoadFailureInfo + "\n" + arg2.Code);
            }
        }

        protected void AppOpen_OnAdDisplayFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2, MaxSdkBase.AdInfo arg3)
        {
            LoadOpenAds();

            if (debuggable) Debug.Log("App open failed to display with error code: " + arg2.Code);
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.appOpen.DisplayFail(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, "",
                    "Display fail with ERROR: " + arg2.AdLoadFailureInfo + "\n" + arg2.Code);
            }
        }

        protected void AppOpen_OnAdDisplayedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            if (debuggable) Debug.Log("App open Max Displayed");
        }

        protected void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AdsManager.Ins.lastTime = AdsManager.PlayTime;

            LoadOpenAds();
            if (isIgnoreBlockAOAfterAO)
            {
                AdsManager.AdsEnqueueCallback(()=>
                {
                    DebugManager.Ins.appOpen.UpdateDebugText($"Show AO admob after AO max");
                    OpenAppAdAdmob.Ins.ShowAppOpenAd();
                });
            }

            if (debuggable) Debug.Log("App open Max Closed");
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.appOpen.Dismissed(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION
                    , "", "Dismissed, last time: " + AdsManager.Ins.lastTime);
            }
        }

        protected void AppOpen_OnAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo adInfo)
        {
            FirebaseEvent.LogEventAdImpression(
                AdjustConst.applovin_max_sdk,
                adInfo.AdUnitIdentifier,
                adInfo.Placement,
                adInfo.AdFormat,
                adInfo.Revenue,
                "USD",
                adInfo.NetworkName);

            if (debuggable) Debug.Log("App Open max paid eCPM: " + adInfo.Revenue);
            if (DebugManager.IsDebug)
            {
                AdsManager.AdsEnqueueCallback(() =>
                {
                    DebugManager.Ins.appOpen.UpdateRev(DebugFullScreenAdsManager.FORMAT.MAXMEDIATION, adInfo.Revenue, "USD");
                });
            }
        }
        #endregion Open Ads -------------------------------
    }
}