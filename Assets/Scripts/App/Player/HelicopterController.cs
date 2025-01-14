using App.Guns;
using App.Util;
using UnityEngine;

namespace App.Player
{
	[RequireComponent(typeof(HelicopterControls))]
	public class HelicopterController : MonoBehaviour
	{
		private const float MaxVerticalAngle = 25f;

		private HelicopterControls controls;

		private AccelerationJoystick accelerationJoystick;

		protected void Awake()
		{
			controls = this.GetComponentSafe<HelicopterControls>();
			accelerationJoystick = ServiceLocator.Get<AccelerationJoystick>();
		}

		private void OnEnable()
		{
			PlayerGunHandle[] componentsInChildren = GetComponentsInChildren<PlayerGunHandle>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}

		private void OnDisable()
		{
			PlayerGunHandle[] componentsInChildren = GetComponentsInChildren<PlayerGunHandle>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}

		protected void FixedUpdate()
		{
			float axis = ETCInput.GetAxis("HorizontalJoystick");
			float axis2 = ETCInput.GetAxis("VerticalJoystick");
			float horizontalLookAxis = InputUtils.GetHorizontalLookAxis();
			float num = (axis2 > 0f) ? accelerationJoystick.GetVerticalAxis() : 0f;
			Vector3 vector = new Vector3(axis, 0f, axis2);
			vector = vector.RotateX(num * 25f);
			controls.Move(vector, horizontalLookAxis);
		}
	}
}
