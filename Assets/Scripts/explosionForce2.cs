using System.Collections;
using UnityEngine;

public class explosionForce2 : MonoBehaviour
{
	public float radius = 5f;

	public float power = 200f;

	public float waitTime = 5f;

	public float damage = 150f;

	private AudioSource myaudio;

	public AudioClip[] explodeSounds;

	public bool Timeoff;

	private Vector3 explosionPos;

	private Collider[] colliders;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(1f);
		myaudio = GetComponent<AudioSource>();
		if (!Timeoff)
		{
			UnityEngine.Object.Destroy(base.gameObject, waitTime);
		}
		explosionPos = base.transform.position;
		colliders = Physics.OverlapSphere(explosionPos, radius);
		Collider[] array = colliders;
		foreach (Collider collider in array)
		{
			if (collider.GetComponent<Rigidbody>() != null)
			{
				collider.GetComponent<Rigidbody>().AddExplosionForce(power * 85f, explosionPos, radius, 3f);
			}
		}
	}
}
