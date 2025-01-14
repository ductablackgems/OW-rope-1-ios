using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class RainFall : MonoBehaviour
	{
		public GameObject Skill;

		public float AreaSize = 20f;

		public int MaxSpawn = 1000;

		public int NumSpawn = 1;

		public float Duration = 3f;

		public float DropRate;

		public Vector3 Offset = Vector3.up;

		private float timeTemp;

		private float timeTempDuration;

		private int count;

		public bool isRaining;

		private void Start()
		{
			StartRain();
			timeTemp = Time.time;
		}

		private void Spawn(Vector3 position)
		{
			if (!(Skill == null))
			{
				Object.Instantiate(Skill, position, Skill.transform.rotation);
			}
		}

		public void StartRain()
		{
			isRaining = true;
			timeTempDuration = Time.time;
		}

		private void Update()
		{
			if (!isRaining)
			{
				return;
			}
			if (count < MaxSpawn && Time.time < timeTempDuration + Duration)
			{
				if (Time.time > timeTemp + DropRate)
				{
					timeTemp = Time.time;
					for (int i = 0; i < NumSpawn; i++)
					{
						count++;
						Spawn(base.transform.position + new Vector3(UnityEngine.Random.Range(0f - AreaSize, AreaSize), 0f, UnityEngine.Random.Range(0f - AreaSize, AreaSize)) + Offset);
					}
				}
			}
			else
			{
				isRaining = false;
			}
		}
	}
}
