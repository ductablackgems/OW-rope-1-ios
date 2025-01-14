using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_LifeTime : MonoBehaviour
	{
		public float LifeTime = 3f;

		public float FadeTime = 1f;

		public GameObject SpawnAfterDead;

		private float timeTemp;

		public ParticleSystem[] Particles;

		public FX_FadeToGround[] Faders;

		public Renderer[] Renderers;

		public Light[] Lights;

		private FX_Tentacle ten;

		private MaterialPropertyBlock block;

		private void Awake()
		{
			ten = GetComponent<FX_Tentacle>();
			if (Particles.Length == 0)
			{
				Particles = base.transform.GetComponentsInChildren<ParticleSystem>();
			}
			if (Faders.Length == 0)
			{
				Faders = base.transform.GetComponentsInChildren<FX_FadeToGround>();
			}
			if (Lights.Length == 0)
			{
				Lights = base.transform.GetComponentsInChildren<Light>();
			}
			for (int i = 0; i < Particles.Length; i++)
			{
				Particles[i].Pause();
				Particles[i].Stop();
				Particles[i].Clear();
			}
			for (int j = 0; j < Particles.Length; j++)
			{
				ParticleSystem.MainModule main = Particles[j].main;
				main.duration = LifeTime - FadeTime;
				main.loop = false;
			}
		}

		private void Start()
		{
			block = new MaterialPropertyBlock();
			timeTemp = Time.time;
			if (SpawnAfterDead == null)
			{
				UnityEngine.Object.Destroy(base.gameObject, LifeTime);
			}
			for (int i = 0; i < Particles.Length; i++)
			{
				Particles[i].Play();
			}
		}

		private void Update()
		{
			float num = Mathf.Clamp(timeTemp + LifeTime - Time.time, 0f, 1f);
			for (int i = 0; i < Renderers.Length; i++)
			{
				Renderers[i].GetPropertyBlock(block);
				block.SetFloat("_Alpha", num);
				block.SetFloat("_Cutoff", 1f - num);
				Renderers[i].SetPropertyBlock(block);
			}
			for (int j = 0; j < Lights.Length; j++)
			{
				if ((bool)Lights[j])
				{
					Lights[j].intensity *= num;
				}
			}
			if (Time.time > timeTemp + LifeTime - 1f)
			{
				if ((bool)ten)
				{
					ten.End(15f);
				}
				for (int k = 0; k < Faders.Length; k++)
				{
					Faders[k].OnDead();
				}
			}
			if (Time.time > timeTemp + LifeTime && SpawnAfterDead != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				Object.Instantiate(SpawnAfterDead, base.transform.position, SpawnAfterDead.transform.rotation);
			}
		}
	}
}
