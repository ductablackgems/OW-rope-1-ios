using App.Dogs;
using UnityEngine;

namespace App.SaveSystem
{
	public sealed class DogSaveEntity : AbstractSaveEntity
	{
		private static readonly string ExperienceKey = "experience";

		private static readonly string IsDeadKey = "isDead";

		private static readonly string LevelKey = "level";

		private static readonly string IsOwnedKey = "isOwned";

		private static readonly string NameKey = "name";

		private static readonly string SlotKey = "slot";

		public string name;

		public int slot;

		public int experience;

		public int level;

		public bool isDead;

		public bool isOwned;

		public string SettingsID
		{
			get;
			private set;
		}

		public DogManager.DogSlot Slot
		{
			get
			{
				return (DogManager.DogSlot)slot;
			}
			set
			{
				slot = (int)value;
			}
		}

		public DogSaveEntity(string parentKey, string key)
		{
			GenerateEntityKey(parentKey, key);
			SettingsID = key;
		}

		public override void Delete()
		{
			DeleteParam(ExperienceKey);
			DeleteParam(IsDeadKey);
			DeleteParam(LevelKey);
			DeleteParam(IsOwnedKey);
			DeleteParam(NameKey);
			DeleteParam(SlotKey);
		}

		public override void Dump()
		{
			UnityEngine.Debug.LogFormat("Entity Key: {0} SettingsID {1} experience: {2}, Level: {3}, isDead: {4}, IsOwned: {5}, Name: {6}, Slot: {7}", base.EntityKey, SettingsID, experience, level, isDead, isOwned, name, slot);
		}

		protected override void LoadData()
		{
			experience = GetInt(ExperienceKey);
			isDead = GetBool(IsDeadKey);
			level = GetInt(LevelKey);
			isOwned = GetBool(IsOwnedKey);
			name = GetString(NameKey);
			slot = GetInt(SlotKey);
		}

		protected override void SaveData(bool includeChildren)
		{
			SaveParam(ExperienceKey, experience);
			SaveParam(IsDeadKey, isDead);
			SaveParam(LevelKey, level);
			SaveParam(IsOwnedKey, isOwned);
			SaveParam(NameKey, name);
			SaveParam(SlotKey, slot);
		}
	}
}
