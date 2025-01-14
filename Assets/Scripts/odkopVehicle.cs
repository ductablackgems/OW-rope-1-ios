using UnityEngine;

public class odkopVehicle : MonoBehaviour
{
	public float radius = 5f;

	public float power = 1000f;

	private void Start()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			component.AddExplosionForce(power, base.transform.position, radius, 3f);
		}
	}
}
