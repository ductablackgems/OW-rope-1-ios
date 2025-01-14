using App.Util;
using UnityEngine;

namespace App.GUI
{
    public class PauseButton : MonoBehaviour
    {
        private Pauser pauser;

        private void Awake()
        {
            pauser = ServiceLocator.Get<Pauser>();
        }

        private void OnClick()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_pause, () =>
            {
                pauser.Pause();
            });
        }
    }
}
