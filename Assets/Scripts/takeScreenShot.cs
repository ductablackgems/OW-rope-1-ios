using UnityEngine;

public class takeScreenShot : MonoBehaviour
{
	public KeyCode key = KeyCode.G;

	private string resolution;

	private void Start()
	{
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.G))
		{
			resolution = 1280 + "X" + 720;
			ScreenCapture.CaptureScreenshot(Application.productName + "_ScreenShot-" + resolution + "-" + PlayerPrefs.GetInt("number", 0) + ".png", 1);
			PlayerPrefs.SetInt("number", PlayerPrefs.GetInt("number", 0) + 1);
		}
	}
}
