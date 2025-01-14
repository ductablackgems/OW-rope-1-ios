using System;
using BG_Library.NET.Ads_core;
using BG_Library.NET.Native_custom;
using BG_Library.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BG_Library.DEBUG;
using Firebase.Analytics;

namespace BG_Library.NET
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Ins { get; private set; }

        [FoldoutGroup("Ins")] public NativeManager NativeNormalManagerIns;
        [FoldoutGroup("Ins")] public NativeBannerManager NativeBannerManagerIns;
        [FoldoutGroup("Ins")] public NativeManager NativeOverlayManagerIns;

        [FoldoutGroup("Ins")] public Level1AdMediation MaxMediationIns;
        [FoldoutGroup("Ins")] public Level2AdMediation MaxVsAdmobFloorIns;
        [FoldoutGroup("Ins")] public Level3AdMediation MaxVsAdmobFloorVsCollapIns;
        [FoldoutGroup("Ins"), SerializeField, ReadOnly] private AdsCore adsCoreIns;

        [FoldoutGroup("Remote"), SerializeField] private StructAdsConfig adsConfig = new StructAdsConfig();
        [FoldoutGroup("Remote"), SerializeField] private NetworkInfor netInfor = new NetworkInfor();

        public AdsCore AdsCoreIns => adsCoreIns;
        public StructAdsConfig AdsConfig => adsConfig;
        public NetworkInfor NetInfor => netInfor;

        [FoldoutGroup("Detail"), ReadOnly] public float lastTime;
        [FoldoutGroup("Detail"), ReadOnly] public bool isGetReward;

        private static float playTime;
        public static float PlayTime => playTime;
        public static bool IAP_RemoveAds;

        private Action rwAction;

        static Queue<Action> AdsThreadCallback = new Queue<Action>();

        public static string LastInterstitialPos;
        public static string LastRewardedPos;
        
        public static bool ignoreInterstitial;
        public static bool ignoreReward;

        public static EOAAction OpenAdAction = EOAAction.NONE;
        public static bool ignoreAppOpenAction;

        private string lastVersion;
        
        private void Awake()
        {
            if (Ins == null)
            {
                Ins = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            IAP_RemoveAds = PlayerPrefs.GetInt("REMOVEADS", 0) == 1;
        }

        private void Start()
        {
            lastVersion  = PlayerPrefs.GetString("LastVersion", Application.version);
            PlayerPrefs.SetString("LastVersion", Application.version);

            Debug.unityLogger.logEnabled = DebugManager.IsDebug;
            StartCoroutine(CountPlayTime());
            StartCoroutine(WaitToTrackUpdate());
        }

        private void Update()
        {
            if (isGetReward)
            {
                isGetReward = false;
                rwAction?.Invoke();
                rwAction = null;
            }

            if (AdsThreadCallback.Count > 0)
            {
                var c = AdsThreadCallback.Dequeue();
                c?.Invoke();
            }
        }

        IEnumerator WaitToTrackUpdate()
        {
            yield return new WaitUntil(() => RemoteConfig.Ins.isFirebaseInitialized);

            if (lastVersion != Application.version)
            {
                FirebaseEvent.LogEvent("bg_update"
                    , new Parameter("version", Application.version)
                    , new Parameter("last_version", lastVersion));
            }
        }

        public static void AdsEnqueueCallback(Action callback)
        {
            if (callback != null)
            {
                AdsThreadCallback.Enqueue(callback);
            }
        }
        
        public void PurchaseRemoveAds()
        {
            IAP_RemoveAds = true;
            PlayerPrefs.SetInt("REMOVEADS", 1);

            if (Ins.adsConfig != null) Ins.adsCoreIns.DestroyBanner();

            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.UpdateRemoveAdsText();
            }
        }

        public void RevertPurchaseRemoveAds()
        {
            IAP_RemoveAds = false;
            PlayerPrefs.SetInt("REMOVEADS", 0);

            ShowBanner();
            
            if (DebugManager.IsDebug)
            {
                DebugManager.Ins.UpdateRemoveAdsText();
            }
        }

        IEnumerator CountPlayTime()
        {
            var temp = new WaitForSecondsRealtime(0.5f);
            while (true)
            {
                yield return temp;
                playTime += 0.5f;
            }
        }

        static bool firstTimeOpenApp = true;

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                if (firstTimeOpenApp)
                {
                    firstTimeOpenApp = false;
                    return;
                }

                if (debuggable) Debug.Log("Show open ads when resume " + Ins.adsConfig.OpenApp.IsShowWhenResume);
                if (Ins.adsConfig.OpenApp.IsShowWhenResume)
                {
                    if (!Ins.adsCoreIns)
                    {
                        return;
                    }

                    if (IAP_RemoveAds)
                    {
                        return;
                    }

                    if (ignoreAppOpenAction)
                    {
                        OpenAdAction = EOAAction.NONE;
                    }
                    
                    if (OpenAdAction != EOAAction.NONE)
                    {
                        DebugManager.Ins.appOpen.UpdateDebugText($"Open ads ignore by {OpenAdAction}");
                        OpenAdAction = EOAAction.NONE;
                        return;
                    }
                    
                    Ins.adsCoreIns.ShowOpenApp();
                }
            }
        }

        public static void InitDataNetwork(string dataNetworkJson)
        {
            Ins.netInfor = JsonTool.DeserializeObject<NetworkInfor>(dataNetworkJson);
            
            if (Ins.netInfor == null)
            {
                Ins.netInfor = new NetworkInfor();
            }
        }

        public static void InitAdsConfig(string adsConfigJson)
        {
            Ins.adsConfig = JsonTool.DeserializeObject<StructAdsConfig>(adsConfigJson);

            if (Ins.adsConfig == null)
            {
                Ins.adsConfig = new StructAdsConfig();
            }
        }
        
        public static void InitMediation()
        {
            if (Ins.netInfor.TypeMediation == 0) // Only use MAX
            {
                Ins.adsCoreIns = Ins.MaxMediationIns;
            }
            else if (Ins.netInfor.TypeMediation == 1) // Use Max and Admob
            {
                Ins.adsCoreIns = Ins.MaxVsAdmobFloorIns;
            }
            else if (Ins.netInfor.TypeMediation == 2)
            {
                Ins.adsCoreIns = Ins.MaxVsAdmobFloorVsCollapIns;
            }

            if (Ins.debuggable) Debug.Log($"Start init mediation {Ins.netInfor.TypeMediation}");
            Ins.adsCoreIns.Init(Ins.netInfor);

            if (Ins.NativeNormalManagerIns.enabled)
            {
                Ins.NativeNormalManagerIns.Init(Ins.netInfor.NativeInforsIns);
            }

            if (Ins.NativeBannerManagerIns.enabled)
            {
                Ins.NativeBannerManagerIns.Init(Ins.netInfor.NativeBannerInforsIns);
            }
        }

        #region Ads count
        public static int GetInterstitialCount => PlayerPrefs.GetInt("Interstitial count", 0);
        public static void SetInterstitialCount()
        {
            PlayerPrefs.SetInt("Interstitial count"
                , PlayerPrefs.GetInt("Interstitial count", 0) + 1);
        }

        public static int GetRewardCount => PlayerPrefs.GetInt("Reward count", 0);
        public static void SetRewardCount()
        {
            PlayerPrefs.SetInt("Reward count"
                , PlayerPrefs.GetInt("Reward count", 0) + 1);
        }
        #endregion

        #region Interstitial
        public static void ShowInterstitial(string pos, Action actionDone = null)
        {
            if (ignoreInterstitial)
            {
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.interstitial.UpdateDebugText($"Request {pos} => Ignore by script editor");
                }
                actionDone?.Invoke();
                return;
            }

            if (IAP_RemoveAds || !Ins.adsCoreIns || !Ins.adsConfig.IsEnableInterstitial)
            {
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.interstitial.UpdateDebugText($"Request {pos} => Set off");
                }
                actionDone?.Invoke();
                return;
            }

            actionDone?.Invoke();

			if (pos == BgAdsConst.break_ads)
			{
                InterstitialDisplay(pos);
                return;
            }

			var structAds = Array.Find(Ins.adsConfig.Interstitial
                , c => c.Pos == pos);

            if (structAds == null || !structAds.IsShow)
            {
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.interstitial.UpdateDebugText($"Request {pos} => Struct ads ignore");
                }
                return;
            }

            var deltaTimeAds = PlayTime - Ins.lastTime;
            var minTime = Mathf.Max(structAds.TimeShow - GetInterstitialCount * Ins.adsConfig.DecreaseInterTPI,
                Ins.adsConfig.MinTimeInterstitialShow);

            if (deltaTimeAds < minTime)
            {
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.interstitial.UpdateDebugText($"Request {pos} => Delta time: {deltaTimeAds} < Interval: {minTime}");
                }
                return;
            }

			InterstitialDisplay(pos);
		}

		static void InterstitialDisplay(string pos)
        {
			FirebaseEvent.LogEvent(BgAdsConst.fa_show_ready, new Parameter(BgAdsConst.ads_pos, pos));
			if (!Ins.AdsCoreIns.IsFAReady())
			{
				FirebaseEvent.LogEvent(BgAdsConst.fa_show_notready, new Parameter(BgAdsConst.ads_pos, pos));
			}

			LastInterstitialPos = pos;
			Ins.adsCoreIns.ShowForceAds();

			if (DebugManager.IsDebug)
			{
				DebugManager.Ins.interstitial.UpdateDebugText($"Request {pos} => Success request");
			}
		}

        public static bool IsCanShowInterstitial(string pos)
        {
            if (ignoreInterstitial)
            {
                return false;
            }

            if (IAP_RemoveAds || !Ins || !Ins.adsCoreIns || !Ins.adsConfig.IsEnableInterstitial)
            {
                return false;
            }
                
            var structAds = Array.Find(Ins.adsConfig.Interstitial
                , c => c.Pos == pos);

            if (structAds == null || !structAds.IsShow)
            {
                return false;
            }
            
            var deltaTimeAds = PlayTime - Ins.lastTime;
            var minTime = Mathf.Max(structAds.TimeShow - GetInterstitialCount * Ins.adsConfig.DecreaseInterTPI,
                Ins.adsConfig.MinTimeInterstitialShow);

            if (deltaTimeAds < minTime)
            {
                return false;
            }
            return true;
        }

        private bool IsAbleInterstitial(StructAdsConfig.InterstitialInfor structAds)
        {
            var interCount = GetInterstitialCount;
            return PlayTime - Ins.lastTime >=
                   Mathf.Max(structAds.TimeShow - interCount * adsConfig.DecreaseInterTPI,
                       adsConfig.MinTimeInterstitialShow);
        }
        #endregion

        #region Reward
        public static void ShowRewardVideo(string pos, Action actionDone)
        {
            if (ignoreReward)
            {
                if (DebugManager.IsDebug)
                {
                    DebugManager.Ins.reward.UpdateDebugText($"Request {pos} => Ignore by script");
                }
                actionDone?.Invoke();
                return;
            }
            
            if (!Ins.adsCoreIns)
            {
                return;
            }

			// if (IAP_RemoveAds)
			// {
			//     actionDone?.Invoke();
			//     return;
			// }

			FirebaseEvent.LogEvent(BgAdsConst.rw_show_ready, new Parameter(BgAdsConst.ads_pos, pos));
			if (!Ins.AdsCoreIns.IsFAReady())
			{
				FirebaseEvent.LogEvent(BgAdsConst.rw_show_notready, new Parameter(BgAdsConst.ads_pos, pos));
			}

			Ins.rwAction = actionDone;

            LastRewardedPos = pos;
            Ins.adsCoreIns.ShowRewarded();
        }

        public static bool IsRewardedReady()
        {
            if (!Ins || !Ins.adsCoreIns)
            {
                return false;
            }

            // if (IAP_RemoveAds)
            // {
            //     return true;
            // }

            return Ins.adsCoreIns.IsRewardedReady();
        }
        #endregion

        #region Banner
        public static bool IsActiveCollapBanner
        {
            get => !IAP_RemoveAds && Ins.MaxVsAdmobFloorVsCollapIns.IsCollapBanner;
            set
            {
                if (IAP_RemoveAds) return;
                Ins.MaxVsAdmobFloorVsCollapIns.IsCollapBanner = value;
            }
        }

        public static void ShowBanner()
        {
            if (!Ins.adsCoreIns)
            {
                return;
            }

            if (IAP_RemoveAds || !Ins.adsConfig.Banner.IsShow) return;
            Ins.adsCoreIns.ShowBanner();
        }

        public static void HideBanner()
        {
            if (!Ins.adsCoreIns)
            {
                return;
            }

            if (IAP_RemoveAds || !Ins.adsConfig.Banner.IsShow) return;
            Ins.adsCoreIns.HiddenBanner();
        }
        #endregion

        #region OpenApp
        public static bool IsOpenAppReady()
        {
            if (!Ins.adsCoreIns)
            {
                return false;
            }

            return !IAP_RemoveAds && Ins.adsCoreIns.IsOpenAppReady();
        }
        #endregion

        #region Native
        public static void ShowNormalNative(NativeUIManager ui)
        {
            if (!Ins.adsCoreIns)
            {
                return;
            }

            if (IAP_RemoveAds || !Ins.adsConfig.IsEnableNative) return;
            Ins.NativeNormalManagerIns.ShowNative(ui);
        }

        public static void FinishNormalNative()
        {
            if (!Ins.adsCoreIns)
            {
                return;
            }

            if (IAP_RemoveAds || !Ins.adsConfig.IsEnableNative) return;
            Ins.NativeNormalManagerIns.FinishNative();
        }

        public static void ShowBannerNative(NativeUIManager ui)
        {
            if (!Ins.adsCoreIns)
            {
                return;
            }

            if (IAP_RemoveAds || !Ins.adsConfig.IsEnableNative) return;
            Ins.NativeBannerManagerIns.ShowNative(ui);
        }

        public static void FinishBannerNative()
        {
            if (!Ins.adsCoreIns)
            {
                return;
            }

            if (IAP_RemoveAds || !Ins.adsConfig.IsEnableNative) return;
            Ins.NativeBannerManagerIns.FinishNative();
        }
        #endregion

        #region Struct
        [Serializable]
        public class StructAdsConfig
        {
            [SerializeField] BannerInfor banner = new BannerInfor();
            [SerializeField] AppOpenInfor openApp = new AppOpenInfor();

            [SerializeField] private bool isEnableInterstitial;
            [SerializeField] private float minTimeInterstitialShow;
            [SerializeField] private float decreaseInterTPI;
            [SerializeField] InterstitialInfor[] interstitial = new InterstitialInfor[0];
            [SerializeField] BreakAdsFA breakAd;

            public bool isEnableNative;

            public BannerInfor Banner => banner;
            public AppOpenInfor OpenApp => openApp;

            public bool IsEnableInterstitial => isEnableInterstitial;
            public float MinTimeInterstitialShow => minTimeInterstitialShow;
            public float DecreaseInterTPI => decreaseInterTPI;

            public InterstitialInfor[] Interstitial
            {
                get => interstitial;
                set => interstitial = value;
            }

            public BreakAdsFA BreakAd => breakAd;

            public bool IsEnableNative => isEnableNative;

            [Serializable]
            public class InterstitialInfor
            {
                [SerializeField] string pos;
                [SerializeField] bool isShow;
                [SerializeField] float timeShow;

                public string Pos => pos;
                public bool IsShow => isShow;
                public float TimeShow => timeShow;

                public InterstitialInfor(string _pos)
                {
                    pos = _pos;
                    isShow = true;
                    timeShow = 10;
                }

                public InterstitialInfor(string _pos, bool _isShow, float _timeShow)
                {
					pos = _pos;
                    isShow = _isShow;
                    timeShow = _timeShow;
				}
			}

            [Serializable]
            public class BreakAdsFA
            {
				[SerializeField] bool isUse;
				[SerializeField] public int notiBeforeBreakAd;
				[SerializeField] public int timeShow;

                public bool IsUse => isUse;
                public int NotiBeforeBreakAd => notiBeforeBreakAd;
                public int TimeShow => timeShow;
			}

			[Serializable]
            public class BannerInfor
            {
                [SerializeField] bool isShow;
                [SerializeField] bool isAutoShow;
                [SerializeField] BannerPosition pos;

                public bool IsShow => isShow;
                public bool IsAutoShow => isAutoShow;
                public BannerPosition Pos => pos;

                public BannerInfor()
                {
                    isShow = false;
                    isAutoShow = false;
                    pos = BannerPosition.BottomCenter;
                }

                public enum BannerPosition
                {
                    TopCenter,
                    BottomCenter
                }
            }

            [Serializable]
            public class AppOpenInfor
            {
                [SerializeField] bool isUseAllAdmob;
                [SerializeField] bool isShowWhenResume;
                [SerializeField] bool isShowAfterFullScreenAds;
                [SerializeField] bool isShowAfterFullScreenAds2;

                public bool IsUseAllAdmob => isUseAllAdmob;
                public bool IsShowWhenResume => isShowWhenResume;
                public bool IsShowAfterFullScreenAds => isShowAfterFullScreenAds;
                public bool IsShowAfterFullScreenAds2 => isShowAfterFullScreenAds2;

                public AppOpenInfor()
                {
                    isUseAllAdmob = false;
                    isShowWhenResume = false;
                    isShowAfterFullScreenAds = false;
                    isShowAfterFullScreenAds2 = false;
                }
            }
        }

        [Serializable]
        public class NetworkInfor
        {
            [SerializeField] int typeMediation;
            [SerializeField] MaxMediationInfor maxInforIns = new MaxMediationInfor();
            [SerializeField] AdmobInfor admobInforIns = new AdmobInfor();
            [SerializeField] NativeInfor nativeInforsIns = new NativeInfor();
            [SerializeField] NativeInfor nativeBannerInforsIns = new NativeInfor();

            public int TypeMediation => typeMediation;
            public MaxMediationInfor MaxInforIns => maxInforIns;
            public AdmobInfor AdmobInforIns => admobInforIns;
            public NativeInfor NativeInforsIns => nativeInforsIns;
            public NativeInfor NativeBannerInforsIns => nativeBannerInforsIns;
        }

        [Serializable]
        public class MaxMediationInfor
        {
            [SerializeField] string maxSDKKey;
            [SerializeField] string idOA;
            [SerializeField] string idFA;
            [SerializeField] string idRW;
            [SerializeField] string idBN;

            public string MaxSDKKey => maxSDKKey;
            public string IdOA => idOA;
            public string IdFA => idFA;
            public string IdRW => idRW;
            public string IdBN => idBN;
        }

        [Serializable]
        public class AdmobInfor
        {
            [SerializeField] AdmobStructID[] listInterstitialIds = new AdmobStructID[0];
            [SerializeField] AdmobStructID[] listRewardIds = new AdmobStructID[0];

            [SerializeField] bool isUseCollapFloor; // Co su dung load cac tang cua collap ko
            [SerializeField] string bannerCollapIdHi; // Id banner, use for Collap banner
            [SerializeField] string bannerCollapIdMe; // Id banner, use for Collap banner
            [SerializeField] string bannerCollapId; // Id banner, use for Collap banner
            [SerializeField] int refreshCollapTime;

            [SerializeField] bool isPercentageAddedValue;
            [SerializeField] float addedValue;
            [SerializeField] int maxJump;

            [SerializeField] bool enableInterstitialFloor;
            [SerializeField] bool enableRewardFloor;

            public AdmobStructID[] ListInterstitialIds => listInterstitialIds;
            public AdmobStructID[] ListRewardIds => listRewardIds;

            public bool IsUseCollapFloor => isUseCollapFloor;
            public string BannerCollapIdHi => bannerCollapIdHi;
            public string BannerCollapIdMe => bannerCollapIdMe;
            public string BannerCollapId => bannerCollapId;
            public int RefreshCollapTime => refreshCollapTime;

            public bool IsPercentageAddedValue => isPercentageAddedValue; // Su dung them % ecpm hay + ecpm
            public float AddedValue => addedValue; // Gia tri duoc them
            public int MaxJump => maxJump; // Buoc nhay

            public bool EnableInterstitialFloor => enableInterstitialFloor;
            public bool EnableRewardFloor => enableRewardFloor;
        }

        [Serializable]
        public class NativeInfor
        {
            [SerializeField] private bool isEnable;
            [SerializeField] private float timeDestroy;
            [SerializeField] private string hiID;
            [SerializeField] private string meID;
            [SerializeField] private string allID;

            public bool IsEnable
            {
                get => isEnable;
                set => isEnable = value;
            }

            public float TimeDestroy
            {
                get => timeDestroy;
                set => timeDestroy = value;
            }

            public string HiID
            {
                get => hiID;
                set => hiID = value;
            }

            public string MeID
            {
                get => meID;
                set => meID = value;
            }

            public string AllID
            {
                get => allID;
                set => allID = value;
            }
        }

        [Serializable]
        public class AdmobStructID
        {
            [SerializeField] private string id;
            [SerializeField] private float value;

            public string Id
            {
                get => id;
                set => id = value;
            }

            public float Value
            {
                get => value;
                set => this.value = value;
            }
        }

        #endregion

        [BoxGroup("EDITOR"), SerializeField] bool debuggable;

#if UNITY_EDITOR
        [BoxGroup("EDITOR"), SerializeField] string st;
        [BoxGroup("EDITOR"), Button(nameof(CreateAdsConfigForm))]
        void CreateAdsConfigForm()
        {
            var ins = new StructAdsConfig();
            ins.Interstitial = new StructAdsConfig.InterstitialInfor[]
            {
                new StructAdsConfig.InterstitialInfor("temp")
            };
            st = JsonTool.SerializeObject(ins);
        }

        [BoxGroup("EDITOR"), Button(nameof(CreateDataNetworkForm))]
        void CreateDataNetworkForm()
        {
            var ins = new NetworkInfor();
            st = JsonTool.SerializeObject(ins);
        }
#endif
    }
}