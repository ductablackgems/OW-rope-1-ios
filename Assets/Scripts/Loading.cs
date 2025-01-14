using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
	private AsyncOperation asyncOperation;

	private bool nacitani;

	private bool done;

	public UISlider loadingUI;

	public GameObject ContinueButton;

	public AudioClip[] soundForLoading;

	private AudioSource audio;

	private int Level = 1;

	private float a;

	private double b;

	private void Awake()
	{
		LocalizationManager.Instance.LoadSystemLanguage();
		audio = GetComponent<AudioSource>();
		StartCoroutine("LoadScene");
		loadingUI.value = 0f;
	}

	private IEnumerator LoadScene()
	{
		yield return new WaitForSeconds(3f);
		nacitani = true;
		asyncOperation = SceneManager.LoadSceneAsync(Level);
		asyncOperation.allowSceneActivation = false;
		while (!asyncOperation.isDone)
		{
			yield return asyncOperation.isDone;
			a = asyncOperation.progress;
			b = Math.Round(a, 2);
			loadingUI.value = (float)b;
			if (!(asyncOperation.progress >= 0.89f) || asyncOperation.allowSceneActivation || done)
			{
				continue;
			}
			yield return new WaitForSeconds(2f);
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine("ContinueGame");
			}
			if (ContinueButton != null)
			{
				if (!ContinueButton.activeSelf)
				{
					audio.PlayOneShot(soundForLoading[1]);
				}
				ContinueButton.SetActive(value: true);
			}
			Invoke("ContinueGame", 30f);
		}
		UnityEngine.Debug.Log("load done");
	}

	public void ProgressSoundLoading()
	{
		if (nacitani)
		{
			audio.PlayOneShot(soundForLoading[0]);
		}
	}

	public void ContinueGame()
	{
		done = true;
		if (!audio.isPlaying)
		{
			audio.PlayOneShot(soundForLoading[1]);
		}
		ContinueButton.SetActive(value: false);
		UnityEngine.Object.Destroy(ContinueButton);
		StartCoroutine("WaitContinueGame");
	}

	private IEnumerator WaitContinueGame()
	{
		yield return new WaitForSeconds(1f);
		asyncOperation.allowSceneActivation = true;
	}
}
