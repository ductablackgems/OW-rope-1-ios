using UnityEngine;

namespace App.SaveSystem
{
	public class SettingsSaveEntity : AbstractSaveEntity
	{
		public float screenSensitivyX;

		public float screenSensitivyY;

		public float sightSetting;

		public float soundVolume;

		public float musicVolume;

		public float graphicQuality;

		public float reklamy;

		public float memoryRam;

		public bool firstLaunch;

		public SettingsSaveEntity(string parentKey, string key)
		{
			GenerateEntityKey(parentKey, key);
		}

		protected override void LoadData()
		{
			screenSensitivyX = GetFloat("screenSensitivyX", 0.38f);
			screenSensitivyY = GetFloat("screenSensitivyY", 0.38f);
			sightSetting = GetFloat("sightSetting", 0.3f);
			soundVolume = GetFloat("soundVolume", 1f);
			musicVolume = GetFloat("musicVolume", 0.35f);
			graphicQuality = GetFloat("graphicQuality", 1f);
			reklamy = GetFloat("reklamy");
			memoryRam = GetFloat("memoryRam", 1024f);
			firstLaunch = GetBool("firstLaunch");
		}

		protected override void SaveData(bool includeChildren)
		{
			SaveParam("screenSensitivyX", screenSensitivyX);
			SaveParam("screenSensitivyY", screenSensitivyY);
			SaveParam("sightSetting", sightSetting);
			SaveParam("soundVolume", soundVolume);
			SaveParam("musicVolume", musicVolume);
			SaveParam("graphicQuality", graphicQuality);
			SaveParam("reklamy", reklamy);
			SaveParam("memoryRam", memoryRam);
			SaveParam("firstLaunch", firstLaunch);
		}

		public override void Delete()
		{
			DeleteParam("screenSensitivyX");
			DeleteParam("screenSensitivyY");
			DeleteParam("sightSetting");
			DeleteParam("soundVolume");
			DeleteParam("musicVolume");
			DeleteParam("graphicQuality");
			DeleteParam("reklamy");
			DeleteParam("memoryRam");
			DeleteParam("firstLaunch");
		}

		public override void Dump()
		{
			UnityEngine.Debug.Log($"{base.EntityKey} screenSensitivyX: {screenSensitivyX}, screenSensitivyY: {screenSensitivyY}, sightSetting: {sightSetting}, soundVolume: {soundVolume}, musicVolume: {musicVolume}, quality: {graphicQuality}, reklamy: {reklamy}, ram: {memoryRam}, firstLaunch {firstLaunch}");
		}
	}
}
