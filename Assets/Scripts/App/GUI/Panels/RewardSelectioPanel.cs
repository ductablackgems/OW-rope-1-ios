using App.GUI.Controls;
using App.Rewards;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI.Panels
{
	public class RewardSelectioPanel : AbstractPanel
	{
		[SerializeField]
		private Button buttonCancel;

		[SerializeField]
		private Button buttonRefresh;

		[SerializeField]
		private Text textHeader;

		[SerializeField]
		private Text textUnavailable;

		private RewardItemControl[] rewardItems;

		private RewardManager rewardManager;

		public override PanelType GetPanelType()
		{
			return PanelType.RewardSelection;
		}

		protected override void Awake()
		{
			base.Awake();
			rewardManager = ServiceLocator.Get<RewardManager>();
			rewardItems = GetComponentsInChildren<RewardItemControl>();
			RewardItemControl[] array = rewardItems;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clicked = OnRewardItemClicked;
			}
			buttonCancel.onClick.AddListener(OnCancelClicked);
			buttonRefresh.onClick.AddListener(OnRefreshClicked);
		}

		private void OnEnable()
		{
			LoadRewardData();
			rewardManager.DataChanged += OnDataRewardDataChanged;
		}

		private void OnDisable()
		{
			rewardManager.DataChanged -= OnDataRewardDataChanged;
		}

		private void OnDataRewardDataChanged(int index, RewardState state)
		{
			LoadRewardData();
		}

		private void OnRewardItemClicked(RewardItemControl item)
		{
			int index = Array.IndexOf(rewardItems, item);
		}

		private void OnCancelClicked()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_close, () =>
			{
				rewardManager.OnRewardSelectionLeaveRequest();
			});
			
		}

		private void OnRefreshClicked()
		{
			CallAdsManager.ShowRewardVideo(() =>
			{
				LoadRewardData();
			});
		}

		private void LoadRewardData()
		{
			textUnavailable.gameObject.SetActive(CallAdsManager.RewardedIsReady());
			buttonRefresh.gameObject.SetActive(CallAdsManager.RewardedIsReady());
			textHeader.gameObject.SetActive(!CallAdsManager.RewardedIsReady());
		}
	}
}
