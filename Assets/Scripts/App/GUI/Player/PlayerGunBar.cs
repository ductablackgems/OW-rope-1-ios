using App.Player;
using App.Weapons;
using UnityEngine;

namespace App.GUI.Player
{
	public class PlayerGunBar : MonoBehaviour
	{
		public UILabel label;

		private ShotController shotController;

		private void Awake()
		{
			shotController = ServiceLocator.GetGameObject("Player").GetComponentSafe<ShotController>();
		}

		private void Update()
		{
			IWeapon weapon = shotController.GetWeapon();
			if (!shotController.Running() || weapon == null)
			{
				label.text = string.Format(LocalizationManager.Instance.GetText(4007));
			}
			else if (weapon.GetGunType() == GunType.Flamethrower)
			{
				label.text = $"{weapon.GetGunName()} {weapon.GetAmmo()}%/{weapon.GetAmmoReserve()}";
			}
			else
			{
				label.text = $"{weapon.GetGunName()} {weapon.GetAmmo()}/{weapon.GetAmmoReserve()}";
			}
		}
	}
}
