using App.Missions;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveEarnMoney : GameplayObjective
	{
		[Header("Objective Earn Money")]
		[SerializeField]
		private int earnAmount;

		private MissionManager missionManager;

		private int currentReward;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			missionManager = ServiceLocator.Get<MissionManager>();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			missionManager.MissionFinished += OnMissionFinished;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			Reset_Internal();
		}

		protected override void OnReset()
		{
			base.OnReset();
			Reset_Internal();
		}

		private void OnMissionFinished()
		{
			currentReward += missionManager.GetRewards();
			if (currentReward >= earnAmount)
			{
				Finish();
			}
		}

		private void Reset_Internal()
		{
			currentReward = 0;
			if (!(missionManager == null))
			{
				missionManager.MissionFinished -= OnMissionFinished;
			}
		}
	}
}
