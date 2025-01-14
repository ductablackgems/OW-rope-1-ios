using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NiobiumStudios
{
	public class DailyRewards : DailyRewardsCore<DailyRewards>
	{
		public delegate void OnClaimPrize(int day);

		public List<Reward> rewards;

		public DateTime lastRewardTime;

		public int availableReward;

		public int lastReward;

		public bool keepOpen = true;

		public static OnClaimPrize onClaimPrize;

		private const string LAST_REWARD_TIME = "LastRewardTime";

		private const string LAST_REWARD = "LastReward";

		private const string FMT = "O";

		private void Start()
		{
			StartCoroutine(InitializeTimer());
		}

		private void Update()
		{
			if (isInitialized)
			{
				now = now.AddSeconds(Time.unscaledDeltaTime);
			}
		}

		private IEnumerator InitializeTimer()
		{
			yield return StartCoroutine(InitializeDate());
			if (isErrorConnect)
			{
				DailyRewardsCore<DailyRewards>.onInitialize(error: true, errorMessage);
				yield break;
			}
			now = now.AddSeconds(-now.Second);
			CheckRewards();
			DailyRewardsCore<DailyRewards>.onInitialize();
		}

		public void CheckRewards()
		{
			string @string = PlayerPrefs.GetString("LastRewardTime");
			lastReward = PlayerPrefs.GetInt("LastReward");
			if (!string.IsNullOrEmpty(@string))
			{
				lastRewardTime = DateTime.ParseExact(@string, "O", CultureInfo.InvariantCulture);
				TimeSpan timeSpan = now - lastRewardTime;
				UnityEngine.Debug.Log("Last claim was " + (long)timeSpan.TotalHours + " hours ago.");
				int num = (int)(Math.Abs(timeSpan.TotalHours) / 24.0);
				switch (num)
				{
				case 0:
					availableReward = 0;
					break;
				case 1:
					if (lastReward == rewards.Count)
					{
						availableReward = 1;
						lastReward = 0;
					}
					else
					{
						availableReward = lastReward + 1;
						UnityEngine.Debug.Log("Player can claim prize " + availableReward);
					}
					break;
				default:
					if (num >= 2)
					{
						availableReward = 1;
						lastReward = 0;
						UnityEngine.Debug.Log("Prize reset ");
					}
					break;
				}
			}
			else
			{
				availableReward = 1;
			}
		}

		public void ClaimPrize()
		{
			if (availableReward > 0)
			{
				if (onClaimPrize != null)
				{
					onClaimPrize(availableReward);
				}
				UnityEngine.Debug.Log("Reward [" + rewards[availableReward - 1] + "] Claimed!");
				PlayerPrefs.SetInt("LastReward", availableReward);
				string value = now.AddSeconds(-now.Second).ToString("O");
				PlayerPrefs.SetString("LastRewardTime", value);
			}
			else if (availableReward == 0)
			{
				UnityEngine.Debug.LogError("Error! The player is trying to claim the same reward twice.");
			}
			CheckRewards();
		}

		public Reward GetReward(int day)
		{
			return rewards[day - 1];
		}

		public void Reset()
		{
			PlayerPrefs.DeleteKey("LastReward");
			PlayerPrefs.DeleteKey("LastRewardTime");
		}
	}
}
