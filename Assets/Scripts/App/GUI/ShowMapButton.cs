using App.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace App.GUI
{
    public class ShowMapButton : MonoBehaviour, IEventSystemHandler
    {
        private PanelsManager panelsManager;

        private void Awake()
        {
            panelsManager = ServiceLocator.Get<PanelsManager>();
        }


        public void ShowBigMap()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_pause, () =>
            {
                panelsManager.ShowPanel(PanelType.Map);
            });
        }
    }
}
