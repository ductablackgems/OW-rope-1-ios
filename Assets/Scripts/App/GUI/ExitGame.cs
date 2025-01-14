using UnityEngine;

namespace App.GUI
{
	public class ExitGame : MonoBehaviour
	{
		public GameObject Banner;

		private PanelsManager panelsManager;

		private void Awake()
		{
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && Banner == null)
			{
				panelsManager.ShowPanel(PanelType.ExitGame);
			}
		}
	}
}
