using App.GUI.Panels;
using App.Util;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class ATMOfferPanel : AbstractPanel
	{
		public enum ATMResult
		{
			Cancel,
			Deposit,
			Withdraw,
			Hack
		}

		[SerializeField]
		private AudioClip clickSound;

		[SerializeField]
		private AudioClip failedSound;

		private Button buttonDeposit;

		private Button buttonWithdraw;

		private Button buttonHack;

		private Button buttonClose;

		private Text textToDeposit;

		private Text textToWithdraw;

		private CameraSounds sounds;

		private ATMResult result;

		private Action<ATMResult> onClose;

		private bool canHack;

		public void Show(int depositMoney, int inventoryMoney, bool canHack, Action<ATMResult> onClose)
		{
			string format = "${0}";
			textToDeposit.text = string.Format(format, inventoryMoney);
			textToWithdraw.text = string.Format(format, depositMoney);
			result = ATMResult.Cancel;
			this.canHack = canHack;
			this.onClose = onClose;
		}

		public override PanelType GetPanelType()
		{
			return PanelType.ATMOffer;
		}

		protected override void Awake()
		{
			base.Awake();
			buttonDeposit = this.GetComponentInChildren<Button>("ButtonDeposit", includeInactive: true);
			buttonWithdraw = this.GetComponentInChildren<Button>("ButtonWithdraw", includeInactive: true);
			buttonHack = this.GetComponentInChildren<Button>("ButtonHack", includeInactive: true);
			buttonClose = this.GetComponentInChildren<Button>("ButtonBack", includeInactive: true);
			sounds = ServiceLocator.Get<CameraSounds>();
			textToDeposit = buttonDeposit.GetComponentInChildren<Text>("TextToDeposit", includeInactive: true);
			textToWithdraw = buttonWithdraw.GetComponentInChildren<Text>("TextToWithdraw", includeInactive: true);
			buttonDeposit.onClick.AddListener(OnButtonDepositClicked);
			buttonWithdraw.onClick.AddListener(OnButtonWithdrawClicked);
			buttonHack.onClick.AddListener(OnButtonHackClicked);
			buttonClose.onClick.AddListener(OnButtonCloseClicked);
		}

		private void OnButtonDepositClicked()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
			{
				Close(ATMResult.Deposit);
			});
		}

		private void OnButtonWithdrawClicked()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
			{
				Close(ATMResult.Withdraw);
			});
		}

		private void OnButtonHackClicked()
		{
			if (!canHack)
			{
				sounds.PlayOneShot(failedSound);
				buttonHack.OnDeselect(null);
			}
			else
			{
				CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
				{
					Close(ATMResult.Hack);
				});
			}
		}

		private void OnButtonCloseClicked()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
			{
				Close(ATMResult.Cancel);
			});
		}

		private void Close(ATMResult result)
		{
			this.result = result;
			PlayClickSound();
			onClose(result);
		}

		private void PlayClickSound()
		{
			if (!(sounds == null))
			{
				sounds.AudioSource.PlayOneShot(clickSound);
			}
		}
	}
}
