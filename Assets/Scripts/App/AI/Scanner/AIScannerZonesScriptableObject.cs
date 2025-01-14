using UnityEngine;

namespace App.AI.Scanner
{
	public class AIScannerZonesScriptableObject : ScriptableObject
	{
		public AIScannerZone[] zones;

		public AIScannerZone GetZone(float distance)
		{
			for (int i = 0; i < zones.Length; i++)
			{
				if (distance <= zones[i].maxDistance)
				{
					return zones[i];
				}
			}
			return null;
		}
	}
}
