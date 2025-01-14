using App.GUI;
using App.SaveSystem;
using System;
using UnityEngine;

namespace App.Missions
{
	public class MissionManager : MonoBehaviour
	{
		private MiniMap miniMap;

		private MissionLabel missionLabel;

		private MissionTimeLabel missionTimeLabel;

		private PlayerSaveEntity playerSave;

		private int counter;

		private int missionKey;

		private int rewards;

		private Action AfterStopMissionCallback;

		public event Action MissionFinished;

		public int StartMission(string jobText, Action afterStopMissionCallback = null)
		{
			rewards = 0;
			missionTimeLabel.AbortCountdown();
			if (missionKey > 0 && AfterStopMissionCallback != null)
			{
				miniMap.DeactivateTargetCursor();
				AfterStopMissionCallback();
			}
			counter++;
			missionKey = counter;
			AfterStopMissionCallback = afterStopMissionCallback;
			missionLabel.Show(jobText);
			return missionKey;
		}

		public bool SetMapTarget(int missionKey, Vector3 position, TargetCursorType targetCursorType)
		{
			if (this.missionKey != missionKey)
			{
				return false;
			}
			miniMap.ActivateTargetCursor(position, targetCursorType);
			return true;
		}

		public bool CompareMission(int missionKey)
		{
			if (this.missionKey > 0)
			{
				return this.missionKey == missionKey;
			}
			return false;
		}

		public bool FinishMission(int missionKey, bool success)
		{
			if (this.missionKey != missionKey)
			{
				return false;
			}
			miniMap.DeactivateTargetCursor();
			if (success)
			{
				missionLabel.Show(LocalizationManager.Instance.GetText(5006), 5f);
				if (rewards > 0)
				{
					playerSave.score += rewards;
					playerSave.Save();
				}
				if (this.MissionFinished != null)
				{
					this.MissionFinished();
				}
			}
			else
			{
				missionLabel.Show(LocalizationManager.Instance.GetText(5007), 5f);
			}
			this.missionKey = 0;
			rewards = 0;
			missionTimeLabel.AbortCountdown();
			if (AfterStopMissionCallback != null)
			{
				AfterStopMissionCallback();
				AfterStopMissionCallback = null;
			}
			return true;
		}

		public bool SetJobText(int missionKey, string jobText)
		{
			if (this.missionKey != missionKey)
			{
				return false;
			}
			missionLabel.Show(jobText);
			return true;
		}

		public bool SetRewards(int missionKey, int rewards)
		{
			if (this.missionKey != missionKey)
			{
				return false;
			}
			this.rewards = rewards;
			return true;
		}

		public int GetRewards()
		{
			return rewards;
		}

		public bool SetRemainTime(int missionKey, float remainTime)
		{
			if (this.missionKey != missionKey)
			{
				return false;
			}
			missionTimeLabel.StartCountdown(remainTime, delegate
			{
				FinishMission(missionKey, success: false);
			});
			return true;
		}

		private void Awake()
		{
			miniMap = ServiceLocator.Get<MiniMap>();
			SharedGui sharedGui = ServiceLocator.Get<SharedGui>();
			missionLabel = sharedGui.missionText.GetComponentInChildrenSafe<MissionLabel>(includeInactive: true);
			missionTimeLabel = sharedGui.missionText.GetComponentInChildrenSafe<MissionTimeLabel>(includeInactive: true);
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
		}
	}
}
