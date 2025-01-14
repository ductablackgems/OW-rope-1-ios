using App.Util;
using UnityEngine;

namespace App.GUI
{
    public class ResumeButton : MonoBehaviour
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
                if (InputUtils.ControlMode == ControlMode.keyboard)
                {
                    InputUtils.LockCursor();
                }
                pauser.Resume();
            });
        }
    }
}
