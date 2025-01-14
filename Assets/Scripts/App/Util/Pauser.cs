using App.GUI;
using App.GUI.Panels;
using UnityEngine;

namespace App.Util
{
	public class Pauser : MonoBehaviour
	{
		private const float PAUSE_TIME_SCALE = 0.0001f;

		private PanelsManager panelsManager;

		public bool endGame;

		public GameObject SharedPanel;

		public bool IsGamePaused => Time.timeScale == 0.0001f;

		public void Pause()
		{
			if (!endGame)
			{
				PauseWithDialog(PanelType.Pause);
			}
		}

		public void Resume()
		{
			Time.timeScale = 1f;
			panelsManager.ShowPanel(panelsManager.PreviousPanel.GetPanelType());
			if (SharedPanel != null)
			{
				SharedPanel.SetActive(value: true);
			}
		}

		public AbstractPanel PauseWithDialog(PanelType panelType)
		{
			Time.timeScale = 0.0001f;
			AbstractPanel result = panelsManager.ShowPanel(panelType);
			if (SharedPanel != null)
			{
				SharedPanel.SetActive(value: false);
			}
			return result;
		}

		private void Awake()
		{
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}
	}
}
