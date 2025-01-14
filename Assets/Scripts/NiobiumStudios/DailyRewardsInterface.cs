using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NiobiumStudios
{
	public class DailyRewardsInterface : MonoBehaviour
	{
		public Canvas canvas;

		public GameObject dailyRewardPrefab;

		[Header("Panel Debug")]
		public GameObject panelDebug;

		public bool isDebug;

		[Header("Panel Reward Message")]
		public GameObject panelReward;

		public Text textReward;

		public Button buttonCloseReward;

		[Header("Panel Reward")]
		public GameObject buttonClaimGo;

		public GameObject buttonCloseGo;

		public Button buttonClaim;

		public Button buttonClose;

		public Text textTimeDue;

		public GridLayoutGroup dailyRewardsGroup;

		public ScrollRect scrollRect;

		public Image imageReward;

		private bool readyToClaim;

		private List<DailyRewardUI> dailyRewardsUI = new List<DailyRewardUI>();

		private void Awake()
		{
			canvas.gameObject.SetActive(value: false);
		}

		private void Start()
		{
			InitializeDailyRewardsUI();
			panelDebug.SetActive(isDebug);
			buttonCloseGo.SetActive(value: false);
			buttonClaim.onClick.AddListener(delegate
			{
				DailyRewardsCore<DailyRewards>.instance.ClaimPrize();
				readyToClaim = false;
				UpdateUI();
			});
			buttonCloseReward.onClick.AddListener(delegate
			{
				bool keepOpen = DailyRewardsCore<DailyRewards>.instance.keepOpen;
				panelReward.SetActive(value: false);
				canvas.gameObject.SetActive(keepOpen);
			});
			buttonClose.onClick.AddListener(delegate
			{
				canvas.gameObject.SetActive(value: false);
			});
		}

		private void OnEnable()
		{
			DailyRewards.onClaimPrize = (DailyRewards.OnClaimPrize)Delegate.Combine(DailyRewards.onClaimPrize, new DailyRewards.OnClaimPrize(OnClaimPrize));
			DailyRewardsCore<DailyRewards>.onInitialize = (DailyRewardsCore<DailyRewards>.OnInitialize)Delegate.Combine(DailyRewardsCore<DailyRewards>.onInitialize, new DailyRewardsCore<DailyRewards>.OnInitialize(OnInitialize));
		}

		private void OnDisable()
		{
			DailyRewards.onClaimPrize = (DailyRewards.OnClaimPrize)Delegate.Remove(DailyRewards.onClaimPrize, new DailyRewards.OnClaimPrize(OnClaimPrize));
			DailyRewardsCore<DailyRewards>.onInitialize = (DailyRewardsCore<DailyRewards>.OnInitialize)Delegate.Remove(DailyRewardsCore<DailyRewards>.onInitialize, new DailyRewardsCore<DailyRewards>.OnInitialize(OnInitialize));
		}

		private void InitializeDailyRewardsUI()
		{
			for (int i = 0; i < DailyRewardsCore<DailyRewards>.instance.rewards.Count; i++)
			{
				int day = i + 1;
				Reward reward = DailyRewardsCore<DailyRewards>.instance.GetReward(day);
				GameObject gameObject = UnityEngine.Object.Instantiate(dailyRewardPrefab);
				DailyRewardUI component = gameObject.GetComponent<DailyRewardUI>();
				component.transform.SetParent(dailyRewardsGroup.transform);
				gameObject.transform.localScale = Vector2.one;
				component.day = day;
				component.reward = reward;
				component.Initialize();
				dailyRewardsUI.Add(component);
			}
		}

		public void UpdateUI()
		{
			DailyRewardsCore<DailyRewards>.instance.CheckRewards();
			bool flag = false;
			int lastReward = DailyRewardsCore<DailyRewards>.instance.lastReward;
			int availableReward = DailyRewardsCore<DailyRewards>.instance.availableReward;
			foreach (DailyRewardUI item in dailyRewardsUI)
			{
				int day = item.day;
				if (day == availableReward)
				{
					item.state = DailyRewardUI.DailyRewardState.UNCLAIMED_AVAILABLE;
					flag = true;
				}
				else if (day <= lastReward)
				{
					item.state = DailyRewardUI.DailyRewardState.CLAIMED;
				}
				else
				{
					item.state = DailyRewardUI.DailyRewardState.UNCLAIMED_UNAVAILABLE;
				}
				item.Refresh();
			}
			buttonClaimGo.SetActive(flag);
			buttonCloseGo.SetActive(!flag);
			if (flag)
			{
				SnapToReward();
				textTimeDue.text = "You can claim your reward!";
			}
			readyToClaim = flag;
		}

		public void SnapToReward()
		{
			Canvas.ForceUpdateCanvases();
			int num = DailyRewardsCore<DailyRewards>.instance.lastReward;
			if (dailyRewardsUI.Count <= num)
			{
				num++;
			}
			RectTransform component = dailyRewardsUI[num].GetComponent<RectTransform>();
			RectTransform content = scrollRect.content;
			float verticalNormalizedPosition = (float)component.GetSiblingIndex() / (float)content.transform.childCount;
			scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
		}

		private void Update()
		{
			if (!readyToClaim)
			{
				TimeSpan timeSpan = (DailyRewardsCore<DailyRewards>.instance.lastRewardTime - DailyRewardsCore<DailyRewards>.instance.now).Add(new TimeSpan(0, 24, 0, 0));
				if (timeSpan.TotalSeconds <= 0.0)
				{
					readyToClaim = true;
					UpdateUI();
					SnapToReward();
				}
				else
				{
					string arg = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
					textTimeDue.text = $"Come back in {arg} for your next reward";
				}
			}
		}

		private void OnClaimPrize(int day)
		{
			panelReward.SetActive(value: true);
			Reward reward = DailyRewardsCore<DailyRewards>.instance.GetReward(day);
			string unit = reward.unit;
			int reward2 = reward.reward;
			imageReward.sprite = reward.sprite;
			if (reward2 > 0)
			{
				textReward.text = $"You got {reward.reward} {unit}!";
			}
			else
			{
				textReward.text = $"You got {unit}!";
			}
		}

		private void OnInitialize(bool error, string errorMessage)
		{
			if (!error)
			{
				bool keepOpen = DailyRewardsCore<DailyRewards>.instance.keepOpen;
				bool flag = DailyRewardsCore<DailyRewards>.instance.availableReward > 0;
				canvas.gameObject.SetActive(keepOpen || (!keepOpen && flag));
				UpdateUI();
				SnapToReward();
			}
		}

		public void OnResetClick()
		{
			DailyRewardsCore<DailyRewards>.instance.Reset();
			DailyRewardsCore<DailyRewards>.instance.lastRewardTime = DateTime.MinValue;
			readyToClaim = false;
			UpdateUI();
		}

		public void OnAdvanceDayClick()
		{
			DailyRewardsCore<DailyRewards>.instance.now = DailyRewardsCore<DailyRewards>.instance.now.AddDays(1.0);
			UpdateUI();
		}

		public void OnAdvanceHourClick()
		{
			DailyRewardsCore<DailyRewards>.instance.now = DailyRewardsCore<DailyRewards>.instance.now.AddHours(1.0);
			UpdateUI();
		}
	}
}
