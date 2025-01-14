using System.Collections.Generic;
using UnityEngine;

namespace App.Weapons
{
	public class FpsWeaponManager : MonoBehaviour
	{
		private readonly Dictionary<GunType, FpsWeapon> weaponsByType = new Dictionary<GunType, FpsWeapon>();

		private FpsWeapon[] weapons;

		private void Awake()
		{
			weapons = GetComponentsInChildren<FpsWeapon>(includeInactive: true);
			HideWeapons();
			FpsWeapon[] array = weapons;
			foreach (FpsWeapon fpsWeapon in array)
			{
				weaponsByType[fpsWeapon.type] = fpsWeapon;
			}
		}

		public FpsWeapon FindWeapon(GunType type)
		{
			if (!weaponsByType.TryGetValue(type, out FpsWeapon value))
			{
				return null;
			}
			return value;
		}

		public void HideWeapons()
		{
			FpsWeapon[] array = weapons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}
	}
}
