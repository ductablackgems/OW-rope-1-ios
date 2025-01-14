using App.Player;
using UnityEngine;

namespace App.GUI
{
	public class SwitchWeaponButton : MonoBehaviour
	{
		private WeaponInventory weaponInvetory;

		private void Awake()
		{
			weaponInvetory = ServiceLocator.Get<WeaponInventory>();
		}

		private void OnClick()
		{
			weaponInvetory.SwitchGun();
		}
	}
}
