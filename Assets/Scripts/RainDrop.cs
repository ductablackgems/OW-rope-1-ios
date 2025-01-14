using UnityEngine;

public class RainDrop : MonoBehaviour
{
	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		UnityEngine.Object.Destroy(base.gameObject, 10f);
	}

	public void OnCollisionEnter(Collision col)
	{
		if (!audioSource.isPlaying)
		{
			audioSource.volume = col.relativeVelocity.magnitude * 0.04f;
			audioSource.pitch = UnityEngine.Random.Range(0.2f, 1.2f);
			audioSource.Play();
		}
	}
}
