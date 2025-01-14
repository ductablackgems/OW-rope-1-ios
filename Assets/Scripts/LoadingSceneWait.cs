using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneWait : MonoBehaviour
{
	private void OnEnable()
	{
		UnityEngine.Debug.Log("OnEnable called");
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		UnityEngine.Debug.Log("OnSceneLoaded: " + scene.name);
		UnityEngine.Debug.Log(mode);
		Invoke("Disable", 2f);
	}

	private void Disable()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		UnityEngine.Debug.Log("OnDisable");
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}
