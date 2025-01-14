using System;
using UnityEngine;

namespace App.GUI.Panels
{
	public abstract class AbstractPanel : MonoBehaviour
	{
		public GameObject[] extraPanels;

		[NonSerialized]
		public SharedGuiType[] sharedGuiTypes = new SharedGuiType[0];

		protected SharedGui sharedGui;

		public abstract PanelType GetPanelType();

		protected virtual void Awake()
		{
			sharedGui = ServiceLocator.Get<SharedGui>();
		}
	}
}
