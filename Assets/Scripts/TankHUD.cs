using UnityEngine;

public class TankHUD : MonoBehaviour
{
	private int currentWeapon;

	private WeaponController weaponManager;

	private void Start()
	{
		weaponManager = GetComponent<WeaponController>();
	}
}
