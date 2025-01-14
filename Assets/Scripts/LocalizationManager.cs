using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LocalizationManager
{
	private const string DEFAULT_LANGUAGE = "en";

	private static LocalizationManager m_Instance;

	private TextAsset m_CurrentAsset;

	private Dictionary<int, string> m_Database = new Dictionary<int, string>(128);

	private Dictionary<int, string> m_DefaultDatabase = new Dictionary<int, string>(128);

	private string m_CurrentLanguage;

	public static LocalizationManager Instance
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance;
			}
			m_Instance = new LocalizationManager();
			return m_Instance;
		}
	}

	private LocalizationManager()
	{
		Load_Internal("en", m_DefaultDatabase);
	}

	public void LoadSystemLanguage()
	{
		string language = "en";
		switch (Application.systemLanguage)
		{
		case SystemLanguage.French:
			language = "fr";
			break;
		case SystemLanguage.Spanish:
			language = "esp";
			break;
		case SystemLanguage.Portuguese:
			language = "pt";
			break;
		case SystemLanguage.Czech:
			language = "cs";
			break;
		case SystemLanguage.Chinese:
			language = "cn";
			break;
		case SystemLanguage.German:
			language = "de";
			break;
		case SystemLanguage.Italian:
			language = "it";
			break;
		case SystemLanguage.Korean:
			language = "kr";
			break;
		case SystemLanguage.Russian:
			language = "ru";
			break;
		case SystemLanguage.Japanese:
			language = "jp";
			break;
		}
		Load(language);
	}

	public void ReloadLanguage()
	{
		m_CurrentLanguage = string.Empty;
		LoadSystemLanguage();
	}

	public void Load(string language)
	{
		Load_Internal(language, m_Database);
	}

	public string GetText(int key)
	{
		if (key == 0)
		{
			return string.Empty;
		}
		string value = string.Empty;
		m_Database.TryGetValue(key, out value);
		if (string.IsNullOrEmpty(value))
		{
			m_DefaultDatabase.TryGetValue(key, out value);
		}
		return value;
	}

	private void Load_Internal(string language, Dictionary<int, string> database)
	{
		if (!(language == m_CurrentLanguage))
		{
			m_CurrentAsset = Resources.Load<TextAsset>($"Texts/{language}");
			if (!(m_CurrentAsset == null))
			{
				string[] lines = m_CurrentAsset.text.Split(new char[2]
				{
					'\r',
					'\n'
				}, StringSplitOptions.RemoveEmptyEntries);
				CreateDatabase(lines, database);
				m_CurrentLanguage = language;
			}
		}
	}

	private void CreateDatabase(string[] lines, Dictionary<int, string> database)
	{
		database.Clear();
		foreach (string text in lines)
		{
			string[] array = text.Split(' ');
			if (array.Length >= 2)
			{
				int result = 0;
				string text2 = array[0];
				if (int.TryParse(text2, out result))
				{
					string value = text.Remove(0, text2.Length + 1);
					database.Add(result, value);
				}
			}
		}
	}
}
