using App.GUI.Panels;
using App.Quests;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
    public class QuestOfferPanel : AbstractPanel
    {
        [SerializeField]
        private RectTransform list;

        [SerializeField]
        private Button buttonAccept;

        [SerializeField]
        private Button buttonAbandon;

        [SerializeField]
        private Button buttonClose;

        [SerializeField]
        private Text textDescription;

        private Quest selectedQuest;

        private QuestProvider questProvider;

        private ConfirmDialog confirmDialog;

        private QuestItemControl itemPrefab;

        private List<QuestItemControl> items = new List<QuestItemControl>(8);

        public void UpdateData(QuestProvider provider)
        {
            questProvider = provider;
            GenerateItems(questProvider.Offer.Count);
            OnShow();
        }

        public override PanelType GetPanelType()
        {
            return PanelType.QuestOffer;
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void OnItemSelected(QuestItemControl itemControl)
        {
            selectedQuest = itemControl.Quest;
            textDescription.text = LocalizationManager.Instance.GetText(selectedQuest.Settings.DescriptionID);
            if (selectedQuest == questProvider.ActiveQuest)
            {
                ShowAbandonButton();
            }
            else
            {
                ShowAcceptButton();
            }
        }

        private void OnButtonAcceptClicked()
        {
            if (questProvider.ActiveQuest != null)
            {
                CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_continue, () =>
                {
                    string text = LocalizationManager.Instance.GetText(6004);
                    text = string.Format(text, LocalizationManager.Instance.GetText(questProvider.ActiveQuest.Settings.NameID));
                    confirmDialog.Show(text, OnConfirmDialogResult);
                });
            }
            else
            {
                CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_continue, () =>
                {
                    Close();
                    questProvider.StartQuest(selectedQuest);
                });
            }
        }

        private void OnButtonAbandonClicked()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_continue, () =>
            {
                questProvider.AbandonQuest();
                ShowAcceptButton();
                MarkActiveQuest();
            });
        }

        private void OnButtonCloseClicked()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_continue, () =>
            {
                Close();
            });

        }

        private void OnConfirmDialogResult(ConfirmDialog.Result result)
        {
            if (result != 0)
            {
                questProvider.AbandonQuest();
                Close();
                questProvider.StartQuest(selectedQuest);
            }
        }

        private void Initialize()
        {
            confirmDialog = GetComponentInChildren<ConfirmDialog>(includeInactive: true);
            confirmDialog.Close();
            ShowAcceptButton();
            buttonAccept.onClick.AddListener(OnButtonAcceptClicked);
            buttonAbandon.onClick.AddListener(OnButtonAbandonClicked);
            buttonClose.onClick.AddListener(OnButtonCloseClicked);
            itemPrefab = list.GetComponentInChildren<QuestItemControl>();
            itemPrefab.Initialize();
            itemPrefab.Selected += OnItemSelected;
            itemPrefab.gameObject.SetActive(value: false);
            items.Add(itemPrefab);
        }

        private void OnShow()
        {
            selectedQuest = null;
            Quest activeQuest = questProvider.ActiveQuest;
            for (int i = 0; i < items.Count; i++)
            {
                QuestItemControl questItemControl = items[i];
                bool flag = i < questProvider.Offer.Count;
                questItemControl.Quest = null;
                questItemControl.gameObject.SetActive(flag);
                if (flag)
                {
                    questItemControl.Quest = questProvider.Offer[i];
                    if (activeQuest != null && questItemControl.Quest == activeQuest)
                    {
                        questItemControl.Select();
                    }
                }
            }
            if (activeQuest == null && items.Count > 0)
            {
                items[0].Select();
            }
            MarkActiveQuest();
        }

        private void ShowAcceptButton()
        {
            buttonAccept.gameObject.SetActive(value: true);
            buttonAbandon.gameObject.SetActive(value: false);
        }

        private void ShowAbandonButton()
        {
            buttonAccept.gameObject.SetActive(value: false);
            buttonAbandon.gameObject.SetActive(value: true);
        }

        private void MarkActiveQuest()
        {
            Quest activeQuest = questProvider.ActiveQuest;
            foreach (QuestItemControl item in items)
            {
                item.Mark(activeQuest != null && item.Quest == activeQuest);
            }
        }

        private void Close()
        {
            questProvider.OnOfferSelectionClose();
        }

        private void GenerateItems(int count)
        {
            while (items.Count < count)
            {
                QuestItemControl questItemControl = Object.Instantiate(itemPrefab, list.transform);
                questItemControl.Selected += OnItemSelected;
                questItemControl.Initialize();
                items.Add(questItemControl);
            }
        }
    }
}
