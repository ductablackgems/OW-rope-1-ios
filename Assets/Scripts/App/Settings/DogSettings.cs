using System;
using UnityEngine;

namespace App.Settings
{
	[Serializable]
	public class DogSettings : ScriptableObject
	{
		[SerializeField]
		private DogSettingsItem[] dogs;

		[Space]
		[SerializeField]
		private string defaultDogID;

		[Space]
		[SerializeField]
		private int expPerKill;

		[Range(0f, 1f)]
		[SerializeField]
		private float deathPenalty = 0.2f;

		[SerializeField]
		private int[] expTable;

		public DogSettingsItem this[string dogID] => GetItem(dogID);

		public string DefaultDogID => defaultDogID;

		public DogSettingsItem[] Dogs => dogs;

		public float DeathPenalty => deathPenalty;

		public int ExpPerKill => expPerKill;

		public int GetLevel(int experience)
		{
			int num = expTable.Length;
			while (num-- > 0)
			{
				if (expTable[num] <= experience)
				{
					return num;
				}
			}
			return 0;
		}

		public int GetMaxLevel()
		{
			return expTable.Length;
		}

		private DogSettingsItem GetItem(string itemID)
		{
			DogSettingsItem[] array = dogs;
			foreach (DogSettingsItem dogSettingsItem in array)
			{
				if (dogSettingsItem.ID == itemID)
				{
					return dogSettingsItem;
				}
			}
			return null;
		}
	}
}
