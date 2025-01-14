using System.Collections.Generic;

namespace O3DWB
{
	public static class UniqueEntityNameGenerator
	{
		public static string GenerateUniqueName(string desiredEntityName, List<string> existingEntityNames)
		{
			string text = desiredEntityName;
			int num = 0;
			while (existingEntityNames.Contains(text))
			{
				text = desiredEntityName + num;
				num++;
			}
			return text;
		}
	}
}
