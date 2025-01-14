using UnityEngine;

namespace App.GUI
{
	public class ExitGameButton : MonoBehaviour
	{
		private PanelsManager panelsManager;

		private void OnClick()
		{
			Application.Quit();
		}

		private void Awake()
		{
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				panelsManager.ShowPanel(PanelType.MainMenu);
			}
		}
	}
}
