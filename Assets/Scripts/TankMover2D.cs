using UnityEngine;

public class TankMover2D : MonoBehaviour
{
	public float Speed = 20f;

	public float TurnSpeed = 100f;

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
		base.transform.position += Vector3.right * UnityEngine.Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
	}
}
