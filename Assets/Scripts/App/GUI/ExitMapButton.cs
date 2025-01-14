using UnityEngine;
using UnityEngine.EventSystems;

namespace App.GUI
{
	public class ExitMapButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private PanelsManager panelsManager;

		private void Awake()
		{
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_minimap_back, () =>
			{
				panelsManager.ShowPanel(panelsManager.PreviousPanel.GetPanelType());
			});
		}
	}
}
