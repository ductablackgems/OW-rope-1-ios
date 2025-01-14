using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
	private AudioSource Audio;

	private void Start()
	{
		Audio = GetComponent<AudioSource>();
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		Audio.Play();
	}
}
