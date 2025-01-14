using UnityEngine;

public class GunHanddle : MonoBehaviour
{
	public WeaponController weapon;

	private void Start()
	{
		weapon = base.transform.GetComponentInChildren(typeof(WeaponController)).GetComponent<WeaponController>();
	}

	private void Update()
	{
		if (Input.GetButton("Fire1") && (bool)weapon)
		{
			weapon.LaunchWeapon();
		}
		Input.GetButton("Fire2");
	}
}
