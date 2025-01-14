using App.Attributes;
using App.Settings;
using System;
using UnityEngine;

namespace App.Quests
{
	public class Quest : MonoBehaviour
	{
		[QuestID]
		[SerializeField]
		private string questID;

		private ObjectiveData objectiveData = new ObjectiveData();

		private GameplayObjective[] objectives;

		private int objectiveIndex;

		private Action stateChanged;

		private bool isInitialized;

		private bool isFinished;

		public string QuestID => questID;

		public bool IsFinished => GetIsFinished();

		public bool IsFailed => GetIsFailed();

		public GameplayObjective CurrentObjective => GetObjectiveAt(objectiveIndex);

		public GameplayObjective LastObjective => GetObjectiveAt(objectives.Length - 1);

		public QuestSettingsItem Settings
		{
			get;
			private set;
		}

		public void Initialize(QuestSettingsItem settings, bool alreadyFinished)
		{
			if (!isInitialized && settings != null && !(settings.ID != questID))
			{
				Settings = settings;
				isInitialized = true;
				isFinished = alreadyFinished;
			}
		}

		public void Activate(Action stateChanged)
		{
			if (!isInitialized)
			{
				return;
			}
			if (isFinished)
			{
				UnityEngine.Debug.LogErrorFormat("Can not start the finished quest {0}", Settings.ID);
				return;
			}
			DestroyQuestObjects();
			if (objectives == null)
			{
				objectives = GetComponentsInChildren<GameplayObjective>();
				GameplayObjective[] array = objectives;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Initialize();
				}
			}
			SetIsActive(isActive: true);
			ResetObjectives();
			if (CurrentObjective == null)
			{
				UnityEngine.Debug.LogErrorFormat("Missing objective at Index [{0}] Objectives QuestID [{1}] ObjectName [{2}]", objectiveIndex, questID, base.name);
				return;
			}
			this.stateChanged = stateChanged;
			CurrentObjective.Activate(objectiveData, OnCurrentObjectiveChanged);
		}

		public void Deactivate()
		{
			if (objectives != null)
			{
				GameplayObjective[] array = objectives;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Deactivate();
				}
				stateChanged = null;
				SetIsActive(isActive: false);
			}
		}

		public void ResetObjectives()
		{
			GameplayObjective[] array = objectives;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResetState();
			}
			objectiveIndex = 0;
		}

		public void MakeQuestAvailable()
		{
			isFinished = false;
		}

		public void DestroyQuestObjects()
		{
			objectiveData.DestroyRelatedObjects();
		}

		public void CleanQuestData()
		{
			objectiveData.OnQuestFinished();
		}

		private void OnCurrentObjectiveChanged()
		{
			stateChanged();
			switch (CurrentObjective.CurrentState)
			{
			case GameplayObjective.State.Failed:
				ProcessFailure();
				break;
			case GameplayObjective.State.Finished:
				ProcessSuccess();
				break;
			}
		}

		private GameplayObjective GetObjectiveAt(int index)
		{
			if (index < 0)
			{
				return null;
			}
			if (objectives == null)
			{
				return null;
			}
			if (index >= objectives.Length)
			{
				return null;
			}
			return objectives[index];
		}

		private void ProcessFailure()
		{
			Deactivate();
		}

		private void ProcessSuccess()
		{
			if (CurrentObjective == LastObjective)
			{
				objectiveData.OnQuestFinished();
				Deactivate();
			}
			else
			{
				CurrentObjective.Deactivate();
				SetNextObjective();
				CurrentObjective.Activate(objectiveData, OnCurrentObjectiveChanged);
			}
		}

		private void SetNextObjective()
		{
			objectiveIndex++;
		}

		private bool GetIsFinished()
		{
			if (isFinished)
			{
				return true;
			}
			if (CurrentObjective == null || LastObjective == null)
			{
				return false;
			}
			if (CurrentObjective != LastObjective)
			{
				return false;
			}
			isFinished = (CurrentObjective.CurrentState == GameplayObjective.State.Finished);
			return isFinished;
		}

		private bool GetIsFailed()
		{
			if (CurrentObjective == null)
			{
				return false;
			}
			return CurrentObjective.CurrentState == GameplayObjective.State.Failed;
		}

		private void SetIsActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}
	}
}
