using System.Collections.Generic;
using BG_Library.DEBUG;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BG_Library.NET.Ads_core
{
    /// <summary>
    /// Max Mediation + Admob (Compare eCPM + Native overlay)
    /// </summary>
    public class Level2AdMediation : Level1AdMediation
    {
        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected List<AdsManager.AdmobStructID> listInterstitialIds;
        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected List<AdsManager.AdmobStructID> listRewardIds;

        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected bool isPercentageAddedValue;
        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected float addedValue;

        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected int maxJump;
        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected double deltaEcpm; // Ecpm chenh lech toi thieu giua Max va Admob

        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected bool enableInterFloor;
        [BoxGroup("ADMOB"), SerializeField, ReadOnly] protected bool enableRewardFloor;

        public override void Init(AdsManager.NetworkInfor netInfor)
        {
            base.Init(netInfor);
            SetupAdmobId(netInfor);
        }

        protected void SetupAdmobId(AdsManager.NetworkInfor netInfor)
        {
            listInterstitialIds = new List<AdsManager.AdmobStructID>(netInfor.AdmobInforIns.ListInterstitialIds);
            listRewardIds = new List<AdsManager.AdmobStructID>(netInfor.AdmobInforIns.ListRewardIds);

            listInterstitialIds.Sort((st1, st2) => st1.Value.CompareTo(st2.Value));
            listRewardIds.Sort((st1, st2) => st1.Value.CompareTo(st2.Value));

            isPercentageAddedValue = netInfor.AdmobInforIns.IsPercentageAddedValue;
            addedValue = netInfor.AdmobInforIns.AddedValue;
            maxJump = netInfor.AdmobInforIns.MaxJump;

            enableInterFloor = netInfor.AdmobInforIns.EnableInterstitialFloor;
            enableRewardFloor = netInfor.AdmobInforIns.EnableRewardFloor;
        }

        #region Interstitial Ad Methods---------------------------------------------------------------------
        private double _maxInterEcpm; // Ecpm da load duoc hien tai cua Max

        private InterstitialAd _admobInterAd; // Ad dung de load admob

        private List<AdsManager.AdmobStructID> _admobInterListIds; // list id admob
        private int _admobInterOrder; // Stt ID dang load trong list

        private double _admobInterEcpm; // Ecpm da load duoc hien tai cua admob
        private bool _admobInterIsLoading;

        private bool AdmobInterIsReady => _admobInterAd != null && _admobInterAd.CanShowAd();

        public override void ShowForceAds()
        {
            if (enableInterFloor && AdmobInterIsReady)
            {
                if (_admobInterEcpm >= _maxInterEcpm + deltaEcpm)
                {
					BreakAdsInterstitial.Ins.WaitToShow();

					isBlockAppOpenAfterAds = true;
					_admobInterAd.Show();
				}
                else
                {
                    ShowMaxFA();
                }

                return;
            }
            ShowMaxFA();

            void ShowMaxFA()
            {
                if (!MaxSdk.IsInterstitialReady(idFAMax))
                {
                    return;
                }
                MaxSdk.ShowInterstitial(idFAMax);
            }
        }

        public override bool IsFAReady()
        {
            if (enableInterFloor && AdmobInterIsReady)
            {
                if (_admobInterEcpm >= _maxInterEcpm + deltaEcpm)
                {
                    return true;
                }
            }

            if (base.IsFAReady()) return true;

            return false;
        }

        protected override void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnInterstitialLoadedEvent(adUnitId, adInfo);

            if (!enableInterFloor)
            {
                return;
            }

            _maxInterEcpm = adInfo.Revenue * 1000;
            deltaEcpm = isPercentageAddedValue ? _maxInterEcpm * addedValue / 100 : addedValue;

            if (debuggable) Debug.Log($"Loaded Inter Max Ecpm: {_maxInterEcpm}");
            SetupListInterAdmobIds(); // Max load xong => load admob
            Admob_LoadInterstitialAd();
        }

        protected override void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _maxInterEcpm = 0;
            base.OnInterstitialDismissedEvent(adUnitId, adInfo);
        }

        private void SetupListInterAdmobIds()
        {
            var minAdmobEcpm = _maxInterEcpm + deltaEcpm;

            _admobInterListIds = EcpmSearch(listInterstitialIds, (float)minAdmobEcpm, maxJump);
            _admobInterOrder = _admobInterListIds.Count - 1;

            if (debuggable)
            {
                for (int i = 0; i < _admobInterListIds.Count; i++)
                {
                    Debug.Log("Searching Admob Id: " + _admobInterListIds[i]);
                }
            }
        }

        public void Admob_LoadInterstitialAd()
        {
            if (!OpenAppAdAdmob.Ins.isInitAdmobComplete)
            {
                return;
            }

            // Neu da co admob tot roi thi ko can load nua
            if (AdmobInterIsReady && (_admobInterEcpm >= _maxInterEcpm + deltaEcpm))
            {
                return;
            }

            if (_admobInterOrder < 0 || _admobInterIsLoading) // Khong con ID trong list nua
            {
                return;
            }
            _admobInterIsLoading = true;

            // Clean up the old ad before loading a new one.
            if (_admobInterAd != null)
            {
                _admobInterAd.Destroy();
                _admobInterAd = null;
            }

            var _id = _admobInterListIds[_admobInterOrder].Id;
            var _value = _admobInterListIds[_admobInterOrder].Value;

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            // send the request to load the ad.
            InterstitialAd.Load(_id, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    _admobInterIsLoading = false;

                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        if (debuggable) Debug.LogError($"Interstitial admob failed to load an ad. Ecpm {_value}, Order {_admobInterOrder}\nwith error : {error}");
                        if (DebugManager.IsDebug)
                        {
                            AdsManager.AdsEnqueueCallback(() =>
                            {
                                DebugManager.Ins.interstitial.LoadFail(DebugFullScreenAdsManager.FORMAT.ADMOB
                                    , $"Floor eCPM: {_value}", $"Load fail with ERROR:\n {error}");
                            });
                        }

                        _admobInterOrder -= 1;
                        Admob_LoadInterstitialAd();

                        return;
                    }

                    if (debuggable) Debug.Log($"Interstitial admob loaded. Ecpm {_value}, Order {_admobInterOrder} \nwith response: {ad.GetResponseInfo()}");
                    if (DebugManager.IsDebug)
                    {
                        AdsManager.AdsEnqueueCallback(() =>
                        {
                            DebugManager.Ins.interstitial.Loaded(DebugFullScreenAdsManager.FORMAT.ADMOB
                                , $"Floor eCPM: {_value}", ad.GetResponseInfo().ToString());
                        });
                    }

                    _admobInterEcpm = _value;
                    _admobInterAd = ad;
                    Admob_RegisterInterEventHandlers(_admobInterAd);
                });

            if (debuggable) Debug.Log($"Loading the interstitial admob. Ecpm {_value}, Order {_admobInterOrder}");
            if (DebugManager.IsDebug)
            {
                AdsManager.AdsEnqueueCallback(() =>
                {
                    DebugManager.Ins.interstitial.Load(DebugFullScreenAdsManager.FORMAT.ADMOB, $"Floor eCPM: {_value}, Order: {_admobInterOrder}");
                });
            }
        }

        void Admob_RegisterInterEventHandlers(InterstitialAd interstitialAd)
        {
            // Raised when the ad is estimated to have earned money.
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
				AdsManager.AdsEnqueueCallback(() =>
				{
					AdsManager.SetInterstitialCount();
					var rev = adValue.Value / 1000000f;
					FirebaseEvent.LogEventAdImpressionPos(
						AdjustConst.admob_sdk,
						string.Empty,
						string.Empty,
						"INTER",
						rev,
						adValue.CurrencyCode,
						"AdMob",
						AdsManager.LastInterstitialPos,
						AdsManager.GetInterstitialCount);

					if (DebugManager.IsDebug)
					{
						DebugManager.Ins.interstitial.UpdateRev(DebugFullScreenAdsManager.FORMAT.ADMOB, rev, adValue.CurrencyCode);
					}
				});
            };
            // Raised when a click is recorded for an ad.
            interstitialAd.OnAdClicked += () =>
            {
                if (debuggable) Debug.Log("Interstitial admob was clicked.");
            };
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
				FirebaseEvent.LogEvent(BgAdsConst.fa_show, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));
			};
            interstitialAd.OnAdClicked += () =>
            {
				FirebaseEvent.LogEvent(BgAdsConst.fa_click, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));
			};
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
				if (debuggable) Debug.Log("Interstitial admob full screen content closed.");
				FirebaseEvent.LogEvent(BgAdsConst.fa_finish, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));

				AdsManager.Ins.lastTime = AdsManager.PlayTime;
				SetTimerBlockActive();

                _admobInterEcpm = 0;

                SetupListInterAdmobIds();
                Admob_LoadInterstitialAd();

				if (isIgnoreBlockAOAfterAds)
                {
                    DebugManager.Ins.appOpen.UpdateDebugText($"Show AO Max after FA admob"); 
                    MaxSdk.ShowAppOpenAd(idOpenAds);
                }
            };
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
				FirebaseEvent.LogEvent(BgAdsConst.fa_failed, new Parameter(BgAdsConst.ads_pos, AdsManager.LastInterstitialPos));

				SetupListInterAdmobIds();
                Admob_LoadInterstitialAd();

                if (debuggable) Debug.LogError("Interstitial admob failed to open full screen content " +
                    "with error : " + error);
                if (DebugManager.IsDebug)
                {
                    AdsManager.AdsEnqueueCallback(() =>
                    {
                        DebugManager.Ins.interstitial.DisplayFail(DebugFullScreenAdsManager.FORMAT.ADMOB, "", $"Display fail with error: {error}");
                    });
                }
            };
        }
        #endregion Interstitial Ad Methods---------------------------------------------------------------------

        #region Rewarded Ad Methods----------------------------------------------------------------
        private double _maxRewardEcpm; // Ecpm da load duoc hien tai cua Max

        private RewardedAd _admobRewardAd; // Ad dung de load admob

        private List<AdsManager.AdmobStructID> admobReward_ListIds; // list id admob
        private int _admobRewardOrder; // Stt ID dang load trong list

        private double _admobRewardEcpm; // Ecpm da load duoc hien tai cua admob
        private bool _admobRewardIsLoading;

        private bool AdmobRewardIsReady => _admobRewardAd != null && _admobRewardAd.CanShowAd();
    
        public override void ShowRewarded()
        {
            if (enableRewardFloor && AdmobRewardIsReady)
            {
                if (_admobRewardEcpm >= _maxRewardEcpm + deltaEcpm)
                {
					BreakAdsInterstitial.Ins.WaitToShow();

					isBlockAppOpenAfterAds = true;
					_admobRewardAd.Show(rw =>
                    {
                        AdsManager.Ins.isGetReward = true;
                    });
                }
                else
                {
                    ShowMaxRewarded();
                }
                
                return;
            }
            ShowMaxRewarded();

            void ShowMaxRewarded()
            {
                if (!MaxSdk.IsRewardedAdReady(idRWMax))
                {
                    return;
                }
                
                MaxSdk.ShowRewardedAd(idRWMax);
            }
        }
        
        public override bool IsRewardedReady()
        {
            return base.IsRewardedReady() || (enableRewardFloor && AdmobRewardIsReady);
        }

        protected override void OnRewardedLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnRewardedLoadedEvent(adUnitId, adInfo);

            if (!enableRewardFloor)
            {
                return;
            }

            _maxRewardEcpm = adInfo.Revenue * 1000;
            deltaEcpm = isPercentageAddedValue ? _maxRewardEcpm * addedValue / 100 : addedValue;

            if (debuggable) Debug.Log($"Loaded Reward Admob Ecpm: {_maxRewardEcpm}");
            SetupListRewardAdmobIds(); // Max load xong => load admob
            Admob_LoadRewardAd();
        }

        protected override void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _maxRewardEcpm = 0;
            base.OnRewardedAdDismissedEvent(adUnitId, adInfo);
        }

        void SetupListRewardAdmobIds()
        {
            double minAdmobEcpm = _maxRewardEcpm + deltaEcpm;

            admobReward_ListIds = EcpmSearch(listRewardIds, (float)minAdmobEcpm, maxJump);
            _admobRewardOrder = admobReward_ListIds.Count - 1;
        }

        public void Admob_LoadRewardAd()
        {
            if (!OpenAppAdAdmob.Ins.isInitAdmobComplete)
            {
                return;
            }

            // Neu da co admob tot roi thi ko can load nua
            if (AdmobRewardIsReady && _admobRewardEcpm >= _maxRewardEcpm + deltaEcpm)
            {
                return;
            }

            if (_admobRewardOrder < 0 || _admobRewardIsLoading) // Khong con ID trong list nua
            {
                return;
            }
            _admobRewardIsLoading = true;

            // Clean up the old ad before loading a new one.
            if (_admobRewardAd != null)
            {
                _admobRewardAd.Destroy();
                _admobRewardAd = null;
            }

            var _id = admobReward_ListIds[_admobRewardOrder].Id;
            var _value = admobReward_ListIds[_admobRewardOrder].Value;
        
            // create our request used to load the ad.
            var adRequest = new AdRequest();
            // send the request to load the ad.
            RewardedAd.Load(_id, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    _admobRewardIsLoading = false;

                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        if (debuggable) Debug.LogError($"Reward admob failed to load an ad. Ecpm {_value}, Order {_admobRewardOrder}\nwith error : {error}");
                        if (DebugManager.IsDebug)
                        {
                            AdsManager.AdsEnqueueCallback(() =>
                            {
                                DebugManager.Ins.reward.LoadFail(DebugFullScreenAdsManager.FORMAT.ADMOB
                                    , $"Floor eCPM: {_value}", $"Load fail with ERROR:\n {error}");
                            });
                        }

                        _admobRewardOrder -= 1;
                        Admob_LoadRewardAd();

                        return;
                    }

                    if (debuggable) Debug.Log($"Reward admob loaded. Ecpm {_value}, Order {_admobRewardOrder} \nwith response: {ad.GetResponseInfo()}");
                    if (DebugManager.IsDebug)
                    {
                        AdsManager.AdsEnqueueCallback(() =>
                        {
                            DebugManager.Ins.reward.Loaded(DebugFullScreenAdsManager.FORMAT.ADMOB
                                , $"Floor eCPM: {_value}", ad.GetResponseInfo().ToString());
                        });
                    }

                    _admobRewardEcpm = _value;
                    _admobRewardAd = ad;
                    Admob_RegisterRewardEventHandlers(_admobRewardAd);
                });

            if (debuggable) Debug.Log($"Loading the reward admob. eCPM {_value}, Order {_admobRewardOrder}");
            if (DebugManager.IsDebug)
            {
                AdsManager.AdsEnqueueCallback(() =>
                {
                    DebugManager.Ins.reward.Load(DebugFullScreenAdsManager.FORMAT.ADMOB, $"Floor eCPM: {_value}, Order: {_admobRewardOrder}");
                });
            }
        }

        void Admob_RegisterRewardEventHandlers(RewardedAd rewardedAd)
        {
            // Raised when the ad is estimated to have earned money.
            rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
				AdsManager.AdsEnqueueCallback(() =>
				{
					AdsManager.SetRewardCount();
					var rev = adValue.Value / 1000000f;
					FirebaseEvent.LogEventAdImpressionPos(
						AdjustConst.admob_sdk,
						string.Empty,
						string.Empty,
						"REWARDED",
						rev,
						adValue.CurrencyCode,
						"Admob",
						AdsManager.LastRewardedPos,
                        AdsManager.GetRewardCount);

					if (DebugManager.IsDebug)
					{
						DebugManager.Ins.reward.UpdateRev(DebugFullScreenAdsManager.FORMAT.ADMOB, rev, adValue.CurrencyCode);
					}
				});
            };
            // Raised when a click is recorded for an ad.
            rewardedAd.OnAdClicked += () =>
            {
				FirebaseEvent.LogEvent(BgAdsConst.rw_click, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));
			};
            rewardedAd.OnAdFullScreenContentOpened += () =>
            {
				FirebaseEvent.LogEvent(BgAdsConst.rw_show, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));
			};
            // Raised when the ad closed full screen content.
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
				if (debuggable) Debug.Log("Rewarded admob full screen content closed.");
				FirebaseEvent.LogEvent(BgAdsConst.rw_finish, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));

				AdsManager.Ins.lastTime = AdsManager.PlayTime;
				SetTimerBlockActive();

                _admobRewardEcpm = 0;

                SetupListRewardAdmobIds();
                Admob_LoadRewardAd();

				if (isIgnoreBlockAOAfterAds)
                {
                    DebugManager.Ins.appOpen.UpdateDebugText($"Show AO Max after RW admob"); 
                    MaxSdk.ShowAppOpenAd(idOpenAds);
                }
            };
            // Raised when the ad failed to open full screen content.
            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
				FirebaseEvent.LogEvent(BgAdsConst.rw_failed, new Parameter(BgAdsConst.ads_pos, AdsManager.LastRewardedPos));

				SetupListRewardAdmobIds();
                Admob_LoadRewardAd();

                if (debuggable) Debug.LogError("Rewarded admob failed to open full screen content " +
                    "with error : " + error);
                if (DebugManager.IsDebug)
                {
                    AdsManager.AdsEnqueueCallback(() =>
                    {
                        DebugManager.Ins.reward.DisplayFail(DebugFullScreenAdsManager.FORMAT.ADMOB, "", $"Display fail with error: {error}");
                    });
                }
            };
        }
        #endregion Rewarded Ad Methods----------------------------------------------------------------

        // Tim ra nhung ID co eCPM lon hon eCPM dua vao
        static List<AdsManager.AdmobStructID> EcpmSearch(List<AdsManager.AdmobStructID> list
            , float ecpm, int jump)
        {
            int left = 0;
            int right = list.Count - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (list[mid].Value >= ecpm)
                {
                    if (mid == 0 || list[mid - 1].Value < ecpm)
                    {
                        int rightIndex = System.Math.Min(mid + jump, list.Count);
                        return list.GetRange(mid, rightIndex - mid);
                    }
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return new List<AdsManager.AdmobStructID>();
        }
    }
}