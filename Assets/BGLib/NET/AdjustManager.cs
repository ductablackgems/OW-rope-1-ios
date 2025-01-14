using System.Collections;
using AdjustSdk;
using BG_Library.Common;
using UnityEngine;
using System.Text.RegularExpressions;
using BG_Library.DEBUG;

namespace BG_Library.NET
{
    public class AdjustManager : MonoBehaviour
    {
        public static AdjustManager Ins { get; private set; }

        public static System.Action OnNetworkReady;

        public AdjustAttribution AdjustAttri;
        public bool AttributionFromAdjustReady;

        public string NetworkName;
        public int UserSourceOrder = -1; // -1 means Default

        void Awake()
        {
            if (Ins == null)
            {
                Ins = this;
            }
        }

        private bool _isEnable = false;

        IEnumerator Start()
        {
            if (!Application.isEditor)
            {
                StartCoroutine(WaitAdjustEnable());
                yield return new WaitUntil(() => _isEnable);
            }
            Adjust.GetAttribution(attribution =>
            {
                AdjustAttri = attribution;
                AttributionFromAdjustReady = true;
            });
            StartCoroutine(WaitDetectedNetwork()); // Wait to Attribution Ready
            StartCoroutine(WaitAttribution());
        }

        IEnumerator WaitAdjustEnable()
        {
            while (!_isEnable)
            {
                Adjust.IsEnabled(enable =>
                {
                    if (enable) _isEnable = true;
                });
                yield return null;
            }
        }

        private IEnumerator WaitDetectedNetwork()
        {
            var startTime = Time.time;

            while (!AttributionFromAdjustReady && Time.time - startTime < 7f)
            {
                yield return null; // Tạm dừng coroutine một frame
            }

            if (AttributionFromAdjustReady) // Lay duoc data tu Adjust
            {
                NetworkName = AdjustAttri.Network;
                NetworkName = ConvertString(NetworkName);
            }
            else // Qua thoi gian cho doi Attribution tu Adjust tra ve
            {
                NetworkName = "time_out";
            }

            yield return new WaitUntil(() => RemoteConfig.Ins.isFirebaseInitialized);
            StartCoroutine(DetectScriptByUserSource());
        }

        private IEnumerator WaitAttribution()
        {
            yield return new WaitUntil(() => AttributionFromAdjustReady);
            OnNetworkReady?.Invoke();
        }

        private IEnumerator DetectScriptByUserSource()
        {
            if (DebugManager.IsDebug)
            {
                var st = PlayerPrefs.GetString(DebugManager.DebugNetworkPath, "");
                if (!string.IsNullOrEmpty(st))
                {
                    NetworkName = st;
                }
            }

            UserSourceOrder = -1;
            if (string.IsNullOrEmpty(NetworkName)) UserSourceOrder = -1;
            else if (NetworkName.Contains("google")) UserSourceOrder = 0;
            else if (NetworkName.Contains("applovin")) UserSourceOrder = 1;
            else if (NetworkName.Contains("mintegral")
                     || NetworkName.Contains("mtg")) UserSourceOrder = 2;
            else if (NetworkName.Contains("unity")) UserSourceOrder = 3;

            if (string.IsNullOrEmpty(NetworkName))
            {
                FirebaseEvent.SetUserProperty("bg_network", "blank");
            }
            else
            {
                FirebaseEvent.SetUserProperty("bg_network", NetworkName);
            }

            yield return new WaitForSecondsRealtime(0.1f); // Doi de Init remote config
            RemoteConfig.Ins.InitializeRemoteConfig(); // Init sau khi xac dinh duoc property
            yield return new WaitUntil(() => RemoteConfig.Ins.isDataFetched);

            CompleteDetected();
        }

        void CompleteDetected()
        {
            if (UserSourceOrder == -1)
            {
                RemoteConfig.Ins.CannotDetectUserFetchComplete();
                return;
            }

            switch (UserSourceOrder)
            {
                case 0: // Google
                    ChangeCustomConfigIfAble(RemoteConfig.Ins.custom_config_google);
                    ChangeAdsConfigIfAble(RemoteConfig.Ins.ads_config_google);
                    break;
                case 1: // Applovin
                    ChangeCustomConfigIfAble(RemoteConfig.Ins.custom_config_applovin);
                    ChangeAdsConfigIfAble(RemoteConfig.Ins.ads_config_applovin);
                    break;
                case 2: // Mtg
                    ChangeCustomConfigIfAble(RemoteConfig.Ins.custom_config_mtg);
                    ChangeAdsConfigIfAble(RemoteConfig.Ins.ads_config_mtg);
                    break;
                case 3: // Unity
                    ChangeCustomConfigIfAble(RemoteConfig.Ins.custom_config_unity);
                    ChangeAdsConfigIfAble(RemoteConfig.Ins.ads_config_unity);
                    break;
            }

            AdsManager.InitAdsConfig(RemoteConfig.Ins.ads_config);
            AdsManager.InitMediation();
            RemoteConfig.OnFetchComplete?.Invoke();
        }

        /// <summary>
        /// Thay kich ban custom_config neu nhu co kich ban khac cua tap user nay, neu khong co => Organic
        /// </summary>
        void ChangeCustomConfigIfAble(string customConfigScript)
        {
            if (!string.IsNullOrEmpty(customConfigScript))
            {
                RemoteConfig.Ins.custom_config = customConfigScript;
            }
        }

        /// <summary>
        /// Thay kich ban ads_config neu nhu co kich ban khac cua tap user nay, neu khong co => Organic
        /// </summary>
        void ChangeAdsConfigIfAble(string adsConfigScript)
        {
            if (!string.IsNullOrEmpty(adsConfigScript))
            {
                RemoteConfig.Ins.ads_config = adsConfigScript;
            }
        }

        public static string ConvertString(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return string.Empty; // Trả về chuỗi rỗng nếu chuỗi đầu vào rỗng
            }

            // Kiểm tra ký tự đầu tiên có phải là số không
            if (char.IsDigit(inputString[0]))
            {
                // Xóa ký tự đầu tiên nếu là số
                inputString = inputString.Substring(1);
            }

            // Thay thế khoảng trắng bằng dấu "_"
            inputString = inputString.Replace(" ", "_");

            // Loại bỏ tất cả các ký tự không phải chữ cái, số hoặc dấu gạch dưới
            string pattern = @"[^a-zA-Z0-9_]"; // Lưu ý dấu "_" trong biểu thức chính quy
            string result = Regex.Replace(inputString, pattern, "");

            // Chuyển đổi toàn bộ chuỗi thành chữ thường
            result = result.ToLower();

            // Cắt chuỗi nếu quá dài
            if (result.Length > 30)
            {
                result = result.Substring(0, 30);
            }

            return result;
        }
    }
}