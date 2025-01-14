using App.Player;
using App.SaveSystem;
using App.Util;
using App.Weapons;
using UnityEngine;

namespace App.GUI
{
	public class CommonHud : MonoBehaviour
	{
		private HUDText hudText;

		private PlayerSaveEntity playerSave;

		private PlayerModel player;

		private int lastMoney;

		private void Awake()
		{
			hudText = this.GetComponentSafe<HUDText>();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			player = ServiceLocator.GetPlayerModel();
			playerSave.OnSave += OnSave;
			player.OnSwitch += OnPlayerSwitch;
			AddEvents(player);
			lastMoney = playerSave.score;
		}

		private void OnDestroy()
		{
			playerSave.OnSave -= OnSave;
			player.OnSwitch -= OnPlayerSwitch;
			RemoveEvents(player);
		}

		private void OnPlayerSwitch(PlayerModel previousModel)
		{
			if (previousModel != null)
			{
				RemoveEvents(previousModel);
			}
			AddEvents(player);
		}

		private void AddEvents(PlayerModel player)
		{
			player.ItemsCollector.OnCollectHealBox += OnCollectHealBox;
			player.ItemsCollector.OnCollectArmor += OnCollectArmor;
			player.WeaponInventory.OnCollectWeapon += OnCollectWeapon;
			player.WeaponInventory.OnCollectAmmo += OnCollectAmmo;
			player.ItemsCollector.OnItemCollected += OnItemCollected;
		}

		private void RemoveEvents(PlayerModel player)
		{
			player.ItemsCollector.OnCollectHealBox -= OnCollectHealBox;
			player.ItemsCollector.OnCollectArmor -= OnCollectArmor;
			player.WeaponInventory.OnCollectWeapon -= OnCollectWeapon;
			player.WeaponInventory.OnCollectAmmo -= OnCollectAmmo;
			player.ItemsCollector.OnItemCollected -= OnItemCollected;
		}

		private void OnSave(AbstractSaveEntity entity)
		{
			if (playerSave.score > lastMoney)
			{
				hudText.Add($"+${playerSave.score - lastMoney:N0}", Color.green, 1.5f);
				lastMoney = playerSave.score;
			}
			else if (playerSave.score < lastMoney)
			{
				hudText.Add($"-${lastMoney - playerSave.score:N0}", Color.white, 1.5f);
				lastMoney = playerSave.score;
			}
		}

		private void OnCollectHealBox(float amount)
		{
			hudText.Add($"+{amount:N0} health", Color.green, 1.5f);
		}

		private void OnCollectWeapon(GunLoad gunLoad, IWeapon weaponLauncher)
		{
			hudText.Add(weaponLauncher.GetGunName(), Color.green, 1.5f);
		}

		private void OnCollectAmmo(GunLoad gunLoad, IWeapon weaponLauncher)
		{
			hudText.Add($"+{gunLoad.GetTotalAmmo():N0} ammo {weaponLauncher.GetGunName()}", Color.green, 1.5f);
		}

		private void OnCollectArmor()
		{
			hudText.Add("+ armor", Color.green, 1.5f);
		}

		private void OnItemCollected(CollectableItem item)
		{
			if (item.NameID != 0)
			{
				string obj = "+ " + LocalizationManager.Instance.GetText(item.NameID);
				hudText.Add(obj, Color.green, 1.5f);
			}
		}
	}
}
