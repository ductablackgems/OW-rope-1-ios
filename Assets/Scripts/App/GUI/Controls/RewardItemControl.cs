using App.Rewards;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI.Controls
{
	public class RewardItemControl : MonoBehaviour
	{
		[SerializeField]
		private GameObject lockObject;

		[SerializeField]
		private GameObject unavailableObject;

		[SerializeField]
		private Button buttonWatchAd;

		[SerializeField]
		private Text textCollected;

		private RewardState currentState;

		public Action<RewardItemControl> Clicked
		{
			get;
			set;
		}

		public RewardState CurrentState
		{
			get
			{
				return currentState;
			}
			set
			{
				SetState(value);
			}
		}

		private void Start()
		{
			buttonWatchAd.onClick.AddListener(OnButtonWatchAdClicked);
		}

		private void OnButtonWatchAdClicked()
		{
			if (Clicked != null)
			{
				CallAdsManager.ShowRewardVideo(() =>
				{
					Clicked(this);
				});
			}
		}

		private void SetState(RewardState nextState)
		{
			currentState = nextState;
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			unavailableObject.gameObject.SetActive(currentState == RewardState.Unavailable);
			lockObject.gameObject.SetActive(currentState == RewardState.Locked);
			buttonWatchAd.gameObject.SetActive(currentState == RewardState.Ready);
			textCollected.gameObject.SetActive(currentState == RewardState.Finished);
		}
	}
}
