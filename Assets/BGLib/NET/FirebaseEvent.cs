using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Firebase.Analytics;
using UnityEngine;

namespace BG_Library.NET
{
    public static class AdjustConst
    {
        public const string applovin_max_sdk = "applovin_max_sdk";
        public const string admob_sdk = "admob_sdk";
    }

    public static class BgAdsConst
    {
        // FA
        public const string fa_show_ready = "bg_fa_show_ready";
        public const string fa_show_notready = "bg_fa_show_notready";
        public const string fa_show = "bg_fa_show";
        public const string fa_click = "bg_fa_click";
        public const string fa_finish = "bg_fa_finish";
        public const string fa_failed = "bg_fa_failed";

        public const string break_ads = "bg_break_ads";

        // REWARD
        public const string rw_show_ready = "bg_rw_show_ready";
        public const string rw_show_notready = "bg_rw_show_notready";
        public const string rw_show = "bg_rw_show";
        public const string rw_click = "bg_rw_click";
        public const string rw_finish = "bg_rw_finish";
        public const string rw_failed = "bg_rw_failed";

        // BN
        public const string bn_show_ready = "bg_bn_show_ready";
        public const string bn_show_notready = "bg_bn_show_notready";
        public const string bn_show = "bg_bn_show";
        public const string bn_click = "bg_bn_click";

        // AO
        public const string oaa_show_ready = "bg_oaa_show_ready";
        public const string oaa_show_notready = "bg_oaa_show_notready";
        public const string oaa_show = "bg_oaa_show";
        public const string oaa_click = "bg_oaa_click";

        // Param
        public const string ads_pos = "bg_ads_pos";
	}

    public class FirebaseEvent : MonoBehaviour
    {
        #region Static methob
        public static void LogEventAdImpression(string platform, string adsUnitName, string adsPlacement,
            string adFormat, double adRev, string currencyCode, string adSource)
        {
            if (RemoteConfig.Ins == null || !RemoteConfig.Ins.isFirebaseInitialized)
            {
                return;
            }

            if (!B(adRev, adSource))
            {
	            return;
            }

			FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression,
				new Parameter[]
				{
					new Parameter(FirebaseAnalytics.ParameterAdPlatform, platform),
					new Parameter(FirebaseAnalytics.ParameterAdUnitName, adsUnitName),
					new Parameter(FirebaseAnalytics.ParameterAdFormat, adFormat),
					new Parameter(FirebaseAnalytics.ParameterValue, adRev),
					new Parameter(FirebaseAnalytics.ParameterCurrency, currencyCode),
					new Parameter(FirebaseAnalytics.ParameterAdSource, adSource)
				}
			);

			FirebaseAnalytics.LogEvent("bg_ad_impression_" + adFormat.ToLower());
			AdjustAdImpression(platform, adRev, currencyCode, adSource, adsUnitName, adsPlacement);
		}

        public static void LogEventAdImpressionPos(string platform, string adsUnitName, string adsPlacement,
			string adFormat, double adRev, string currencyCode, string adSource, string adsPos, int adsCount)
        {
			if (RemoteConfig.Ins == null || !RemoteConfig.Ins.isFirebaseInitialized)
			{
				return;
			}
			
			if (!B(adRev, adSource))
			{
				return;
			}

			FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression,
				new Parameter[]
				{
					new Parameter(FirebaseAnalytics.ParameterAdPlatform, platform),
					new Parameter(FirebaseAnalytics.ParameterAdUnitName, adsUnitName),
					new Parameter(FirebaseAnalytics.ParameterAdFormat, adFormat),
					new Parameter(FirebaseAnalytics.ParameterValue, adRev),
					new Parameter(FirebaseAnalytics.ParameterCurrency, currencyCode),
					new Parameter(FirebaseAnalytics.ParameterAdSource, adSource),
					new Parameter("bg_ads_pos", adsPos),
                    new Parameter("bg_ads_count", adsCount.ToString())
				}
			);

			FirebaseAnalytics.LogEvent("bg_ad_impression_" + adFormat.ToLower(),
				new Parameter("bg_ads_pos", adsPos));
			AdjustAdImpression(platform, adRev, currencyCode, adSource, adsUnitName, adsPlacement);
		}

        static void AdjustAdImpression(string platform, double adRev, string currencyCode
	        , string adSource, string adsUnitName, string adsPlacment)
        {
	        if (!B(adRev, adSource))
	        {
		        return;
	        }

	        /*AdjustWrapper.TrackAdRevenue(platform, adRev, currencyCode, adSource, adsUnitName, adsPlacment);*/
	        AdjustSdk.AdjustAdRevenue adRevenue = new AdjustSdk.AdjustAdRevenue(platform);
			adRevenue.SetRevenue(adRev, currencyCode);
			adRevenue.AdRevenueNetwork = adSource;
			adRevenue.AdRevenueUnit = adsUnitName;
			adRevenue.AdRevenuePlacement = adsPlacment;

			AdjustSdk.Adjust.TrackAdRevenue(adRevenue);
        }

        static bool B(double adRev, string adSource)
        {
	        #if UNITY_ANDROID
	        if (adRev > 0.3f || string.IsNullOrEmpty(adSource))
	        {
		        return false;
	        }
	        #endif
	        
	        return true;
        }
        
        public static void SetUserProperty(string propertiesName, string value)
        {
            FirebaseAnalytics.SetUserProperty(propertiesName, value);
        }

        public static void LogEvent(string eventName)
        {
            if (RemoteConfig.Ins == null || !RemoteConfig.Ins.isFirebaseInitialized)
            {
                return;
            }

            FirebaseAnalytics.LogEvent(eventName);

            if (NetConfigsSO.Ins.firebaseDebuggable) Debug.Log($"<color=blue>FIREBASE EVENT:</color> {eventName}");
        }

        public static void LogEvent(string eventName, Parameter parameter)
        {
            if (RemoteConfig.Ins == null || !RemoteConfig.Ins.isFirebaseInitialized)
            {
                return;
            }

            FirebaseAnalytics.LogEvent(eventName, parameter);
            if (NetConfigsSO.Ins.firebaseDebuggable) Debug.Log($"<color=blue>FIREBASE EVENT:</color> {eventName}");
        }

        public static void LogEvent(string eventName, params Parameter[] parameters)
        {
            if (RemoteConfig.Ins == null || !RemoteConfig.Ins.isFirebaseInitialized)
            {
                return;
            }

            FirebaseAnalytics.LogEvent(eventName, parameters);
            if (NetConfigsSO.Ins.firebaseDebuggable) Debug.Log($"<color=blue>FIREBASE EVENT:</color> {eventName}");
        }

        public static void TrackScene(string sceneName)
        {
            var st = sceneName.ToLower();
            st = st.Replace(' ', '_');

            LogEvent(FirebaseAnalytics.EventScreenView,
                new Parameter(FirebaseAnalytics.ParameterScreenName, st),
                new Parameter(FirebaseAnalytics.ParameterScreenClass, "UnityPlayerActivity"));
            if (NetConfigsSO.Ins.firebaseDebuggable) Debug.Log($"<color=blue>FIREBASE LOG SCENE:</color> {sceneName}");
        }
        #endregion
    }
}