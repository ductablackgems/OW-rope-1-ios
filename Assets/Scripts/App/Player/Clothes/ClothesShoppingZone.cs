using App.Camera;
using App.GUI;
using App.GUI.Panels;
using App.Player.Clothes.GUI;
using App.Util;
using UnityEngine;

namespace App.Player.Clothes
{
	public class ClothesShoppingZone : MonoBehaviour
	{
		private ClothesPanel clothesPanel;

		public DressStand dressStand;

		private Transform playerTransform;

		private PlayerController playerController;

		private CharacterModesHandler characterModesHandler;

		private CameraManager cameraManager;

		private PanelsManager panelsManager;

		private TriggerMonitor playerTrigger = new TriggerMonitor();

		private GamePanel gamePanel;

		private ClothesShopCamera clothesShopCamera;

		public bool IsPlayerIn()
		{
			return playerTrigger.IsTriggered();
		}

		private void Awake()
		{
			clothesPanel = ServiceLocator.Get<ClothesPanel>();
			clothesShopCamera = ServiceLocator.Get<ClothesShopCamera>();
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
			playerController = playerTransform.GetComponentSafe<PlayerController>();
			characterModesHandler = playerTransform.GetComponentSafe<CharacterModesHandler>();
			cameraManager = ServiceLocator.Get<CameraManager>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
			gamePanel = ServiceLocator.Get<GamePanel>();
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
					clothesPanel.EditedClothesKind = clothesPanel.defaultClothesKind;
					cameraManager.SetClothesShopCamera();
					panelsManager.ShowPanel(PanelType.ClothesShop);
					playerTransform.position = dressStand.transform.position;
					playerTransform.rotation = dressStand.transform.rotation;
					characterModesHandler.TryStopAll();
					playerController.controlled = false;
				});
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				gamePanel.clothesShoppingZone = this;
				playerTrigger.MarkTrigger(other);
				if ((object)clothesShopCamera.dressStand != dressStand)
				{
					clothesShopCamera.dressStand = dressStand;
				}
			}
		}
	}
}
