using App.GUI;
using App.Missions;
using App.Quests;
using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Vehicles.Car
{
	public class PoliceCarDispatching : MonoBehaviour
	{
		private const float TIME_LIMIT = 120f;

		private MissionManager missionManager;

		private CrimeObjectives crimeObjectives;

		private VehicleModesHandler vehicleModesHandler;

		private int missionKey = -1;

		private DurationTimer objectiveTimer = new DurationTimer();

		private void Awake()
		{
			missionManager = ServiceLocator.Get<MissionManager>();
			crimeObjectives = ServiceLocator.Get<CrimeObjectives>();
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
		}

		private void Update()
		{
			if (vehicleModesHandler.mode != VehicleMode.Player)
			{
				objectiveTimer.Stop();
				return;
			}
			if (objectiveTimer.Done())
			{
				objectiveTimer.Stop();
				crimeObjectives.ActivateRandomObjective(OnObjectiveFinished);
				missionKey = missionManager.StartMission(GetObjectiveText(), OnMissionEnd);
				SetMapTarget(missionKey);
				missionManager.SetRemainTime(missionKey, 120f);
			}
			if (!missionManager.CompareMission(missionKey) && !objectiveTimer.Running())
			{
				objectiveTimer.Run(4f);
			}
		}

		private void OnObjectiveFinished(bool success)
		{
			FinishMission(success);
		}

		private void OnMissionEnd()
		{
			crimeObjectives.DeactivateObjective();
		}

		private string GetObjectiveText()
		{
			if (!(crimeObjectives.CurrentObjective != null))
			{
				return string.Empty;
			}
			return crimeObjectives.CurrentObjective.ShortDecription;
		}

		private void SetMapTarget(int missionKey)
		{
			GameplayObjective currentObjective = crimeObjectives.CurrentObjective;
			if (!(currentObjective == null))
			{
				missionManager.SetMapTarget(missionKey, currentObjective.Position, TargetCursorType.Police);
			}
		}

		private void FinishMission(bool isSuccess)
		{
			if (isSuccess)
			{
				missionManager.SetRewards(missionKey, crimeObjectives.GetRaward());
			}
			missionManager.FinishMission(missionKey, isSuccess);
		}
	}
}
