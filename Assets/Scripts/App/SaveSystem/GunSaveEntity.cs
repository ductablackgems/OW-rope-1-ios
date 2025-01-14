using UnityEngine;

namespace App.SaveSystem
{
	public class GunSaveEntity : AbstractSaveEntity
	{
		public readonly GunType GunType;

		public readonly bool IsGrenade;

		public readonly bool IsGun;

		public bool buyed;

		public int ammo;

		public int ammoReserve;

		public GunSaveEntity(string parentKey, string key, GunType gunType, bool isGrenade)
		{
			GenerateEntityKey(parentKey, key);
			GunType = gunType;
			IsGrenade = isGrenade;
			IsGun = (!isGrenade && gunType != GunType.StickyRocket);
		}

		protected override void LoadData()
		{
			buyed = GetBool("buyed", !IsGun);
			ammo = GetInt("ammo");
			ammoReserve = GetInt("ammoReserve");
		}

		protected override void SaveData(bool includeChildren)
		{
			SaveParam("buyed", buyed);
			SaveParam("ammo", ammo);
			SaveParam("ammoReserve", ammoReserve);
		}

		public override void Delete()
		{
			DeleteParam("buyed");
			DeleteParam("ammo");
			DeleteParam("ammoReserve");
		}

		public override void Dump()
		{
			UnityEngine.Debug.Log($"{base.EntityKey} buyed: {buyed}, ammo: {ammo}, ammoReserve: {ammoReserve}");
		}
	}
}
