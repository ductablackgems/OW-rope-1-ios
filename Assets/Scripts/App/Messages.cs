using System;
using System.Collections.Generic;

namespace App
{
	public class Messages
	{
		private class Message
		{
			public string ID;

			public List<Subscriber> Subscribers = new List<Subscriber>(4);

			public void Remove(object owner, Action<object, object> callback)
			{
				int num = Subscribers.Count;
				Subscriber subscriber;
				do
				{
					if (num-- > 0)
					{
						subscriber = Subscribers[num];
						continue;
					}
					return;
				}
				while (subscriber.Owner != owner || subscriber.Callback != callback);
				Subscribers.RemoveAt(num);
				subscriber.ReturnToCache();
			}

			public void Reset()
			{
				ID = string.Empty;
				Subscribers.Clear();
			}

			public bool IsExistingSubscriber(object owner)
			{
				for (int i = 0; i < Subscribers.Count; i++)
				{
					if (Subscribers[i].Owner == owner)
					{
						return true;
					}
				}
				return false;
			}
		}

		private class Subscriber
		{
			public object Owner;

			public Action<object, object> Callback;

			private Action<Subscriber> returnToCache;

			public Subscriber(Action<Subscriber> returnToCacheCallback)
			{
				returnToCache = returnToCacheCallback;
			}

			public void ReturnToCache()
			{
				returnToCache(this);
			}
		}

		private List<Message> messages = new List<Message>(32);

		private List<Message> messageCache = new List<Message>(32);

		private List<Subscriber> subscriberCache = new List<Subscriber>(32);

		public void Send(string messageID, object sender, object param = null)
		{
			Message message = FindMessage(messageID);
			if (message != null)
			{
				List<Subscriber> subscribers = message.Subscribers;
				for (int i = 0; i < subscribers.Count; i++)
				{
					subscribers[i].Callback(sender, param);
				}
			}
		}

		public void Subscribe(string messageID, object owner, Action<object, object> onMessageReceived)
		{
			Message message = FindMessage(messageID);
			if (message == null)
			{
				message = (GetCachedItem(messageCache) ?? new Message());
				message.ID = messageID;
				messages.Add(message);
			}
			if (!message.IsExistingSubscriber(owner))
			{
				Subscriber cachedSubscriber = GetCachedSubscriber();
				cachedSubscriber.Owner = owner;
				cachedSubscriber.Callback = onMessageReceived;
				message.Subscribers.Add(cachedSubscriber);
			}
		}

		public void Unsubscribe(string messageID, object subscriber, Action<object, object> onMessageReceived)
		{
			Message message = FindMessage(messageID);
			if (message != null)
			{
				message.Remove(subscriber, onMessageReceived);
				if (message.Subscribers.Count == 0)
				{
					ReturnMessage(message);
				}
			}
		}

		private Message FindMessage(string messageID)
		{
			for (int i = 0; i < messages.Count; i++)
			{
				Message message = messages[i];
				if (message.ID == messageID)
				{
					return message;
				}
			}
			return null;
		}

		private void ReturnMessage(Message message)
		{
			messages.Remove(message);
			message.Reset();
			messageCache.Add(message);
		}

		private Subscriber GetCachedSubscriber()
		{
			return GetCachedItem(subscriberCache) ?? new Subscriber(ReturnSubscriber);
		}

		private void ReturnSubscriber(Subscriber subscriber)
		{
			subscriber.Owner = null;
			subscriber.Callback = null;
			subscriberCache.Add(subscriber);
		}

		private static T GetCachedItem<T>(List<T> cache) where T : class
		{
			if (cache.Count == 0)
			{
				return null;
			}
			int index = cache.Count - 1;
			T result = cache[index];
			cache.RemoveAt(index);
			return result;
		}
	}
}
