using AdjustSdk;
using BG_Library.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BG_Library.NET
{
    [CreateAssetMenu(fileName = "Configs SO", menuName = "BG/NET/Create Configs SO")]
    public class NetConfigsSO : ScriptableObject
    {
        public static NetConfigsSO Ins
        {
            get
            {
                if (!ins)
                {
                    ins = LoadSource.LoadObject<NetConfigsSO>("Configs SO");
                }

                return ins;
            }
        }
        private static NetConfigsSO ins;

        [BoxGroup("Debug"), SerializeField] public bool firebaseDebuggable;
        [BoxGroup("Events"), SerializeField] public bool useAdImpressionValue;

        [BoxGroup("Adjust"), SerializeField, ReadOnly] public AdjustVersion adjustVersion = AdjustVersion.Adjust_v5;
        [BoxGroup("Adjust"), SerializeField] private string adjustEventIAPAndroid;
        [BoxGroup("Adjust"), SerializeField] private string adjustEventIAPIOS;

        [BoxGroup("AO"), SerializeField] public bool useAppOpenAdmob;
        [BoxGroup("AO"), SerializeField] public bool ignoreAppOpenFirstTime;
        [BoxGroup("AO"), SerializeField] private float waitAppOpenTimer;
        [BoxGroup("AO"), SerializeField] private string appOpenIdAdmobAndroid;
        [BoxGroup("AO"), SerializeField] private string appOpenIdAdmobIOS;

        [BoxGroup("Break ads FA")] public bool breakAdsIgnoreTimeScale = true;

		[BoxGroup("Configs Collap")] public bool isUseCollapByDefault = true;
        
        [BoxGroup("SetDefault Remote Configs"), SerializeField] private string dataNetworkAndroid;
        [BoxGroup("SetDefault Remote Configs"), SerializeField] private string dataNetworkIOS;
        
        [BoxGroup("SetDefault Remote Configs"), SerializeField, Space(10)] private string adsConfigsAndroid;
        [BoxGroup("SetDefault Remote Configs"), SerializeField] private string adsConfigsIOS;
        
        [BoxGroup("SetDefault Remote Configs"), SerializeField, Space(10)] private string customConfigsAndroid;
        [BoxGroup("SetDefault Remote Configs"), SerializeField] private string customConfigsIOS;

        public float WaitAppOpenTimer
        {
            get
            {
                #if UNITY_EDITOR
                    return 0;
                #else
                    return waitAppOpenTimer;
                #endif
            }
        }

        public string AppOpenIdAdmob
        {
            get
            {
                #if UNITY_ANDROID
                    return appOpenIdAdmobAndroid;
                #elif UNITY_IPHONE
                    return appOpenIdAdmobIOS;
                #else
                    return "unused";
                #endif
            }
        }

        public string AdjustEventIAP
        {
            get
            {
                #if UNITY_ANDROID
                    return adjustEventIAPAndroid;
                #elif UNITY_IPHONE
                    return adjustEventIAPIOS;
                #else
                    return "unused";
                #endif
            }
        }
        
        #region Remote Config SetDefault
        public string DataNetworkDefault
        {
            get
            {
                #if UNITY_ANDROID
                    return dataNetworkAndroid;
                #elif UNITY_IPHONE
                    return dataNetworkIOS;
                #else
                    return "";
                #endif
            }
        }
        
        public string AdsConfigsDefault
        {
            get
            {
                #if UNITY_ANDROID
                    return adsConfigsAndroid;
                #elif UNITY_IPHONE
                    return adsConfigsIOS;
                #else
                    return "";
                #endif
            }
        }

        public string CustomConfigsDefault
        {
            get
            {
                #if UNITY_ANDROID
                    return customConfigsAndroid;
                #elif UNITY_IPHONE
                    return customConfigsIOS;
                #else
                    return "";
                #endif
            }
        }
        #endregion
    }
}
