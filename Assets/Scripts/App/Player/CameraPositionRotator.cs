using App.Util;
using UnityEngine;

namespace App.Player
{
	public class CameraPositionRotator : MonoBehaviour
	{
		public float maxVerticalAngle = 50f;

		public float speed = 30f;

		public float verticalAngleOffset;

		public bool supportYRotation;

		private AccelerationJoystick accelerationJoystick;

		protected void Awake()
		{
			accelerationJoystick = ServiceLocator.Get<AccelerationJoystick>();
		}

		protected void FixedUpdate()
		{
			float x = accelerationJoystick.GetVerticalAxis() * maxVerticalAngle + verticalAngleOffset;
			float y = supportYRotation ? (base.transform.localRotation.eulerAngles.y + InputUtils.GetHorizontalLookAxis()) : 0f;
			base.transform.localRotation = Quaternion.Euler(x, y, 0f);
		}
	}
}
