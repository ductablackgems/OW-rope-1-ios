using UnityEngine;

namespace App.Weapons
{
	[RequireComponent(typeof(AudioSource))]
	public class Gun : MonoBehaviour
	{
		public GunType gunType;

		public AudioClip fireClip;

		private AudioSource audioSource;

		public void Fire()
		{
			audioSource.clip = fireClip;
			audioSource.PlayOneShot(fireClip);
		}

		protected void Awake()
		{
			audioSource = this.GetComponentSafe<AudioSource>();
		}
	}
}
