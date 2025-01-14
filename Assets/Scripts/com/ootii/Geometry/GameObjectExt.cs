using System;
using System.Reflection;
using UnityEngine;

namespace com.ootii.Geometry
{
	public static class GameObjectExt
	{
		public static bool IsChildOf(this GameObject rThis, GameObject rParent)
		{
			if (rThis == null)
			{
				return false;
			}
			if (rParent == null)
			{
				return false;
			}
			Transform transform = rParent.transform;
			Transform transform2 = rThis.transform;
			while (transform2 != null)
			{
				if (transform2 == transform)
				{
					return true;
				}
				transform2 = transform2.parent;
			}
			return false;
		}

		public static object GetComponentInParents(this GameObject rThis, Type rType)
		{
			if (rThis == null)
			{
				return null;
			}
			Transform transform = rThis.transform;
			while (transform != null)
			{
				Component component = transform.gameObject.GetComponent(rType);
				if (component != null)
				{
					return component;
				}
				transform = transform.parent;
			}
			return null;
		}

		public static T GetComponentInParents<T>(this GameObject rThis) where T : Component
		{
			if (rThis == null)
			{
				return null;
			}
			Transform transform = rThis.transform;
			while (transform != null)
			{
				T component = transform.GetComponent<T>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					return component;
				}
				transform = transform.parent;
			}
			return null;
		}

		public static T GetCopyOf<T>(this Component rThis, T rOther) where T : Component
		{
			Type type = rThis.GetType();
			if (type != rOther.GetType())
			{
				return null;
			}
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			PropertyInfo[] properties = type.GetProperties(bindingAttr);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanWrite)
				{
					try
					{
						propertyInfo.SetValue(rThis, propertyInfo.GetValue(rOther, null), null);
					}
					catch
					{
					}
				}
			}
			FieldInfo[] fields = type.GetFields(bindingAttr);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(rThis, fieldInfo.GetValue(rOther));
			}
			return rThis as T;
		}

		public static T GetOrAddComponent<T>(this Component rComponent) where T : Component
		{
			T val = rComponent.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = rComponent.gameObject.AddComponent<T>();
			}
			return val;
		}

		public static T GetOrAddComponent<T>(this GameObject rGameObject) where T : Component
		{
			T val = rGameObject.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = rGameObject.AddComponent<T>();
			}
			return val;
		}
	}
}
