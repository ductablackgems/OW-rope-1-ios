using App.Settings;
using App.Vehicles;
using UnityEngine;

namespace App.Util
{
	public class InputUtils
	{
		public static readonly InputButton Attack;

		public static readonly InputButton TankMinigun;

		public static readonly InputButton TankRockets;

		public static readonly InputButton HeliMinigun;

		public static readonly InputButton HeliRockets;

		public static readonly InputButton MechPrimaryAttack;

		public static readonly InputButton MechSecondaryAttack;

		public static readonly InputButton Eyes;

		public static readonly InputButton WaterShoot;

		public static readonly InputButton FlyDown;

		public static readonly InputButton FlyUp;

		public static readonly InputButton Kick;

		public static readonly InputButton RunFast;

		public static readonly InputButton Vehicle;

		public static readonly InputButton LiftContainer;

		public static readonly InputButton DumpStart;

		public static readonly InputButton DumpEnd;

		public static readonly InputButton PitchforkUp;

		public static readonly InputButton PitchforkDown;

		public static readonly InputButton Map;

		public static readonly InputButton Pause;

		public static readonly InputButton GarageDoors;

		public static readonly InputButton StartShopping;

		public static readonly InputButton Jump;

		public static readonly InputButton VehicleJump;

		public static readonly InputButton Fly;

		public static readonly InputButton Grenade;

		public static readonly InputButton Magic;

		public static readonly InputButton Rope;

		public static readonly InputButton SwitchCamera;

		public static readonly InputButton SwitchWeapon;

		public static readonly InputButton SwitchGrenade;

		public static readonly InputButton MagicShield;

		public static readonly InputButton MechFly;

		public static readonly InputButton MechFlyUp;

		public static readonly InputButton MechFlyDown;

		public static readonly InputButton Interact;

		public static readonly InputButton ScopeAim;

		public static readonly InputButton AirplaneReset;

		public static readonly InputButton DogAttack;

		private static ControlMode controlMode;

		public static ControlMode ControlMode => GetControlMode();

		public static bool HasCursorControl => ControlMode == ControlMode.keyboard;

		static InputUtils()
		{
			Attack = new InputButton("AttackButton", "Fire1");
			TankMinigun = new InputButton("TankMinigun", "Fire1");
			TankRockets = new InputButton("TankRockets", "Fire2");
			HeliMinigun = new InputButton("MiniGunButton", "Fire1");
			HeliRockets = new InputButton("RocketsButton", "Fire2");
			MechPrimaryAttack = new InputButton("MechPrimaryAttack", "Fire1");
			MechSecondaryAttack = new InputButton("MechSecondaryAttack", "Fire2");
			Eyes = new InputButton("oci", "Fire2");
			WaterShoot = new InputButton("WaterShootButton", "Fire1");
			FlyDown = new InputButton("FlyDownButton", "FlyDown");
			FlyUp = new InputButton("FlyUpButton", "FlyUp");
			Kick = new InputButton("kick", "Kick");
			RunFast = new InputButton("RunFastButton", "RunFast");
			Vehicle = new InputButton("VehicleButton", "Vehicle");
			LiftContainer = new InputButton("LiftContainerButton", "LiftContainer");
			DumpStart = new InputButton("DumpStartButton", "DumpStart");
			DumpEnd = new InputButton("DumpEndButton", "DumpEnd");
			PitchforkUp = new InputButton("PitchforkUp", "PitchforkUp");
			PitchforkDown = new InputButton("PitchforkDown", "PitchforkDown");
			Map = new InputButton(string.Empty, "Map");
			Pause = new InputButton(string.Empty, "Pause");
			GarageDoors = new InputButton("GarageDoorsButton", "GarageDoors");
			StartShopping = new InputButton("StartShoppingButton", "StartShoping");
			Jump = new InputButton("JumpButton", "Jump");
			VehicleJump = new InputButton("VehicleJumpButton", "Jump");
			Fly = new InputButton("FlyButton", "Fly");
			Grenade = new InputButton("GrenadeButton", "Grenade");
			Magic = new InputButton("btnMagicAttack", "MagicAttack");
			Rope = new InputButton("RopeButton", "Rope");
			SwitchCamera = new InputButton(string.Empty, "SwitchCamera");
			SwitchWeapon = new InputButton(string.Empty, "SwitchWeapon");
			SwitchGrenade = new InputButton("SwitchGrenadeButton", "SwitchGrenade");
			MagicShield = new InputButton("btnMagicShield", "MagicShield");
			MechFly = new InputButton("MechTechnologyButton", string.Empty);
			MechFlyUp = new InputButton("MechFlyUpButton", string.Empty);
			MechFlyDown = new InputButton("MechFlyDownButton", string.Empty);
			Interact = new InputButton("InteractButton", string.Empty);
			ScopeAim = new InputButton("AimButton", string.Empty);
			AirplaneReset = new InputButton("AirplaneResetButton", string.Empty);
			DogAttack = new InputButton("DogAttackButton", string.Empty);
			controlMode = SettingsManager.GameSettings.ControlMode;
		}

