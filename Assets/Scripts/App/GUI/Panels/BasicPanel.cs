using System.Linq;
using UnityEngine;

namespace App.GUI.Panels
{
	public class BasicPanel : AbstractPanel
	{
		public PanelType panelType;

		public SharedGuiType[] visibleSharedGuiTypes;

		public override PanelType GetPanelType()
		{
			return panelType;
		}

		protected override void Awake()
		{
			base.Awake();
			sharedGuiTypes = visibleSharedGuiTypes;
		}

		private void OnEnable()
		{
			SharedGuiType[] allTypes = sharedGui.AllTypes;
			foreach (SharedGuiType sharedGuiType in allTypes)
			{
				GameObject guiObject = sharedGui.GetGuiObject(sharedGuiType);
				if (guiObject != null && visibleSharedGuiTypes.Contains(sharedGuiType))
				{
					guiObject.SetActive(value: true);
				}
			}
		}
	}
}
