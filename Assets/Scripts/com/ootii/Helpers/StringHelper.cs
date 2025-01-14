using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.ootii.Helpers
{
	public class StringHelper
	{
		public const int MAX_STRINGS = 100;

		public static string[] SharedStrings = new string[100];

		public static string ToSimpleString(int rValue)
		{
			return $"{rValue}";
		}

		public static string ToSimpleString(float rValue)
		{
			return $"{rValue:f2}";
		}

		public static string ToSimpleString(bool rValue)
		{
			if (!rValue)
			{
				return "false";
			}
			return "true";
		}

		public static string ToSimpleString(string rValue)
		{
			return rValue;
		}

		public static string ToSimpleString(Vector2 rValue)
		{
			return $"{rValue.x:f2}, {rValue.y:f2}";
		}

		public static string ToSimpleString(Vector3 rValue)
		{
			return $"{rValue.x:f2}, {rValue.y:f2}, {rValue.z:f2}";
		}

		public static string ToSimpleString(Vector4 rValue)
		{
			return $"{rValue.x:f2}, {rValue.y:f2}, {rValue.z:f2}, {rValue.w:f2}";
		}

		public static string ToSimpleString(Quaternion rValue)
		{
			Vector3 eulerAngles = rValue.eulerAngles;
			return $"{eulerAngles.x:f5}, {eulerAngles.y:f5}, {eulerAngles.z:f5}";
		}

		public static string ToSimpleString(Transform rValue)
		{
			if (rValue == null)
			{
				return "null";
			}
			return $"{rValue.name}";
		}

		public static string ToSimpleString(GameObject rValue)
		{
			if (rValue == null)
			{
				return "null";
			}
			return $"{rValue.name}";
		}

		public static string ToSimpleString(Object rValue)
		{
			if (rValue == null)
			{
				return "null";
			}
			return $"{rValue.name}";
		}

		public static string ToSimpleString(object rValue)
		{
			if (rValue == null)
			{
				return "null";
			}
			return $"{rValue.ToString()}";
		}

		public static string ToString(Vector3 rInput)
		{
			return $"[m:{rInput.magnitude:f6} x:{rInput.x:f6} y:{rInput.y:f6} z:{rInput.z:f6}]";
		}

		public static string ToString(Quaternion rInput)
		{
			Vector3 eulerAngles = rInput.eulerAngles;
			float angle = 0f;
			Vector3 axis = Vector3.zero;
			rInput.ToAngleAxis(out angle, out axis);
			return $"[p:{eulerAngles.x:f4} y:{eulerAngles.y:f4} r:{eulerAngles.z:f4} x:{rInput.x:f7} y:{rInput.y:f7} z:{rInput.z:f7} w:{rInput.w:f7} angle:{angle:f7} axis:{ToString(axis)}]";
		}

		public static string FormatCamelCase(string rInput)
		{
			return Regex.Replace(Regex.Replace(rInput, "(\\P{Ll})(\\P{Ll}\\p{Ll})", "$1 $2"), "(\\p{Ll})(\\P{Ll})", "$1 $2");
		}

		public static string CleanString(string rInput)
		{
			return rInput.Replace(" ", string.Empty).Replace("_", string.Empty).ToLower();
		}

		public static int Split(string rString, char rDelimiter)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < rString.Length; i++)
			{
				if (rString[i] == rDelimiter)
				{
					SharedStrings[num] = rString.Substring(num2, i - num2);
					num++;
					num2 = i + 1;
				}
			}
			SharedStrings[num] = rString.Substring(num2, rString.Length - num2);
			return num + 1;
		}

		public static string[] Split(string rString, string rDelimiter, string rQualifier, bool rIgnoreCase)
		{
			int num = 0;
			bool flag = false;
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < rString.Length - 1; i++)
			{
				if (rQualifier != null && string.Compare(rString.Substring(i, rQualifier.Length), rQualifier, rIgnoreCase) == 0)
				{
					flag = !flag;
				}
				else if (!flag && string.Compare(rString.Substring(i, rDelimiter.Length), rDelimiter, rIgnoreCase) == 0)
				{
					arrayList.Add(rString.Substring(num, i - num));
					num = i + rDelimiter.Length;
				}
			}
			if (num < rString.Length)
			{
				arrayList.Add(rString.Substring(num, rString.Length - num));
			}
			string[] array = new string[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public static string ToTitleCase(string rString)
		{
			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(rString.ToLowerInvariant());
		}
	}
}
