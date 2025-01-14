using com.ootii.Helpers;
using System;

namespace com.ootii.Base
{
	public class BaseNameAttribute : Attribute
	{
		protected string mValue;

		public string Value => mValue;

		public BaseNameAttribute(string rValue)
		{
			mValue = rValue;
		}

		public static string GetName(Type rType)
		{
			string result = rType.Name;
			BaseNameAttribute attribute = ReflectionHelper.GetAttribute<BaseNameAttribute>(rType);
			if (attribute != null)
			{
				result = attribute.Value;
			}
			return result;
		}
	}
}
