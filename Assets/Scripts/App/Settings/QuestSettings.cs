using UnityEngine;

namespace App.Settings
{
	public class QuestSettings : ScriptableObject
	{
		public QuestSettingsItem[] Quests;

		public QuestSettingsItem this[string questID] => GetQuest(questID);

		private QuestSettingsItem GetQuest(string questID)
		{
			QuestSettingsItem quest_Internal = GetQuest_Internal(questID);
			if (quest_Internal == null)
			{
				UnityEngine.Debug.LogErrorFormat("QuestID [{0}] does not exist", questID);
			}
			return quest_Internal;
		}

		private QuestSettingsItem GetQuest_Internal(string questID)
		{
			if (string.IsNullOrEmpty(questID))
			{
				return null;
			}
			if (Quests == null)
			{
				return null;
			}
			for (int i = 0; i < Quests.Length; i++)
			{
				QuestSettingsItem questSettingsItem = Quests[i];
				if (questSettingsItem == null)
				{
					UnityEngine.Debug.LogErrorFormat("Null Quest Settings found at index {0}", i);
				}
				else if (questSettingsItem.ID == questID)
				{
					return questSettingsItem;
				}
			}
			return null;
		}
	}
}
