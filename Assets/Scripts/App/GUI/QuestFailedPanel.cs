using App.GUI.Panels;
using App.Quests;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
    public class QuestFailedPanel : AbstractPanel
    {
        [SerializeField]
        private Text textName;

        [SerializeField]
        private Button buttonClose;

        private Action close;

        public void Show(Quest quest, Action close)
        {
            textName.text = LocalizationManager.Instance.GetText(quest.Settings.NameID);
            this.close = close;
        }

        public override PanelType GetPanelType()
        {
            return PanelType.QuestFailed;
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void OnCloseClicked()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_continue, () =>
            {
                close();
            });
        }

        private void Initialize()
        {
            buttonClose.onClick.AddListener(OnCloseClicked);
        }
    }
}
