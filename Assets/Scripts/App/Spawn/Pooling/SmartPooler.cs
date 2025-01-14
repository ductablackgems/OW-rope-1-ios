using System.Collections.Generic;
using UnityEngine;

namespace App.Spawn.Pooling
{
	public class SmartPooler : MonoBehaviour
	{
		private Dictionary<int, Stack<GameObject>> pooledObjects = new Dictionary<int, Stack<GameObject>>();

		private Transform inactiveParent;

		public GameObject Pop(GameObject prefab, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
		{
			Poolable poolable;
			return Pop(prefab, out poolable, position, rotation);
		}

		public GameObject Pop(GameObject prefab, out Poolable poolable, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
		{
			if (!pooledObjects.TryGetValue(prefab.GetInstanceID(), out Stack<GameObject> value))
			{
				value = new Stack<GameObject>();
				pooledObjects.Add(prefab.GetInstanceID(), value);
			}
			GameObject gameObject = (value.Count > 0) ? value.Pop() : null;
			if (gameObject == null)
			{
				gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);
				poolable = gameObject.GetComponent<Poolable>();
				if (poolable == null)
				{
					poolable = gameObject.AddComponent<Poolable>();
				}
				poolable.Register(value, inactiveParent);
				poolable.IsNew = true;
			}
			else
			{
				poolable = gameObject.GetComponent<Poolable>();
				poolable.IsNew = false;
				gameObject.transform.position = position;
				gameObject.transform.rotation = rotation;
			}
			gameObject.transform.parent = null;
			if (poolable.GetResetor() != null)
			{
				poolable.GetResetor().ResetStates();
			}
			return gameObject;
		}

		private void Awake()
		{
			inactiveParent = new GameObject("InactiveParent").transform;
			inactiveParent.gameObject.SetActive(value: false);
			inactiveParent.parent = base.transform;
		}
	}
}
