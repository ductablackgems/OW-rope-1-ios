using App.GUI;
using App.Interaction;
using App.Util;
using System;
using UnityEngine;

namespace App.ATM
{
    public class ATMInteractiveObject : InteractiveObject
    {
        [Serializable]
        public class HackingSettings
        {
            public int MinTime = 5;

            public int MaxTime = 20;

            public float TimeReductionPerAttempt = 0.5f;

            public float TimePenalizationSeconds = 1f;

            public int MinReward = 1000;

            public int MaxReward = 10000;

            public int RewardIncreasePerAttempt = 500;
        }

        [SerializeField]
        private HackingSettings hackSettings;

        private Pauser pauser;

        private ATMManager manager;

        private float lastReward;

        [SerializeField] HackingResultPanel hackingResultPanel;

        public int HackedCount
        {
            get;
            private set;
        }

        public void Initialize(ATMManager manager)
        {
            this.manager = manager;
            Initialize();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            pauser = ServiceLocator.Get<Pauser>();
        }

        protected override void OnInteract()
        {
            base.OnInteract();
            ShowOfferDialog();
        }

        private void OnOfferDialogClose(ATMOfferPanel.ATMResult result)
        {
            pauser.Resume();
            switch (result)
            {
                case ATMOfferPanel.ATMResult.Cancel:
                    break;
                case ATMOfferPanel.ATMResult.Deposit:
                    manager.DepositMoney();
                    break;
                case ATMOfferPanel.ATMResult.Withdraw:
                    manager.WithdrawMoney();
                    break;
                case ATMOfferPanel.ATMResult.Hack:
                    ShowHackPanel();
                    break;
            }
        }

        private void OnHackingFinished(HackingPanel.Result result)
        {
            pauser.Resume();
            if (result != 0)
            {
                Hacked(result == HackingPanel.Result.Success);
            }
        }

        private void ShowOfferDialog()
        {
            (pauser.PauseWithDialog(PanelType.ATMOffer) as ATMOfferPanel).Show(manager.GetDeposit(), manager.GetInventoryMoney(), CanHack(), OnOfferDialogClose);
        }

        private void ShowHackPanel()
        {
            float value = (float)hackSettings.MaxTime - (float)HackedCount * hackSettings.TimeReductionPerAttempt;
            value = Mathf.Clamp(value, hackSettings.MinTime, hackSettings.MaxTime);
            (pauser.PauseWithDialog(PanelType.HackingScreen) as HackingPanel).StartGame(value, hackSettings.TimePenalizationSeconds, OnHackingFinished);
        }

        private void Hacked(bool success)
        {
            if (!success)
            {
                manager.HackingFailed(this);
                return;
            }
            //int value = hackSettings.MinReward + HackedCount * hackSettings.RewardIncreasePerAttempt;
            //value = Mathf.Clamp(value, hackSettings.MinReward, hackSettings.MaxReward);
            HackedCount++;
            HackedCount = Mathf.Clamp(HackedCount, 1, hackSettings.MaxReward / hackSettings.MinReward);
            manager.HackingSucceded(hackSettings.MinReward, HackedCount);
            lastReward = hackSettings.MinReward;
        }

        private bool CanHack()
        {
            if (lastReward >= (float)hackSettings.MaxReward)
            {
                return false;
            }
            return true;
        }
    }
}
