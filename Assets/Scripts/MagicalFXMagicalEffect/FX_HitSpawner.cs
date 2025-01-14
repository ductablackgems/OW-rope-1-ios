using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class FX_HitSpawner : MonoBehaviour
	{
		public GameObject FXSpawn;

		public bool DestoyOnHit;

		public bool FixRotation;

		public float LifeTimeAfterHit = 1f;

		public float LifeTime;

		public ParticleSystem[] Particles;

		public Renderer[] Renderers;

		private void Start()
		{
			if (Particles.Length == 0)
			{
				Particles = base.transform.GetComponentsInChildren<ParticleSystem>();
			}
		}

		private void Spawn()
		{
			if (FXSpawn != null)
			{
				Quaternion rotation = base.transform.rotation;
				if (!FixRotation)
				{
					rotation = FXSpawn.transform.rotation;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(FXSpawn, base.transform.position, rotation);
				if (LifeTime > 0f)
				{
					UnityEngine.Object.Destroy(gameObject.gameObject, LifeTime);
				}
			}
			if (DestoyOnHit)
			{
				for (int i = 0; i < Particles.Length; i++)
				{
					Particles[i].Stop();
				}
				for (int j = 0; j < Renderers.Length; j++)
				{
					Renderers[j].enabled = false;
				}
				UnityEngine.Object.Destroy(base.gameObject, LifeTimeAfterHit);
				if ((bool)base.gameObject.GetComponent<Collider>())
				{
					base.gameObject.GetComponent<Collider>().enabled = false;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			Spawn();
		}

		private void OnCollisionEnter(Collision collision)
		{
			Spawn();
		}
	}
}
