using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace com.ootii.Helpers
{
	public class ReflectionHelper
	{
		public static bool IsSubclassOf(Type rType, Type rBaseType)
		{
			if (rType == rBaseType || rType.IsSubclassOf(rBaseType))
			{
				return true;
			}
			return false;
		}

		public static bool IsAssignableFrom(Type rType, Type rDerivedType)
		{
			if (rType == rDerivedType || rType.IsAssignableFrom(rDerivedType))
			{
				return true;
			}
			return false;
		}

		public static T GetAttribute<T>(Type rObjectType)
		{
			object[] customAttributes = rObjectType.GetCustomAttributes(typeof(T), inherit: true);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				return default(T);
			}
			return (T)customAttributes[0];
		}

		public static T[] GetAttributes<T>(Type rObjectType)
		{
			object[] customAttributes = rObjectType.GetCustomAttributes(typeof(T), inherit: true);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				return new T[1];
			}
			return customAttributes as T[];
		}

		public static bool IsDefined(Type rObjectType, Type rType)
		{
			object[] customAttributes = rObjectType.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsDefined(FieldInfo rFieldInfo, Type rType)
		{
			object[] customAttributes = rFieldInfo.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsDefined(MemberInfo rMemberInfo, Type rType)
		{
			object[] customAttributes = rMemberInfo.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsDefined(PropertyInfo rPropertyInfo, Type rType)
		{
			object[] customAttributes = rPropertyInfo.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				return true;
			}
			return false;
		}

		public static void SetProperty(object rObject, string rName, object rValue)
		{
			PropertyInfo[] properties = rObject.GetType().GetProperties();
			if (properties == null || properties.Length == 0)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < properties.Length)
				{
					if (properties[num].Name == rName && properties[num].CanWrite)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			properties[num].SetValue(rObject, rValue, null);
		}

		public static bool IsTypeValid(string rType)
		{
			try
			{
				return Type.GetType(rType) != null;
			}
			catch
			{
				return false;
			}
		}

		public static bool IsPrimitive(Type rType)
		{
			return rType.IsPrimitive;
		}

		public static bool IsValueType(Type rType)
		{
			return rType.IsValueType;
		}

		public static bool IsGenericType(Type rType)
		{
			return rType.IsGenericType;
		}

		public static object GetDefaultValue(Type rType)
		{
			if (rType.IsValueType)
			{
				return Activator.CreateInstance(rType);
			}
			Vector3 vector = default(Vector3);
			return vector.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(rType)
				.Invoke(vector, null);
		}

		public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> rExpression)
		{
			if (rExpression.Body.NodeType == ExpressionType.MemberAccess)
			{
				MemberExpression memberExpression = rExpression.Body as MemberExpression;
				List<string> list = new List<string>();
				while (memberExpression != null)
				{
					list.Add(memberExpression.Member.Name);
					memberExpression = (memberExpression.Expression as MemberExpression);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int num = list.Count - 1; num >= 0; num--)
				{
					stringBuilder.Append(list[num]);
					if (num > 0)
					{
						stringBuilder.Append('.');
					}
				}
				return stringBuilder.ToString();
			}
			throw new InvalidOperationException();
		}

		public static string GetFieldName<TType, TValue>(TValue rValue)
		{
			EqualityComparer<TValue> lComparer = (EqualityComparer<TValue>)EqualityComparer<TValue>.Default;
			FieldInfo fieldInfo = (from field in typeof(TType).GetFields(BindingFlags.Static | BindingFlags.Public)
				where field.FieldType == typeof(TValue)
				select field).FirstOrDefault((FieldInfo field) => ((EqualityComparer<TValue>)lComparer).Equals(rValue, (TValue)field.GetValue(null)));
			if (!(fieldInfo != null))
			{
				return string.Empty;
			}
			return fieldInfo.Name;
		}
	}
}
