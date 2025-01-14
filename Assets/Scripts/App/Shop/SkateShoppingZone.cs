using App.GUI;
using App.Prefabs;
using App.SaveSystem;
using App.Spawn;
using App.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shop
{
	public class SkateShoppingZone : ShoppingZone
	{
		private List<Transform> spawnpoints = new List<Transform>(16);

		private VehicleSpawner spawner;

		private int layerMask;

		private PlayerSaveEntity playerSave;

		private VehiclePrefabId[] prefabs;

		public bool BuyItem(SkateShopItem item)
		{
			if (item == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(item.ID))
			{
				UnityEngine.Debug.LogErrorFormat("TID not assigned for SkateShopItem {0}", base.name);
				return false;
			}
			if (playerSave.score < item.price)
			{
				return false;
			}
			VehiclePrefabId vehiclePrefabId = Array.Find(prefabs, (VehiclePrefabId obj) => obj.tid == item.ID);
			if (vehiclePrefabId == null)
			{
				UnityEngine.Debug.LogErrorFormat("Unknown Skateboard prefab with ID {0}", item.ID);
				return false;
			}
			playerSave.score -= item.price;
			playerSave.Save();
			Spawn(vehiclePrefabId);
			return true;
		}

		private void Spawn(VehiclePrefabId prefab)
		{
			Transform spawnPoint = GetSpawnPoint();
			spawner.SpawnVehicle(spawnPoint.transform.position, prefab.gameObject).transform.forward = spawnPoint.forward;
		}

		protected override PanelType GetPanelType()
		{
			return PanelType.SkateShop;
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			GetComponentsInChildren(spawnpoints);
			spawnpoints.RemoveAt(0);
			spawner = ServiceLocator.Get<VehicleSpawner>();
			layerMask = LayerMask.GetMask("Player", "Impact");
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			prefabs = ServiceLocator.Get<PrefabsContainer>().vehiclePrefabs.skateboardPrefabs;
			spawner = ServiceLocator.Get<VehicleSpawner>();
			SkateShopItem[] componentsInChildren = sliderControl.GetComponentsInChildren<SkateShopItem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Initialize(this);
			}
		}

		private Transform GetSpawnPoint()
		{
			int index = UnityEngine.Random.Range(0, spawnpoints.Count);
			Transform result = spawnpoints[index];
			foreach (Transform spawnpoint in spawnpoints)
			{
				if (PhysicsUtils.IsValidPosition(spawnpoint.position, 1f, layerMask))
				{
					return spawnpoint;
				}
			}
			return result;
		}
	}
}
