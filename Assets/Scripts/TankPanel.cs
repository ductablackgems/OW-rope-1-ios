using App;
using App.GUI;
using App.GUI.Panels;
using UnityEngine;

public class TankPanel : AbstractPanel
{
	public GameObject minigunButton;

	public GameObject rocketsButton;

	private TankDriver tankDriver;

	public override PanelType GetPanelType()
	{
		return PanelType.Tank;
	}

	private void OnEnable()
	{
		sharedGui.vehicleButton.SetActive(value: true);
		sharedGui.miniMap.SetActive(value: true);
		sharedGui.pauseButton.SetActive(value: true);
		sharedGui.crosshair.SetActive(value: true);
		sharedGui.vehicleForwardButton.SetActive(value: true);
		sharedGui.vehicleBackButton.SetActive(value: true);
		sharedGui.steerLeftButton.SetActive(value: true);
		sharedGui.steerRightButton.SetActive(value: true);
		sharedGui.ShowPlayerStats(show: true);
		if (sharedGui.questInfo != null)
		{
			sharedGui.questInfo.SetActive(value: true);
		}
		minigunButton.SetActive(tankDriver.HasMinigun());
		rocketsButton.SetActive(tankDriver.HasRockets());
	}

	protected override void Awake()
	{
		base.Awake();
		sharedGuiTypes = new SharedGuiType[13]
		{
			SharedGuiType.VehicleButton,
			SharedGuiType.MiniMap,
			SharedGuiType.PauseButton,
			SharedGuiType.Crosshair,
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
		tankDriver = gameObject.GetComponentSafe<TankDriver>();
	}
}
