using App.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace App.Prefabs
{
	public class GunPrefabsScriptableObject : ScriptableObject
	{
		public GameObject[] weaponLauncherPrefabs;

		public GameObject[] collectableGunLoadPrefabs;

		public WeaponInfo[] grenadePrefabs;

		private Dictionary<GunType, GameObject> sortedWeaponLauncherPrefabs;

		private Dictionary<GunType, GameObject> sortedCollectableGunLoadPrefabs;

		private Dictionary<GunType, WeaponInfo> sortedGrenadePrefabs;

		public Dictionary<GunType, GameObject> GetSortedWeaponLauncherPrefabs()
		{
			if (sortedWeaponLauncherPrefabs == null)
			{
				sortedWeaponLauncherPrefabs = new Dictionary<GunType, GameObject>();
				GameObject[] array = weaponLauncherPrefabs;
				foreach (GameObject gameObject in array)
				{
					IWeapon componentSafe = gameObject.GetComponentSafe<IWeapon>();
					sortedWeaponLauncherPrefabs.Add(componentSafe.GetGunType(), gameObject);
				}
			}
			return sortedWeaponLauncherPrefabs;
		}

		public Dictionary<GunType, GameObject> GetSortedCollectableGunLoadPrefabs()
		{
			if (sortedCollectableGunLoadPrefabs == null)
			{
				sortedCollectableGunLoadPrefabs = new Dictionary<GunType, GameObject>();
				GameObject[] array = collectableGunLoadPrefabs;
				foreach (GameObject gameObject in array)
				{
					CollectableGunLoad componentSafe = gameObject.GetComponentSafe<CollectableGunLoad>();
					sortedCollectableGunLoadPrefabs.Add(componentSafe.gunLoad.gunType, gameObject);
				}
			}
			return sortedCollectableGunLoadPrefabs;
		}

		public Dictionary<GunType, WeaponInfo> GetSortedGrenadePrefabs()
		{
			if (sortedGrenadePrefabs == null)
			{
				sortedGrenadePrefabs = new Dictionary<GunType, WeaponInfo>();
				WeaponInfo[] array = grenadePrefabs;
				foreach (WeaponInfo weaponInfo in array)
				{
					sortedGrenadePrefabs.Add(weaponInfo.gunType, weaponInfo);
				}
			}
			return sortedGrenadePrefabs;
		}
	}
}
