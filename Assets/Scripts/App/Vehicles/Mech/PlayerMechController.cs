using App.Util;
using UnityEngine;

namespace App.Vehicles.Mech
{
	public class PlayerMechController : MonoBehaviour
	{
		private MechController controller;

		private MechManager manager;

		private void Awake()
		{
			controller = this.GetComponentSafe<MechController>();
			manager = this.GetComponentSafe<MechManager>();
		}

		private void Update()
		{
			if (manager.IsActive)
			{
				Vector2 move = new Vector2(ETCInput.GetAxis("HorizontalJoystick"), ETCInput.GetAxis("VerticalJoystick"));
				controller.Move(move);
				if (InputUtils.MechFly.IsDown)
				{
					controller.ToggleTechnology();
				}
				if (InputUtils.MechFlyUp.IsPressed)
				{
					controller.MoveUp();
				}
				if (InputUtils.MechFlyDown.IsPressed)
				{
					controller.MoveDown();
				}
			}
		}
	}
}
