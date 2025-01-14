using App.Util;
using App.Vehicles.Airplane;
using UnityEngine;

namespace App.Player
{
	public class PlayerAirplaneController : MonoBehaviour
	{
		private AirplaneController controller;

		private AirplaneManager manager;

		private void Awake()
		{
			controller = GetComponent<AirplaneController>();
			manager = GetComponent<AirplaneManager>();
		}

		private void FixedUpdate()
		{
			if (manager.IsActive)
			{
				float horizontalLookAxis = InputUtils.GetHorizontalLookAxis();
				float verticalLookAxis = InputUtils.GetVerticalLookAxis();
				float horizontalJoystick = InputUtils.GetHorizontalJoystick();
				float verticalJoystick = InputUtils.GetVerticalJoystick();
				bool airBrakes = verticalJoystick < -0.8f;
				controller.Move(horizontalLookAxis, verticalLookAxis, horizontalJoystick, verticalJoystick, airBrakes);
			}
		}

		private void Update()
		{
			if (InputUtils.AirplaneReset.IsDown)
			{
				controller.Respawn();
			}
		}
	}
}
