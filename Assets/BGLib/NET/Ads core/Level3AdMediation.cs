using System;
using System.Collections;
using BG_Library.DEBUG;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BG_Library.NET.Ads_core
{
	/// <summary>
	/// Max Mediation + Admob (Compare eCPM + Native overlay) + Collap banner
	/// </summary>
	public class Level3AdMediation : Level2AdMediation
	{
		[BoxGroup("COLLAP"), SerializeField, ReadOnly] bool isUseCollapFloor;
		[BoxGroup("COLLAP"), SerializeField, ReadOnly] string bannerIdHi;
		[BoxGroup("COLLAP"), SerializeField, ReadOnly] string bannerIdMe;
		[BoxGroup("COLLAP"), SerializeField, ReadOnly] string bannerIdAll;
		[BoxGroup("COLLAP"), SerializeField, ReadOnly] int refreshTime;

		[BoxGroup("COLLAP"), SerializeField, ReadOnly, Space(5)] bool isBannerShow;
		[BoxGroup("COLLAP"), SerializeField, ReadOnly] float timer;
		[BoxGroup("COLLAP"), SerializeField, ReadOnly] public bool IsCollapBanner;

		private int _currentRequestOrder;
		private string _currentId;

		// Reload when load fail
		private bool _isWaitingReloadAd;
		private float _reloadTimer;

		protected override void Update()
		{
			base.Update();

			if (_isWaitingReloadAd)
			{
				_reloadTimer += Time.unscaledDeltaTime;
				if (_reloadTimer >= 20)
				{
					_reloadTimer = 0;
					_isWaitingReloadAd = false;
					RequestBanner();
				}
			}

			if (!isBannerShow)
			{
				return;
			}

			timer += Time.unscaledDeltaTime;
			if (timer > refreshTime) // Refresh collap
			{
				if (debuggable) Debug.Log("Refresh banner collap");

				ResetState();
				RequestBanner();
				timer = 0;
			}
		}

		public override void Init(AdsManager.NetworkInfor netInfor)
		{
			maxSDK = netInfor.MaxInforIns.MaxSDKKey;
			idFAMax = netInfor.MaxInforIns.IdFA;
			idRWMax = netInfor.MaxInforIns.IdRW;
			idOpenAds = netInfor.MaxInforIns.IdOA;

			isIgnoreBlockAOAfterAds = AdsManager.Ins.AdsConfig.OpenApp.IsShowAfterFullScreenAds;
			isIgnoreBlockAOAfterAO = AdsManager.Ins.AdsConfig.OpenApp.IsShowAfterFullScreenAds2;

			MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
			{
				isInitMaxComplete = true;

				InitOpenAds();
				InitializeInterstitialAds();
				InitializeRewardedAds();

				//ManagerExistingPrivacySettings();
			};

			MaxSdk.SetSdkKey(maxSDK);
			MaxSdk.InitializeSdk();

			SetupAdmobId(netInfor);

			isUseCollapFloor = netInfor.AdmobInforIns.IsUseCollapFloor;
			bannerIdHi = netInfor.AdmobInforIns.BannerCollapIdHi;
			bannerIdMe = netInfor.AdmobInforIns.BannerCollapIdMe;
			bannerIdAll = netInfor.AdmobInforIns.BannerCollapId;

			refreshTime = netInfor.AdmobInforIns.RefreshCollapTime;

			if (AdsManager.Ins.AdsConfig.Banner.IsShow
				&& AdsManager.Ins.AdsConfig.Banner.IsAutoShow
				&& !AdsManager.Ins.NetInfor.NativeBannerInforsIns.IsEnable
				&& !AdsManager.IAP_RemoveAds)
			{
				IsCollapBanner = NetConfigsSO.Ins.isUseCollapByDefault;
				StartCoroutine(WaitToShowCollapBanner());
			}
		}

		IEnumerator WaitToShowCollapBanner()
		{
			yield return new WaitUntil(()=> OpenAppAdAdmob.Ins.isInitAdmobComplete);
			RequestBanner();
		}

		#region BANNER
		protected BannerView bannerView;

		public override void ShowBanner()
		{
			if (bannerView != null)
			{
				isBannerShow = true;
				bannerView.Show();
				
				if (debuggable) Debug.Log("Banner Collap Show Start.");
				if (DebugManager.IsDebug)
				{
					DebugManager.Ins.banner.Show(DebugBannerManager.FORMAT.CO_ADMOB, $"Level eCPM: {_currentRequestOrder}");
				}
			}
			else
			{
				RequestBanner();
			}
		}

		public override void HiddenBanner()
		{
			if (bannerView != null)
			{
				isBannerShow = false;
				
				bannerView.Hide();
				if (DebugManager.IsDebug)
				{
					DebugManager.Ins.banner.Hide(DebugBannerManager.FORMAT.CO_ADMOB);
				}
			}
		}

		public override void DestroyBanner()
		{
			if (bannerView != null)
			{
				isBannerShow = false;

				bannerView.Destroy();
				bannerView = null;
			}
		}

		private void RequestBanner()
		{
			if (!AdsManager.Ins.AdsConfig.Banner.IsShow)
			{
				return;
			}

			_isWaitingReloadAd = false;
			_reloadTimer = 0;

			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			DestroyBanner();

			UpdateId();
			if (debuggable) Debug.Log($"Banner Collap {_currentRequestOrder} Request starting {_currentId}...");

			var adRequest = new AdRequest();
			AdSize adaptiveSize =
				AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
				
			// Create an extra parameter that aligns the bottom of the expanded ad to the
			// bottom of the bannerView.
			
			if (IsCollapBanner)
			{
				adRequest.Extras.Add("collapsible", "bottom");
				adRequest.Extras.Add("collapsible_request_id", Guid.NewGuid().ToString());
			}
			bannerView = new BannerView(_currentId, adaptiveSize, AdPosition.Bottom);

			bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
			bannerView.OnBannerAdLoadFailed += OnBannerAdFailedToLoad;
			bannerView.OnAdFullScreenContentOpened += OnBannerAdOpened;
			bannerView.OnAdFullScreenContentClosed += OnBannerAdClosed;

			bannerView.OnAdPaid += OnBannerAdPaid;

			bannerView.LoadAd(adRequest);

			if (DebugManager.IsDebug)
			{
				AdsManager.AdsEnqueueCallback(() =>
				{
					DebugManager.Ins.banner.Load(DebugBannerManager.FORMAT.CO_ADMOB
						, $"Level eCPM: {_currentRequestOrder}", $"ID: {_currentId}");
				});
			}
		}

		private void UpdateId()
		{
			if (!isUseCollapFloor)
			{
				_currentId = bannerIdAll;
				return;
			}

			switch (_currentRequestOrder)
			{
				case 0:
					_currentId = bannerIdHi;
					break;
				case 1:
					_currentId = bannerIdMe;
					break;
				case 2:
					_currentId = bannerIdAll;
					break;
				default:
					_currentId = "";
					break;
			}
		}

		private void OnBannerAdLoaded()
		{
			isBannerShow = true;
			bannerRetryAttempt = 0;

			if (debuggable) Debug.Log($"Banner Collap {_currentRequestOrder} Loaded Success {_currentId}");
			if (DebugManager.IsDebug)
			{
				AdsManager.AdsEnqueueCallback(() =>
				{
					DebugManager.Ins.banner.Show(DebugBannerManager.FORMAT.CO_ADMOB, $"Level eCPM: {_currentRequestOrder}");
				});
			}

			_currentRequestOrder = 0;
		}

		private void OnBannerAdFailedToLoad(LoadAdError adError)
		{
			if (debuggable) Debug.LogError($"Banner Collap {_currentRequestOrder} OnAdFailedToLoad {_currentId}");

			if (!isUseCollapFloor)
			{
				_isWaitingReloadAd = true;
				return;
			}

			isBannerShow = false;
			_currentRequestOrder += 1;
			if (_currentRequestOrder < 3) // Giam eCPM de load lai
			{
				RequestBanner();
			}
			else // Khi load fail het se doi de load lai het
			{
				if (DebugManager.IsDebug)
				{
					var temp = _currentRequestOrder;
					AdsManager.AdsEnqueueCallback(() =>
					{
						DebugManager.Ins.banner.LoadFail(DebugBannerManager.FORMAT.CO_ADMOB
							, $"Level eCPM: {temp}", $"ID: {_currentId}");
					});
				}

				_currentRequestOrder = 0;

				if (debuggable) Debug.Log($"Banner Collap Delay reload 20s");
				_isWaitingReloadAd = true;
			}
		}

		private void OnBannerAdOpened()
		{
			if (debuggable) Debug.Log("Banner Collap OnAdOpened...");
		}

		private void OnBannerAdClosed()
		{
			if (debuggable) Debug.Log("Banner Collap OnAdClosed.");
		}

		void OnBannerAdPaid(AdValue adValue)
		{
			var rev = adValue.Value / 1000000f;
			FirebaseEvent.LogEventAdImpression(
				AdjustConst.admob_sdk,
				string.Empty,
				string.Empty,
				"BANNER",
				rev,
				adValue.CurrencyCode,
				"AdMob");

			if (debuggable) Debug.Log($"Banner Collap {_currentRequestOrder} Paid Value {_currentId} {rev}{adValue.CurrencyCode}");
			if (DebugManager.IsDebug)
			{
				AdsManager.AdsEnqueueCallback(() =>
				{
					DebugManager.Ins.banner.UpdateRev(DebugBannerManager.FORMAT.CO_ADMOB, rev, adValue.CurrencyCode);
				});
			}
		}

		void ResetState()
		{
			_currentRequestOrder = 0;
			_currentId = bannerIdHi;

			_isWaitingReloadAd = false;
			_reloadTimer = 0;
		}
		#endregion
	}
}