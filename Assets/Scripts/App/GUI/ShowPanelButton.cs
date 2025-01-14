using UnityEngine;

namespace App.GUI
{
	public class ShowPanelButton : MonoBehaviour
	{
		public PanelType panelType;

		private PanelsManager panelsManager;

		private void Awake()
		{
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}

		private void OnClick()
		{
			if(panelType == PanelType.Options)
			{
				CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.menu_weapon, () =>
				{

				});
			}
			panelsManager.ShowPanel(panelType);
		}
	}
}
