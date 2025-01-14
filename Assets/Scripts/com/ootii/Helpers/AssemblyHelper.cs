using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.ootii.Helpers
{
	public static class AssemblyHelper
	{
		private static string _AssemblyInfo;

		private static List<Type> mFoundTypes;

		public static string AssemblyInfo
		{
			get
			{
				if (_AssemblyInfo == null)
				{
					_AssemblyInfo = Assembly.GetAssembly(typeof(AssemblyHelper)).FullName;
				}
				return _AssemblyInfo;
			}
		}

		public static List<Type> FoundTypes
		{
			get
			{
				if (mFoundTypes == null)
				{
					List<Type> list = new List<Type>();
					Assembly[] assemblies = InterfaceHelper.GetAssemblies();
					foreach (Assembly assembly in assemblies)
					{
						list.AddRange(InterfaceHelper.GetTypes(assembly));
					}
					mFoundTypes = (from x in list
						orderby x.Name
						select x).ToList();
				}
				return mFoundTypes;
			}
		}

		public static string GetAssemblyQualifiedName(string rClassName, bool rThisAssembly = true)
		{
			string result = rClassName + ", " + AssemblyInfo;
			if (rThisAssembly)
			{
				return result;
			}
			foreach (Type foundType in FoundTypes)
			{
				if (foundType.FullName == rClassName)
				{
					return foundType.AssemblyQualifiedName;
				}
			}
			return result;
		}

		public static Type ResolveType(string rTypeString)
		{
			bool rNameChanged;
			return ResolveType(rTypeString, out rNameChanged);
		}

		public static Type ResolveType(string rTypeString, out bool rNameChanged)
		{
			rNameChanged = false;
			Type type = Type.GetType(rTypeString);
			if (type != null)
			{
				return type;
			}
			if (rTypeString.Contains(",") && rTypeString.Contains("Version="))
			{
				int length = rTypeString.IndexOf(",", StringComparison.Ordinal);
				rTypeString = rTypeString.Substring(0, length);
			}
			string str = rTypeString;
			rNameChanged = true;
			type = Type.GetType(str + ", " + AssemblyInfo);
			if (type != null)
			{
				return type;
			}
			type = Type.GetType(str + ", Assembly - CSharp, Version = 0.0.0.0, Culture = neutral, PublicKeyToken = null");
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = InterfaceHelper.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (!(assembly.FullName == AssemblyInfo) && !assembly.FullName.Contains("Assembly-CSharp"))
				{
					type = Type.GetType(str + ", " + assembly.FullName);
					if (type != null)
					{
						return type;
					}
				}
			}
			rNameChanged = false;
			return null;
		}
	}
}
