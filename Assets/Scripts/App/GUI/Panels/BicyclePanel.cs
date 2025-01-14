using App.Player;
using UnityEngine;

namespace App.GUI.Panels
{
	public class BicyclePanel : AbstractPanel
	{
		public GameObject jumpButton;

		private BicycleDriver bicycleDriver;

		private GyroboardDriver gyroboardDriver;

		private SkateboardDriver skateboardDriver;

		public override PanelType GetPanelType()
		{
			return PanelType.Bicycle;
		}

		private void OnEnable()
		{
			jumpButton.SetActive(skateboardDriver.Running());
			sharedGui.miniMap.SetActive(value: true);
			sharedGui.pauseButton.SetActive(value: true);
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
			sharedGuiTypes = new SharedGuiType[12]
			{
				SharedGuiType.VehicleButton,
				SharedGuiType.MiniMap,
				SharedGuiType.PauseButton,
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
			GameObject gameObject = ServiceLocator.GetGameObject("Player");
			bicycleDriver = gameObject.GetComponentSafe<BicycleDriver>();
			gyroboardDriver = gameObject.GetComponentSafe<GyroboardDriver>();
			skateboardDriver = gameObject.GetComponentSafe<SkateboardDriver>();
		}

		private void Update()
		{
			sharedGui.vehicleButton.SetActive(bicycleDriver.Running() || gyroboardDriver.Running() || skateboardDriver.Running());
		}
	}
}
