using System;

namespace App.Weapons
{
	[Serializable]
	public struct GunLoad
	{
		public GunType gunType;

		public int ammo;

		public int reserveAmmo;

		public GunLoad(GunType gunType, int ammo, int reserveAmmo)
		{
			this.gunType = gunType;
			this.ammo = ammo;
			this.reserveAmmo = reserveAmmo;
		}

		public int GetTotalAmmo()
		{
			return ammo + reserveAmmo;
		}
	}
}
