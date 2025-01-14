using App.Player;
using UnityEngine;

namespace App.GUI.Panels
{
	public class HelicopterPanel : AbstractPanel
	{
		private HelicopterDriver helicopterDriver;

		public override PanelType GetPanelType()
		{
			return PanelType.Helicopter;
		}

		private void OnEnable()
		{
			sharedGui.miniMap.SetActive(value: true);
			sharedGui.crosshair.SetActive(value: true);
			sharedGui.leftJoystick.SetActive(value: true);
			sharedGui.pauseButton.SetActive(value: true);
			sharedGui.ShowPlayerStats(show: true);
			if (sharedGui.questInfo != null)
			{
				sharedGui.questInfo.SetActive(value: true);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			sharedGuiTypes = new SharedGuiType[10]
			{
				SharedGuiType.VehicleButton,
				SharedGuiType.MiniMap,
				SharedGuiType.PauseButton,
				SharedGuiType.LeftJoystick,
				SharedGuiType.Crosshair,
				SharedGuiType.QuestInfo,
				SharedGuiType.HealthInfo,
				SharedGuiType.ArmorInfo,
				SharedGuiType.EnergyInfo,
				SharedGuiType.CapacityInfo
			};
			GameObject gameObject = ServiceLocator.GetGameObject("Player");
			helicopterDriver = gameObject.GetComponentSafe<HelicopterDriver>();
		}

		private void Update()
		{
			sharedGui.vehicleButton.SetActive(helicopterDriver.CanLand());
		}
	}
}
