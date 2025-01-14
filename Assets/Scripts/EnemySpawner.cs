using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject ObjectSpawn;

	public float ScaleMult = 1f;

	public bool RandomScale = true;

	private float timeSpawnTemp;

	public float TimeSpawn = 20f;

	public float ObjectCount;

	public int Radiun;

	public float Delay = 3f;

	private int numberOfSpawned;

	private void Start()
	{
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = false;
		}
		timeSpawnTemp = Time.time + Delay;
	}

	private void Update()
	{
		if ((bool)ObjectSpawn && (float)GameObject.FindGameObjectsWithTag("Enemy").Length < ObjectCount && Time.time >= timeSpawnTemp + TimeSpawn)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ObjectSpawn, base.transform.position + new Vector3(UnityEngine.Random.Range(-Radiun, Radiun), base.transform.position.y, UnityEngine.Random.Range(-Radiun, Radiun)), Quaternion.identity);
			if ((bool)gameObject.GetComponent<DamageManager>())
			{
				gameObject.GetComponent<DamageManager>().HP += (int)((float)numberOfSpawned * 1.7f);
			}
			if ((bool)gameObject.GetComponent<EnemyDead>())
			{
				gameObject.GetComponent<EnemyDead>().MoneyPlus += (int)((float)numberOfSpawned * 0.1f);
			}
			float num = gameObject.transform.localScale.x;
			if (RandomScale)
			{
				num = (float)UnityEngine.Random.Range(0, 100) * 0.01f;
			}
			gameObject.transform.localScale = new Vector3(num, num, num) * ScaleMult;
			timeSpawnTemp = Time.time;
			numberOfSpawned++;
		}
	}
}
