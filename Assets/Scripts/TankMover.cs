using UnityEngine;

public class TankMover : MonoBehaviour
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
		base.transform.Rotate(new Vector3(0f, UnityEngine.Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime, 0f));
		base.transform.position += base.transform.forward * UnityEngine.Input.GetAxis("Vertical") * Speed * Time.deltaTime;
	}
}
