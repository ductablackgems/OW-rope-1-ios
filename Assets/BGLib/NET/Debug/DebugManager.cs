using System.IO;
using AdjustSdk;
using BG_Library.Common;
using BG_Library.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using BG_Library.IAP;

namespace BG_Library.DEBUG
{
    public enum ADS_STATUS
    {
        LOADING,
        LOADED,
        LOADFAIL,
        DISPLAYED,
        DISPLAYFAIL,
        SHOWING,
        HIDE,
        DISMISSED,
        IMPRESSION,
    }

    public class DebugManager : MonoBehaviour
    {
        public static DebugManager Ins { get; private set; }

        public static bool IsDebug { get; private set; }
        public const string DebugNetworkPath = "debug_source_path";

        [FoldoutGroup("GENERAL"), SerializeField] private Text versionText;

        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private GameObject debugCanvas;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private GameObject openDebugBtn;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private GameObject debugPanel;

        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text firebaseText;
        
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text adsConfigText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text adsConfigGGText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text adsConfigApplovinText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text adsConfigMtgText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text adsConfigUnityText;

        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text customConfigText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text customConfigGGText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text customConfigApplovinText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text customConfigMtgText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text customConfigUnityText;
        
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text dataNetworkText;
        [FoldoutGroup("REMOTE CONFIG"), SerializeField] private Text localNetText;

        [FoldoutGroup("STATUS")] public DebugBannerManager banner;
        [FoldoutGroup("STATUS")] public DebugFullScreenAdsManager interstitial;
        [FoldoutGroup("STATUS")] public DebugFullScreenAdsManager reward;
        [FoldoutGroup("STATUS")] public DebugFullScreenAdsManager appOpen;
        [FoldoutGroup("STATUS")] public DebugNativeManager native;

        [FoldoutGroup("IAP")] public Text removeAdsText;
        [FoldoutGroup("IAP")] public Text ignoreIAPText;

        [FoldoutGroup("ADJUST")] public Text adjustStatusText;
        [FoldoutGroup("ADJUST")] public Text adjustIDText;
        [FoldoutGroup("ADJUST")] public Text adjustStartType;
        [FoldoutGroup("ADJUST")] public Text adjustNote;
        
        [FoldoutGroup("ADJUST"), Header("Attribute")] public Text adjustTrackerToken;
        [FoldoutGroup("ADJUST")] public Text adjustTrackerName;
        [FoldoutGroup("ADJUST")] public Text adjustNetwork;
        [FoldoutGroup("ADJUST")] public Text adjustCampaign;
        [FoldoutGroup("ADJUST")] public Text adjustAdgroup;
        [FoldoutGroup("ADJUST")] public Text adjustCreative;
        [FoldoutGroup("ADJUST")] public Text adjustClickLabel;
        [FoldoutGroup("ADJUST")] public Text adjustCostType;
        [FoldoutGroup("ADJUST")] public Text adjustCostAmount;
        [FoldoutGroup("ADJUST")] public Text adjustCostCurrency;
        [FoldoutGroup("ADJUST")] public Text adjustFbInstallReferrer;

        [FoldoutGroup("ADJUST"), Header("Source")] public Image[] adjustSourceImages;
        [FoldoutGroup("ADJUST")] public Text[] adjustSourceTexts;
        [FoldoutGroup("ADJUST")] public Text adjustSourceNormalNetwork;

        [FoldoutGroup("ADJUST")] public Text adjustAdsConfig;
        [FoldoutGroup("ADJUST")] public Text adjustCustomConfig;

