using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Spawn.Pooling
{
	public class Pooler<T> : MonoBehaviour where T : Component
	{
		private HashSet<T> allItems = new HashSet<T>();

		private HashSet<T> pooledItems = new HashSet<T>();

		private Transform parentForUnused;

		protected HashSet<T> AllItems => allItems;

		public virtual T Pop()
		{
			if (pooledItems.Count == 0)
			{
				return null;
			}
			T val = pooledItems.ElementAt(UnityEngine.Random.Range(0, pooledItems.Count));
			pooledItems.Remove(val);
			val.transform.parent = null;
			return val;
		}

		public void Push(T item)
		{
			pooledItems.Add(item);
			item.transform.parent = parentForUnused;
		}

		public bool OwningItem(T item)
		{
			return allItems.Contains(item);
		}

		public bool IsEmpty()
		{
			return pooledItems.Count == 0;
		}

		public int Count()
		{
			return pooledItems.Count;
		}

		protected virtual void Awake()
		{
			GameObject gameObject = new GameObject("PooledItems");
			gameObject.SetActive(value: false);
			parentForUnused = gameObject.transform;
			parentForUnused.parent = base.transform;
			T[] componentsInChildren = GetComponentsInChildren<T>();
			foreach (T item in componentsInChildren)
			{
				allItems.Add(item);
				Push(item);
			}
		}
	}
}
