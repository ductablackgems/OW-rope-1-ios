using UnityEngine;

public class MenuGUI : MonoBehaviour
{
	public Texture2D logo;

	public GUISkin skin;

	private void Start()
	{
	}

	private void Update()
	{
		MouseLock.MouseLocked = false;
	}

	private void OnGUI()
	{
		if ((bool)skin)
		{
			GUI.skin = skin;
		}
		GUI.skin.button.fontSize = 18;
		GUI.DrawTexture(new Rect(Screen.width / 2 - logo.width / 2, 10f, logo.width, logo.height), logo);
		if (GUI.Button(new Rect(Screen.width / 2 - 400, 300f, 200f, 30f), "Demo 1"))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Demo1");
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 400, 340f, 200f, 30f), "Demo 2"))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Demo2");
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 400, 380f, 200f, 30f), "3D First Person"))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Demo3");
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 400, 420f, 200f, 30f), "2D Side Scrolling"))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Demo2D");
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 400, 460f, 200f, 30f), "Tower Defend"))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("TD");
		}
		if (GUI.Button(new Rect(Screen.width - 320, Screen.height - 50, 300f, 30f), "Get this project"))
		{
			Application.OpenURL("https://www.assetstore.unity3d.com/#/content/7676");
		}
	}
}
