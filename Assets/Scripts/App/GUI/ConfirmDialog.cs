using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class ConfirmDialog : MonoBehaviour
	{
		public enum Result
		{
			Cancel,
			Accept
		}

		[SerializeField]
		private Button buttonAccept;

		[SerializeField]
		private Button buttonClose;

		[SerializeField]
		private Text textDescription;

		private Action<Result> resultCallback;

		public void Show(string text, Action<Result> callback)
		{
			textDescription.text = text;
			resultCallback = callback;
			buttonAccept.onClick.AddListener(OnButtonAcceptClicked);
			buttonClose.onClick.AddListener(OnButtonCloseClicked);
			SetActive(isActive: true);
		}

		public void Close()
		{
			buttonAccept.onClick.RemoveAllListeners();
			buttonClose.onClick.RemoveAllListeners();
			SetActive(isActive: false);
		}

		private void OnButtonAcceptClicked()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
			{
				resultCallback(Result.Accept);
				Close();
			});
		}

		private void OnButtonCloseClicked()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
			{
				resultCallback(Result.Cancel);
				Close();
			});
		}

		private void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}
	}
}
