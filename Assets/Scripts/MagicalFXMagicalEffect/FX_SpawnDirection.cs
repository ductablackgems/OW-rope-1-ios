using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class FX_SpawnDirection : MonoBehaviour
	{
		public int Number = 10;

		public float Frequency = 1f;

		public bool FixRotation;

		public bool Normal;

		public GameObject FXSpawn;

		public float LifeTime;

		public float TimeSpawn;

		private float timeTemp;

		public bool UseObjectForward = true;

		public Vector3 Direction = Vector3.forward;

		public Vector3 Noise = Vector3.zero;

		private int counter;

		private void Start()
		{
			counter = 0;
			timeTemp = Time.time;
			if (!(TimeSpawn <= 0f))
			{
				return;
			}
			for (int i = 0; i < Number - 1; i++)
			{
				if (UseObjectForward)
				{
					Direction = base.transform.forward;
				}
				Spawn(base.transform.position + Direction * Frequency * i);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void Update()
		{
			if (counter >= Number - 1)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (TimeSpawn > 0f && Time.time > timeTemp + TimeSpawn)
			{
				if (UseObjectForward)
				{
					Direction = base.transform.forward + new Vector3(base.transform.right.x * UnityEngine.Random.Range(0f - Noise.x, Noise.x), base.transform.right.y * UnityEngine.Random.Range(0f - Noise.y, Noise.y), base.transform.right.z * UnityEngine.Random.Range(0f - Noise.z, Noise.z)) * 0.01f;
				}
				Spawn(base.transform.position + Direction * Frequency * counter);
				counter++;
				timeTemp = Time.time;
			}
		}

		private void Spawn(Vector3 position)
		{
			if (FXSpawn != null)
			{
				Quaternion rotation = base.transform.rotation;
				if (!FixRotation)
				{
					rotation = FXSpawn.transform.rotation;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(FXSpawn, position, rotation);
				if (Normal)
				{
					gameObject.transform.forward = base.transform.forward;
				}
				if (LifeTime > 0f)
				{
					UnityEngine.Object.Destroy(gameObject.gameObject, LifeTime);
				}
			}
		}
	}
}
