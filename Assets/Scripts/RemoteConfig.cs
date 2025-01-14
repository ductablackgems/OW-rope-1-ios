// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Firebase.Extensions;
// using System;
// using System.Threading.Tasks;
//
// public class RemoteConfig : MonoBehaviour
// {
//     public static RemoteConfig instance;
//     public Action OnFetchDone;
//     public bool isDataFetched = false;
//
//     public int time_wait_ads = 30;
//     public int isShowBanner = 0;
//     public int isCoolDownFirst = 0;
//     public int isShowOpenAds = 0;
//     public int free_coin_enable = 0;
//     public int free_coin_first_time = 60;
//     public int free_coin_interval = 60;
//     public int internet_panel_enable = 1;
//     public int internet_panel_count = 4;
//     public string dataNetwork;
//     public string ads_config;
//     public string dataShowAds;
//
//
//     protected bool isFirebaseInitialized = false;
//     Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
//
//
//
//     // Start is called before the first frame update
//     void Awake()
//     {
//         instance = this;
//     }
//
//     void Start()
//     {
//         Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
//         {
//             dependencyStatus = task.Result;
//             if (dependencyStatus == Firebase.DependencyStatus.Available)
//             {
//                 InitializeFirebase();
//             }
//             else
//             {
//                 Debug.LogError(
//                   "Could not resolve all Firebase dependencies: " + dependencyStatus);
//             }
//         });
//     }
//
//     void InitializeFirebase()
//     {
//         LoadData();
//         Dictionary<string, object> defaults =
//           new Dictionary<string, object>
//           {
//               { "time_wait_ads", time_wait_ads },
//               { "isShowBanner", isShowBanner },
//               { "isCoolDownFirst", isCoolDownFirst },
//               { "isShowOpenAds", isShowOpenAds },
//               { "free_coin_enable", free_coin_enable },
//               { "free_coin_first_time", free_coin_first_time },
//               { "free_coin_interval", free_coin_interval },
//               { "internet_panel_enable", internet_panel_enable },
//               { "internet_panel_count", internet_panel_count },
//               { "dataShowAds", dataShowAds },
//               { "dataNetwork", dataNetwork },
//               { "ads_config", ads_config }
//           };
//
//         Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
//
//         Debug.Log("RemoteConfig configured and ready!");
//
//         isFirebaseInitialized = true;
//         FetchDataAsync();
//     }
//
//     public void FetchDataAsync()
//     {
//         // FetchAsync only fetches new data if the current data is older than the provided
//         // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
//         // By default the timespan is 12 hours, and for production apps, this is a good
//         // number.  For this example though, it's set to a timespan of zero, so that
//         // changes in the console will always show up immediately.
//         System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
//             TimeSpan.Zero);
//         fetchTask.ContinueWithOnMainThread(FetchComplete);
//     }
//
//     void FetchComplete(Task fetchTask)
//     {
//         if (fetchTask.IsCanceled)
//         {
//             Debug.Log("Fetch canceled.");
//         }
//         else if (fetchTask.IsFaulted)
//         {
//             Debug.Log("Fetch encountered an error.");
//         }
//         else if (fetchTask.IsCompleted)
//         {
//             Debug.Log("Fetch completed successfully!");
//         }
//
//         var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
//         switch (info.LastFetchStatus)
//         {
//             case Firebase.RemoteConfig.LastFetchStatus.Success:
//                 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
//                 Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
//                                        info.FetchTime));
//                 RefrectProperties();
//                 break;
//             case Firebase.RemoteConfig.LastFetchStatus.Failure:
//                 switch (info.LastFetchFailureReason)
//                 {
//                     case Firebase.RemoteConfig.FetchFailureReason.Error:
//                         Debug.Log("Fetch failed for unknown reason");
//                         break;
//                     case Firebase.RemoteConfig.FetchFailureReason.Throttled:
//                         Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
//                         break;
//                 }
//                 break;
//             case Firebase.RemoteConfig.LastFetchStatus.Pending:
//                 Debug.Log("Latest Fetch call still pending.");
//                 break;
//         }
//
//         if (OnFetchDone != null) OnFetchDone.Invoke();
//     }
//
//     private void RefrectProperties()
//     {
//         time_wait_ads = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("time_wait_ads").DoubleValue;
//         isShowBanner = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("isShowBanner").DoubleValue;
//         isCoolDownFirst = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("isCoolDownFirst").DoubleValue;
//         isShowOpenAds = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("isShowOpenAds").DoubleValue;
//         free_coin_enable = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("free_coin_enable").DoubleValue;
//         free_coin_first_time = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("free_coin_first_time").DoubleValue;
//         free_coin_interval = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("free_coin_interval").DoubleValue;
//         internet_panel_enable = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("internet_panel_enable").DoubleValue;
//         internet_panel_count = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("internet_panel_count").DoubleValue;
//         dataNetwork = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("dataNetwork").StringValue;
//         ads_config = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("ads_config").StringValue;
//
//         SaveData();
//         isDataFetched = true;
//
//         Debug.Log("RemoteConfig Init Complete");
//         GetComponent<AdsManager>().StartInit(dataNetwork, ads_config);
//     }
//
//
//     // save cache data
//     public void SaveData()
//     {
//         PlayerPrefs.SetInt("time_wait_ads", time_wait_ads);
//         PlayerPrefs.SetInt("isShowBanner", isShowBanner);
//         PlayerPrefs.SetInt("isCoolDownFirst", isCoolDownFirst);
//         PlayerPrefs.SetInt("isShowOpenAds", isShowOpenAds);
//         PlayerPrefs.SetInt("free_coin_enable", free_coin_enable);
//         PlayerPrefs.SetInt("free_coin_first_time", free_coin_first_time);
//         PlayerPrefs.SetInt("free_coin_interval", free_coin_interval);
//         PlayerPrefs.SetInt("internet_panel_enable", internet_panel_enable);
//         PlayerPrefs.SetInt("internet_panel_count", internet_panel_count);
//         PlayerPrefs.SetString("dataNetwork", dataNetwork);
//         PlayerPrefs.SetString("ads_config", ads_config);
//     }
//
//     //load cache data
//     private void LoadData()
//     {
//         time_wait_ads = PlayerPrefs.GetInt("time_wait_ads", 30);
//         isShowBanner = PlayerPrefs.GetInt("isShowBanner", 0);
//         isCoolDownFirst = PlayerPrefs.GetInt("isCoolDownFirst", 0);
//         isShowOpenAds = PlayerPrefs.GetInt("isShowOpenAds", 0);
//         free_coin_enable = PlayerPrefs.GetInt("free_coin_enable", 0);
//         free_coin_first_time = PlayerPrefs.GetInt("free_coin_first_time", 60);
//         free_coin_interval = PlayerPrefs.GetInt("free_coin_interval", 60);
//         internet_panel_enable = PlayerPrefs.GetInt("internet_panel_enable", 1);
//         internet_panel_count = PlayerPrefs.GetInt("internet_panel_count", 4);
//         dataNetwork = PlayerPrefs.GetString("dataNetwork");
//         ads_config = PlayerPrefs.GetString("ads_config", ads_config);
//     }
// }
