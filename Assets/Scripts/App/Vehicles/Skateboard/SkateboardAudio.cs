using App.Vehicles.Bicycle;
using UnityEngine;

namespace App.Vehicles.Skateboard
{
	public class SkateboardAudio : MonoBehaviour
	{
		public float maxVolume = 0.7f;

		public float maxVolumeSpeed = 10f;

		public float minPitch = 0.8f;

		public float maxPitch = 1.5f;

		public float minSpeed = 1f;

		public float maxSpeed = 5f;

		public AudioSource jumpAudioSource;

		public AudioClip jumpSound;

		public AudioClip landSound;

		public AudioClip[] crashSounds;

		private AudioSource loopSource;

		private Rigidbody _rigidbody;

		private PlayerSkateboardController playerSkateboardController;

		private StreetVehicleCrasher crasher;

		public void OnJump()
		{
			jumpAudioSource.PlayOneShot(jumpSound);
		}

		public void OnLand()
		{
			jumpAudioSource.PlayOneShot(landSound);
		}

		private void Awake()
		{
			loopSource = this.GetComponentSafe<AudioSource>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			playerSkateboardController = this.GetComponentSafe<PlayerSkateboardController>();
			crasher = this.GetComponentSafe<StreetVehicleCrasher>();
			crasher.OnCrash += OnCrash;
			loopSource.enabled = false;
		}

		private void OnDestroy()
		{
			crasher.OnCrash -= OnCrash;
		}

		private void OnDisable()
		{
			loopSource.enabled = false;
		}

		private void Update()
		{
			if (playerSkateboardController.InAir)
			{
				loopSource.volume = 0f;
				return;
			}
			float magnitude = _rigidbody.velocity.magnitude;
			if (magnitude < minSpeed)
			{
				loopSource.enabled = false;
				return;
			}
			loopSource.enabled = true;
			float t = (magnitude - minSpeed) / (maxSpeed - minSpeed);
			float t2 = (magnitude - minSpeed) / (maxVolumeSpeed - minSpeed);
			loopSource.volume = Mathf.Lerp(0f, maxVolume, t2);
			loopSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
		}

		private void OnCrash()
		{
			jumpAudioSource.PlayOneShot(crashSounds[Random.Range(0, crashSounds.Length - 1)]);
		}
	}
}
