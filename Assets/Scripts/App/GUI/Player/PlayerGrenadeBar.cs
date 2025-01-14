using App.Player;
using App.Player.GrenadeThrow;
using App.SaveSystem;
using App.Util;
using App.Weapons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI.Player
{
	public class PlayerGrenadeBar : MonoBehaviour
	{
		public Text label;

		public ETCButton button;

		public Image image;

		public ETCButton throwGrenadeButton;

		public Image throwGrenadeImage;

		private GrenadeThrowController grenadeThrowController;

		private WeaponInventory weaponInventory;

		private Dictionary<GunType, WeaponInfo> grenadePrefabs;

		private GunType lastGrenadeType;

		private int lastAmmo = -1;

		private void Awake()
		{
			GameObject gameObject = ServiceLocator.GetGameObject("Player");
			grenadeThrowController = gameObject.GetComponentSafe<GrenadeThrowController>();
			weaponInventory = gameObject.GetComponentSafe<WeaponInventory>();
			grenadePrefabs = ServiceLocator.Get<PrefabsContainer>().gunPrefabs.GetSortedGrenadePrefabs();
		}

		private void Update()
		{
			if (InputUtils.SwitchGrenade.IsDown)
			{
				weaponInventory.SwitchGrenade();
			}
			GunSaveEntity grenadeSave = grenadeThrowController.GetGrenadeSave();
			if (grenadeSave == null)
			{
				button.gameObject.SetActive(value: false);
				return;
			}
			button.gameObject.SetActive(value: true);
			if (lastGrenadeType != grenadeThrowController.grenadeType)
			{
				lastGrenadeType = grenadeThrowController.grenadeType;
				if (grenadePrefabs.TryGetValue(lastGrenadeType, out WeaponInfo value))
				{
					image.sprite = value.icon;
					// button.normalSprite = value.icon;
					// button.pressedSprite = value.icon;
					// throwGrenadeImage.sprite = value.icon;
					// throwGrenadeButton.normalSprite = value.icon;
					// throwGrenadeButton.pressedSprite = value.icon;
				}
			}
			if (lastAmmo != grenadeSave.ammo)
			{
				lastAmmo = grenadeSave.ammo;
				label.text = lastAmmo.ToString();
			}
		}
	}
}
