using App;
using App.GUI;
using App.GUI.Panels;
using App.Player;
using App.Vehicles.Airplane;
using UnityEngine;
using UnityEngine.UI;

public class AirplanePanel : AbstractPanel
{
	[SerializeField]
	private ETCButton buttonReset;

	[SerializeField]
	private Text textWarning;

	[SerializeField]
	private Text textCountdown;

	private AirplaneController controller;

	private AirplaneDriver driver;

	private float countdown;

	protected override void Awake()
	{
		base.Awake();
		sharedGuiTypes = new SharedGuiType[6]
		{
			SharedGuiType.VehicleButton,
			SharedGuiType.MiniMap,
			SharedGuiType.PauseButton,
			SharedGuiType.LeftJoystick,
			SharedGuiType.Crosshair,
			SharedGuiType.QuestInfo
		};
		PlayerModel playerModel = ServiceLocator.GetPlayerModel();
		driver = playerModel.Transform.GetComponent<AirplaneDriver>();
		ServiceLocator.Messages.Subscribe(MessageID.Airplane.BorderEnter, this, OnAirplaneBorderEnter);
		ServiceLocator.Messages.Subscribe(MessageID.Airplane.BorderLeave, this, OnAirplaneBorderLeave);
	}

	public override PanelType GetPanelType()
	{
		return PanelType.Airplane;
	}

	private void OnEnable()
	{
		sharedGui.vehicleButton.SetActive(value: true);
		sharedGui.miniMap.SetActive(value: true);
		sharedGui.pauseButton.SetActive(value: true);
		sharedGui.crosshair.SetActive(value: true);
		sharedGui.leftJoystick.SetActive(value: true);
		if (sharedGui.questInfo != null)
		{
			sharedGui.questInfo.SetActive(value: true);
		}
		buttonReset.gameObject.SetActive(value: false);
		ShowWarning(0f);
		controller = driver.Vehicle.GetComponent<AirplaneController>();
	}

	private void OnDisable()
	{
		ShowWarning(0f);
		controller = null;
	}

	private void Update()
	{
		if (!(driver == null))
		{
			UpdateResetButton();
			UpdateWarningText();
		}
	}

	private void OnAirplaneBorderEnter(object sender, object data)
	{
		countdown = 0f;
	}

	private void OnAirplaneBorderLeave(object sender, object data)
	{
		countdown = (float)data;
	}

	private void UpdateResetButton()
	{
		if (!(controller == null))
		{
			buttonReset.gameObject.SetActive(controller.IsStucked);
		}
	}

	private void UpdateWarningText()
	{
		if (countdown > 0f)
		{
			countdown = Mathf.Max(0f, countdown - Time.deltaTime);
		}
		ShowWarning(countdown);
	}

	private void ShowWarning(float countdown)
	{
		bool flag = countdown > 0f;
		textWarning.gameObject.SetActive(flag);
		textCountdown.gameObject.SetActive(flag);
		if (flag)
		{
			textCountdown.text = countdown.ToString("00");
		}
	}
}
