using App.Quests;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Missions
{
	public class CrimeObjectives : MonoBehaviour
	{
		[SerializeField]
		private int rewardEliminate = 1000;

		[SerializeField]
		private int rewardDestroyVehicle = 1000;

		private List<GameplayObjective> allObjectives = new List<GameplayObjective>(16);

		private List<GameplayObjective> availableObjectives = new List<GameplayObjective>(16);

		private ObjectiveData data = new ObjectiveData();

		private Action<bool> onObjectiveFinished;

		public GameplayObjective CurrentObjective
		{
			get;
			private set;
		}

		public void ActivateRandomObjective(Action<bool> objectiveFinishedCallback)
		{
			onObjectiveFinished = objectiveFinishedCallback;
			data.CleanNPC();
			data.CleanVehicle(destroy: true);
			CurrentObjective = GetRandomObjective();
			CurrentObjective.Initialize();
			CurrentObjective.Activate(data, OnObjectiveChanged);
			availableObjectives.Remove(CurrentObjective);
		}

		public void DeactivateObjective()
		{
			if (!(CurrentObjective == null) && CurrentObjective.IsActive)
			{
				CleanObjectve();
			}
		}

		public int GetRaward()
		{
			if (CurrentObjective is ObjectiveKillTarget)
			{
				return rewardEliminate;
			}
			return rewardDestroyVehicle;
		}

		private void Awake()
		{
			GetComponentsInChildren(includeInactive: true, allObjectives);
			foreach (GameplayObjective allObjective in allObjectives)
			{
				allObjective.Deactivate();
			}
		}

		private void OnObjectiveChanged()
		{
			if (!(CurrentObjective == null))
			{
				GameplayObjective.State currentState = CurrentObjective.CurrentState;
				if (currentState != GameplayObjective.State.Active && currentState != 0)
				{
					onObjectiveFinished(currentState == GameplayObjective.State.Finished);
					CleanObjectve();
				}
			}
		}

		private GameplayObjective GetRandomObjective()
		{
			if (availableObjectives.Count == 0)
			{
				availableObjectives.AddRange(allObjectives);
			}
			int index = UnityEngine.Random.Range(0, availableObjectives.Count);
			return availableObjectives[index];
		}

		private void CleanObjectve()
		{
			if (!(CurrentObjective == null))
			{
				CurrentObjective.Deactivate();
				CurrentObjective = null;
				data.CleanNPC();
				data.CleanVehicle();
			}
		}
	}
}
