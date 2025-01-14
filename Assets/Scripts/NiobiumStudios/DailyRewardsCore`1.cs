using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace NiobiumStudios
{
	public abstract class DailyRewardsCore<T> : MonoBehaviour where T : Component
	{
		public delegate void OnInitialize(bool error = false, string errorMessage = "");

		public bool isSingleton = true;

		public string worldClockUrl = "http://worldclockapi.com/api/json/est/now";

		public string worldClockFMT = "yyyy-MM-dd'T'HH:mmzzz";

		public bool useWorldClockApi = true;

		public WorldClock worldClock;

		public string errorMessage;

		public bool isErrorConnect;

		public DateTime now;

		public int maxRetries = 3;

		private int connectrionRetries;

		public static OnInitialize onInitialize;

		public bool isInitialized;

		private static T _instance;

		public static T instance
		{
			get
			{
				if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
				{
					_instance = UnityEngine.Object.FindObjectOfType<T>();
					if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
					{
						_instance = new GameObject
						{
							hideFlags = HideFlags.HideAndDontSave
						}.AddComponent<T>();
					}
				}
				return _instance;
			}
		}

		public IEnumerator InitializeDate()
		{
			if (useWorldClockApi)
			{
				WWW www = new WWW(worldClockUrl);
				while (!www.isDone)
				{
					yield return null;
				}
				if (!string.IsNullOrEmpty(www.error))
				{
					if (connectrionRetries < maxRetries)
					{
						connectrionRetries++;
						UnityEngine.Debug.LogError("Error Loading World Clock. Retrying connection " + connectrionRetries);
						yield return StartCoroutine(InitializeDate());
					}
					else
					{
						isErrorConnect = true;
						UnityEngine.Debug.LogError("Error Loading World Clock:" + www.error);
						errorMessage = www.error;
					}
				}
				else
				{
					string text = www.text;
					worldClock = JsonUtility.FromJson<WorldClock>(text);
					string currentDateTime = worldClock.currentDateTime;
					now = DateTime.ParseExact(currentDateTime, worldClockFMT, CultureInfo.InvariantCulture);
					now = now.AddSeconds(DateTime.Now.Second);
					string str = $"{now.Year:D4}/{now.Month:D2}/{now.Day:D2} {now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";
					MonoBehaviour.print("World Clock Time: " + str);
					isInitialized = true;
				}
			}
			else
			{
				now = DateTime.Now;
				isInitialized = true;
			}
		}

		protected virtual void Awake()
		{
			if (isSingleton)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
			{
				_instance = (this as T);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
