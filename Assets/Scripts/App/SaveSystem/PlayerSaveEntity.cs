using App.Settings;
using System.Collections.Generic;
using _0.Custom.Scripts.Game;
using _0.DucLib.Scripts.Common;
using UnityEngine;

namespace App.SaveSystem
{
	public class PlayerSaveEntity : AbstractSaveEntity
	{
		private const string GunKeyPrefix = "Gun@";

		private readonly GunType[] GunTypes = new GunType[9]
		{
			GunType.Pistol,
			GunType.Revolver,
			GunType.Cq16,
			GunType.Qk47,
			GunType.Carbine,
			GunType.Shotgun,
			GunType.Minigun,
			GunType.Rpg,
			GunType.Flamethrower
		};

		public readonly GunType[] GrenadeTypes = new GunType[2]
		{
			GunType.Grenade,
			GunType.Molotov
		};

		public int score;

		public int bankDeposit;

		public int characterIndex;

		public int colorIndex;

		public float armor;

		public HashSet<string> buyedClothes;

		public HashSet<string> wearedClothes;

		public HashSet<string> finishedQuests;

		public HashSet<string> buyedAirplanes;

		public Dictionary<GunType, GunSaveEntity> guns;

		public GarageSaveEntity[] garages;

		public List<DogSaveEntity> dogs = new List<DogSaveEntity>(8);

		public PlayerSaveEntity(string parentKey, string key)
		{
			GenerateEntityKey(parentKey, key);
			garages = new GarageSaveEntity[4];
			for (int i = 0; i < garages.Length; i++)
			{
				garages[i] = new GarageSaveEntity(base.EntityKey, "garage" + i);
			}
			guns = new Dictionary<GunType, GunSaveEntity>();
			GunType[] gunTypes = GunTypes;
			foreach (GunType gunType in gunTypes)
			{
				GunSaveEntity value = new GunSaveEntity(base.EntityKey, "Gun@" + gunType, gunType, isGrenade: false);
				guns.Add(gunType, value);
			}
			gunTypes = GrenadeTypes;
			foreach (GunType gunType2 in gunTypes)
			{
				GunSaveEntity value2 = new GunSaveEntity(base.EntityKey, "Gun@" + gunType2, gunType2, isGrenade: true);
				guns.Add(gunType2, value2);
			}
			GunSaveEntity value3 = new GunSaveEntity(base.EntityKey, "Gun@" + GunType.StickyRocket, GunType.StickyRocket, isGrenade: false);
			guns.Add(GunType.StickyRocket, value3);
			DogSettingsItem[] array = SettingsManager.DogSettings.Dogs;
			foreach (DogSettingsItem dogSettingsItem in array)
			{
				dogs.Add(new DogSaveEntity(base.EntityKey, dogSettingsItem.ID));
			}
		}

		public GunSaveEntity GetGunSave(GunType gunType)
		{
			if (!guns.TryGetValue(gunType, out GunSaveEntity value))
			{
				UnityEngine.Debug.LogErrorFormat("Gun type '{0}' is unsupported.", gunType);
				return null;
			}
			return value;
		}

		public DogSaveEntity GetDogSave(string settingsID)
		{
			foreach (DogSaveEntity dog in dogs)
			{
				if (dog.SettingsID == settingsID)
				{
					return dog;
				}
			}
			return null;
		}

		protected override void LoadData()
		{
			score = GetInt("score", CustomConfig.CustomConfigValue.startGem);
			characterIndex = GetInt("characterIndex");
			colorIndex = GetInt("colorIndex");
			armor = GetFloat("armor");
			buyedClothes = GetStringHashSet("buyedClothes");
			wearedClothes = GetStringHashSet("wearedClothes");
			finishedQuests = GetStringHashSet("finishedQuests");
			bankDeposit = GetInt("bankDeposit");
			buyedAirplanes = GetStringHashSet("buyedAirplanes");
			for (int i = 0; i < garages.Length; i++)
			{
				garages[i].Load();
			}
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in guns)
			{
				gun.Value.Load();
			}
			foreach (DogSaveEntity dog in dogs)
			{
				dog.Load();
			}
		}

		protected override void SaveData(bool includeChildren)
		{
			SaveParam("score", score);
			SaveParam("characterIndex", characterIndex);
			SaveParam("colorIndex", colorIndex);
			SaveParam("armor", armor);
			SaveParam("buyedClothes", buyedClothes);
			SaveParam("wearedClothes", wearedClothes);
			SaveParam("finishedQuests", finishedQuests);
			SaveParam("bankDeposit", bankDeposit);
			SaveParam("buyedAirplanes", buyedAirplanes);
			if (includeChildren)
			{
				for (int i = 0; i < garages.Length; i++)
				{
					garages[i].Save();
				}
				foreach (KeyValuePair<GunType, GunSaveEntity> gun in guns)
				{
					gun.Value.Save();
				}
				foreach (DogSaveEntity dog in dogs)
				{
					dog.Save();
				}
			}
		}

		public override void Delete()
		{
			DeleteParam("score");
			DeleteParam("characterIndex");
			DeleteParam("colorIndex");
			DeleteParam("armor");
			DeleteParam("buyedClothes");
			DeleteParam("wearedClothes");
			DeleteParam("finishedQuests");
			DeleteParam("bankDeposit");
			DeleteParam("buyedAirplanes");
			for (int i = 0; i < garages.Length; i++)
			{
				garages[i].Delete();
			}
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in guns)
			{
				gun.Value.Delete();
			}
			foreach (DogSaveEntity dog in dogs)
			{
				dog.Delete();
			}
		}

		public override void Dump()
		{
			UnityEngine.Debug.Log(string.Format("{0} score: {1}, characterIndex: {2}, colorIndex: {3}, armor: {4}, " + "buyedClothes(Count): {5}, wearedClothes(Count): {6}, finishedQuest(Count): {7}, " + "bankDeposit: {8}, buyedAirplains: {9}", base.EntityKey, score, characterIndex, colorIndex, armor, buyedClothes.Count, wearedClothes.Count, finishedQuests.Count, bankDeposit, buyedAirplanes.Count));
			for (int i = 0; i < garages.Length; i++)
			{
				garages[i].Dump();
			}
			foreach (KeyValuePair<GunType, GunSaveEntity> gun in guns)
			{
				gun.Value.Dump();
			}
			foreach (DogSaveEntity dog in dogs)
			{
				dog.Dump();
			}
		}
	}
}
