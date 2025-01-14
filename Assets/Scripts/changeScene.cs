using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{
	public string sceneName;

	public void button()
	{
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}
}
