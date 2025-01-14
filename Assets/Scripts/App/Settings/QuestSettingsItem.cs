using App.GameConfig;
using App.Quests;
using System;
using UnityEngine;

namespace App.Settings
{
	[Serializable]
	public class QuestSettingsItem : SettingsItem
	{
		[Space]
		public QuestType Type;

		[Space]
		[LocalizeID]
		public int NameID;

		[LocalizeID]
		public int DescriptionID;

		[Header("Rewards")]
		public QuestReward StartReward;

		public QuestReward FinishReward;

		[Header("Availability")]
		public GameTitle[] UnsupportedGames;

		public bool IsGameSupported(GameTitle title)
		{
			for (int i = 0; i < UnsupportedGames.Length; i++)
			{
				if (UnsupportedGames[i] == title)
				{
					return false;
				}
			}
			return true;
		}
	}
}
