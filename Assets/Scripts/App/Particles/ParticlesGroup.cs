using UnityEngine;

namespace App.Particles
{
	public class ParticlesGroup : MonoBehaviour
	{
		private ParticleSystem[] systems;

		private bool initialized;

		public bool Awaken
		{
			get;
			private set;
		}

		public void Play()
		{
			Init();
			Awaken = false;
			ParticleSystem[] array = systems;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
		}

		public void Stop()
		{
			Init();
			Awaken = false;
			ParticleSystem[] array = systems;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop();
			}
		}

		public void Clear()
		{
			Init();
			ParticleSystem[] array = systems;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
		}

		private void Awake()
		{
			Init();
		}

		private void OnEnable()
		{
			Awaken = true;
		}

		private void Init()
		{
			if (!initialized)
			{
				initialized = true;
				systems = GetComponentsInChildren<ParticleSystem>();
			}
		}
	}
}
