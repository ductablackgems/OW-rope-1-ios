using App.Dogs;
using App.Garage;
using App.Interaction;
using App.Player;
using App.Player.Clothes;
using App.Player.GrenadeThrow;
using App.Settings;
using App.Shop;
using UnityEngine;

namespace App.GUI.Panels
{
	public class GamePanel : AbstractPanel
	{
		public GameObject startShopingButton;

		public GameObject garageDoorsButton;

		public GameObject grenadeButton;

		public GameObject dogAttackButton;

		private GrenadeThrowController playerGrenadeController;

		public ShoppingZone shoppingZone;

		public ClothesShoppingZone clothesShoppingZone;

		public GarageControlZone garageControlZone;

		private AimingController aimingController;

		private PlayerModel player;

		private VehicleSensor playerCarSensor;

		private ETCButton vehicleETCButton;

		private AbstractVehicleDriver[] drivers;

		private CarDriver carDriver;

		private float updateVehicleButtonTimer;

		public override PanelType GetPanelType()
		{
			return PanelType.Game;
		}

		private void OnEnable()
		{
			sharedGui.miniMap.SetActive(value: true);
			sharedGui.pauseButton.SetActive(value: true);
			// sharedGui.weaponInfo.SetActive(value: true);
			sharedGui.leftJoystick.SetActive(value: true);
			sharedGui.crosshair.SetActive(value: true);
			sharedGui.missionText.SetActive(value: true);
			sharedGui.ShowPlayerStats(show: true);
			sharedGui.hitIndicator.SetActive(value: true);
			if (sharedGui.questInfo != null)
			{
				sharedGui.questInfo.SetActive(value: true);
			}
			if (sharedGui.flyButton != null)
			{
				sharedGui.flyButton.SetActive(value: true);
			}
			sharedGui.switchGrenadeButton.SetActive(value: true);
			sharedGui.grenadeButton.SetActive(value: true);
			if (sharedGui.laserButton != null)
			{
				sharedGui.laserButton.SetActive(value: true);
			}
			SetDogAttackButton();
		}

		private void OnDisable()
		{
			if (sharedGui != null && sharedGui.aimButton != null)
			{
				sharedGui.aimButton.SetActive(value: false);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			sharedGuiTypes = new SharedGuiType[19]
			{
				SharedGuiType.VehicleButton,
				SharedGuiType.MiniMap,
				SharedGuiType.PauseButton,
				SharedGuiType.WeaponInfo,
				SharedGuiType.ArmorInfo,
				SharedGuiType.HealthInfo,
				SharedGuiType.EnergyInfo,
				SharedGuiType.CapacityInfo,
				SharedGuiType.LeftJoystick,
				SharedGuiType.Crosshair,
				SharedGuiType.MissionText,
				SharedGuiType.FlyButton,
				SharedGuiType.SwitchGrenadeButton,
				SharedGuiType.GrenadeButton,
				SharedGuiType.LaserButton,
				SharedGuiType.MagicAttackButton,
				SharedGuiType.MagicShieldButton,
				SharedGuiType.QuestInfo,
				SharedGuiType.HitIndicator
			};
			player = ServiceLocator.GetPlayerModel();
			GameObject gameObject = player.GameObject;
			drivers = gameObject.GetComponents<AbstractVehicleDriver>();
			carDriver = gameObject.GetComponentSafe<CarDriver>();
			playerCarSensor = gameObject.GetComponentInChildrenSafe<VehicleSensor>();
			aimingController = gameObject.GetComponentSafe<AimingController>();
			playerGrenadeController = gameObject.GetComponentInChildrenSafe<GrenadeThrowController>();
			shoppingZone = ServiceLocator.Get<ShoppingZone>();
			clothesShoppingZone = ServiceLocator.Get<ClothesShoppingZone>(showError: false);
			garageControlZone = ServiceLocator.Get<GarageControlZone>();
			vehicleETCButton = sharedGui.vehicleButton.GetComponent<ETCButton>();
		}

		private void Update()
		{
			UpdateVehicleButtonIcon();
			UpdateInteractionButton();
			startShopingButton.SetActive(shoppingZone.IsPlayerIn() || (clothesShoppingZone != null && clothesShoppingZone.IsPlayerIn()));
			garageDoorsButton.SetActive(garageControlZone.IsPlayerIn);
			grenadeButton.SetActive(playerGrenadeController.grenadeType != GunType.Unknown);
			sharedGui.aimButton.SetActive(aimingController.CanAim);
		}

		private void UpdateVehicleButtonIcon()
		{
			updateVehicleButtonTimer -= Time.deltaTime;
			if (updateVehicleButtonTimer > 0f)
			{
				return;
			}
			updateVehicleButtonTimer = 0.25f;
			AbstractVehicleDriver abstractVehicleDriver = null;
			for (int i = 0; i < drivers.Length; i++)
			{
				AbstractVehicleDriver abstractVehicleDriver2 = drivers[i];
				if (abstractVehicleDriver2.Runnable() || abstractVehicleDriver2.Running())
				{
					abstractVehicleDriver = abstractVehicleDriver2;
					break;
				}
			}
			if (abstractVehicleDriver == null && playerCarSensor.Triggered && !carDriver.Running())
			{
				abstractVehicleDriver = carDriver;
			}
			SetVehicleButtonIcon(abstractVehicleDriver);
		}

		private void SetVehicleButtonIcon(AbstractVehicleDriver driver)
		{
			if (vehicleETCButton == null)
			{
				return;
			}
			if (driver == null)
			{
				vehicleETCButton.gameObject.SetActive(value: false);
				return;
			}
			Sprite vehicleIcon = SettingsManager.GameSettings.GetVehicleIcon(driver.DriverType);
			// if (vehicleETCButton.normalSprite != vehicleIcon || vehicleETCButton.pressedSprite != vehicleIcon)
			// {
			// 	ETCInput.SetButtonSprite(vehicleETCButton.name, vehicleIcon, vehicleIcon, vehicleETCButton.normalColor);
			// }
			vehicleETCButton.gameObject.SetActive(value: true);
		}

		private void UpdateInteractionButton()
		{
			if (!(sharedGui.interactButton == null))
			{
				InteractiveObject interactiveObject = player.PlayerMonitor.InteractiveObjectSensor.InteractiveObject;
				sharedGui.interactButton.SetActive(interactiveObject != null);
			}
		}

		private void SetDogAttackButton()
		{
			if (!(dogAttackButton == null))
			{
				bool active = ServiceLocator.Get<DogManager>().Dogs.Count > 0;
				dogAttackButton.SetActive(active);
			}
		}
	}
}
