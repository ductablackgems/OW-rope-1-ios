using App.GameConfig;
using App.GUI;
using App.GUI.Controls;
using App.Rewards;
using App.SaveSystem;
using App.Settings;
using App.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Quests
{
	public class QuestManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject questsContainer;

		[SerializeField]
		private QuestProvider[] questProviders;

		[Space]
		[SerializeField]
		private bool EnableLogs;

		private List<Quest> questList = new List<Quest>(64);

		private RewardManager rewardManager;

		private Pauser pauser;

		private QuestInfoControl questInfo;

		private PlayerSaveEntity playerSave;

		public List<Quest> QuestList => questList;

		public Quest CurrentQuest;

		public QuestProvider[] QuestProviders => questProviders;

		public event Action<Quest> QuestChanged;

		public void StartQuest(string questID)
		{
			DeactivateQuestObjectives();
			CurrentQuest = GetQuest(questID);
			if (!(CurrentQuest == null))
			{
				CurrentQuest.Activate(OnCurrentQuestStateChanged);
			}
		}

		public void AbandonCurrentQuest()
		{
			if (CurrentQuest == null)
			{
				UnityEngine.Debug.LogError("Trying to Abandon Current Quest, but the Quest is not set");
				return;
			}
			LogFormat("Quest Abandoned [{0}]", CurrentQuest.Settings.ID);
			CurrentQuest.DestroyQuestObjects();
			DeactivateQuestObjectives();
			CurrentQuest = null;
		}

		private void Awake()
		{
			Initialize();
		}

		private void OnCurrentQuestStateChanged()
		{
			LogFormat("Quest [{0}] Objective [{1}] State [{2}]", CurrentQuest.Settings.ID, CurrentQuest.CurrentObjective.name, CurrentQuest.CurrentObjective.CurrentState);
			if (this.QuestChanged != null)
			{
				this.QuestChanged(CurrentQuest);
			}
			UpdateQuestInfo(CurrentQuest.CurrentObjective);
			if (CurrentQuest.IsFinished)
			{
				FinishCurrentQuest();
			}
			else if (CurrentQuest.IsFailed)
			{
				FailCurrentQuest();
			}
			else
			{
				TryShowObjectiveNotification();
			}
		}

		private void OnSuccessDialogClose()
		{
			DeactivateQuestObjectives();
			pauser.Resume();
			rewardManager.AssignQuestReward(CurrentQuest);
			CurrentQuest = null;
		}

		private void OnFailedDialogClose()
		{
			pauser.Resume();
		}

		private void GetQuestList(QuestProvider provider, List<Quest> results)
		{
			results.Clear();
			LoadQuests(provider.QuestType, results);
			if (results.Count <= 0)
			{
				foreach (Quest quest in questList)
				{
					if (quest.Settings.Type == provider.QuestType)
					{
						MakeQuestAvailable(quest);
					}
				}
				LoadQuests(provider.QuestType, results);
			}
		}

		private void LoadQuests(QuestType type, List<Quest> results)
		{
			foreach (Quest quest in questList)
			{
				if (!GetIsQuestFinished(quest.QuestID) && quest.Settings.Type == type)
				{
					results.Add(quest);
				}
			}
		}

		private void MakeQuestAvailable(Quest quest)
		{
			quest.MakeQuestAvailable();
			playerSave.finishedQuests.Remove(quest.Settings.ID);
		}

		private void Initialize()
		{
			rewardManager = ServiceLocator.Get<RewardManager>();
			pauser = ServiceLocator.Get<Pauser>();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			SharedGui sharedGui = ServiceLocator.Get<SharedGui>();
			questInfo = ((sharedGui.questInfo != null) ? sharedGui.questInfo.GetComponent<QuestInfoControl>() : null);
			LoadQuests();
			LoadQuestProviders();
		}

		private Quest GetQuest(string questID)
		{
			foreach (Quest quest in questList)
			{
				if (quest.QuestID == questID)
				{
					return quest;
				}
			}
			UnityEngine.Debug.LogError("Game does not define the Quest with QuestID " + questID);
			return null;
		}

		private void DeactivateQuestObjectives()
		{
			if (!(CurrentQuest == null))
			{
				CurrentQuest.Deactivate();
			}
		}

		private void FinishCurrentQuest()
		{
			LogFormat("Quest [{0}] Finished", CurrentQuest.Settings.ID);
			if (CurrentQuest.CurrentObjective.CurrentState != 0)
			{
				MarkQuestFinished(CurrentQuest);
				(pauser.PauseWithDialog(PanelType.QuestCompleted) as QuestCompletedPanel).Show(CurrentQuest, OnSuccessDialogClose);
			}
		}

		private void FailCurrentQuest()
		{
			LogFormat("Quest [{0}] Failed", CurrentQuest.Settings.ID);
			if (CurrentQuest.CurrentObjective.CurrentState != 0)
			{
				CurrentQuest.CleanQuestData();
				DeactivateQuestObjectives();
				(pauser.PauseWithDialog(PanelType.QuestFailed) as QuestFailedPanel).Show(CurrentQuest, OnFailedDialogClose);
				CurrentQuest = null;
			}
		}

		private void LoadQuests()
		{
			if (questsContainer == null)
			{
				return;
			}
			GameTitle title = ServiceLocator.Get<ConfigContainer>().gameConfig.title;
			Quest[] componentsInChildren = questsContainer.GetComponentsInChildren<Quest>(includeInactive: true);
			questList.Clear();
			QuestSettingsItem[] quests = SettingsManager.QuestSettings.Quests;
			foreach (QuestSettingsItem settings in quests)
			{
				if (!settings.IsDisabled && settings.IsGameSupported(title))
				{
					Quest quest = Array.Find(componentsInChildren, (Quest obj) => obj.QuestID == settings.ID);
					if (quest == null)
					{
						UnityEngine.Debug.LogWarning("Scene does not contain the Quest with ID " + settings.ID);
						continue;
					}
					bool isQuestFinished = GetIsQuestFinished(settings.ID);
					quest.Initialize(settings, isQuestFinished);
					questList.Add(quest);
					quest.gameObject.SetActive(value: false);
				}
			}
		}

		private bool GetIsQuestFinished(string questID)
		{
			HashSet<string> finishedQuests = playerSave.finishedQuests;
			if (finishedQuests == null)
			{
				return false;
			}
			foreach (string item in finishedQuests)
			{
				if (item == questID)
				{
					return true;
				}
			}
			return false;
		}

		private void MarkQuestFinished(Quest quest)
		{
			if (!(quest == null) && quest.Settings.Type != QuestType.Repeatable && !playerSave.finishedQuests.Contains(quest.Settings.ID))
			{
				playerSave.finishedQuests.Add(quest.Settings.ID);
				playerSave.Save();
			}
		}

		private void LoadQuestProviders()
		{
			QuestProvider[] array = questProviders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Initialize(this, GetQuestList);
			}
		}

		private void TryShowObjectiveNotification()
		{
			GameplayObjective currentObjective = CurrentQuest.CurrentObjective;
			if (currentObjective.IsActive && !string.IsNullOrEmpty(currentObjective.LongDescription))
			{
				QuestNotificationPanel obj = pauser.PauseWithDialog(PanelType.QuestNotification) as QuestNotificationPanel;
				QuestProvider provider = GetProvider(CurrentQuest);
				string name = (currentObjective.HeaderTextID != 0) ? LocalizationManager.Instance.GetText(currentObjective.HeaderTextID) : ((provider != null) ? provider.Name : string.Empty);
				obj.SetText(name, currentObjective.LongDescription, OnQuestNotificationClose);
			}
		}

		private void OnQuestNotificationClose()
		{
			pauser.Resume();
		}

		private QuestProvider GetProvider(Quest quest)
		{
			for (int i = 0; i < questProviders.Length; i++)
			{
				QuestProvider questProvider = questProviders[i];
				if (questProvider.Offer.Contains(quest))
				{
					return questProvider;
				}
			}
			return null;
		}

		private void UpdateQuestInfo(GameplayObjective objective)
		{
			if (!(questInfo == null))
			{
				if (objective.CurrentState == GameplayObjective.State.Active)
				{
					questInfo.Show(objective);
				}
				else
				{
					questInfo.Hide();
				}
			}
		}

		private void LogFormat(string text, params object[] args)
		{
		}
	}
}
