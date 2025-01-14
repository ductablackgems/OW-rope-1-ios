using com.ootii.Helpers;
using System;

namespace com.ootii.Base
{
	public class BaseDescriptionAttribute : Attribute
	{
		protected string mValue;

		public string Value => mValue;

		public BaseDescriptionAttribute(string rValue)
		{
			mValue = rValue;
		}

		public static string GetDescription(Type rType)
		{
			string result = "";
			BaseDescriptionAttribute attribute = ReflectionHelper.GetAttribute<BaseDescriptionAttribute>(rType);
			if (attribute != null)
			{
				result = attribute.Value;
			}
			return result;
		}
	}
}
