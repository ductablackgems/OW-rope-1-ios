using UnityEngine;

public class MoverBullet : WeaponBase
{
	public int Lifetime;

	public float Speed = 80f;

	public float SpeedMax = 80f;

	public float SpeedMult = 1f;

	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, Lifetime);
	}

	private void FixedUpdate()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			if (!RigidbodyProjectile)
			{
				GetComponent<Rigidbody>().velocity = base.transform.forward * Speed;
			}
			else if (GetComponent<Rigidbody>().velocity.normalized != Vector3.zero)
			{
				base.transform.forward = GetComponent<Rigidbody>().velocity.normalized;
			}
			if (Speed < SpeedMax)
			{
				Speed += SpeedMult * Time.fixedDeltaTime;
			}
		}
	}
}
