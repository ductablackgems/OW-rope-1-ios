using UnityEngine;

namespace MagicalFX
{
	public class FX_LifeTime : MonoBehaviour
	{
		public float LifeTime = 3f;

		public GameObject SpawnAfterDead;

		private float timeTemp;

		private void Start()
		{
			if (SpawnAfterDead == null)
			{
				UnityEngine.Object.Destroy(base.gameObject, LifeTime);
			}
			else
			{
				timeTemp = Time.time;
			}
		}

		private void Update()
		{
			if (SpawnAfterDead != null && Time.time > timeTemp + LifeTime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				Object.Instantiate(SpawnAfterDead, base.transform.position, SpawnAfterDead.transform.rotation);
			}
		}
	}
}
