using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LlockhamIndustries.Misc
{
	public class SceneMenu
	{
		private int buttonCount = 5;

		private GameObject window;

		public void Open(GameObject Canvas)
		{
			if (window == null)
			{
				GenerateMenu(Canvas);
			}
			window.SetActive(value: true);
		}

		public void Close()
		{
			if (window != null && window.activeInHierarchy)
			{
				window.SetActive(value: false);
			}
		}

		public void GenerateMenu(GameObject Canvas)
		{
			window = new GameObject("Scene Menu");
			window.transform.SetParent(Canvas.transform, worldPositionStays: false);
			RectTransform rectTransform = window.AddComponent<RectTransform>();
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.offsetMax = new Vector2(50f, 0f);
			rectTransform.offsetMin = new Vector2(-50f, -(buttonCount * 20) + 8);
			window.AddComponent<Image>().color = new Color(0.28f, 0.28f, 0.28f, 1f);
			GenerateButton("Scene 1", 1, delegate
			{
				LoadScene(0);
			});
			GenerateButton("Scene 2", 2, delegate
			{
				LoadScene(1);
			});
			GenerateButton("Scene 3", 3, delegate
			{
				LoadScene(2);
			});
			GenerateButton("Scene 4", 4, delegate
			{
				LoadScene(3);
			});
			GenerateButton("Cancel", 5, delegate
			{
				Close();
			});
		}

		public void GenerateButton(string Text, int Index, UnityAction Action)
		{
			GameObject gameObject = new GameObject("Scene Button");
			gameObject.transform.SetParent(window.transform, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			float y = 1f - ((float)Index - 1f) / (float)buttonCount;
			rectTransform.anchorMax = new Vector2(0.5f, y);
			rectTransform.anchorMin = new Vector2(0.5f, y);
			rectTransform.offsetMax = new Vector2(40f, 0f);
			rectTransform.offsetMin = new Vector2(-40f, -16f);
			gameObject.AddComponent<Image>().color = new Color(0.22f, 0.22f, 0.22f, 1f);
			gameObject.AddComponent<Button>().onClick.AddListener(Action);
			GameObject gameObject2 = new GameObject("Button Text");
			gameObject2.transform.SetParent(rectTransform, worldPositionStays: false);
			RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
			rectTransform2.anchorMax = new Vector2(0f, 0f);
			rectTransform2.anchorMin = new Vector2(0f, 0f);
			rectTransform2.offsetMax = new Vector2(0f, 0f);
			rectTransform2.offsetMin = new Vector2(0f, 0f);
		}

		public void LoadScene(int Index)
		{
			SceneManager.LoadScene(Index, LoadSceneMode.Single);
		}
	}
}
