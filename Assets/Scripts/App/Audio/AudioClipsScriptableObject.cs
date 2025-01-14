using UnityEngine;

namespace App.Audio
{
	public class AudioClipsScriptableObject : ScriptableObject
	{
		public AudioClip[] clips;

		public AudioClip GetRandomClip()
		{
			return clips[Random.Range(0, clips.Length)];
		}
	}
}
