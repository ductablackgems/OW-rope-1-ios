using UnityEngine;

namespace App.Player
{
	public class PlayerLoader : MonoBehaviour
	{
		private void Start()
		{
			this.GetComponentSafe<WeaponInventory>();
		}
	}
}
