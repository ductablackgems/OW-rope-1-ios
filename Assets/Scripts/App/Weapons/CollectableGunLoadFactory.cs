using App.Prefabs;
using System.Collections.Generic;
using UnityEngine;

namespace App.Weapons
{
	public class CollectableGunLoadFactory : MonoBehaviour
	{
		private Dictionary<GunType, GameObject> prefabs;

		private Dictionary<GunType, GameObject> weaponLauncherPrefabs;

		public GameObject Create(GunType gunType)
		{
			if (!prefabs.TryGetValue(gunType, out GameObject value) || !weaponLauncherPrefabs.TryGetValue(gunType, out GameObject value2))
			{
				UnityEngine.Debug.LogErrorFormat("Prefabs for gun type '{0}' not found", gunType);
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(value);
			CollectableGunLoad componentSafe = gameObject.GetComponentSafe<CollectableGunLoad>();
			WeaponLauncher componentSafe2 = value2.GetComponentSafe<WeaponLauncher>();
			componentSafe.gunLoad = new GunLoad(gunType, componentSafe2.AmmoMax, componentSafe2.AmmoCommonReserve);
			return gameObject;
		}

		private void Awake()
		{
			GunPrefabsScriptableObject gunPrefabs = ServiceLocator.Get<PrefabsContainer>().gunPrefabs;
			prefabs = gunPrefabs.GetSortedCollectableGunLoadPrefabs();
			weaponLauncherPrefabs = gunPrefabs.GetSortedWeaponLauncherPrefabs();
		}
	}
}
