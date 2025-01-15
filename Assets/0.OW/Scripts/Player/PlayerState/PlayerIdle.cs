using System;
using _0.DucLib.Scripts.Common;
using _0.DucLib.Scripts.Machine;

namespace _0.OW.Scripts.Player.PlayerState
{
    public class PlayerIdle : DTState
    {
        private bool isPlayAnim = false;
        public override void Enter()
        {
            base.Enter();
            if (!isPlayAnim)
            {
                isPlayAnim = true;
                OWManager.instance.playerController.anim.PlayIdle();
            }
        }

        public override void Exit()
        {
            base.Exit();
            isPlayAnim = false;
        }
    }
}