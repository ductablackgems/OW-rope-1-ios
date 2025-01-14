using App.GUI.Panels;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
    public class HackingResultPanel : AbstractPanel
    {
        public enum Result
        {
            Close,
            WatchAd
        }

        [SerializeField]
        private Button buttonClose;

        [SerializeField]
        private Button buttonWatchAd;

        [SerializeField]
        private Text textReward;

        [SerializeField]
        private Text textValueVideoReward;

        [SerializeField]
        private Text textAdsUnavailable;

        private Action<Result> onClose;

        public void Show(int reward, bool isAdAvailable, Action<Result> onClose, int hackCount)
        {
            this.onClose = onClose;
            textReward.text = $"${reward}";
            buttonWatchAd.gameObject.SetActive(isAdAvailable);
            textAdsUnavailable.gameObject.SetActive(!isAdAvailable);
            textValueVideoReward.text = "x" + (hackCount + 1);
        }

        public override PanelType GetPanelType()
        {
            return PanelType.HackingResult;
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void OnButtonCloseClicked()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
            {
                onClose(Result.Close);
            });
        }

        private void OnButtonWatchAdClicked()
        {
            CallAdsManager.ShowRewardVideo(() =>
            {
                onClose(Result.WatchAd);
            });
        }

        private void Initialize()
        {
            buttonClose.onClick.AddListener(OnButtonCloseClicked);
            buttonWatchAd.onClick.AddListener(OnButtonWatchAdClicked);
            textAdsUnavailable.gameObject.SetActive(value: false);
        }
    }
}
