using App.Vehicles.Tank;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
	private TankController tankController;

	private TankManager tankManager;

	private float horizontalInput;

	private void Awake()
	{
		tankController = this.GetComponentSafe<TankController>();
		tankManager = this.GetComponentSafe<TankManager>();
	}

	private void OnEnable()
	{
		horizontalInput = 0f;
	}

	private void FixedUpdate()
	{
		if (tankManager.Active)
		{
			UpdateHorizontalInput();
			float verticalInput = GetVerticalInput();
			tankController.Move(verticalInput, horizontalInput);
		}
	}

	private void UpdateHorizontalInput()
	{
		bool num = ETCInput.GetButton("SteerRightButton") || UnityEngine.Input.GetAxis("Horizontal") > 0f;
		bool flag = ETCInput.GetButton("SteerLeftButton") || UnityEngine.Input.GetAxis("Horizontal") < 0f;
		float maxDelta = 5f * Time.fixedDeltaTime;
		if (num | flag)
		{
			if (flag)
			{
				horizontalInput = Mathf.MoveTowards(horizontalInput, -1f, maxDelta);
			}
			else
			{
				horizontalInput = Mathf.MoveTowards(horizontalInput, 1f, maxDelta);
			}
		}
		else
		{
			horizontalInput = Mathf.MoveTowards(horizontalInput, 0f, maxDelta);
		}
	}

	private float GetVerticalInput()
	{
		bool num = ETCInput.GetButton("VehicleForwardButton") || UnityEngine.Input.GetAxis("Vertical") > 0f;
		bool flag = ETCInput.GetButton("VehicleBackButton") || UnityEngine.Input.GetAxis("Vertical") < 0f;
		return num ? 1 : (flag ? (-1) : 0);
	}
}
