using UnityEngine;

namespace App.Shop
{
	public class ShopSounds : MonoBehaviour
	{
		public AudioClip slideSound;

		public AudioClip buySound;

		public AudioClip NoMoneySound;

		public AudioSource audioSource;

		public void Play(AudioClip sound)
		{
			audioSource.PlayOneShot(sound);
		}

		private void Awake()
		{
			audioSource = this.GetComponentSafe<AudioSource>();
		}
	}
}
