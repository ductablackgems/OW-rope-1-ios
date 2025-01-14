using App.Camera;
using App.GUI;
using App.GUI.Panels;
using App.Shop.GUI;
using App.Shop.GunSlider;
using App.Util;
using UnityEngine;

namespace App.Shop
{
	public class ShoppingZone : MonoBehaviour
	{
		public GunSliderControl sliderControl;

		private CameraManager cameraManager;

		private PanelsManager panelsManager;

		private TriggerMonitor playerTrigger = new TriggerMonitor();

		private GamePanel gamePanel;

		private GameObject cam;

		private GunShopCamera gunShopCamera;

		public SlideGunShopButton slideGunShopButtonLeft;

		public SlideGunShopButton slideGunShopButtonRight;

		public ExitShoppingButton exitShoppingButton;

		public bool IsPlayerIn()
		{
			return playerTrigger.IsTriggered();
		}

		protected virtual PanelType GetPanelType()
		{
			return PanelType.GunShop;
		}

		protected virtual void OnAwake()
		{
		}

		private void Awake()
		{
			cameraManager = ServiceLocator.GetGameObject("MainCamera").GetComponent<CameraManager>();
			panelsManager = ServiceLocator.GetGameObject("gui").GetComponent<PanelsManager>();
			gamePanel = ServiceLocator.Get<GamePanel>();
			gunShopCamera = ServiceLocator.GetGameObject("MainCamera").GetComponent<GunShopCamera>();
			OnAwake();
		}

		private void FixedUpdate()
		{
			playerTrigger.FixedUpdate();
		}

		private void Update()
		{
			if (playerTrigger.IsTriggered() && InputUtils.StartShopping.IsDown)
			{
				CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_shopping, () =>
				{
					sliderControl.Run();
					cameraManager.SetGunShopCamera();
					panelsManager.ShowPanel(GetPanelType());
				});
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				gamePanel.shoppingZone = this;
				gunShopCamera.sliderControl = sliderControl;
				slideGunShopButtonLeft.gunSlider = sliderControl;
				slideGunShopButtonRight.gunSlider = sliderControl;
				exitShoppingButton.sliderControl = sliderControl;
				playerTrigger.MarkTrigger(other);
			}
		}
	}
}
