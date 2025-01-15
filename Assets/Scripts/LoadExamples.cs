using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadExamples : MonoBehaviour
{
	public void LoadExample(string level)
	{
		SceneManager.LoadScene(level);
	}
}
