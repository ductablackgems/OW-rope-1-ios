using App.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
	public class ServiceLocator
	{
		private static GameObject helperGameobject;

		private static Dictionary<Type, UnityEngine.Object> services;

		private static PlayerModel playerModel;

		private static Messages messages;

		public static Messages Messages => GetMessages();

		public static T Get<T>(bool showError = true) where T : Component
		{
			return (T)Get(typeof(T), showError);
		}

		public static UnityEngine.Object Get(Type type, bool showError = true)
		{
			if (helperGameobject == null)
			{
				Init();
			}
			if (!services.TryGetValue(type, out UnityEngine.Object value))
			{
				value = UnityEngine.Object.FindObjectOfType(type);
				services.Add(type, value);
			}
			if (showError && value == null)
			{
				UnityEngine.Debug.LogError($"Service '{type}' was not found.");
			}
			return value;
		}

		public static GameObject GetGameObject(string tag, bool showError = true)
		{
			GameObject gameObject = null;
			try
			{
				gameObject = GameObject.FindWithTag(tag);
				if (!showError)
				{
					return gameObject;
				}
				if (!(gameObject == null))
				{
					return gameObject;
				}
				UnityEngine.Debug.LogError($"Game object with tag '{tag}' was not found.");
				return gameObject;
			}
			catch (UnityException)
			{
				if (!showError)
				{
					return gameObject;
				}
				UnityEngine.Debug.LogError($"Game object with tag '{tag}' was not found.");
				return gameObject;
			}
		}

		public static GameObject[] GetGameObjects(string tag)
		{
			try
			{
				return GameObject.FindGameObjectsWithTag(tag);
			}
			catch (UnityException)
			{
				return new GameObject[0];
			}
		}

		public static PlayerModel GetPlayerModel()
		{
			if (helperGameobject == null)
			{
				Init();
			}
			return playerModel;
		}

		public static void SendMessage(string messageID, object sender, object data = null)
		{
			Messages.Send(messageID, sender, data);
		}

		public static void SubscibeMessage(string messageID, object subscriber, Action<object, object> callback)
		{
			Messages.Subscribe(messageID, subscriber, callback);
		}

		private static void Init()
		{
			helperGameobject = new GameObject("ServiceLocatorHelper");
			services = new Dictionary<Type, UnityEngine.Object>();
			playerModel = new PlayerModel(GetGameObject("Player", showError: false));
			messages = new Messages();
		}

		private static Messages GetMessages()
		{
			if (helperGameobject == null)
			{
				Init();
			}
			return messages;
		}
	}
}
