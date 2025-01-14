using BG_Library.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG_Library.NET
{
	public class BreakAdsInterstitial : MonoBehaviour
	{
		public static BreakAdsInterstitial Ins { get; private set; }

		public static System.Action OnStartCountdown;
		public static System.Action<int> OnBreakAdsFAComing;

		public static System.Action OnPause;

		/// <summary>
		/// Khi resume co dang chuan bi show Break ads khong, neu co thi con bao nhieu thoi gian nua
		/// </summary>
		public static System.Action<bool, float> OnResume;

		[SerializeField] bool isPause;
		[SerializeField] float timer;
		[SerializeField] float timeToNoti;
		[SerializeField] bool isCallNoti;

		Coroutine wait;

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

			RemoteConfig.OnFetchComplete += () =>
			{
				WaitToShow();
			};
		}

		public bool IsPause 
		{
			get => isPause;
			set
			{
				isPause = value;

				if (isPause)
				{
					OnPause?.Invoke();
				}
				else
				{
					if (isCallNoti)
					{
						OnResume?.Invoke(true, AdsManager.Ins.AdsConfig.BreakAd.TimeShow - timer);
					}
					else
					{
						OnResume?.Invoke(false, 0);
					}
				}
			}
		}

		public void WaitToShow()
		{
			if (AdsManager.IAP_RemoveAds || !AdsManager.Ins.AdsConfig.IsEnableInterstitial 
				|| !AdsManager.Ins.AdsConfig.BreakAd.IsUse)
			{
				return;
			}

			if (wait != null)
			{
				StopCoroutine(wait);
			}
			wait = StartCoroutine(BreakAds());
		}

		IEnumerator BreakAds()
		{
			OnStartCountdown?.Invoke();

			timer = 0f;
			timeToNoti = AdsManager.Ins.AdsConfig.BreakAd.timeShow - AdsManager.Ins.AdsConfig.BreakAd.notiBeforeBreakAd;
			isCallNoti = false;

			while (true)
			{
				yield return null;
				if (!IsPause)
				{
					timer += NetConfigsSO.Ins.breakAdsIgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
				}

				if (timer >= timeToNoti && !isCallNoti)
				{
					isCallNoti = true;
					OnBreakAdsFAComing?.Invoke(AdsManager.Ins.AdsConfig.BreakAd.notiBeforeBreakAd);
				}

				if (timer >= AdsManager.Ins.AdsConfig.BreakAd.timeShow)
				{
					timer = 0;
					isCallNoti = false;

					AdsManager.ShowInterstitial(BgAdsConst.break_ads);
					break;
				}
			}
		}
	}
}