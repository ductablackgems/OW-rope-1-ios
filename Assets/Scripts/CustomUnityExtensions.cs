using System.Collections.Generic;
using UnityEngine;

public static class CustomUnityExtensions
{
	private static Queue<Transform> m_Queue = new Queue<Transform>(128);

	public static string GetFullPath(this GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		string text = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return text;
	}

	public static T GetComponentSafe<T>(this Component component)
	{
		T component2 = component.GetComponent<T>();
		if (component2 == null)
		{
			UnityEngine.Debug.LogError($"Component '{typeof(T)}' not found on game object with name '{component.gameObject.name}'.");
		}
		return component2;
	}

	public static T GetComponentSafe<T>(this GameObject gameObject)
	{
		T component = gameObject.GetComponent<T>();
		if (component == null)
		{
			UnityEngine.Debug.LogError($"Component '{typeof(T)}' not found on game object with name '{gameObject.name}'.");
		}
		return component;
	}

	public static T GetComponentInChildrenSafe<T>(this Component component, bool includeInactive = false)
	{
		T componentInChildren = component.GetComponentInChildren<T>(includeInactive);
		if (componentInChildren == null)
		{
			UnityEngine.Debug.LogError($"Component '{typeof(T)}' not found in children of game object with name '{component.gameObject.name}'.");
		}
		return componentInChildren;
	}

	public static T GetComponentInChildrenSafe<T>(this GameObject gameObject, bool includeInactive = false)
	{
		T componentInChildren = gameObject.GetComponentInChildren<T>(includeInactive);
		if (componentInChildren == null)
		{
			UnityEngine.Debug.LogError($"Component '{typeof(T)}' not found in children of game object with name '{gameObject.name}'.");
		}
		return componentInChildren;
	}

	public static T GetComponentInParentsSafe<T>(this Component component)
	{
		T componentInParents = component.GetComponentInParents<T>();
		if (componentInParents == null)
		{
			UnityEngine.Debug.LogError($"Component '{typeof(T)}' not found on parents of game object with name '{component.gameObject.name}'.");
		}
		return componentInParents;
	}

	public static T GetComponentInParentsSafe<T>(this GameObject gameObject)
	{
		T componentInParents = gameObject.GetComponentInParents<T>();
		if (componentInParents == null)
		{
			UnityEngine.Debug.LogError($"Component '{typeof(T)}' not found on parents of game object with name '{gameObject.name}'.");
		}
		return componentInParents;
	}

	public static T GetComponentInParents<T>(this Component component)
	{
		Transform transform = component.transform;
		while (transform != null)
		{
			T component2 = transform.GetComponent<T>();
			if (component2 != null)
			{
				return component2;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	public static T GetComponentInParents<T>(this GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	public static T GetComponentInChildren<T>(this Component @this, string name, bool includeInactive = false) where T : Component
	{
		if (@this == null)
		{
			return null;
		}
		T val = @this.transform.FindComponentDeep<T>(name);
		if ((Object)val == (Object)null)
		{
			return null;
		}
		if (!includeInactive && !val.gameObject.activeSelf)
		{
			return null;
		}
		return val;
	}

	public static bool CompareTagSafe(this GameObject gameObject, string tag)
	{
		if (gameObject == null)
		{
			return false;
		}
		return gameObject.CompareTag(tag);
	}

	private static T FindComponentDeep<T>(this Transform @this, string name) where T : Component
	{
		m_Queue.Clear();
		m_Queue.Enqueue(@this);
		while (m_Queue.Count > 0)
		{
			Transform transform = m_Queue.Dequeue();
			if (transform.name == name)
			{
				T component = transform.GetComponent<T>();
				if ((Object)component != (Object)null)
				{
					return component;
				}
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				m_Queue.Enqueue(transform.GetChild(i));
			}
		}
		return null;
	}
}
