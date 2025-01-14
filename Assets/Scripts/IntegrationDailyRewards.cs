using NiobiumStudios;
using System;
using UnityEngine;

public class IntegrationDailyRewards : MonoBehaviour
{
	private void OnEnable()
	{
		DailyRewards.onClaimPrize = (DailyRewards.OnClaimPrize)Delegate.Combine(DailyRewards.onClaimPrize, new DailyRewards.OnClaimPrize(OnClaimPrizeDailyRewards));
	}

	private void OnDisable()
	{
		DailyRewards.onClaimPrize = (DailyRewards.OnClaimPrize)Delegate.Remove(DailyRewards.onClaimPrize, new DailyRewards.OnClaimPrize(OnClaimPrizeDailyRewards));
	}

	public void OnClaimPrizeDailyRewards(int day)
	{
		Reward reward = DailyRewardsCore<DailyRewards>.instance.GetReward(day);
		MonoBehaviour.print(reward.unit);
		MonoBehaviour.print(reward.reward);
	}
}
