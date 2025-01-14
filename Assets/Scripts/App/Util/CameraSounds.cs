using UnityEngine;

namespace App.Util
{
	public class CameraSounds : MonoBehaviour
	{
		public AudioSource AudioSource
		{
			get;
			private set;
		}

		public void PlayOneShot(AudioClip clip)
		{
			if (!(AudioSource == null))
			{
				AudioSource.PlayOneShot(clip);
			}
		}

		private void Awake()
		{
			AudioSource = this.GetComponentInChildren<AudioSource>("Sounds", includeInactive: true);
		}
	}
}