		public static bool GetVehicleButtonDown(VehicleType vehicle, string buttonName)
		{
			return GetVehicleInputButton(vehicle, buttonName)?.IsDown ?? false;
		}

		public static bool GetVehicleButton(VehicleType vehicle, string buttonName)
		{
			return GetVehicleInputButton(vehicle, buttonName)?.IsPressed ?? false;
		}

		public static bool GetButton(string buttonName)
		{
			if (!GetIsInputSupportedMode())
			{
				return false;
			}
			if (GetIsGamePaused())
			{
				return false;
			}
			return Input.GetButton(buttonName);
		}

		public static bool GetButtonDown(string buttonName)
		{
			if (!GetIsInputSupportedMode())
			{
				return false;
			}
			if (GetIsGamePaused())
			{
				return false;
			}
			return Input.GetButtonDown(buttonName);
		}

		public static float GetAxis(string axisName)
		{
			if (!GetIsInputSupportedMode())
			{
				return 0f;
			}
			return UnityEngine.Input.GetAxis(axisName);
		}

		public static float GetHorizontalLookAxis()
		{
			if (!GetIsInputSupportedMode())
			{
				return ETCInput.GetAxis("Horizontal");
			}
			return UnityEngine.Input.GetAxis("Mouse X");
		}

		public static float GetVerticalLookAxis()
		{
			if (!GetIsInputSupportedMode())
			{
				return ETCInput.GetAxis("Vertical");
			}
			return UnityEngine.Input.GetAxis("Mouse Y");
		}

		public static float GetHorizontalJoystick()
		{
			if (!GetIsInputSupportedMode())
			{
				return ETCInput.GetAxis("HorizontalJoystick");
			}
			return UnityEngine.Input.GetAxis("Horizontal");
		}

		public static float GetVerticalJoystick()
		{
			if (!GetIsInputSupportedMode())
			{
				return ETCInput.GetAxis("VerticalJoystick");
			}
			return UnityEngine.Input.GetAxis("Vertical");
		}

		public static void LockCursor()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public static void UnlockCuros()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		public static bool GetIsGamePaused()
		{
			return Time.timeScale < 0.01f;
		}

		private static bool GetIsInputSupportedMode()
		{
			return GetControlMode() == ControlMode.keyboard;
		}

		private static ControlMode GetControlMode()
		{
			return controlMode;
		}

		private static InputButton GetVehicleInputButton(VehicleType vehicle, string buttonName)
		{
			switch (vehicle)
			{
			case VehicleType.Helicopter:
				if (buttonName == HeliMinigun.ButtonName)
				{
					return HeliMinigun;
				}
				if (buttonName == HeliRockets.ButtonName)
				{
					return HeliRockets;
				}
				break;
			case VehicleType.Tank:
				if (buttonName == TankMinigun.ButtonName)
				{
					return TankMinigun;
				}
				if (buttonName == TankRockets.ButtonName)
				{
					return TankRockets;
				}
				break;
			case VehicleType.Mech:
				if (buttonName == MechPrimaryAttack.ButtonName)
				{
					return MechPrimaryAttack;
				}
				if (buttonName == MechSecondaryAttack.ButtonName)
				{
					return MechSecondaryAttack;
				}
				break;
			}
			return null;
		}
	}
}
