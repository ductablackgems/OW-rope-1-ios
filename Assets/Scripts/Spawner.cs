using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject ObjectSpawn;

	public float ScaleMult = 1f;

	public bool RandomScale = true;

	private float timeSpawnTemp;

	public float TimeSpawn = 20f;

	public float ObjectCount;

	public int Radiun;

	public bool LockX;

	public bool LockY;

	public bool LockZ;

	private void Start()
	{
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = false;
		}
	}

	private void Update()
	{
		if ((bool)ObjectSpawn && (float)GameObject.FindGameObjectsWithTag("Enemy").Length < ObjectCount && Time.time >= timeSpawnTemp + TimeSpawn)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ObjectSpawn, base.transform.position + new Vector3(UnityEngine.Random.Range(-Radiun, Radiun), base.transform.position.y, UnityEngine.Random.Range(-Radiun, Radiun)), Quaternion.identity);
			float num = gameObject.transform.localScale.x;
			if (RandomScale)
			{
				num = (float)UnityEngine.Random.Range(0, 100) * 0.01f;
			}
			gameObject.transform.localScale = new Vector3(num, num, num) * ScaleMult;
			gameObject.transform.position = AxisLock(gameObject.transform.position);
			timeSpawnTemp = Time.time;
		}
	}

	public Vector3 AxisLock(Vector3 axis)
	{
		if (LockX)
		{
			axis.x = base.transform.position.x;
		}
		if (LockY)
		{
			axis.y = base.transform.position.y;
		}
		if (LockZ)
		{
			axis.z = base.transform.position.z;
		}
		return axis;
	}
}
