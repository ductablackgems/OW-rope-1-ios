using System.Collections;
using UnityEngine;

public class MolotovSoundBreak : MonoBehaviour
{
	public AudioClip[] impactsGlass;

	public AudioClip Fire;

	private AudioSource audioSource;

	private void OnEnable()
	{
		StartCoroutine(Audio());
	}

	private IEnumerator Audio()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.PlayOneShot(impactsGlass[Random.RandomRange(0, 3)], 1f);
		yield return new WaitForSeconds(0.3f);
		audioSource.PlayOneShot(Fire, 1f);
	}
}
