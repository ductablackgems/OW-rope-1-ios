using System;

namespace FluffyUnderware.Curvy
{
	public static class EnumExt
	{
		public static bool HasFlag(this Enum variable, params Enum[] flags)
		{
			if (flags.Length == 0)
			{
				throw new ArgumentNullException("flags");
			}
			Type type = variable.GetType();
			for (int i = 0; i < flags.Length; i++)
			{
				if (!Enum.IsDefined(type, flags[i]))
				{
					throw new ArgumentException($"Enumeration type mismatch.  The flag is of type '{flags[i].GetType()}', was expecting '{type}'.");
				}
				ulong num = Convert.ToUInt64(flags[i]);
				if ((Convert.ToUInt64(variable) & num) == num)
				{
					return true;
				}
			}
			return false;
		}

		public static T Set<T>(this Enum value, T append)
		{
			return value.Set(append, OnOff: true);
		}

		public static T Set<T>(this Enum value, T append, bool OnOff)
		{
			if (append == null)
			{
				throw new ArgumentNullException("append");
			}
			Type type = value.GetType();
			if (OnOff)
			{
				return (T)Enum.Parse(type, (Convert.ToUInt64(value) | Convert.ToUInt64(append)).ToString());
			}
			return (T)Enum.Parse(type, (Convert.ToUInt64(value) & ~Convert.ToUInt64(append)).ToString());
		}

		public static T SetAll<T>(this Enum value)
		{
			Type type = value.GetType();
			string[] names = Enum.GetNames(type);
			foreach (string value2 in names)
			{
				value.Set(Enum.Parse(type, value2));
			}
			return (T)(object)value;
		}
	}
}
