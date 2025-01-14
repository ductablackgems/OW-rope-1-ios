using App.Camera;
using App.Dogs;
using App.GUI.Panels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public sealed class DogShopPanel : AbstractPanel
	{
		private enum State
		{
			Buy,
			Equip,
			Unequip
		}

		private const float HIDE_ERROR_TEXT_TIME = 3f;

		[SerializeField]
		private Transform dogPosition;

		[Header("Controls")]
		[SerializeField]
		private Button buttonAction;

		[SerializeField]
		private Button buttonBack;

		[SerializeField]
		private Button buttonPrev;

		[SerializeField]
		private Button buttonNext;

		[SerializeField]
		private Button buttonRevive;

		[SerializeField]
		private Text textName;

		[SerializeField]
		private Text textPrice;

		[SerializeField]
		private Text textReviveError;

		[SerializeField]
		private Text textDead;

		[SerializeField]
		private GameObject reviveControl;

		[Header("Texts")]
		[SerializeField]
		private int buyTextID;

		[SerializeField]
		private int equipTextID;

		[SerializeField]
		private int unequipTextID;

		[SerializeField]
		private int reviveTextID;

		private Text actionText;

		private int currentIndex;

		private State dogState;

		private DogManager manager;

		private DogEquipDialog equipDialog;

		private CameraManager cameraManager;

		private DurationTimer hideErrorTimer = new DurationTimer();

		private List<Dog> allDogs = new List<Dog>(8);

		private Dog SelectedDog => allDogs[currentIndex];

		public void Show()
		{
			manager.LodAllDogs(allDogs);
			ShowDog(SelectedDog);
			ShowPlayer(show: false);
		}

		public void Close()
		{
			PanelsManager panelsManager = ServiceLocator.Get<PanelsManager>();
			panelsManager.ShowPanel(panelsManager.PreviousPanel.GetPanelType());
			ShowPlayer(show: true);
			if (SelectedDog != null)
			{
				SelectedDog.SetActive(isActive: false);
			}
			cameraManager.SetMenuCamera();
		}

		public override PanelType GetPanelType()
		{
			return PanelType.DogShop;
		}

		protected override void Awake()
		{
			Initialize();
		}

		private void Update()
		{
			if (hideErrorTimer.Done())
			{
				ShowReviveError(show: false);
			}
		}

		private void OnDogRevived(object sender, object data)
		{
			if (!(sender as Dog != SelectedDog))
			{
				ShowDog(SelectedDog);
			}
		}

		private void OnButtonBackClicked()
		{
			Close();
		}

		private void OnButtonActionClicked()
		{
			switch (dogState)
			{
			case State.Buy:
				manager.Buy(SelectedDog);
				break;
			case State.Equip:
				ShowEquipPanel();
				break;
			case State.Unequip:
				manager.Unequip(SelectedDog);
				break;
			}
			ShowDog(SelectedDog);
		}

		private void OnButtonPrevClicked()
		{
			SelectedDog.SetActive(isActive: false);
			currentIndex = ((currentIndex > 0) ? (currentIndex - 1) : (allDogs.Count - 1));
			ShowDog(SelectedDog);
		}

		private void OnButtonNextClicked()
		{
			SelectedDog.SetActive(isActive: false);
			currentIndex = ((currentIndex < allDogs.Count - 1) ? (currentIndex + 1) : 0);
			ShowDog(SelectedDog);
		}

		private void OnEquipDialogResult(DogManager.DogSlot slot)
		{
			manager.Equip(SelectedDog, slot);
			equipDialog.Close();
			ShowDog(SelectedDog);
		}

		private void OnButtonReviveClicked()
		{
			if (!manager.CanReviveWithAds())
			{
				ShowReviveError(show: true);
			}
			else
			{
				manager.ReviveWithAd(SelectedDog);
			}
		}

		private void Initialize()
		{
			manager = ServiceLocator.Get<DogManager>();
			cameraManager = ServiceLocator.Get<CameraManager>();
			actionText = buttonAction.GetComponentInChildren<Text>();
			equipDialog = GetComponentInChildren<DogEquipDialog>(includeInactive: true);
			equipDialog.Initialize(OnEquipDialogResult);
			buttonAction.onClick.AddListener(OnButtonActionClicked);
			buttonBack.onClick.AddListener(OnButtonBackClicked);
			buttonPrev.onClick.AddListener(OnButtonPrevClicked);
			buttonNext.onClick.AddListener(OnButtonNextClicked);
			buttonRevive.onClick.AddListener(OnButtonReviveClicked);
			ServiceLocator.SubscibeMessage(MessageID.Dog.Revived, this, OnDogRevived);
		}

		private void ShowDog(Dog dog)
		{
			dog.SetActive(isActive: true);
			dog.transform.SetPositionAndRotation(dogPosition.position, dogPosition.rotation);
			dog.ResetScale();
			State state = dog.IsOwned ? ((!dog.IsEquipped) ? State.Equip : State.Unequip) : State.Buy;
			SetDogState(state);
		}

		private void SetDogState(State state)
		{
			dogState = state;
			int key = 0;
			switch (state)
			{
			case State.Buy:
				key = buyTextID;
				break;
			case State.Equip:
				key = equipTextID;
				break;
			case State.Unequip:
				key = unequipTextID;
				break;
			}
			textPrice.gameObject.SetActive(state == State.Buy);
			textPrice.text = $"${SelectedDog.Settings.Price}";
			actionText.text = LocalizationManager.Instance.GetText(key);
			textName.text = SelectedDog.Name;
			bool active = SelectedDog.Health.Dead();
			reviveControl.SetActive(active);
			textDead.gameObject.SetActive(active);
			ShowReviveError(show: false);
		}

		private void ShowEquipPanel()
		{
			equipDialog.Show(GetSlotName);
		}

		private string GetSlotName(DogManager.DogSlot slot)
		{
			foreach (Dog allDog in allDogs)
			{
				if (allDog.Slot == slot)
				{
					return allDog.Name;
				}
			}
			return string.Empty;
		}

		private void ShowPlayer(bool show)
		{
			ServiceLocator.GetPlayerModel().GameObject.SetActive(show);
		}

		private void ShowReviveError(bool show)
		{
			textReviveError.gameObject.SetActive(show);
			buttonRevive.gameObject.SetActive(!show);
			if (show)
			{
				hideErrorTimer.Run(3f);
			}
			else
			{
				hideErrorTimer.Stop();
			}
		}
	}
}
