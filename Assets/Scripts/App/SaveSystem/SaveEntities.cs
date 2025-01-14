using App.GameConfig;
using UnityEngine;

namespace App.SaveSystem
{
	public class SaveEntities : MonoBehaviour
	{
		private const string RootKeyPrefix = "SaveRoot";

		private PlayerSaveEntity playerSave;

		private SettingsSaveEntity settingsSave;

		private bool initialized;

		public PlayerSaveEntity PlayerSave
		{
			get
			{
				if (!initialized)
				{
					Init();
				}
				return playerSave;
			}
		}

		public SettingsSaveEntity SettingsSave
		{
			get
			{
				if (!initialized)
				{
					Init();
				}
				return settingsSave;
			}
		}

		public void Dump()
		{
			playerSave.Dump();
			settingsSave.Dump();
		}

		private void Awake()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void Init()
		{
			initialized = true;
			GameConfigScriptableObject gameConfig = ServiceLocator.Get<ConfigContainer>().gameConfig;
			string str = "";
			switch (gameConfig.title)
			{
			case GameTitle.Stickman:
				str = "Stickman";
				break;
			case GameTitle.Ironman:
				str = "Ironman";
				break;
			case GameTitle.Angel:
				str = "Angel";
				break;
			case GameTitle.FemaleHero:
				str = "FemaleHero";
				break;
			}
			playerSave = new PlayerSaveEntity("SaveRoot" + str, "playerSave");
			playerSave.Load();
			settingsSave = new SettingsSaveEntity("SaveRoot", "settingsSave");
			settingsSave.Load();
		}
	}
}
