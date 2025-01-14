using NiobiumStudios;
using System;
using UnityEngine;

public class IntegrationTimedRewards : MonoBehaviour
{
	private void OnEnable()
	{
		TimedRewards.onClaimPrize = (TimedRewards.OnClaimPrize)Delegate.Combine(TimedRewards.onClaimPrize, new TimedRewards.OnClaimPrize(OnClaimPrizeTimedRewards));
	}

	private void OnDisable()
	{
		TimedRewards.onClaimPrize = (TimedRewards.OnClaimPrize)Delegate.Remove(TimedRewards.onClaimPrize, new TimedRewards.OnClaimPrize(OnClaimPrizeTimedRewards));
	}

	public void OnClaimPrizeTimedRewards(int index)
	{
		Reward reward = DailyRewardsCore<TimedRewards>.instance.GetReward(index);
		MonoBehaviour.print(reward.unit);
		MonoBehaviour.print(reward.reward);
	}
}
