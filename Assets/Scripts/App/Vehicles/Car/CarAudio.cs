using App.Vehicles.Motorbike;
using UnityEngine;

namespace App.Vehicles.Car
{
	public class CarAudio : MonoBehaviour
	{
		public enum EngineAudioOptions
		{
			Simple,
			FourChannel
		}

		public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;

		public AudioClip lowAccelClip;

		public AudioClip lowDecelClip;

		public AudioClip highAccelClip;

		public AudioClip highDecelClip;

		public float pitchMultiplier = 1f;

		public float lowPitchMin = 1f;

		public float lowPitchMax = 6f;

		public float highPitchMultiplier = 0.25f;

		public float maxRolloffDistance = 500f;

		public float dopplerLevel = 1f;

		public bool useDoppler = true;

		private AudioSource m_LowAccel;

		private AudioSource m_LowDecel;

		private AudioSource m_HighAccel;

		private AudioSource m_HighDecel;

		private bool m_StartedSound;

		private ISoundableCar soundableCar;

		private CarSounds carSounds;

		private Rigidbody _rigidbody;

		private int frames;

		private GameObject hrac;

		private float distance;

		private int omezeni = 1;

		private int kooficient = 1;

		private void Awake()
		{
			carSounds = this.GetComponentInChildrenSafe<CarSounds>();
			_rigidbody = this.GetComponentInChildrenSafe<Rigidbody>();
		}

		private void OnDisable()
		{
			StopSound();
		}

		private void StartSound()
		{
			soundableCar = GetComponent<CarController>();
			if (soundableCar == null)
			{
				soundableCar = GetComponent<MotorbikeControl>();
			}
			if (engineSoundStyle == EngineAudioOptions.FourChannel)
			{
				m_HighAccel = SetUpEngineAudioSource(highAccelClip);
				m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
				m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
				m_HighDecel = SetUpEngineAudioSource(highDecelClip);
			}
			else
			{
				m_HighAccel = this.GetComponentSafe<AudioSource>();
				m_HighAccel.clip = highAccelClip;
				m_HighAccel.Play();
			}
			m_StartedSound = true;
		}

		private void StopSound()
		{
			if (engineSoundStyle == EngineAudioOptions.FourChannel)
			{
				AudioSource[] components = GetComponents<AudioSource>();
				for (int i = 0; i < components.Length; i++)
				{
					UnityEngine.Object.Destroy(components[i]);
				}
			}
			else
			{
				AudioSource[] components = GetComponents<AudioSource>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].Stop();
				}
			}
			m_StartedSound = false;
		}

		private void Start()
		{
			hrac = GameObject.FindGameObjectWithTag("Player");
		}

		private void Update()
		{
			frames++;
			if (frames % omezeni == 0)
			{
				FrameOptimalUpdate();
				if (hrac != null)
				{
					distance = Vector3.Distance(base.transform.position, hrac.transform.position);
				}
				if (distance < 30f)
				{
					omezeni = 1;
				}
				else
				{
					omezeni = 1;
				}
			}
		}

		private void FrameOptimalUpdate()
		{
			float sqrMagnitude = (UnityEngine.Camera.main.transform.position - base.transform.position).sqrMagnitude;
			if (m_StartedSound && sqrMagnitude > maxRolloffDistance * maxRolloffDistance)
			{
				StopSound();
			}
			if (!m_StartedSound && sqrMagnitude < maxRolloffDistance * maxRolloffDistance)
			{
				StartSound();
			}
			if (m_StartedSound)
			{
				float b = ULerp(lowPitchMin, lowPitchMax, soundableCar.GetRevs());
				b = Mathf.Min(lowPitchMax, b);
				if (engineSoundStyle == EngineAudioOptions.Simple)
				{
					m_HighAccel.pitch = b * pitchMultiplier * highPitchMultiplier;
					m_HighAccel.dopplerLevel = (useDoppler ? dopplerLevel : 0f);
					m_HighAccel.volume = 1f;
					return;
				}
				m_LowAccel.pitch = b * pitchMultiplier;
				m_LowDecel.pitch = b * pitchMultiplier;
				m_HighAccel.pitch = b * highPitchMultiplier * pitchMultiplier;
				m_HighDecel.pitch = b * highPitchMultiplier * pitchMultiplier;
				float num = Mathf.Abs(soundableCar.GetAccelerationInput());
				float num2 = 1f - num;
				float num3 = Mathf.InverseLerp(0.2f, 0.8f, soundableCar.GetRevs());
				float num4 = 1f - num3;
				num3 = 1f - (1f - num3) * (1f - num3);
				num4 = 1f - (1f - num4) * (1f - num4);
				num = 1f - (1f - num) * (1f - num);
				num2 = 1f - (1f - num2) * (1f - num2);
				m_LowAccel.volume = num4 * num;
				m_LowDecel.volume = num4 * num2;
				m_HighAccel.volume = num3 * num;
				m_HighDecel.volume = num3 * num2;
				m_HighAccel.dopplerLevel = (useDoppler ? dopplerLevel : 0f);
				m_LowAccel.dopplerLevel = (useDoppler ? dopplerLevel : 0f);
				m_HighDecel.dopplerLevel = (useDoppler ? dopplerLevel : 0f);
				m_LowDecel.dopplerLevel = (useDoppler ? dopplerLevel : 0f);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!carSounds.crash.isPlaying)
			{
				carSounds.crash.volume = Mathf.Clamp(_rigidbody.velocity.magnitude / 10f, 0.1f, 1f);
				carSounds.crash.Play();
			}
		}

		private AudioSource SetUpEngineAudioSource(AudioClip clip)
		{
			AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.clip = clip;
			audioSource.volume = 0f;
			audioSource.loop = true;
			audioSource.time = UnityEngine.Random.Range(0f, clip.length);
			audioSource.Play();
			audioSource.minDistance = 5f;
			audioSource.maxDistance = maxRolloffDistance;
			audioSource.dopplerLevel = 0f;
			return audioSource;
		}

		private static float ULerp(float from, float to, float value)
		{
			return (1f - value) * from + value * to;
		}
	}
}
