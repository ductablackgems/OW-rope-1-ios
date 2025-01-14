using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace com.ootii.Helpers
{
	public static class InterfaceHelper
	{
		private static Dictionary<Type, Type[]> mInterfaceTypes;

		static InterfaceHelper()
		{
			mInterfaceTypes = new Dictionary<Type, Type[]>();
		}

		public static T[] GetComponents<T>()
		{
			List<T> list = new List<T>();
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(MonoBehaviour));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is T)
				{
					T item = (T)(object)array[i];
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public static T[] GetComponents<T>(GameObject rGameObject)
		{
			List<T> list = new List<T>();
			Component[] components = rGameObject.GetComponents(typeof(MonoBehaviour));
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i] is T)
				{
					T item = (T)(object)components[i];
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public static T GetComponent<T>(GameObject rGameObject)
		{
			if (rGameObject == null)
			{
				return default(T);
			}
			Type[] interfaceTypes = GetInterfaceTypes(typeof(T));
			if (interfaceTypes == null || interfaceTypes.Length == 0)
			{
				return default(T);
			}
			foreach (Type type in interfaceTypes)
			{
				if (type.IsSubclassOf(typeof(Component)))
				{
					object component = rGameObject.GetComponent(type);
					if (component != null)
					{
						return (T)component;
					}
				}
			}
			return default(T);
		}

		public static T[] FindComponentsOfType<T>()
		{
			Type[] interfaceTypes = GetInterfaceTypes(typeof(T));
			if (interfaceTypes == null || interfaceTypes.Length == 0)
			{
				return null;
			}
			List<T> list = new List<T>();
			foreach (Type type in interfaceTypes)
			{
				if (type.IsSubclassOf(typeof(Component)))
				{
					list.AddRange(UnityEngine.Object.FindObjectsOfType(type).Cast<T>());
				}
			}
			return list.Distinct().ToArray();
		}

		public static Type[] GetInterfaceTypes(Type rInterface)
		{
			if (!rInterface.IsInterface)
			{
				return null;
			}
			if (!mInterfaceTypes.ContainsKey(rInterface))
			{
				Assembly[] assemblies = GetAssemblies();
				if (assemblies != null && assemblies.Length != 0)
				{
					List<Type> list = new List<Type>();
					for (int i = 0; i < assemblies.Length; i++)
					{
						try
						{
							Type[] types = GetTypes(assemblies[i]);
							if (types != null && types.Length != 0)
							{
								for (int j = 0; j < types.Length; j++)
								{
									if (rInterface.IsAssignableFrom(types[j]) && types[j] != rInterface)
									{
										list.Add(types[j]);
									}
								}
							}
						}
						catch
						{
						}
					}
					mInterfaceTypes.Add(rInterface, list.ToArray());
				}
			}
			return mInterfaceTypes[rInterface];
		}

		public static Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public static Type[] GetTypes(this Assembly assembly)
		{
			return assembly.GetTypes();
		}
	}
}
