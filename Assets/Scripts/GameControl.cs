using App;
using App.GUI;
using App.Player;
using App.Player.Clothes;
using App.Shop;
using App.Util;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl manager;

    public static float accelFwd;

    public static float accelBack;

    public static float steerAmount;

    public static bool shift;

    public static bool brake;

    public static bool driving;

    public static bool jump;

    public ControlMode controlMode = ControlMode.simple;

    public GameObject getInVehicle;

    public Text hintGarbageTruck;

    public GameObject[] touchControlsToHide;

    private float drivingTimer;

    private Pauser pauser;

    private ShoppingZone shoppingZone;

    private ClothesShoppingZone clothesShoppingZone;

    private PanelsManager panelsManager;

    private PlayerMonitor playerMonitor;

    public void VehicleAccelForward(float amount)
    {
        accelFwd = amount;
    }

    public void VehicleAccelBack(float amount)
    {
        accelBack = amount;
    }

    public void VehicleSteer(float amount)
    {
        steerAmount = amount;
    }

    public void VehicleHandBrake(bool HBrakeing)
    {
        brake = HBrakeing;
    }

    public void VehicleShift(bool Shifting)
    {
        shift = Shifting;
    }

    public void GetInVehicle()
    {
        if (drivingTimer == 0f)
        {
            driving = true;
            drivingTimer = 3f;
        }
    }

    public void GetOutVehicle()
    {
        if (drivingTimer == 0f)
        {
            driving = false;
            drivingTimer = 3f;
        }
    }

    public void Jumping()
    {
        jump = true;
    }

    private void Awake()
    {
        manager = this;
    }

    private void Start()
    {
        pauser = ServiceLocator.Get<Pauser>();
        shoppingZone = ServiceLocator.Get<ShoppingZone>();
        clothesShoppingZone = ServiceLocator.Get<ClothesShoppingZone>(showError: false);
        panelsManager = ServiceLocator.Get<PanelsManager>();
        playerMonitor = ServiceLocator.Get<PlayerMonitor>();
    }

    private void Update()
    {
        drivingTimer = Mathf.MoveTowards(drivingTimer, 0f, Time.deltaTime);
        if (InputUtils.ControlMode == ControlMode.keyboard)
        {
            UpdateKeyboardControl();
            UpdateHint();
        }
    }

    private void LateUpdate()
    {
        if (InputUtils.ControlMode == ControlMode.keyboard)
        {
            UpdateCursor();
            UpdateHiddenControls();
        }
    }

    private void UpdateKeyboardControl()
    {
        if (InputUtils.Pause.IsDown)
        {
            pauser.Pause();
        }
    }

    private bool GetEnableCursor()
    {
        if (IsPlayerInShop())
        {
            return true;
        }
        if (InputUtils.GetIsGamePaused())
        {
            return true;
        }
        return false;
    }

    private bool IsPlayerInShop()
    {
        if (shoppingZone != null && shoppingZone.IsPlayerIn() && panelsManager.CompareActivePanel(PanelType.GunShop))
        {
            return true;
        }
        if (clothesShoppingZone != null && clothesShoppingZone.IsPlayerIn() && panelsManager.CompareActivePanel(PanelType.ClothesShop))
        {
            return true;
        }
        return false;
    }

    private void UpdateHiddenControls()
    {
        for (int i = 0; i < touchControlsToHide.Length; i++)
        {
            GameObject gameObject = touchControlsToHide[i];
            if (!(gameObject == null))
            {
                gameObject.SetActive(value: false);
            }
        }
    }

    private void UpdateHint()
    {
        if (!(hintGarbageTruck == null))
        {
            bool active = playerMonitor.UsingGarbageTruck() && !InputUtils.GetIsGamePaused();
            hintGarbageTruck.gameObject.SetActive(active);
        }
    }

    private void UpdateCursor()
    {
        if (GetEnableCursor())
        {
            InputUtils.UnlockCuros();
        }
        else
        {
            InputUtils.LockCursor();
        }
    }
}
