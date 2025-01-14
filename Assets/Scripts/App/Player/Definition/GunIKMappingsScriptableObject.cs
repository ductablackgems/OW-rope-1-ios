using System.Collections.Generic;
using UnityEngine;

namespace App.Player.Definition
{
	public class GunIKMappingsScriptableObject : ScriptableObject
	{
		public List<GunIKMapping> mappings;

		public int GetIndex(GunType gunType)
		{
			for (int i = 0; i < mappings.Count; i++)
			{
				if (mappings[i].gunType == gunType)
				{
					return i;
				}
			}
			return -1;
		}

		public bool TryGetMapping(GunType gunType, out GunIKMapping mapping)
		{
			for (int i = 0; i < mappings.Count; i++)
			{
				if (mappings[i].gunType == gunType)
				{
					mapping = mappings[i];
					return true;
				}
			}
			mapping = default(GunIKMapping);
			return false;
		}
	}
}
