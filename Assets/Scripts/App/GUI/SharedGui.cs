using App.GUI.Panels;
using App.Player;
using System.Linq;
using UnityEngine;

namespace App.GUI
{
	public class SharedGui : MonoBehaviour
	{
		public readonly SharedGuiType[] AllTypes = new SharedGuiType[24]
		{
			SharedGuiType.VehicleButton,
			SharedGuiType.MiniMap,
			SharedGuiType.PauseButton,
			SharedGuiType.WeaponInfo,
			SharedGuiType.LeftJoystick,
			SharedGuiType.Crosshair,
			SharedGuiType.MissionText,
			SharedGuiType.FlyButton,
			SharedGuiType.SwitchGrenadeButton,
			SharedGuiType.GrenadeButton,
			SharedGuiType.LaserButton,
			SharedGuiType.VehicleForwardButton,
			SharedGuiType.VehicleBackButton,
			SharedGuiType.SteerLeftButton,
			SharedGuiType.SteerRightButton,
			SharedGuiType.MagicAttackButton,
			SharedGuiType.MagicShieldButton,
			SharedGuiType.InteractButton,
			SharedGuiType.QuestInfo,
			SharedGuiType.HealthInfo,
			SharedGuiType.ArmorInfo,
			SharedGuiType.EnergyInfo,
			SharedGuiType.CapacityInfo,
			SharedGuiType.HitIndicator
		};

		public GameObject vehicleButton;

		public GameObject miniMap;

		public GameObject pauseButton;

		// public GameObject weaponInfo;

		public GameObject healthInfo;

		public GameObject armorInfo;

		public GameObject energyInfo;

		public GameObject capacityInfo;

		public GameObject leftJoystick;

		public GameObject crosshair;

		public GameObject missionText;

		public GameObject flyButton;

		public GameObject switchGrenadeButton;

		public GameObject grenadeButton;

		public GameObject laserButton;

		public GameObject vehicleForwardButton;

		public GameObject vehicleBackButton;

		public GameObject steerLeftButton;

		public GameObject steerRightButton;

		public GameObject magicButton;

		public GameObject magicShieldButton;

		public GameObject interactButton;

		public GameObject questInfo;

		public GameObject hitIndicator;

		public GameObject aimButton;

		public GameObject stickRocketButton;

		public GameObject launchRocketsButton;

		private bool menu;

		public GameObject MenuPanel;

		public GameObject[] Panels;

		private PlayerModel player;

		private PanelsManager panelsManager;

		public void Fix(AbstractPanel activePanel)
		{
			SharedGuiType[] allTypes = AllTypes;
			foreach (SharedGuiType sharedGuiType in allTypes)
			{
				GameObject guiObject = GetGuiObject(sharedGuiType);
				if (guiObject != null && (activePanel == null || !activePanel.sharedGuiTypes.Contains(sharedGuiType)))
				{
					bool active = false;
					guiObject.SetActive(active);
				}
			}
		}

		public GameObject GetGuiObject(SharedGuiType type)
		{
			switch (type)
			{
			case SharedGuiType.VehicleButton:
				return vehicleButton;
			case SharedGuiType.MagicAttackButton:
				return magicButton;
			case SharedGuiType.MagicShieldButton:
				return magicShieldButton;
			case SharedGuiType.MiniMap:
				return miniMap;
			case SharedGuiType.PauseButton:
				return pauseButton;
			// case SharedGuiType.WeaponInfo:
			// 	return weaponInfo;
			case SharedGuiType.LeftJoystick:
				return leftJoystick;
			case SharedGuiType.Crosshair:
				return crosshair;
			case SharedGuiType.MissionText:
				return missionText;
			case SharedGuiType.FlyButton:
				return flyButton;
			case SharedGuiType.SwitchGrenadeButton:
				return switchGrenadeButton;
			case SharedGuiType.GrenadeButton:
				return grenadeButton;
			case SharedGuiType.LaserButton:
				return laserButton;
			case SharedGuiType.VehicleForwardButton:
				return vehicleForwardButton;
			case SharedGuiType.VehicleBackButton:
				return vehicleBackButton;
			case SharedGuiType.SteerLeftButton:
				return steerLeftButton;
			case SharedGuiType.SteerRightButton:
				return steerRightButton;
			case SharedGuiType.InteractButton:
				return interactButton;
			case SharedGuiType.QuestInfo:
				return questInfo;
			case SharedGuiType.HealthInfo:
				return healthInfo;
			case SharedGuiType.ArmorInfo:
				return armorInfo;
			case SharedGuiType.EnergyInfo:
				return energyInfo;
			case SharedGuiType.CapacityInfo:
				return capacityInfo;
			case SharedGuiType.HitIndicator:
				return hitIndicator;
			default:
				return null;
			}
		}

		public void ShowPlayerStats(bool show)
		{
			healthInfo.SetActive(show);
			armorInfo.SetActive(show);
			energyInfo.SetActive(show);
			capacityInfo.SetActive(show);
		}

		private void Awake()
		{
			player = ServiceLocator.GetPlayerModel();
		}

		private void Update()
		{
			if (menu)
			{
				if (panelsManager.CompareActivePanel(PanelType.ClothesShop) || panelsManager.CompareActivePanel(PanelType.CharacterSelector))
				{
					player.Transform.eulerAngles = new Vector3(0f, 153f, 0f);
				}
				else
				{
					player.Transform.eulerAngles = new Vector3(0f, -144f, 0f);
				}
			}
		}
	}
}
