using UnityEngine;

namespace App.Util
{
	public class CollectableArmorFactory : MonoBehaviour
	{
		public GameObject armorPrefab;

		public GameObject Create()
		{
			return UnityEngine.Object.Instantiate(armorPrefab);
		}
	}
}
