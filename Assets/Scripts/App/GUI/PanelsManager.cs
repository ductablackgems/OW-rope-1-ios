using App.GUI.Panels;
using System.Linq;
using UnityEngine;

namespace App.GUI
{
	public class PanelsManager : MonoBehaviour
	{
		public GameObject[] roots;

		public GameObject loadingPanel;

		public PanelType defaultPanelType = PanelType.Game;

		private SharedGui sharedGui;

		private AbstractPanel[] panels;

		public AbstractPanel ActivePanel
		{
			get;
			private set;
		}

		public AbstractPanel PreviousPanel
		{
			get;
			private set;
		}

		public AbstractPanel ShowPanel(PanelType type)
		{
			PreviousPanel = ActivePanel;
			ActivePanel = null;
			AbstractPanel[] array = panels;
			foreach (AbstractPanel abstractPanel in array)
			{
				if (abstractPanel.GetPanelType() != type)
				{
					abstractPanel.gameObject.SetActive(value: false);
					GameObject[] extraPanels = abstractPanel.extraPanels;
					for (int j = 0; j < extraPanels.Length; j++)
					{
						extraPanels[j].SetActive(value: false);
					}
				}
			}
			array = panels;
			foreach (AbstractPanel abstractPanel2 in array)
			{
				if (abstractPanel2.GetPanelType() == type)
				{
					ActivePanel = abstractPanel2;
					abstractPanel2.gameObject.SetActive(value: true);
					GameObject[] extraPanels = abstractPanel2.extraPanels;
					for (int j = 0; j < extraPanels.Length; j++)
					{
						extraPanels[j].SetActive(value: true);
					}
					break;
				}
			}
			sharedGui.Fix(ActivePanel);
			return ActivePanel;
		}

		public bool HasPanel(PanelType type)
		{
			AbstractPanel[] array = panels;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetPanelType() == type)
				{
					return true;
				}
			}
			return false;
		}

		public bool CompareActivePanel(PanelType type)
		{
			if (ActivePanel != null)
			{
				return ActivePanel.GetPanelType() == type;
			}
			return false;
		}

		private void Awake()
		{
			sharedGui = ServiceLocator.Get<SharedGui>();
			panels = new AbstractPanel[0];
			GameObject[] array = roots;
			foreach (GameObject gameObject in array)
			{
				panels = panels.Concat(gameObject.GetComponentsInChildren<AbstractPanel>(includeInactive: true)).ToArray();
			}
			if (loadingPanel != null)
			{
				loadingPanel.SetActive(value: true);
			}
		}

		private void Start()
		{
			ShowPanel(defaultPanelType);
		}
	}
}
