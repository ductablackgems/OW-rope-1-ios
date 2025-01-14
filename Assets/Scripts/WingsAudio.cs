using UnityEngine;

public class WingsAudio : MonoBehaviour
{
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void OnSound()
	{
		audioSource.Play();
	}
}
