using App.Util;
using UnityEngine;

public class exploze : MonoBehaviour
{
	public float radius = 5f;

	public float power = 10f;

	public AudioClip[] ragdoly;

	private void OnEnable()
	{
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, radius);
		foreach (Collider collider in array)
		{
			if (collider.name != "Spiderman")
			{
				if (collider.tag == "ragdolly")
				{
					collider.GetComponent<AudioSource>().PlayOneShot(ragdoly[Random.Range(0, ragdoly.Length)]);
				}
				if (collider.tag == "zniceny")
				{
					collider.GetComponent<AudioSource>().Play();
				}
				if (collider.name.StartsWith("Casual") || collider.name.StartsWith("Rebel") || collider.tag == "Vehicle")
				{
					collider.GetComponent<Health>().Kill();
					UnityEngine.Object.Destroy(collider.gameObject);
				}
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(power, position, radius, 3f);
				}
			}
		}
	}
}
