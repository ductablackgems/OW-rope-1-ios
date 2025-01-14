using UnityEngine;

namespace App.Settings
{
	public static class SettingsManager
	{
		public static readonly GameSettings GameSettings;

		public static readonly QuestSettings QuestSettings;

		public static readonly DogSettings DogSettings;

		static SettingsManager()
		{
			GameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
			QuestSettings = Resources.Load<QuestSettings>("Settings/QuestSettings");
			DogSettings = Resources.Load<DogSettings>("Settings/DogSettings");
		}
	}
}
