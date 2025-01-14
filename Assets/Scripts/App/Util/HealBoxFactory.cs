using UnityEngine;

namespace App.Util
{
	public class HealBoxFactory : MonoBehaviour
	{
		public GameObject healBoxPrefab;

		public GameObject Create()
		{
			return UnityEngine.Object.Instantiate(healBoxPrefab);
		}
	}
}
