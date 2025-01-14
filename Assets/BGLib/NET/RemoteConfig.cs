using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BG_Library.DEBUG;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BG_Library.NET
{
    public class RemoteConfig : MonoBehaviour
    {
        public static RemoteConfig Ins;

        public static Action OnFetchComplete;
        public static Action OnAllJsonsComplete;

        [BoxGroup("Status")] public bool isDataFetched = false;
        [BoxGroup("Status")] public bool isFirebaseInitialized = false;
        [BoxGroup("Status"), SerializeField] Firebase.DependencyStatus dependencyStatus 
            = Firebase.DependencyStatus.UnavailableOther;

        [FoldoutGroup("Fetch data")] public string data_network;
        
        [FoldoutGroup("Fetch data")] public string ads_config;
        [FoldoutGroup("Fetch data")] public string ads_config_google;
        [FoldoutGroup("Fetch data")] public string ads_config_applovin;
        [FoldoutGroup("Fetch data")] public string ads_config_mtg;
        [FoldoutGroup("Fetch data")] public string ads_config_unity;

        [FoldoutGroup("Fetch data")] public string custom_config;
        [FoldoutGroup("Fetch data")] public string custom_config_google;
        [FoldoutGroup("Fetch data")] public string custom_config_applovin;
        [FoldoutGroup("Fetch data")] public string custom_config_mtg;
        [FoldoutGroup("Fetch data")] public string custom_config_unity;

        // Start is called before the first frame update
        private void Awake()
        {
            Ins = this;
        }

        private void Start()
        {
            InitFirebase();
        }

        public void InitFirebase()
        {
            DebugManager.Ins.UpdateFirebaseText("Init Firebase");
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    isFirebaseInitialized = true;
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        public void InitializeRemoteConfig()
        {
            var isCache = false;
            
            var st0= NetConfigsSO.Ins.DataNetworkDefault;
            if (PlayerPrefs.HasKey("dataNetwork"))
            {
                st0 = PlayerPrefs.GetString("dataNetwork");
                isCache = true;
            }
            
            var st1= NetConfigsSO.Ins.AdsConfigsDefault;
            if (PlayerPrefs.HasKey("ads_config"))
            {
                st1 = PlayerPrefs.GetString("ads_config");
            }
            
            var st2= NetConfigsSO.Ins.CustomConfigsDefault;
            if (PlayerPrefs.HasKey("custom_config"))
            {
                st2 = PlayerPrefs.GetString("custom_config");
            }

            Dictionary<string, object> defaults =
                new Dictionary<string, object>
                {
                    { "dataNetwork", st0 },
                    { "ads_config", st1 },
                    { "custom_config", st2 },
                };

            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            DebugManager.Ins.UpdateFirebaseText("RemoteConfig configured and ready, cache: " + isCache);

            FetchDataAsync();
        }

        public void FetchDataAsync()
        {
            // FetchAsync only fetches new data if the current data is older than the provided
            // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
            // By default the timespan is 12 hours, and for production apps, this is a good
            // number.  For this example though, it's set to a timespan of zero, so that
            // changes in the console will always show up immediately.
            Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                DebugManager.Ins.UpdateFirebaseText("Fetch canceled");

                RefrectProperties();
                return;
            }
            if (fetchTask.IsFaulted)
            {
                DebugManager.Ins.UpdateFirebaseText("Fetch canceled");

                RefrectProperties();
                return;
            }
            if (fetchTask.IsCompletedSuccessfully)
            {
                //DebugManager.Ins.UpdateFirebaseText("Fetch completed successfully!");
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(
                        task =>
                        {
                            RefrectProperties();
                        });
                    DebugManager.Ins.UpdateFirebaseText(
                        string.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            DebugManager.Ins.UpdateFirebaseText("Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            DebugManager.Ins.UpdateFirebaseText("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    RefrectProperties();
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    DebugManager.Ins.UpdateFirebaseText("Latest Fetch call still pending.");
                    RefrectProperties();
                    break;
                default:
                    RefrectProperties();
                    break;
            }
        }

        private void RefrectProperties()
        {
            data_network = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("dataNetwork").StringValue;

            ads_config = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("ads_config").StringValue;
            ads_config_google = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("ads_config_google").StringValue;
            ads_config_applovin = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("ads_config_applovin").StringValue;
            ads_config_mtg = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("ads_config_mtg").StringValue;
            ads_config_unity = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("ads_config_unity").StringValue;

            custom_config = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("custom_config").StringValue;
            custom_config_google = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("custom_config_google").StringValue;
            custom_config_applovin = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("custom_config_applovin").StringValue;
            custom_config_mtg = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("custom_config_mtg").StringValue;
            custom_config_unity = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("custom_config_unity").StringValue;

            OnAllJsonsComplete?.Invoke();
            AdsManager.InitDataNetwork(data_network);

            SaveData();
            isDataFetched = true;
        }

        public void CannotDetectUserFetchComplete()
        {
            AdsManager.InitAdsConfig(ads_config);
            AdsManager.InitMediation();

            OnFetchComplete?.Invoke();
        }

		// save cache data
		public void SaveData()
        {
            PlayerPrefs.SetString("dataNetwork", data_network);

            PlayerPrefs.SetString("ads_config", ads_config);
            PlayerPrefs.SetString("ads_config_google", ads_config_google);
            PlayerPrefs.SetString("ads_config_applovin", ads_config_applovin);
            PlayerPrefs.SetString("ads_config_mtg", ads_config_mtg);
            PlayerPrefs.SetString("ads_config_unity", ads_config_unity);

            PlayerPrefs.SetString("custom_config", custom_config);
            PlayerPrefs.SetString("custom_config_google", custom_config_google);
            PlayerPrefs.SetString("custom_config_applovin", custom_config_applovin);
            PlayerPrefs.SetString("custom_config_mtg", custom_config_mtg);
            PlayerPrefs.SetString("custom_config_unity", custom_config_unity);
        }
    }
}