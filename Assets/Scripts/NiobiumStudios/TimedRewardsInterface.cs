using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NiobiumStudios
{
	public class TimedRewardsInterface : MonoBehaviour
	{
		public GameObject canvas;

		[Header("Panel Debug")]
		public GameObject panelDebug;

		public bool isDebug;

		[Header("Panel Reward")]
		public Button buttonClaim;

		public Text textInfo;

		[Header("Panel Reward Message")]
		public GameObject panelReward;

		public Text textReward;

		public Button buttonCloseReward;

		public Image imageRewardMessage;

		[Header("Panel Available Rewards")]
		public GameObject panelAvailableRewards;

		public GameObject rewardPrefab;

		public GridLayoutGroup rewardsGroup;

		public ScrollRect scrollRect;

		private List<TimedRewardUI> rewardsUI = new List<TimedRewardUI>();

		private void Awake()
		{
			canvas.SetActive(value: false);
			if (!isDebug)
			{
				panelDebug.SetActive(value: false);
			}
		}

		private void Start()
		{
			InitializeAvailableRewardsUI();
			buttonClaim.interactable = false;
			panelAvailableRewards.SetActive(value: false);
			buttonClaim.onClick.AddListener(delegate
			{
				buttonClaim.interactable = false;
				if (DailyRewardsCore<TimedRewards>.instance.rewards.Count == 1)
				{
					ClaimReward(0);
				}
				else
				{
					panelAvailableRewards.SetActive(value: true);
				}
			});
			buttonCloseReward.onClick.AddListener(delegate
			{
				panelAvailableRewards.SetActive(value: false);
				panelReward.SetActive(value: false);
			});
		}

		private void OnEnable()
		{
			TimedRewards.onCanClaim = (TimedRewards.OnCanClaim)Delegate.Combine(TimedRewards.onCanClaim, new TimedRewards.OnCanClaim(OnCanClaim));
			DailyRewardsCore<TimedRewards>.onInitialize = (DailyRewardsCore<TimedRewards>.OnInitialize)Delegate.Combine(DailyRewardsCore<TimedRewards>.onInitialize, new DailyRewardsCore<TimedRewards>.OnInitialize(OnInitialize));
		}

		private void OnDisable()
		{
			TimedRewards.onCanClaim = (TimedRewards.OnCanClaim)Delegate.Remove(TimedRewards.onCanClaim, new TimedRewards.OnCanClaim(OnCanClaim));
			DailyRewardsCore<TimedRewards>.onInitialize = (DailyRewardsCore<TimedRewards>.OnInitialize)Delegate.Remove(DailyRewardsCore<TimedRewards>.onInitialize, new DailyRewardsCore<TimedRewards>.OnInitialize(OnInitialize));
		}

		private void Update()
		{
			TimeSpan timer = DailyRewardsCore<TimedRewards>.instance.timer;
			if (timer.TotalSeconds > 0.0)
			{
				textInfo.text = $"{timer.Hours:D2}:{timer.Minutes:D2}:{timer.Seconds:D2}";
			}
		}

		private void InitializeAvailableRewardsUI()
		{
			if (DailyRewardsCore<TimedRewards>.instance.rewards.Count > 1)
			{
				for (int i = 0; i < DailyRewardsCore<TimedRewards>.instance.rewards.Count; i++)
				{
					Reward reward = DailyRewardsCore<TimedRewards>.instance.GetReward(i);
					GameObject gameObject = UnityEngine.Object.Instantiate(rewardPrefab);
					TimedRewardUI component = gameObject.GetComponent<TimedRewardUI>();
					component.index = 0;
					component.transform.SetParent(rewardsGroup.transform);
					gameObject.transform.localScale = Vector2.one;
					component.button.onClick.AddListener(OnClickReward(i));
					component.reward = reward;
					component.Initialize();
					rewardsUI.Add(component);
				}
			}
		}

		private UnityAction OnClickReward(int index)
		{
			return delegate
			{
				panelAvailableRewards.SetActive(value: false);
				ClaimReward(index);
			};
		}

		public void OnResetClick()
		{
			DailyRewardsCore<TimedRewards>.instance.Reset();
			buttonClaim.interactable = true;
		}

		private void ClaimReward(int index)
		{
			DailyRewardsCore<TimedRewards>.instance.ClaimReward(index);
			panelReward.SetActive(value: true);
			Reward reward = DailyRewardsCore<TimedRewards>.instance.GetReward(index);
			string unit = reward.unit;
			int reward2 = reward.reward;
			imageRewardMessage.sprite = reward.sprite;
			if (reward2 > 0)
			{
				textReward.text = $"You got {reward.reward} {unit}!";
			}
			else
			{
				textReward.text = $"You got {unit}!";
			}
		}

		private void OnCanClaim()
		{
			buttonClaim.interactable = true;
			textInfo.text = "Reward Ready!";
		}

		private void OnInitialize(bool error, string errorMessage)
		{
			if (!error)
			{
				canvas.gameObject.SetActive(value: true);
			}
		}
	}
}
