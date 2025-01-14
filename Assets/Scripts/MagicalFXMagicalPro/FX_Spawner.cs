using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_Spawner : MonoBehaviour
	{
		public bool FixRotation;

		public bool Normal;

		public GameObject FXSpawn;

		public float LifeTime;

		public float TimeSpawn;

		private float timeTemp;

		private void Start()
		{
			timeTemp = Time.time;
			if (FXSpawn != null && TimeSpawn <= 0f)
			{
				Quaternion rotation = base.transform.rotation;
				if (!FixRotation)
				{
					rotation = FXSpawn.transform.rotation;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(FXSpawn, base.transform.position, rotation);
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

		private void Update()
		{
			if (!(TimeSpawn > 0f) || !(Time.time > timeTemp + TimeSpawn))
			{
				return;
			}
			if (FXSpawn != null)
			{
				Quaternion rotation = base.transform.rotation;
				if (!FixRotation)
				{
					rotation = FXSpawn.transform.rotation;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(FXSpawn, base.transform.position, rotation);
				if (Normal)
				{
					gameObject.transform.forward = base.transform.forward;
				}
				if (LifeTime > 0f)
				{
					UnityEngine.Object.Destroy(gameObject.gameObject, LifeTime);
				}
			}
			timeTemp = Time.time;
		}
	}
}
