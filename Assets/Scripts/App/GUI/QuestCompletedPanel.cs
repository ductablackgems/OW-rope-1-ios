using App.GUI.Panels;
using App.Quests;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class QuestCompletedPanel : AbstractPanel
	{
		[SerializeField]
		private Text textName;

		[SerializeField]
		private Text textReward;

		[SerializeField]
		private Button buttonClose;

		private Action close;

		public void Show(Quest quest, Action close)
		{
			textName.text = LocalizationManager.Instance.GetText(quest.Settings.NameID);
			textReward.text = quest.Settings.FinishReward.Amount.ToString();
			this.close = close;
		}

		public override PanelType GetPanelType()
		{
			return PanelType.QuestCompleted;
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
