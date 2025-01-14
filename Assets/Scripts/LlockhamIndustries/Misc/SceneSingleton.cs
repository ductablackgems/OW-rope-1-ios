using UnityEngine;
using UnityEngine.UI;

namespace LlockhamIndustries.Misc
{
	public class SceneSingleton : MonoBehaviour
	{
		private static SceneSingleton system;

		private GameObject canvas;

		private SceneMenu menu = new SceneMenu();

		public static SceneSingleton System
		{
			get
			{
				if (system == null)
				{
					new GameObject("Scene Manager").AddComponent<SceneSingleton>();
				}
				return system;
			}
		}

		private void OnEnable()
		{
			if (system == null)
			{
				system = this;
			}
			else if (this != system)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				OpenMenu();
			}
		}

		public void OpenMenu()
		{
			if (canvas == null)
			{
				GrabCanvas();
			}
			menu.Open(System.canvas);
		}

		public void CloseMenu()
		{
			menu.Close();
		}

		private void GrabCanvas()
		{
			Object[] array = UnityEngine.Object.FindObjectsOfType<Canvas>();
			Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Canvas canvas = (Canvas)array2[i];
				if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					this.canvas = canvas.gameObject;
					break;
				}
			}
			if (this.canvas == null)
			{
				this.canvas = new GameObject("Canvas");
				this.canvas.AddComponent<Canvas>();
				this.canvas.AddComponent<CanvasScaler>();
				this.canvas.AddComponent<GraphicRaycaster>();
			}
		}
	}
}