        private void Awake()
        {
            if (Ins == null)
            {
                Ins = this;
            }

#if UNITY_EDITOR || IGNORE_ADS
            IsDebug = true;
#else
            IsDebug = Debug.isDebugBuild;
#endif

            Debug.Log("NET debug: " + IsDebug);
            if (!IsDebug)
            {
                debugCanvas.SetActive(false);
                return;
            }

            versionText.text = "VER: " + Application.version + "| LAST VER: " 
                + PlayerPrefs.GetString("LastVersion", Application.version);

            debugCanvas.SetActive(true);
            openDebugBtn.SetActive(true);
            debugPanel.SetActive(false);

            adsConfigText.text = "LOADING ...";
            adsConfigGGText.text = "LOADING ...";
            adsConfigApplovinText.text = "LOADING ...";
            adsConfigMtgText.text = "LOADING ...";
            adsConfigUnityText.text = "LOADING ...";

            customConfigText.text = "LOADING ...";
            dataNetworkText.text = "LOADING ...";

            localNetText.text = "APP OPEN\n";
            localNetText.text += "\nEnable: " + NetConfigsSO.Ins.useAppOpenAdmob;
            localNetText.text += "\nWait time: " + NetConfigsSO.Ins.WaitAppOpenTimer;
            localNetText.text += "\nID Admob: " + NetConfigsSO.Ins.AppOpenIdAdmob;

            banner.Init();
            interstitial.Init();
            reward.Init();
            appOpen.Init();
            native.Init();

            RemoteConfig.OnAllJsonsComplete += () =>
            {
                CheckRemoteConfig(RemoteConfig.Ins.ads_config, NetConfigsSO.Ins.AdsConfigsDefault, adsConfigText);
                CheckRemoteConfig(RemoteConfig.Ins.ads_config_google, NetConfigsSO.Ins.AdsConfigsDefault, adsConfigGGText);
                CheckRemoteConfig(RemoteConfig.Ins.ads_config_applovin, NetConfigsSO.Ins.AdsConfigsDefault, adsConfigApplovinText);
                CheckRemoteConfig(RemoteConfig.Ins.ads_config_mtg, NetConfigsSO.Ins.AdsConfigsDefault, adsConfigMtgText);
                CheckRemoteConfig(RemoteConfig.Ins.ads_config_unity, NetConfigsSO.Ins.AdsConfigsDefault, adsConfigUnityText);

                CheckRemoteConfig(RemoteConfig.Ins.custom_config, NetConfigsSO.Ins.CustomConfigsDefault, customConfigText);
                CheckRemoteConfig(RemoteConfig.Ins.custom_config_google, NetConfigsSO.Ins.CustomConfigsDefault, customConfigGGText);
                CheckRemoteConfig(RemoteConfig.Ins.custom_config_applovin, NetConfigsSO.Ins.CustomConfigsDefault, customConfigApplovinText);
                CheckRemoteConfig(RemoteConfig.Ins.custom_config_mtg, NetConfigsSO.Ins.CustomConfigsDefault, customConfigMtgText);
                CheckRemoteConfig(RemoteConfig.Ins.custom_config_unity, NetConfigsSO.Ins.CustomConfigsDefault, customConfigUnityText);

                CheckRemoteConfig(RemoteConfig.Ins.data_network, NetConfigsSO.Ins.DataNetworkDefault, dataNetworkText);
            };

            RemoteConfig.OnFetchComplete += () =>
            {
                CheckRemoteConfig(RemoteConfig.Ins.ads_config, NetConfigsSO.Ins.AdsConfigsDefault, adjustAdsConfig);
                CheckRemoteConfig(RemoteConfig.Ins.custom_config, NetConfigsSO.Ins.CustomConfigsDefault, adjustCustomConfig);

                adjustSourceNormalNetwork.text = AdjustManager.Ins.NetworkName;
            };

            AdjustManager.OnNetworkReady += () =>
            {
                adjustNote.text = "Attribution: " + AdjustManager.Ins.NetworkName;
                
                var adjustAttri = AdjustManager.Ins.AdjustAttri;
                // Lay duoc nguon user tu adjust, chi user tai game lan dau moi co
                if (AdjustManager.Ins.AttributionFromAdjustReady)
                {
                    adjustTrackerToken.text = adjustAttri.TrackerToken;
                    adjustTrackerName.text = adjustAttri.TrackerName;
                    adjustNetwork.text = adjustAttri.Network;
                    adjustCampaign.text = adjustAttri.Campaign;
                    adjustAdgroup.text = adjustAttri.Adgroup;
                    adjustCreative.text = adjustAttri.Creative;
                    adjustClickLabel.text = adjustAttri.ClickLabel;
                    adjustCostType.text = adjustAttri.CostType;
                    adjustCostAmount.text = adjustAttri.CostAmount.ToString();
                    adjustCostCurrency.text = adjustAttri.CostCurrency;
                    adjustFbInstallReferrer.text = adjustAttri.FbInstallReferrer;
                }
            };
        }

