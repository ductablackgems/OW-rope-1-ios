using App.Camera;
using App.GUI;
using App.GUI.Panels;
using App.Shop.GunSlider;
using App.Util;
using App.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player
{
	public class PCKeysController : MonoBehaviour
	{
		public List<GameObject> objectForHide;

		[Header("CHARACTER")]
		public KeyCode frontCode;

		public KeyCode backCode;

		public KeyCode leftCode;

		public KeyCode rightCode;

		public KeyCode jumpCode;

		public KeyCode flyCode;

		public KeyCode flyUpCode;

		public KeyCode flyDownCode;

		public KeyCode crouchCode;

		public KeyCode sprintCode;

		public KeyCode equipCode;

		public KeyCode ropeCode;

		public AttackControl attackCode;

		public AttackControl laserCode;

		public AttackControl magicShield;

		[Header("COMMON VEHICLE")]
		public KeyCode vehicleFrontCode;

		public KeyCode vehicleBackCode;

		public KeyCode vehicleLeftCode;

		public KeyCode vehicleRightCode;

		public KeyCode vehicleAlternativeFrontCode;

		public KeyCode vehicleAlternativeBackCode;

		public KeyCode vehicleAlternativeLeftCode;

		public KeyCode vehicleAlternativeRightCode;

		[Header("GARBAGE TRUCK VEHICLE")]
		public KeyCode garbageCargoCode;

		public KeyCode garbagePitchforkCode;

		[Header("FIRE TRUCK VEHICLE")]
		public AttackControl fireCannonCode;

		[Header("TANK VEHICLE")]
		public AttackControl tankMiniGunCode;

		public AttackControl tankRocketsCode;

		[Header("HELICOPTER VEHICLE")]
		public AttackControl helicopterAttack001Code;

		public AttackControl helicopterAttack002Code;

		[Header("OTHER")]
		public KeyCode mapCode;

		public KeyCode shopCode;

		public KeyCode garageCode;

		public KeyCode cameraSwitchCode;

		public KeyCode pauseCode;

		public KeyCode leftArrow;

		public KeyCode rightArrow;

		public KeyCode buyCode;

		private WeaponInventory weaponInvenoty;

		private PlayerController playerController;

		private PanelsManager panelsManager;

		private Pauser pauser;

		private GunSliderControl sliderControl;

		private CameraManager cameraManager;

		public MapPanel mapPanel;

		private bool mapShowed;

		private bool pauseShowed;

		private bool shoppingShowed;

		private static PCKeysController instance;

		public float MouseMultiplier => 10f;

		public float Horizontal
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					if (!Input.GetKey(rightCode))
					{
						if (!Input.GetKey(leftCode))
						{
							return 0f;
						}
						return -1f;
					}
					return 1f;
				}
				return 0f;
			}
		}

		public float Vertical
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					if (!Input.GetKey(frontCode))
					{
						if (!Input.GetKey(backCode))
						{
							return 0f;
						}
						return -1f;
					}
					return 1f;
				}
				return 0f;
			}
		}

		public bool Sprint
		{
			get
			{
				if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKey(sprintCode);
				}
				return false;
			}
		}

		public bool Crouche
		{
			get
			{
				if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKey(crouchCode);
				}
				return false;
			}
		}

		public bool Jump
		{
			get
			{
				if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKey(jumpCode);
				}
				return false;
			}
		}

		public bool Fly
		{
			get
			{
				if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKey(flyCode);
				}
				return false;
			}
		}

		public bool FlyUp
		{
			get
			{
				if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKey(flyUpCode);
				}
				return false;
			}
		}

		public bool FlyDown
		{
			get
			{
				if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKey(flyDownCode);
				}
				return false;
			}
		}

		public bool Equip
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					return UnityEngine.Input.GetKeyDown(equipCode);
				}
				return false;
			}
		}

		public bool Rope
		{
			get
			{
				if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKeyDown(ropeCode);
				}
				return false;
			}
		}

		public bool Laser
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (laserCode.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(laserCode.keyCode);
					}
					return false;
				}
				if (laserCode.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)laserCode.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool Attack
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (attackCode.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKeyDown(attackCode.keyCode);
					}
					return false;
				}
				if (attackCode.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return Input.GetMouseButtonDown((int)attackCode.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool Attacking
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (attackCode.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(attackCode.keyCode);
					}
					return false;
				}
				if (attackCode.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)attackCode.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool MagicShield
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (magicShield.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(magicShield.keyCode);
					}
					return false;
				}
				if (magicShield.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && !playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)magicShield.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool VehicleFront
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					if (!Input.GetKey(vehicleFrontCode))
					{
						return UnityEngine.Input.GetKey(vehicleAlternativeFrontCode);
					}
					return true;
				}
				return false;
			}
		}

		public bool VehicleBack
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					if (!Input.GetKey(vehicleBackCode))
					{
						return UnityEngine.Input.GetKey(vehicleAlternativeBackCode);
					}
					return true;
				}
				return false;
			}
		}

		public bool VehicleLeft
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					if (!Input.GetKey(vehicleLeftCode))
					{
						return UnityEngine.Input.GetKey(vehicleAlternativeLeftCode);
					}
					return true;
				}
				return false;
			}
		}

		public bool VehicleRight
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					if (!Input.GetKey(vehicleRightCode))
					{
						return UnityEngine.Input.GetKey(vehicleAlternativeRightCode);
					}
					return true;
				}
				return false;
			}
		}

		public bool CargoAction
		{
			get
			{
				if (playerController != null && playerController.Controlled && playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKeyDown(garbageCargoCode);
				}
				return false;
			}
		}

		public bool PitchforkAction
		{
			get
			{
				if (playerController != null && playerController.Controlled && playerController.UseVehicle())
				{
					return UnityEngine.Input.GetKeyDown(garbagePitchforkCode);
				}
				return false;
			}
		}

		public bool WaterShoot
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (fireCannonCode.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(fireCannonCode.keyCode);
					}
					return false;
				}
				if (fireCannonCode.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)fireCannonCode.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool TankMinigun
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (tankMiniGunCode.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(tankMiniGunCode.keyCode);
					}
					return false;
				}
				if (tankMiniGunCode.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)tankMiniGunCode.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool TankRockets
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (tankRocketsCode.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(tankRocketsCode.keyCode);
					}
					return false;
				}
				if (tankRocketsCode.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)tankRocketsCode.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool HelicopterAttack1
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (helicopterAttack001Code.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(helicopterAttack001Code.keyCode);
					}
					return false;
				}
				if (helicopterAttack001Code.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)helicopterAttack001Code.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool HelicopterAttack2
		{
			get
			{
				if (mapShowed || pauseShowed || shoppingShowed)
				{
					return false;
				}
				if (helicopterAttack002Code.controlType == ControlType.Keys)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return UnityEngine.Input.GetKey(helicopterAttack002Code.keyCode);
					}
					return false;
				}
				if (helicopterAttack002Code.controlType == ControlType.Mouse)
				{
					if (playerController != null && playerController.Controlled && playerController.UseVehicle())
					{
						return Input.GetMouseButton((int)helicopterAttack002Code.mouseButtonType);
					}
					return false;
				}
				return false;
			}
		}

		public bool ShowMap => UnityEngine.Input.GetKeyDown(mapCode);

		public bool GoToShop
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					return UnityEngine.Input.GetKeyDown(shopCode);
				}
				return false;
			}
		}

		public bool GoToGarage
		{
			get
			{
				if (playerController != null && playerController.Controlled)
				{
					return UnityEngine.Input.GetKeyDown(garageCode);
				}
				return false;
			}
		}

		public bool Pause => UnityEngine.Input.GetKeyDown(pauseCode);

		public bool Left => UnityEngine.Input.GetKeyDown(leftArrow);

		public bool Right => UnityEngine.Input.GetKeyDown(rightArrow);

		public bool Buy => UnityEngine.Input.GetKeyDown(buyCode);

		public bool Escape => UnityEngine.Input.GetKeyDown(KeyCode.Escape);

		public bool IsPC
		{
			get;
			private set;
		}

		public static PCKeysController Instance => instance;

		private void Awake()
		{
			instance = this;
			playerController = ServiceLocator.GetGameObject("Player").GetComponent<PlayerController>();
			weaponInvenoty = ServiceLocator.GetGameObject("Player").GetComponent<WeaponInventory>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
			pauser = ServiceLocator.Get<Pauser>();
			sliderControl = ServiceLocator.Get<GunSliderControl>();
			cameraManager = ServiceLocator.Get<CameraManager>();
		}

		private void Start()
		{
			if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WebGLPlayer)
			{
				IsPC = true;
				for (int i = 0; i < objectForHide.Count; i++)
				{
					objectForHide[i].SetActive(value: false);
				}
			}
		}

		private void Update()
		{
			if (!IsPC)
			{
				return;
			}
			if (pauseShowed || mapShowed || shoppingShowed)
			{
				ActivateCursor();
			}
			else
			{
				DeactivateCursor();
			}
			if (ShowMap)
			{
				OnClickMap();
			}
			if (mapShowed && UnityEngine.Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				if (!mapPanel)
				{
					mapPanel = ServiceLocator.Get<MapPanel>();
				}
				if ((bool)mapPanel)
				{
					if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f && mapPanel.bigMapView.orthographicSize < -80f)
					{
						mapPanel.ZoomMap(50f * Time.deltaTime * 5000f * 25f);
					}
					if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f && mapPanel.bigMapView.orthographicSize > -1000f)
					{
						mapPanel.ZoomMap(-50f * Time.deltaTime * 5000f * 25f);
					}
				}
			}
			if (!mapShowed && playerController.Controlled && !playerController.UseVehicle() && UnityEngine.Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
				{
					NextGun();
				}
				if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
				{
					PrevGun();
				}
			}
			if (Pause)
			{
				OnClickPause();
			}
			if (Escape)
			{
				if (mapShowed)
				{
					OnClickMap();
				}
				else if (pauseShowed)
				{
					OnClickPause();
				}
				else if (shoppingShowed)
				{
					HideShop();
				}
				else
				{
					ShowPause();
				}
			}
		}

		public bool InHideObjects(GameObject obj)
		{
			if (!IsPC)
			{
				return false;
			}
			for (int i = 0; i < objectForHide.Count; i++)
			{
				if (objectForHide[i] == obj)
				{
					return true;
				}
			}
			return false;
		}

		public void NextGun()
		{
			weaponInvenoty.SwitchGun();
		}

		public void PrevGun()
		{
			weaponInvenoty.SwitchGunBack();
		}

		public void ShowBigMap()
		{
			mapShowed = true;
			panelsManager.ShowPanel(PanelType.Map);
		}

		public void HideBigMap()
		{
			panelsManager.ShowPanel(panelsManager.PreviousPanel.GetPanelType());
			mapShowed = false;
		}

		public void OnClickMap()
		{
			if (!mapShowed)
			{
				ShowBigMap();
			}
			else
			{
				HideBigMap();
			}
		}

		public void ShowPause()
		{
			pauseShowed = true;
			pauser.Pause();
			ActivateCursor();
		}

		public void HidePause()
		{
			pauser.Resume();
			pauseShowed = false;
			DeactivateCursor();
		}

		public void OnClickPause()
		{
			if (!pauseShowed)
			{
				ShowPause();
			}
			else
			{
				HidePause();
			}
		}

		public void ShowShop()
		{
			shoppingShowed = true;
		}

		public void HideShop()
		{
			shoppingShowed = false;
			sliderControl.Stop();
			cameraManager.SetPlayerCamera();
			panelsManager.ShowPanel(PanelType.Game);
		}

		public void OnClickShop()
		{
			if (!shoppingShowed)
			{
				ShowShop();
			}
			else
			{
				HideShop();
			}
		}

		public void ActivateCursor()
		{
			if (!Cursor.visible || Cursor.lockState != 0)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}

		public void DeactivateCursor()
		{
			if (Cursor.visible || Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		public bool GetVehicleFireButton(VehicleType vehicleType, string buttonName)
		{
			switch (vehicleType)
			{
			case VehicleType.Tank:
				if (Instance.tankMiniGunCode.alternativeButtonName == buttonName)
				{
					return Instance.TankMinigun;
				}
				if (Instance.tankRocketsCode.alternativeButtonName == buttonName)
				{
					return Instance.TankRockets;
				}
				break;
			case VehicleType.Helicopter:
				if (Instance.helicopterAttack001Code.alternativeButtonName == buttonName)
				{
					return Instance.HelicopterAttack1;
				}
				if (Instance.helicopterAttack002Code.alternativeButtonName == buttonName)
				{
					return Instance.HelicopterAttack2;
				}
				break;
			}
			return false;
		}
	}
}
