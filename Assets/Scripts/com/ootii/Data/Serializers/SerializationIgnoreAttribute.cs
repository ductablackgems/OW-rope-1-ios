using System;

namespace com.ootii.Data.Serializers
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public class SerializationIgnoreAttribute : Attribute
	{
	}
}
