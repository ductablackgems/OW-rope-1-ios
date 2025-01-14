using System.Collections.Generic;
using UnityEngine;

namespace App.SaveSystem
{
	public abstract class AbstractSaveEntity
	{
		public delegate void EntityEventHandler(AbstractSaveEntity entity);

		private string entityKey;

		protected string EntityKey => entityKey;

		public event EntityEventHandler OnLoad;

		public event EntityEventHandler OnSave;

		public void Load()
		{
			LoadData();
			if (this.OnLoad != null)
			{
				this.OnLoad(this);
			}
		}

		public void Save(bool includeChildren = true)
		{
			SaveData(includeChildren);
			if (this.OnSave != null)
			{
				this.OnSave(this);
			}
		}

		public abstract void Delete();

		public abstract void Dump();

		protected abstract void SaveData(bool includeChildren);

		protected abstract void LoadData();

		protected void GenerateEntityKey(string parentKey, string key)
		{
			entityKey = $"{parentKey}>{key}";
		}

		protected string GetString(string key)
		{
			return PlayerPrefs.GetString(GetFullParamKey(key));
		}

		protected int GetInt(string key, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(GetFullParamKey(key), defaultValue);
		}

		protected float GetFloat(string key, float defaultValue = 0f)
		{
			return PlayerPrefs.GetFloat(GetFullParamKey(key), defaultValue);
		}

		protected bool GetBool(string key, bool defaultValue = false)
		{
			return PlayerPrefs.GetInt(GetFullParamKey(key), defaultValue ? 1 : 0) > 0;
		}

		protected string[] GetStringArray(string key)
		{
			return PlayerPrefsX.GetStringArray(GetFullParamKey(key));
		}

		protected HashSet<string> GetStringHashSet(string key)
		{
			return new HashSet<string>(PlayerPrefsX.GetStringArray(GetFullParamKey(key)));
		}

		protected void SaveParam(string key, string value)
		{
			PlayerPrefs.SetString(GetFullParamKey(key), value);
		}

		protected void SaveParam(string key, int value)
		{
			PlayerPrefs.SetInt(GetFullParamKey(key), value);
		}

		protected void SaveParam(string key, float value)
		{
			PlayerPrefs.SetFloat(GetFullParamKey(key), value);
		}

		protected void SaveParam(string key, bool value)
		{
			PlayerPrefs.SetInt(GetFullParamKey(key), value ? 1 : 0);
		}

		protected void SaveParam(string key, string[] value)
		{
			PlayerPrefsX.SetStringArray(GetFullParamKey(key), value);
		}

		protected void SaveParam(string key, HashSet<string> value)
		{
			string[] array = new string[value.Count];
			value.CopyTo(array);
			PlayerPrefsX.SetStringArray(GetFullParamKey(key), array);
		}

		protected void DeleteParam(string key)
		{
			PlayerPrefs.DeleteKey(GetFullParamKey(key));
		}

		private string GetFullParamKey(string key)
		{
			return $"{entityKey}|Param:{key}";
		}
	}
}
