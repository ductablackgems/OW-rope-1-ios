using UnityEngine;

namespace App.Vehicles
{
	public class VehicleInputsHelper
	{
		public static float GetVerticalInput()
		{
			bool num = ETCInput.GetButton("VehicleForwardButton") || UnityEngine.Input.GetAxis("Vertical") > 0f;
			bool flag = ETCInput.GetButton("VehicleBackButton") || UnityEngine.Input.GetAxis("Vertical") < 0f;
			return num ? 1 : (flag ? (-1) : 0);
		}

		public static float GetHorizonatalInput(float horizontalInput)
		{
			bool num = ETCInput.GetButton("SteerRightButton") || UnityEngine.Input.GetAxis("Horizontal") > 0f;
			bool flag = ETCInput.GetButton("SteerLeftButton") || UnityEngine.Input.GetAxis("Horizontal") < 0f;
			float maxDelta = 5f * Time.fixedDeltaTime;
			horizontalInput = ((!(num | flag)) ? Mathf.MoveTowards(horizontalInput, 0f, maxDelta) : ((!flag) ? Mathf.MoveTowards(horizontalInput, 1f, maxDelta) : Mathf.MoveTowards(horizontalInput, -1f, maxDelta)));
			return horizontalInput;
		}
	}
}
