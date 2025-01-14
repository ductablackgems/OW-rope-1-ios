using UnityEngine;

public class Explosion : MonoBehaviour
{
	public int Force;

	public int Radius;

	public AudioClip[] Sounds;

	private void Start()
	{
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, Radius);
		if (Sounds.Length != 0)
		{
			AudioSource.PlayClipAtPoint(Sounds[Random.Range(0, Sounds.Length)], base.transform.position);
		}
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider.GetComponent<Rigidbody>())
			{
				collider.GetComponent<Rigidbody>().AddExplosionForce(Force, position, Radius, 3f);
			}
		}
	}
}
