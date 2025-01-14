using App.Util;
using UnityEngine;

namespace App.Player
{
	public class AccelerationJoystick : MonoBehaviour
	{
		public float maxVerticalDeviceAngle = 10f;

		private float verticalAxis;

		public float GetVerticalAxis()
		{
			return verticalAxis;
		}

		protected void FixedUpdate()
		{
			float verticalLookAxis = InputUtils.GetVerticalLookAxis();
			verticalAxis = Mathf.Clamp(verticalAxis - verticalLookAxis / maxVerticalDeviceAngle, -1f, 1f);
		}
	}
}
