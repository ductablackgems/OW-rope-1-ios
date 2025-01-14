using UnityEngine;

public class ExplosionObject : MonoBehaviour
{
	public Vector3 Force;

	public GameObject Prefab;

	public int Num;

	public int Scale = 20;

	public AudioClip[] Sounds;

	public float LifeTimeObject = 2f;

	public bool RandomScale;

	private void Start()
	{
		if (Sounds.Length != 0)
		{
			AudioSource.PlayClipAtPoint(Sounds[Random.Range(0, Sounds.Length)], base.transform.position);
		}
		if (!Prefab)
		{
			return;
		}
		for (int i = 0; i < Num; i++)
		{
			Vector3 b = new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 20), UnityEngine.Random.Range(-10, 10)) / 10f;
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab, base.transform.position + b, UnityEngine.Random.rotation);
			UnityEngine.Object.Destroy(gameObject, LifeTimeObject);
			float num = Scale;
			if (RandomScale)
			{
				num = UnityEngine.Random.Range(1, Scale);
			}
			if (num > 0f)
			{
				gameObject.transform.localScale = new Vector3(num, num, num);
			}
			if ((bool)gameObject.GetComponent<Rigidbody>())
			{
				Vector3 force = new Vector3(UnityEngine.Random.Range(0f - Force.x, Force.x), UnityEngine.Random.Range(0f - Force.y, Force.y), UnityEngine.Random.Range(0f - Force.z, Force.z));
				gameObject.GetComponent<Rigidbody>().AddForce(force);
			}
		}
	}
}
