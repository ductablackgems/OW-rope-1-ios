using App.Dogs;
using App.GUI.Panels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class DogRevivePanel : AbstractPanel
	{
		private const float REFRESH_ADS_INTERVAL = 1f;

		[SerializeField]
		private Text textHeader;

		[SerializeField]
		private Text textNotAvailable;

		[SerializeField]
		private Button buttonClose;

		private DurationTimer refreshTimer = new DurationTimer();

		private List<DogReviveControl> controls = new List<DogReviveControl>(2);

		private DogReviveControl dogToRevive;

		private Action close;

		private Action<Dog> revive;

		private Func<bool> canRevive;

		private bool isInitialized;

		public void Initialize(Action<Dog> reviveCallback, Func<bool> canReviveCallback, Action closeCallback)
		{
			if (!isInitialized)
			{
				revive = reviveCallback;
				canRevive = canReviveCallback;
				close = closeCallback;
				GetComponentsInChildren(includeInactive: true, controls);
				foreach (DogReviveControl control in controls)
				{
					control.Initialize(OnReviveRequested);
				}
				buttonClose.onClick.AddListener(ButtonCloseClick);
				ServiceLocator.SubscibeMessage(MessageID.Dog.Revived, this, OnDogRevived);
				isInitialized = true;
			}
		}

		public void Show(List<Dog> dogs)
		{
			CleanControls();
			bool flag = canRevive();
			UpdateAdsAvailability(flag);
			int num = 0;
			foreach (Dog dog in dogs)
			{
				if (!dog.IsAlive)
				{
					DogReviveControl dogReviveControl = controls[num];
					dogReviveControl.CurrentState = (flag ? DogReviveControl.State.Available : DogReviveControl.State.Unavailable);
					dogReviveControl.Dog = dog;
					dogReviveControl.gameObject.SetActive(value: true);
					num++;
				}
			}
		}

		public void UpdateAdsAvailability(bool isAdAvailable)
		{
			textNotAvailable.gameObject.SetActive(!isAdAvailable);
			textHeader.gameObject.SetActive(isAdAvailable);
			if (!isAdAvailable)
			{
				foreach (DogReviveControl control in controls)
				{
					if (control.CurrentState == DogReviveControl.State.Available)
					{
						control.CurrentState = DogReviveControl.State.Unavailable;
					}
				}
				refreshTimer.Run(1f);
			}
		}

		public override PanelType GetPanelType()
		{
			return PanelType.DogRevive;
		}

		private void Update()
		{
			UpdateControlsState();
		}

		private void OnDogRevived(object sender, object data)
		{
			Dog y = sender as Dog;
			foreach (DogReviveControl control in controls)
			{
				if (!(control.Dog != y))
				{
					control.CurrentState = DogReviveControl.State.Revived;
					control.gameObject.SetActive(value: false);
				}
			}
		}

		private void OnReviveRequested(DogReviveControl control)
		{
			if (!(dogToRevive != null))
			{
				revive.SafeInvoke(control.Dog);
			}
		}

		private void ButtonCloseClick()
		{
			close();
		}

		private void CleanControls()
		{
			foreach (DogReviveControl control in controls)
			{
				control.gameObject.SetActive(value: false);
				control.Dog = null;
				control.CurrentState = DogReviveControl.State.Unavailable;
			}
		}

		private void UpdateControlsState()
		{
			if (refreshTimer.Done())
			{
				if (!canRevive())
				{
					refreshTimer.Run(1f);
					return;
				}
				refreshTimer.Stop();
				foreach (DogReviveControl control in controls)
				{
					if (control.CurrentState == DogReviveControl.State.Unavailable)
					{
						control.CurrentState = DogReviveControl.State.Available;
					}
				}
			}
		}
	}
}
