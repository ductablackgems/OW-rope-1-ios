using App;
using App.GUI;
using App.GUI.Controls;
using App.GUI.Panels;
using App.Player;
using App.Vehicles.Mech;
using UnityEngine;

public class MechPanel : AbstractPanel
{
	public GameObject buttonFlyUp;

	public GameObject buttonFlyDown;

	public ETCButton buttonTechnology;

	[Header("Icons")]
	public Sprite ImageFly;

	public Sprite ImageTransform;

	public Sprite ImageMech;

	private WeaponSwitchControl[] weaponControls;

	private MechController mechController;

	public MechDriver Driver
	{
		get;
		private set;
	}

	public override PanelType GetPanelType()
	{
		return PanelType.Mech;
	}

	private void OnEnable()
	{
		sharedGui.vehicleButton.SetActive(value: true);
		sharedGui.miniMap.SetActive(value: true);
		sharedGui.pauseButton.SetActive(value: true);
		sharedGui.crosshair.SetActive(value: true);
		sharedGui.leftJoystick.SetActive(value: true);
		sharedGui.hitIndicator.SetActive(value: true);
		if (sharedGui.questInfo != null)
		{
			sharedGui.questInfo.SetActive(value: true);
		}
		buttonFlyUp.SetActive(value: false);
		buttonFlyDown.SetActive(value: false);
		ConfigureWeaponControls();
		mechController = ((Driver.Vehicle != null) ? Driver.Vehicle.GetComponent<MechController>() : null);
		ConfigureTechnologyControls();
	}

	protected override void Awake()
	{
		base.Awake();
		sharedGuiTypes = new SharedGuiType[7]
		{
			SharedGuiType.VehicleButton,
			SharedGuiType.MiniMap,
			SharedGuiType.PauseButton,
			SharedGuiType.LeftJoystick,
			SharedGuiType.Crosshair,
			SharedGuiType.QuestInfo,
			SharedGuiType.HitIndicator
		};
		GameObject gameObject = ServiceLocator.GetGameObject("Player");
		Driver = gameObject.GetComponentSafe<MechDriver>();
		weaponControls = GetComponentsInChildren<WeaponSwitchControl>(includeInactive: true);
	}

	private void Update()
	{
		UpdateFlyingControls();
	}

	private void ConfigureWeaponControls()
	{
		if (!(Driver.Vehicle == null))
		{
			WeaponController[] componentsInChildren = Driver.Vehicle.GetComponentsInChildren<WeaponController>();
			for (int i = 0; i < weaponControls.Length; i++)
			{
				bool flag = i < componentsInChildren.Length;
				weaponControls[i].Controller = (flag ? componentsInChildren[i] : null);
			}
		}
	}

	private void UpdateFlyingControls()
	{
		if (!(mechController == null))
		{
			buttonFlyUp.SetActive(mechController.IsFlying);
			buttonFlyDown.SetActive(mechController.IsFlying);
		}
	}

	private void ConfigureTechnologyControls()
	{
		// if (!(mechController == null))
		// {
		// 	MechTechnology technology = mechController.Technology;
		// 	if (!(technology == null))
		// 	{
		// 		Sprite sprite = technology.CanFly ? ImageFly : ImageTransform;
		// 		// buttonTechnology.normalSprite = sprite;
		// 		// buttonTechnology.pressedSprite = sprite;
		// 	}
		// }
	}
}
