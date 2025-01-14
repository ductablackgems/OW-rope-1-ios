using System;
using BG_Library.DEBUG;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BG_Library.NET.Native_custom
{
    public class NativeManager : MonoBehaviour
    {
        [BoxGroup("Configs"), SerializeField, ReadOnly] protected string hiId = "";
        [BoxGroup("Configs"), SerializeField, ReadOnly] protected string meId = "";
        [BoxGroup("Configs"), SerializeField, ReadOnly] protected string allId = "";
        [BoxGroup("Configs"), SerializeField, ReadOnly] protected float timeDestroy;
        [BoxGroup("Configs"), SerializeField, ReadOnly] protected bool isEnableAd;

        [BoxGroup("Debug"), SerializeField, ReadOnly] protected bool isShowing;
        [BoxGroup("Debug"), SerializeField, ReadOnly] protected float refreshTimer;
        [BoxGroup("Debug"), SerializeField, ReadOnly] protected bool ableAdsButNotShow; // Ads da available nhung chua duoc hien thi len UI
        [BoxGroup("Debug"), SerializeField, ReadOnly] protected bool enableRefreshTimer; // Chi duoc bat len khi Native duoc show thanh cong

        [BoxGroup("Config"), SerializeField] protected bool debuggable;
        protected bool isBanner;

        private int _currentRequestOrder; // Stt ID dang load NA: Hi => Me => All
        private string _currentId; // ID hien tai

        private bool _isLoadingNativeAds; // Native co dang duoc load hay khong
        private bool _isLoadingAgainWhenFail; // Doi 1 thoi gian roi load lai NA khi bi loi ko load duoc
        private float _reloadTimer; // dem thoi gian delay de loai lai NA

        private NativeAd _nativeAd;
        private NativeUIManager _currentNativeUI;

        protected virtual void Update()
        {
            if (!isEnableAd)
            {
                return;
            }

            if (_isLoadingAgainWhenFail) // khi khong load duoc id nao => doi 30s roi load lai
            {
                _reloadTimer += Time.deltaTime;
                if (_reloadTimer >= 30)
                {
                    _reloadTimer = 0;
                    _isLoadingAgainWhenFail = false;
                    RequestNativeAd();
                }
            }

            if (!isShowing) // Neu dang ko show Native thi bo qua
            {
                return;
            }

            if (enableRefreshTimer) // Refresh lai NA sau 1 khoang thoi gian nhat dinh (chi xay ra khi NA dang show)
            {
                refreshTimer -= Time.deltaTime;
                if (refreshTimer <= 0)
                {
                    enableRefreshTimer = false;
                    RefreshNative();
                }
            }

            if (!ableAdsButNotShow) // Show lai Native khi load duoc
            {
                return;
            }

            refreshTimer = timeDestroy;
            enableRefreshTimer = true;

            ableAdsButNotShow = false;
            DisplayNativeOnScreen();

            if (debuggable) Debug.Log("Displayed Refresh Native");
        }

        public virtual void Init(AdsManager.NativeInfor infor)
        {
            hiId = infor.HiID;
            meId = infor.MeID;
            allId = infor.AllID;
            timeDestroy = infor.TimeDestroy;
            isEnableAd = infor.IsEnable;

            if (!isEnableAd)
            {
                return;
            }

            ResetState();
            RequestNativeAd();
        }

        /// <summary>
        /// Hien thi Native, neu Native chua ss => load lai
        /// </summary>
        public void ShowNative(NativeUIManager ui)
        {
            if (AdsManager.IAP_RemoveAds)
            {
                return;
            }

            if (CheckNative())
            {
                _currentNativeUI = ui;

                // Dang duoc show roi, khong dang ki UI bang Update nua
                isShowing = true;
                ableAdsButNotShow = false;

                // Bat dau dem gio de refresh ads
                refreshTimer = timeDestroy;
                enableRefreshTimer = true;

                DisplayNativeOnScreen();
                if (debuggable) Debug.Log("Displayed Native");

                if (DebugManager.IsDebug)
                {
                    AdsManager.AdsEnqueueCallback(() =>
                    {
                        if (isBanner)
                        {
                            DebugManager.Ins.banner.Show(DebugBannerManager.FORMAT.NA_ADMOB);
                            DebugManager.Ins.banner.SetCheckToggle(DebugBannerManager.FORMAT.NA_ADMOB, true);
                        }
                        else
                        {
                            DebugManager.Ins.native.Show(DebugNativeManager.FORMAT.NA_ADMOB);
                            DebugManager.Ins.native.SetCheckToggle(DebugNativeManager.FORMAT.NA_ADMOB, true);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Khong hien thi Native nua, sau do refresh load lai luon
        /// </summary>
        public void FinishNative()
        {
            if (!isShowing)
            {
                return;
            }

            ResetState(); // Reset lai toan bo state

            DestroyNative(); // Xoa NativeAd
            RequestNativeAd(); // Request lai 1 ads moi

            _currentNativeUI.Deactive(); // deactive UI
            _currentNativeUI = null;

            if (debuggable) Debug.Log("Finish Native");

            if (DebugManager.IsDebug)
            {
                AdsManager.AdsEnqueueCallback(() =>
                {
                    if (isBanner)
                    {
                        DebugManager.Ins.banner.Hide(DebugBannerManager.FORMAT.NA_ADMOB);
                        DebugManager.Ins.banner.SetCheckToggle(DebugBannerManager.FORMAT.NA_ADMOB, false);
                    }
                    else
                    {
                        DebugManager.Ins.native.Hide(DebugNativeManager.FORMAT.NA_ADMOB);
                        DebugManager.Ins.native.SetCheckToggle(DebugNativeManager.FORMAT.NA_ADMOB, false);
                    }
                });
            }
        }

        /// <summary>
        /// Load lai Native hien tai va van tiep tuc hien thi
        /// </summary>
        private void RefreshNative()
        {
            if (!isShowing)
            {
                return;
            }

            DestroyNative(); // Xoa native hien tai
            RequestNativeAd(); // Request native moi

            if (debuggable) Debug.Log("Refresh Native");
        }

        private void DestroyNative()
        {
            if (_nativeAd == null)
            {
                return;
            }

            _nativeAd.Destroy();
            _nativeAd = null;
            _currentNativeUI.SetDefaultWhenDestroy();
        }

        public bool CheckNative()
        {
            if (!isEnableAd) // Ko enable tren remote
            {
                return false;
            }

            if (_nativeAd != null) // Chua duoc load
            {
                return true;
            }

            RequestNativeAd();
            return false;
        }

        /// <summary>
        /// 0: Hi, 1: Me, 2: All
        /// </summary>
        private void RequestNativeAd()
        {
            if (_nativeAd != null || _isLoadingNativeAds)
            {
                return;
            }

            _isLoadingNativeAds = true;

            UpdateId();
            if (debuggable) Debug.Log($"Native start request {_currentRequestOrder}");

            var adLoader = new AdLoader.Builder(_currentId)
                .ForNativeAd()
                .Build();
            adLoader.OnNativeAdLoaded += AdLoader_OnNativeAdLoaded;
            adLoader.OnAdFailedToLoad += AdLoader_OnAdFailedToLoad;
            adLoader.LoadAd(new AdRequest());

            if (DebugManager.IsDebug)
            {
                var temp = _currentRequestOrder;
                AdsManager.AdsEnqueueCallback(() =>
                {
                    if (isBanner)
                    {
                        DebugManager.Ins.banner.Load(DebugBannerManager.FORMAT.NA_ADMOB
                            , $"Level eCPM: {temp}", $"ID: {_currentId}");
                    }
                    else
                    {
                        DebugManager.Ins.native.Load(DebugNativeManager.FORMAT.NA_ADMOB
                            , $"Level eCPM: {temp}", $"ID: {_currentId}");
                    }
                });
            }
        }

        private void UpdateId()
        {
            switch (_currentRequestOrder)
            {
                case 0:
                    _currentId = hiId;
                    break;
                case 1:
                    _currentId = meId;
                    break;
                case 2:
                    _currentId = allId;
                    break;
                default:
                    _currentId = "";
                    break;
            }
        }

        private void AdLoader_OnNativeAdLoaded(object sender, NativeAdEventArgs e)
        {
            if (debuggable) Debug.Log($"Native Loaded Success {_currentRequestOrder}");

            if (DebugManager.IsDebug)
            {
                var temp = _currentRequestOrder;
                AdsManager.AdsEnqueueCallback(() =>
                {
                    if (isBanner)
                    {
                        DebugManager.Ins.banner.Loaded(DebugBannerManager.FORMAT.NA_ADMOB
                            , $"Level eCPM: {temp}", $"ID: {_currentId}");
                    }
                    else
                    {
                        DebugManager.Ins.native.Loaded(DebugNativeManager.FORMAT.NA_ADMOB
                            , $"Level eCPM: {temp}", $"ID: {_currentId}");
                    }
                });
            }

            _isLoadingNativeAds = false;

            _currentRequestOrder = 0;
            _nativeAd = e.nativeAd;
            _nativeAd.OnPaidEvent += AdLoader_OnPaidEvent;

            ableAdsButNotShow = true;
        }

        private void AdLoader_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            if (debuggable) Debug.Log($"Native Loaded Fail {_currentRequestOrder}");
            
            _isLoadingNativeAds = false;

            if(_isLoadingAgainWhenFail)
            {
                return;
            }

            _currentRequestOrder += 1;
            if (_currentRequestOrder < 3) // Giam eCPM de load lai
            {
                RequestNativeAd();
            }
            else // Khi load fail het se doi de load lai het
            {
                if (DebugManager.IsDebug)
                {
                    var temp = _currentRequestOrder;
                    AdsManager.AdsEnqueueCallback(() =>
                    {
                        if (isBanner)
                        {
                            DebugManager.Ins.banner.LoadFail(DebugBannerManager.FORMAT.NA_ADMOB
                                , $"Level eCPM: {temp}", "Delay load 30s, " + e.LoadAdError.GetMessage());
                        }
                        else
                        {
                            DebugManager.Ins.native.LoadFail(DebugNativeManager.FORMAT.NA_ADMOB
                                , $"Level eCPM: {temp}", "Delay load 30s, " + e.LoadAdError.GetMessage());
                        }
                    });
                }

                ResetState();

                if (debuggable) Debug.Log($"Delay reload 30s");
                _isLoadingAgainWhenFail = true;

                LoadFailAllNA();
            }
        }

        private void AdLoader_OnPaidEvent(object sender, AdValueEventArgs e)
        {
            var rev = e.AdValue.Value / 1000000f;
            FirebaseEvent.LogEventAdImpression(
                AdjustConst.admob_sdk,
                string.Empty,
                string.Empty,
                "NATIVE",
                rev,
                e.AdValue.CurrencyCode,
                "AdMob");

                if (debuggable) Debug.Log(String.Format("Native Admob paid {0} {1}.",
                    rev, e.AdValue.CurrencyCode));
                if (DebugManager.IsDebug)
                {
                    AdsManager.AdsEnqueueCallback(()=>
                    {
                        if (isBanner)
                        {
                            DebugManager.Ins.banner.UpdateRev(DebugBannerManager.FORMAT.NA_ADMOB, rev, e.AdValue.CurrencyCode);
                        }
                        else
                        {
                            DebugManager.Ins.native.UpdateRev(DebugNativeManager.FORMAT.NA_ADMOB, rev, e.AdValue.CurrencyCode);
                        }
                    });
                }
        }

        private void ResetState()
        {
            isShowing = false;
            refreshTimer = 0;

            _reloadTimer = 0;
            _isLoadingAgainWhenFail = false;

            _currentRequestOrder = 0;
            _currentId = allId;

            _isLoadingNativeAds = false;

            ableAdsButNotShow = false;
            enableRefreshTimer = false;
        }

        #region Virtual Methob
        protected virtual void LoadFailAllNA()
        {

        }

        protected virtual void DisplayNativeOnScreen()
        {
            if (_currentNativeUI == null)
            {
                if (debuggable) Debug.Log("Null Native UI");
                return;
            }
            _currentNativeUI.Init(_nativeAd);
        }
        #endregion
    }
}