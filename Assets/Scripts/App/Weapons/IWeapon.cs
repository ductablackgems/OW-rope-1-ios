using System;
using UnityEngine;

namespace App.Weapons
{
	public interface IWeapon
	{
		float Range
		{
			get;
		}

		GameObject GetGameObject();

		GunType GetGunType();

		bool IsAutomat();

		void FocusMissileOuterTo(Vector3 target);

		void Shoot();

		Transform GetLeftHandPosition();

		void RegisterOnShot(Action OnShot);

		void UnregisterOnShot(Action OnShot);

		void RegisterOnReload(Action OnReload);

		void UnregisterOnReload(Action OnReload);

		int GetAmmo();

		void SetAmmo(int ammo);

		void SetAmmoReserve(int ammoReserve);

		int GetAmmoReserve();

		int GetAmmoCommonReserve();

		string GetGunName();

		bool Shooting();

		void SetOwner(GameObject owner);
	}
}
