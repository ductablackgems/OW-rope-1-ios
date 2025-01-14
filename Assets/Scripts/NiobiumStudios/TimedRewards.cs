using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NiobiumStudios
{
	public class TimedRewards : DailyRewardsCore<TimedRewards>
	{
		public delegate void OnCanClaim();

		public delegate void OnClaimPrize(int index);

		public DateTime lastRewardTime;

		public TimeSpan timer;

		public float maxTime = 3600f;

		public List<Reward> rewards;

		public static OnCanClaim onCanClaim;

		public static OnClaimPrize onClaimPrize;

		private bool canClaim;

		private const string TIMED_REWARDS_TIME = "TimedRewardsTime";

		private const string FMT = "O";

		private void Start()
		{
			StartCoroutine(InitializeTimer());
		}

		private IEnumerator InitializeTimer()
		{
			yield return StartCoroutine(InitializeDate());
			if (isErrorConnect)
			{
				DailyRewardsCore<TimedRewards>.onInitialize(error: true, errorMessage);
				yield break;
			}
			string @string = PlayerPrefs.GetString("TimedRewardsTime");
			if (!string.IsNullOrEmpty(@string))
			{
				lastRewardTime = DateTime.ParseExact(@string, "O", CultureInfo.InvariantCulture);
				timer = (lastRewardTime - now).Add(TimeSpan.FromSeconds(maxTime));
			}
			else
			{
				timer = TimeSpan.FromSeconds(maxTime);
			}
			DailyRewardsCore<TimedRewards>.onInitialize();
		}

		private void Update()
		{
			if (!isInitialized)
			{
				return;
			}
			now = now.AddSeconds(Time.unscaledDeltaTime);
			if (canClaim)
			{
				return;
			}
			timer = timer.Subtract(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
			if (timer.TotalSeconds <= 0.0)
			{
				canClaim = true;
				if (onCanClaim != null)
				{
					onCanClaim();
				}
			}
			else
			{
				PlayerPrefs.SetString("TimedRewardsTime", now.Add(timer - TimeSpan.FromSeconds(maxTime)).ToString("O"));
			}
		}

		public void ClaimReward(int rewardIdx)
		{
			PlayerPrefs.SetString("TimedRewardsTime", now.Add(timer - TimeSpan.FromSeconds(maxTime)).ToString("O"));
			timer = TimeSpan.FromSeconds(maxTime);
			canClaim = false;
			if (onClaimPrize != null)
			{
				onClaimPrize(rewardIdx);
			}
		}

		public void Reset()
		{
			PlayerPrefs.DeleteKey("TimedRewardsTime");
			canClaim = true;
			timer = TimeSpan.FromSeconds(0.0);
			if (onCanClaim != null)
			{
				onCanClaim();
			}
		}

		public Reward GetReward(int index)
		{
			return rewards[index];
		}
	}
}
