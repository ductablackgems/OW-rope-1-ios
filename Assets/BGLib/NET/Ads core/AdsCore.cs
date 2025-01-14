using UnityEngine;
using UnityEngine.Serialization;

namespace BG_Library.NET.Ads_core
{
	public abstract class AdsCore : MonoBehaviour
	{
		[SerializeField] protected bool debuggable;
		public abstract void Init(AdsManager.NetworkInfor netStats);

		public abstract void ShowForceAds();
		public abstract bool IsFAReady();

		public abstract void ShowRewarded();
		public abstract bool IsRewardedReady();

		public abstract void ShowBanner();
		public abstract void HiddenBanner();
		public abstract void DestroyBanner();

		public abstract bool IsOpenAppReady();
		public abstract void ShowOpenApp();
	}
}