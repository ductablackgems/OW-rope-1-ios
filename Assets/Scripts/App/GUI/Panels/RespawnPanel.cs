using App.Rewards;
using App.Spawn;
using App.Weapons;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI.Panels
{
    public class RespawnPanel : AbstractPanel
    {
        [SerializeField]
        private Button buttonReward;

        [SerializeField]
        private Button buttonCancel;

        [SerializeField]
        private Text textHeader;

        [SerializeField]
        private Text textLostItems;

        [SerializeField]
        private Text textGetHeader;

        [SerializeField]
        private Text textGetItems;

        [Header("Texts")]
        [SerializeField]
        private string headerTextDead = "DEAD!";

        [SerializeField]
        private string headerTextBusted = "BUSTED!";

        [SerializeField]
        private string armorLostText = "Armor";

        [SerializeField]
        private string nothingLostText = "Nothing";

        private PanelsManager panelsManager;

        private PlayerRespawner playerRespawner;

        private RewardManager rewards;

        private StringBuilder strBuilder = new StringBuilder();

        public override PanelType GetPanelType()
        {
            return PanelType.Respawn;
        }

        protected override void Awake()
        {
            base.Awake();
            panelsManager = ServiceLocator.Get<PanelsManager>();
            playerRespawner = ServiceLocator.Get<PlayerRespawner>();
            rewards = ServiceLocator.Get<RewardManager>();
            GameObject gameObject = ServiceLocator.GetGameObject("MusicPlayer", showError: false);
            buttonReward.onClick.AddListener(OnButtonRewardClicked);
            buttonCancel.onClick.AddListener(OnButtonCancelClicked);
        }

        private void OnEnable()
        {
            PauseGame(isPause: true);
            SetHeader();
            SetLostItemsDescription();
            SetObtainedItemsDescription();
        }

        private void OnDestroy()
        {
            if (buttonReward != null)
            {
                buttonReward.onClick.RemoveAllListeners();
            }
            if (buttonCancel != null)
            {
                buttonCancel.onClick.RemoveAllListeners();
            }
        }

        private void OnButtonRewardClicked()
        {
            CallAdsManager.ShowRewardVideo(() =>
            {
                PauseGame(isPause: false);
                rewards.AssignRewardToPlayer(2);
                panelsManager.ShowPanel(PanelType.Game);
                ServiceLocator.Messages.Send(MessageID.Player.RespawnDialogClosed, this);
            });
        }

        private void OnButtonCancelClicked()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_die, () =>
            {
                PauseGame(isPause: false);
                rewards.AssignWaitingRewards();
                panelsManager.ShowPanel(PanelType.Game);
                ServiceLocator.Messages.Send(MessageID.Player.RespawnDialogClosed, this);
            });
        }

        private void PauseGame(bool isPause)
        {
            Time.timeScale = (isPause ? 0.0001f : 1f);
        }

        private void SetLostItemsDescription()
        {
            int lostMoney = playerRespawner.LostMoney;
            strBuilder.Length = 0;
            if (lostMoney > 0)
            {
                strBuilder.AppendLine($"{lostMoney} $");
            }
            GunType lostWeapon = playerRespawner.LostWeapon;
            if (lostWeapon != 0)
            {
                IWeapon weapon = ServiceLocator.GetPlayerModel().ShotController.GetWeapon(lostWeapon);
                if (weapon != null)
                {
                    strBuilder.AppendLine(weapon.GetGunName());
                }
            }
            if (playerRespawner.LostArmor > 0f)
            {
                strBuilder.AppendLine(LocalizationManager.Instance.GetText(4004));
            }
            if (strBuilder.Length == 0)
            {
                strBuilder.AppendLine(LocalizationManager.Instance.GetText(4027));
                textLostItems.color = Color.white;
            }
            else
            {
                textLostItems.color = Color.red;
            }
            textLostItems.text = strBuilder.ToString();
        }

        private void SetObtainedItemsDescription()
        {
            bool flag = rewards.Rewards.Count > 0;
            textGetHeader.gameObject.SetActive(flag);
            textGetItems.gameObject.SetActive(flag);
            if (flag)
            {
                strBuilder.Length = 0;
                foreach (string reward in rewards.Rewards)
                {
                    if (!string.IsNullOrEmpty(reward))
                    {
                        strBuilder.AppendLine(reward);
                    }
                }
                textGetItems.text = strBuilder.ToString();
            }
        }

        private void SetHeader()
        {
            if (playerRespawner.RespawnReason == PlayerRespawner.RespawnType.Busted)
            {
                textHeader.text = LocalizationManager.Instance.GetText(4009);
            }
            else
            {
                textHeader.text = LocalizationManager.Instance.GetText(4008);
            }
        }
    }
}