        private void Start()
        {
            if (!IsDebug)
            {
                return;
            }

            var isPortrait = Screen.height > Screen.width;
            var canvasScale = debugCanvas.GetComponent<CanvasScaler>();
            canvasScale.referenceResolution = isPortrait ? new Vector2(1080, 1920) : new Vector2(1920, 1080);

            removeAdsText.text = AdsManager.IAP_RemoveAds.ToString();
            ignoreIAPText.text = IAPManager.IsEnableDebugIap.ToString();

            // Adjust
            var adjustObj = FindObjectOfType<Adjust>();
            if (adjustObj == null)
            {
                adjustStatusText.text = "Adjust object not found";
                adjustStatusText.color = Color.red;
                adjustIDText.text = "";
                adjustStartType.text = "";
            }
            else
            {
                adjustStatusText.text = "Adjust available " + adjustObj.environment + ": " +
                (NetConfigsSO.Ins.adjustVersion == AdjustVersion.Adjust_v4 ? "v4" : "v5");
                adjustStatusText.color = Color.green;
                adjustIDText.text = adjustObj.appToken;

                adjustStartType.text = adjustObj.startManually.ToString();
            }
        }

        void CheckRemoteConfig(string remoteSt, string defaultSt, Text displayText)
        {
            displayText.color = Color.red;
            JObject parsedJson;

            string st1 = "";
            try
            {
                parsedJson = JObject.Parse(remoteSt);
                st1 = parsedJson.ToString(Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                displayText.text = "Remote config: Error json format";
            }

            string st2 = "";
            try
            {
                parsedJson = JObject.Parse(defaultSt);
                st2 = parsedJson.ToString(Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                displayText.text = "Default config: Error json format";
            }

            if (!string.IsNullOrEmpty(st1) && !string.IsNullOrEmpty(st2))
            {
                displayText.color = CompareJsonStrings(st1, st2) ? Color.green : Color.white;
                displayText.text = st1;
            }
        }

        public void UpdateFirebaseText(string st)
        {
            if (!IsDebug) return;
            firebaseText.text = "REMOTE STT: " + st;
        }

        public static bool CompareJsonStrings(string json1, string json2)
        {
            var obj1 = JToken.Parse(json1);
            var obj2 = JToken.Parse(json2);

            return JToken.DeepEquals(obj1, obj2);
        }

        public void ToggleRemoveAds()
        {
            if (!AdsManager.IAP_RemoveAds)
            {
                AdsManager.Ins.PurchaseRemoveAds();
            }
            else
            {
                AdsManager.Ins.RevertPurchaseRemoveAds();
            }
        }

        public void UpdateRemoveAdsText()
        {
            removeAdsText.text = AdsManager.IAP_RemoveAds.ToString();
        }

        public void ToggleIgnoreIAP()
        {
            IAPManager.IsEnableDebugIap = !IAPManager.IsEnableDebugIap;
            ignoreIAPText.text = IAPManager.IsEnableDebugIap.ToString();
        }

        public void Adjust_OnClickUserSource(int order)
        {
            var network = "";
            if (order == 0)
            {
                network = "organic";
            }
            else if (order == 1)
            {
                network = "google";
            }
            else if (order == 2)
            {
                network = "applovin";
            }
            else if (order == 3)
            {
                network = "mtg";
            }
            else if(order == 4)
            {
                network = "unity";
            }
            
            PlayerPrefs.SetString(DebugNetworkPath, network);
            PlayerPrefs.Save();
            adjustSourceNormalNetwork.text = network;

            SelectedAdjustSourceBtn(order);
            
            Debug.Log("Re-open app for applying");
        }

        void SelectedAdjustSourceBtn(int order)
        {
            for (int i = 0; i < adjustSourceImages.Length; i++)
            {
                var b = order == i;
                adjustSourceImages[i].color = b ? Color.green : Color.white;
                adjustSourceTexts[i].color = b ? Color.white : Color.black;
            }
        }

        [Button("CLEAR DATA")]
        void ClearData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            // Xóa dữ liệu trong Persistent Data Path
            string persistentDataPath = Application.persistentDataPath;
            if (Directory.Exists(persistentDataPath))
            {
                Directory.Delete(persistentDataPath, true);
            }

            // Xóa dữ liệu trong Cache Path
            string cachePath = Application.temporaryCachePath;
            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
            }

            // Đảm bảo thư mục tồn tại lại sau khi xóa
            Directory.CreateDirectory(persistentDataPath);
            Directory.CreateDirectory(cachePath);

            Debug.Log("App data cleared!");
        }
    }
}