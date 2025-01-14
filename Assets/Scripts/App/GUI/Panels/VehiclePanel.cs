using App.Player;
using UnityEngine;

namespace App.GUI.Panels
{
	public class VehiclePanel : AbstractPanel
	{
		public GameObject attackButton;

		public GameObject liftButton;

		public GameObject pitchforkUpButton;

		public GameObject pitchforkDownButton;

		public GameObject dumpStartButton;

		public GameObject dumpEndButton;

		private PlayerMonitor playerMonitor;

		public override PanelType GetPanelType()
		{
			return PanelType.Vehicle;
		}

		private void OnEnable()
		{
			sharedGui.vehicleButton.SetActive(value: true);
			sharedGui.miniMap.SetActive(value: true);
			sharedGui.pauseButton.SetActive(value: true);
			sharedGui.missionText.SetActive(value: true);
			sharedGui.vehicleForwardButton.SetActive(value: true);
			sharedGui.vehicleBackButton.SetActive(value: true);
			sharedGui.steerLeftButton.SetActive(value: true);
			sharedGui.steerRightButton.SetActive(value: true);
			sharedGui.ShowPlayerStats(show: true);
			if (sharedGui.questInfo != null)
			{
				sharedGui.questInfo.SetActive(value: true);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			sharedGuiTypes = new SharedGuiType[13]
			{
				SharedGuiType.VehicleButton,
				SharedGuiType.MiniMap,
				SharedGuiType.PauseButton,
				SharedGuiType.MissionText,
				SharedGuiType.VehicleForwardButton,
				SharedGuiType.VehicleBackButton,
				SharedGuiType.SteerLeftButton,
				SharedGuiType.SteerRightButton,
				SharedGuiType.QuestInfo,
				SharedGuiType.HealthInfo,
				SharedGuiType.ArmorInfo,
				SharedGuiType.EnergyInfo,
				SharedGuiType.CapacityInfo
			};
			playerMonitor = ServiceLocator.Get<PlayerMonitor>();
		}

		private void Update()
		{
			attackButton.SetActive(playerMonitor.UsingFiretruck());
			if (liftButton != null)
			{
				liftButton.SetActive(playerMonitor.canCollectContainer());
				pitchforkUpButton.SetActive(playerMonitor.canPitchforkUp());
				pitchforkDownButton.SetActive(playerMonitor.canPitchforkDown());
				dumpStartButton.SetActive(playerMonitor.canStartDump());
				dumpEndButton.SetActive(playerMonitor.canEndDump());
			}
		}
	}
}
