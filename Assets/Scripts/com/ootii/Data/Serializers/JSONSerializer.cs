using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Utilities;
using com.ootii.Utilities.Debug;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace com.ootii.Data.Serializers
{
	public class JSONSerializer
	{
		public const string RootObjectID = "[OOTII_ROOT]";

		public static GameObject RootObject;

		public static string Serialize(object rObject, bool rIncludeProperties)
		{
			if (rObject == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append("__Type");
			stringBuilder.Append(" : ");
			stringBuilder.Append("\"");
			stringBuilder.Append(rObject.GetType().AssemblyQualifiedName);
			stringBuilder.Append("\"");
			if (ReflectionHelper.IsPrimitive(rObject.GetType()))
			{
				string value = SerializeValue(rObject);
				stringBuilder.Append(", ");
				stringBuilder.Append("__Value");
				stringBuilder.Append(" : ");
				stringBuilder.Append(value);
			}
			else if (rObject.GetType() == typeof(string))
			{
				string value2 = SerializeValue(rObject);
				stringBuilder.Append(", ");
				stringBuilder.Append("__Value");
				stringBuilder.Append(" : ");
				stringBuilder.Append(value2);
			}
			else
			{
				if (rIncludeProperties)
				{
					PropertyInfo[] properties = rObject.GetType().GetProperties();
					foreach (PropertyInfo propertyInfo in properties)
					{
						if (propertyInfo.CanRead && propertyInfo.CanWrite && !ReflectionHelper.IsDefined(propertyInfo, typeof(SerializationIgnoreAttribute)))
						{
							string name = propertyInfo.Name;
							object obj = null;
							try
							{
								obj = propertyInfo.GetValue(rObject, null);
								if (obj == null)
								{
									continue;
								}
							}
							catch
							{
								obj = rObject;
							}
							string value3 = SerializeValue(obj);
							stringBuilder.Append(", ");
							stringBuilder.Append(name);
							stringBuilder.Append(" : ");
							stringBuilder.Append(value3);
						}
					}
				}
				FieldInfo[] fields = rObject.GetType().GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral && !ReflectionHelper.IsDefined(fieldInfo, typeof(NonSerializedAttribute)) && (fieldInfo.IsPublic || ReflectionHelper.IsDefined(fieldInfo, typeof(SerializeField)) || ReflectionHelper.IsDefined(fieldInfo, typeof(SerializableAttribute))))
					{
						string name2 = fieldInfo.Name;
						object value4 = fieldInfo.GetValue(rObject);
						if (value4 != null)
						{
							string value5 = SerializeValue(value4);
							stringBuilder.Append(", ");
							stringBuilder.Append(name2);
							stringBuilder.Append(" : ");
							stringBuilder.Append(value5);
						}
					}
				}
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static string SerializeValue(string rName, object rValue)
		{
			if (rValue == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(rName);
			stringBuilder.Append(" : ");
			stringBuilder.Append(SerializeValue(rValue));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static Type GetType(string rJSON)
		{
			JSONNode jSONNode = JSONNode.Parse(rJSON);
			if (jSONNode == null)
			{
				return null;
			}
			string value = jSONNode["__Type"].Value;
			if (value == null || value.Length == 0)
			{
				return null;
			}
			return AssemblyHelper.ResolveType(value);
		}

		public static Type GetType(string rJSON, string rTypeKey, out bool rUpdateType)
		{
			rUpdateType = false;
			JSONNode jSONNode = JSONNode.Parse(rJSON);
			if (jSONNode == null)
			{
				return null;
			}
			string value = jSONNode[rTypeKey].Value;
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			return AssemblyHelper.ResolveType(value, out rUpdateType);
		}

		public static T DeserializeValue<T>(string rJSON)
		{
			Type typeFromHandle = typeof(T);
			if (rJSON == null || rJSON.Length == 0)
			{
				return default(T);
			}
			JSONNode jSONNode = JSONNode.Parse(rJSON);
			if (jSONNode == null || jSONNode.Count == 0)
			{
				return default(T);
			}
			object obj = DeserializeValue(typeFromHandle, jSONNode[0]);
			if (obj == null || obj.GetType() != typeFromHandle)
			{
				return default(T);
			}
			return (T)obj;
		}

		public static T Deserialize<T>(string rJSON)
		{
			return (T)Deserialize(rJSON);
		}

		public static object Deserialize(string rJSON)
		{
			JSONNode jSONNode = JSONNode.Parse(rJSON);
			if (jSONNode == null)
			{
				return null;
			}
			string value = jSONNode["__Type"].Value;
			if (value == null || value.Length == 0)
			{
				return null;
			}
			Type type = AssemblyHelper.ResolveType(value);
			if (ReflectionHelper.IsPrimitive(type))
			{
				JSONNode rValue = jSONNode["__Value"];
				return DeserializeValue(type, rValue);
			}
			if (type == typeof(string))
			{
				JSONNode rValue2 = jSONNode["__Value"];
				return DeserializeValue(type, rValue2);
			}
			object obj = null;
			try
			{
				obj = Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log($"JSONSerializer.Deserialize() {value} {ex.Message} {ex.StackTrace}");
			}
			if (obj == null)
			{
				return null;
			}
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.CanWrite || ReflectionHelper.IsDefined(propertyInfo, typeof(SerializationIgnoreAttribute)))
				{
					continue;
				}
				JSONNode jSONNode2 = jSONNode[propertyInfo.Name];
				if (jSONNode2 != null)
				{
					object obj2 = DeserializeValue(propertyInfo.PropertyType, jSONNode2);
					if (obj2 != null)
					{
						propertyInfo.SetValue(obj, obj2, null);
					}
				}
			}
			FieldInfo[] fields = obj.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral || ReflectionHelper.IsDefined(fieldInfo, typeof(NonSerializedAttribute)))
				{
					continue;
				}
				JSONNode jSONNode3 = jSONNode[fieldInfo.Name];
				if (jSONNode3 != null)
				{
					object obj3 = DeserializeValue(fieldInfo.FieldType, jSONNode3);
					if (obj3 != null)
					{
						fieldInfo.SetValue(obj, obj3);
					}
				}
			}
			return obj;
		}

		public static void DeserializeInto(string rJSON, ref object rObject)
		{
			if (rJSON == null || rJSON.Length == 0)
			{
				return;
			}
			JSONNode jSONNode = JSONNode.Parse(rJSON);
			if (jSONNode == null || jSONNode.Count == 0)
			{
				return;
			}
			if (rObject == null)
			{
				string value = jSONNode["__Type"].Value;
				if (value == null || value.Length == 0)
				{
					return;
				}
				try
				{
					rObject = Activator.CreateInstance(AssemblyHelper.ResolveType(value));
				}
				catch (Exception ex)
				{
					Log.ConsoleWriteError($"JSONSerializer.DeserializeInto() {value} {ex.Message} {ex.StackTrace}");
				}
				if (rObject == null)
				{
					return;
				}
			}
			FieldInfo[] fields = rObject.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral || ReflectionHelper.IsDefined(fieldInfo, typeof(NonSerializedAttribute)))
				{
					continue;
				}
				JSONNode jSONNode2 = jSONNode[fieldInfo.Name];
				if (jSONNode2 != null)
				{
					object obj = DeserializeValue(fieldInfo.FieldType, jSONNode2);
					if (obj != null)
					{
						fieldInfo.SetValue(rObject, obj);
					}
				}
			}
			PropertyInfo[] properties = rObject.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.CanWrite || ReflectionHelper.IsDefined(propertyInfo, typeof(SerializationIgnoreAttribute)))
				{
					continue;
				}
				JSONNode jSONNode3 = jSONNode[propertyInfo.Name];
				if (jSONNode3 != null)
				{
					object obj2 = DeserializeValue(propertyInfo.PropertyType, jSONNode3);
					if (obj2 != null)
					{
						propertyInfo.SetValue(rObject, obj2, null);
					}
				}
			}
		}

		private static string SerializeValue(object rValue)
		{
			if (rValue == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder("");
			Type type = rValue.GetType();
			if (type == typeof(string))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append((string)rValue);
				stringBuilder.Append("\"");
			}
			else if (type == typeof(int))
			{
				stringBuilder.Append(((int)rValue).Serialize());
			}
			else if (type == typeof(float))
			{
				stringBuilder.Append(((float)rValue).Serialize());
			}
			else if (type == typeof(bool))
			{
				stringBuilder.Append(((bool)rValue).ToString());
			}
			else if (type == typeof(Vector2))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(((Vector2)rValue).Serialize());
				stringBuilder.Append("\"");
			}
			else if (type == typeof(Vector3))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(((Vector3)rValue).Serialize());
				stringBuilder.Append("\"");
			}
			else if (type == typeof(Vector4))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(((Vector4)rValue).Serialize());
				stringBuilder.Append("\"");
			}
			else if (type == typeof(Quaternion))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(((Quaternion)rValue).Serialize());
				stringBuilder.Append("\"");
			}
			else if (type == typeof(HumanBodyBones))
			{
				stringBuilder.Append(((int)rValue).ToString());
			}
			else if (type == typeof(Transform))
			{
				Transform transform = rValue as Transform;
				if (transform != null)
				{
					string text = (RootObject != null) ? GetFullPath(RootObject.transform) : "";
					string text2 = GetFullPath(transform);
					if ((float)text.Length > 0f)
					{
						text2 = ReplaceFirst(text2, text, "[OOTII_ROOT]");
					}
					stringBuilder.Append("\"");
					stringBuilder.Append(text2);
					stringBuilder.Append("\"");
				}
			}
			else if (type == typeof(GameObject))
			{
				GameObject gameObject = rValue as GameObject;
				if (gameObject != null)
				{
					string text3 = (RootObject != null) ? GetFullPath(RootObject.transform) : "";
					string text4 = GetFullPath(gameObject.transform);
					if ((float)text3.Length > 0f)
					{
						text4 = ReplaceFirst(text4, text3, "[OOTII_ROOT]");
					}
					stringBuilder.Append("\"");
					stringBuilder.Append(text4);
					stringBuilder.Append("\"");
				}
			}
			else if (type == typeof(Component))
			{
				Component component = rValue as Component;
				if (component != null)
				{
					string text5 = (RootObject != null) ? GetFullPath(RootObject.transform) : "";
					string text6 = GetFullPath(component.transform);
					if ((float)text5.Length > 0f)
					{
						text6 = ReplaceFirst(text6, text5, "[OOTII_ROOT]");
					}
					stringBuilder.Append("\"");
					stringBuilder.Append(text6);
					stringBuilder.Append("\"");
				}
			}
			else if (rValue is IList)
			{
				stringBuilder.Append("[");
				for (int i = 0; i < ((IList)rValue).Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(SerializeValue(((IList)rValue)[i]));
				}
				stringBuilder.Append("]");
			}
			else if (rValue is IDictionary)
			{
				stringBuilder.Append("[");
				foreach (object key in ((IDictionary)rValue).Keys)
				{
					string value = SerializeValue(key);
					string value2 = SerializeValue(((IDictionary)rValue)[key]);
					stringBuilder.Append("{ ");
					stringBuilder.Append(value);
					stringBuilder.Append(" : ");
					stringBuilder.Append(value2);
					stringBuilder.Append(" }");
				}
				stringBuilder.Append("]");
			}
			else if (type == typeof(AnimationCurve))
			{
				stringBuilder.Append("\"");
				AnimationCurve animationCurve = rValue as AnimationCurve;
				for (int j = 0; j < animationCurve.keys.Length; j++)
				{
					Keyframe keyframe = animationCurve.keys[j];
					stringBuilder.Append(keyframe.time.ToString("f5", CultureInfo.InvariantCulture) + "|" + keyframe.value.ToString("f5", CultureInfo.InvariantCulture) + "|" + keyframe.tangentMode.ToString(CultureInfo.InvariantCulture) + "|" + keyframe.inTangent.ToString("f5", CultureInfo.InvariantCulture) + "|" + keyframe.outTangent.ToString("f5", CultureInfo.InvariantCulture));
					if (j < animationCurve.keys.Length - 1)
					{
						stringBuilder.Append(";");
					}
				}
				stringBuilder.Append("\"");
			}
			else
			{
				stringBuilder.Append(Serialize(rValue, rIncludeProperties: false));
			}
			return stringBuilder.ToString();
		}

		private static object DeserializeValue(Type rType, JSONNode rValue)
		{
			if (rValue == null)
			{
				return ReflectionHelper.GetDefaultValue(rType);
			}
			if (rType == typeof(string))
			{
				return rValue.Value;
			}
			if (rType == typeof(int))
			{
				return rValue.AsInt;
			}
			if (rType == typeof(float))
			{
				return rValue.AsFloat;
			}
			if (rType == typeof(bool))
			{
				return rValue.AsBool;
			}
			if (rType == typeof(Vector2))
			{
				return Vector2.zero.FromString(rValue.Value);
			}
			if (rType == typeof(Vector3))
			{
				return Vector3.zero.FromString(rValue.Value);
			}
			if (rType == typeof(Vector4))
			{
				return Vector4.zero.FromString(rValue.Value);
			}
			if (rType == typeof(Quaternion))
			{
				return Quaternion.identity.FromString(rValue.Value);
			}
			if (rType == typeof(HumanBodyBones))
			{
				return (HumanBodyBones)rValue.AsInt;
			}
			if (rType == typeof(Transform))
			{
				string text = rValue.Value;
				Transform transform = null;
				if (text.Contains("[OOTII_ROOT]") && RootObject != null)
				{
					text = rValue.Value.Replace("[OOTII_ROOT]", "");
					if (text.Length > 0 && text.Substring(0, 1) == "/")
					{
						text = text.Substring(1);
					}
					transform = ((text.Length == 0) ? RootObject.transform : RootObject.transform.Find(text));
				}
				else
				{
					GameObject gameObject = GameObject.Find(text);
					if (gameObject != null)
					{
						transform = gameObject.transform;
					}
				}
				if (transform == null)
				{
					UnityEngine.Debug.LogWarning("ootii.JSONSerializer.DeserializeValue - Transform name '" + text + "' not found, resulting in null");
					return null;
				}
				return transform;
			}
			if (rType == typeof(GameObject))
			{
				string text2 = rValue.Value;
				Transform transform2 = null;
				if (text2.Contains("[OOTII_ROOT]") && RootObject != null)
				{
					text2 = rValue.Value.Replace("[OOTII_ROOT]", "");
					if (text2.Length > 0 && text2.Substring(0, 1) == "/")
					{
						text2 = text2.Substring(1);
					}
					transform2 = ((text2.Length == 0) ? RootObject.transform : RootObject.transform.Find(text2));
				}
				else
				{
					GameObject gameObject2 = GameObject.Find(text2);
					if (gameObject2 != null)
					{
						transform2 = gameObject2.transform;
					}
				}
				if (transform2 == null)
				{
					UnityEngine.Debug.LogWarning("ootii.JSONSerializer.DeserializeValue - GameObject name '" + text2 + "' not found, resulting in null");
					return null;
				}
				return transform2.gameObject;
			}
			if (ReflectionHelper.IsAssignableFrom(typeof(Component), rType))
			{
				string text3 = rValue.Value;
				Transform transform3 = null;
				if (text3.Contains("[OOTII_ROOT]") && RootObject != null)
				{
					text3 = rValue.Value.Replace("[OOTII_ROOT]", "");
					if (text3.Length > 0 && text3.Substring(0, 1) == "/")
					{
						text3 = text3.Substring(1);
					}
					transform3 = ((text3.Length == 0) ? RootObject.transform : RootObject.transform.Find(text3));
				}
				else
				{
					GameObject gameObject3 = GameObject.Find(text3);
					if (gameObject3 != null)
					{
						transform3 = gameObject3.transform;
					}
				}
				if (transform3 == null)
				{
					UnityEngine.Debug.LogWarning("ootii.JSONSerializer.DeserializeValue - Component  name '" + text3 + "' not found, resulting in null");
					return null;
				}
				return transform3.gameObject.GetComponent(rType);
			}
			if (typeof(IList).IsAssignableFrom(rType))
			{
				IList list = null;
				Type type = rType;
				JSONArray asArray = rValue.AsArray;
				if (ReflectionHelper.IsGenericType(rType))
				{
					type = rType.GetGenericArguments()[0];
					list = (Activator.CreateInstance(rType) as IList);
				}
				else if (rType.IsArray)
				{
					type = rType.GetElementType();
					list = Array.CreateInstance(type, asArray.Count);
				}
				for (int i = 0; i < asArray.Count; i++)
				{
					JSONNode rValue2 = asArray[i];
					object value = DeserializeValue(type, rValue2);
					if (list.Count > i)
					{
						list[i] = value;
					}
					else
					{
						list.Add(value);
					}
				}
				return list;
			}
			if (typeof(IDictionary).IsAssignableFrom(rType))
			{
				if (!ReflectionHelper.IsGenericType(rType))
				{
					return null;
				}
				Type rType2 = rType.GetGenericArguments()[0];
				Type rType3 = rType.GetGenericArguments()[1];
				IDictionary dictionary = Activator.CreateInstance(rType) as IDictionary;
				JSONArray asArray2 = rValue.AsArray;
				for (int j = 0; j < asArray2.Count; j++)
				{
					JSONNode jSONNode = asArray2[j];
					foreach (string key3 in jSONNode.AsObject.Dictionary.Keys)
					{
						object key = DeserializeValue(rType2, key3);
						object value2 = DeserializeValue(rType3, jSONNode[key3]);
						if (dictionary.Contains(key))
						{
							dictionary[key] = value2;
						}
						else
						{
							dictionary.Add(key, value2);
						}
					}
				}
				return dictionary;
			}
			if (rType == typeof(AnimationCurve))
			{
				if (rValue.Value.Length > 0)
				{
					AnimationCurve animationCurve = new AnimationCurve();
					string[] array = rValue.Value.Split(';');
					for (int k = 0; k < array.Length; k++)
					{
						string[] array2 = array[k].Split('|');
						if (array2.Length == 5)
						{
							int result = 0;
							float result2 = 0f;
							Keyframe key2 = default(Keyframe);
							if (float.TryParse(array2[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
							{
								key2.time = result2;
							}
							if (float.TryParse(array2[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
							{
								key2.value = result2;
							}
							if (int.TryParse(array2[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
							{
								key2.tangentMode = result;
							}
							if (float.TryParse(array2[3], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
							{
								key2.inTangent = result2;
							}
							if (float.TryParse(array2[4], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
							{
								key2.outTangent = result2;
							}
							animationCurve.AddKey(key2);
						}
					}
					return animationCurve;
				}
			}
			else if (rType == typeof(Keyframe))
			{
				return new Keyframe(rValue["time"].AsFloat, rValue["value"].AsFloat, rValue["inTangent"].AsFloat, rValue["outTangent"].AsFloat);
			}
			return Deserialize(rValue.ToString());
		}

		private static bool IsSimpleType(Type rType)
		{
			if (rType == typeof(string))
			{
				return true;
			}
			if (rType == typeof(int))
			{
				return true;
			}
			if (rType == typeof(float))
			{
				return true;
			}
			if (rType == typeof(bool))
			{
				return true;
			}
			if (rType == typeof(Vector2))
			{
				return true;
			}
			if (rType == typeof(Vector3))
			{
				return true;
			}
			if (rType == typeof(Vector4))
			{
				return true;
			}
			if (rType == typeof(Quaternion))
			{
				return true;
			}
			if (rType == typeof(HumanBodyBones))
			{
				return true;
			}
			if (rType == typeof(Transform))
			{
				return true;
			}
			return false;
		}

		public static string GetFullPath(Transform rTransform)
		{
			string text = "";
			Transform transform = rTransform;
			while (transform != null)
			{
				if (text.Length > 0)
				{
					text = "/" + text;
				}
				text = transform.name + text;
				transform = transform.parent;
			}
			return text;
		}

		public static string ReplaceFirst(string rText, string rSearch, string rReplace)
		{
			int num = rText.IndexOf(rSearch);
			if (num < 0)
			{
				return rText;
			}
			return rText.Substring(0, num) + rReplace + rText.Substring(num + rSearch.Length);
		}
	}
}
