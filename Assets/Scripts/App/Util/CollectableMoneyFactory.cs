using UnityEngine;

namespace App.Util
{
	public class CollectableMoneyFactory : MonoBehaviour
	{
		public GameObject moneyPrefab;

		public CollectableMoney Create(int value)
		{
			CollectableMoney componentSafe = UnityEngine.Object.Instantiate(moneyPrefab).GetComponentSafe<CollectableMoney>();
			componentSafe.value = value;
			return componentSafe;
		}
	}
}
