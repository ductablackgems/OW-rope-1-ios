using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse")]
public class MouseCameraControl : MonoBehaviour
{
	public enum MouseButton
	{
		Left,
		Right,
		Middle,
		None
	}

	[Serializable]
	public class Modifiers
	{
		public bool leftAlt;

		public bool leftControl;

		public bool leftShift;

		public bool checkModifiers()
		{
			if ((!leftAlt ^ UnityEngine.Input.GetKey(KeyCode.LeftAlt)) && (!leftControl ^ UnityEngine.Input.GetKey(KeyCode.LeftControl)))
			{
				return !leftShift ^ UnityEngine.Input.GetKey(KeyCode.LeftShift);
			}
			return false;
		}
	}

	[Serializable]
	public class MouseControlConfiguration
	{
		public bool activate;

		public MouseButton mouseButton;

		public Modifiers modifiers;

		public float sensitivity;

		public bool isActivated()
		{
			if (activate && Input.GetMouseButton((int)mouseButton))
			{
				return modifiers.checkModifiers();
			}
			return false;
		}
	}

	[Serializable]
	public class MouseScrollConfiguration
	{
		public bool activate;

		public Modifiers modifiers;

		public float sensitivity;

		public bool isActivated()
		{
			if (activate)
			{
				return modifiers.checkModifiers();
			}
			return false;
		}
	}

	public MouseControlConfiguration yaw = new MouseControlConfiguration
	{
		mouseButton = MouseButton.Right,
		sensitivity = 10f
	};

	public MouseControlConfiguration pitch = new MouseControlConfiguration
	{
		mouseButton = MouseButton.Right,
		modifiers = new Modifiers
		{
			leftControl = true
		},
		sensitivity = 10f
	};

	public MouseControlConfiguration roll = new MouseControlConfiguration();

	public MouseControlConfiguration verticalTranslation = new MouseControlConfiguration
	{
		mouseButton = MouseButton.Middle,
		sensitivity = 2f
	};

	public MouseControlConfiguration horizontalTranslation = new MouseControlConfiguration
	{
		mouseButton = MouseButton.Middle,
		sensitivity = 2f
	};

	public MouseControlConfiguration depthTranslation = new MouseControlConfiguration
	{
		mouseButton = MouseButton.Left,
		sensitivity = 2f
	};

	public MouseScrollConfiguration scroll = new MouseScrollConfiguration
	{
		sensitivity = 2f
	};

	public string mouseHorizontalAxisName = "Mouse X";

	public string mouseVerticalAxisName = "Mouse Y";

	public string scrollAxisName = "Mouse ScrollWheel";

	private void LateUpdate()
	{
		if (yaw.isActivated())
		{
			float yAngle = UnityEngine.Input.GetAxis(mouseHorizontalAxisName) * yaw.sensitivity;
			base.transform.Rotate(0f, yAngle, 0f);
		}
		if (pitch.isActivated())
		{
			float num = UnityEngine.Input.GetAxis(mouseVerticalAxisName) * pitch.sensitivity;
			base.transform.Rotate(0f - num, 0f, 0f);
		}
		if (roll.isActivated())
		{
			float zAngle = UnityEngine.Input.GetAxis(mouseHorizontalAxisName) * roll.sensitivity;
			base.transform.Rotate(0f, 0f, zAngle);
		}
		if (verticalTranslation.isActivated())
		{
			float y = UnityEngine.Input.GetAxis(mouseVerticalAxisName) * verticalTranslation.sensitivity;
			base.transform.Translate(0f, y, 0f);
		}
		if (horizontalTranslation.isActivated())
		{
			float x = UnityEngine.Input.GetAxis(mouseHorizontalAxisName) * horizontalTranslation.sensitivity;
			base.transform.Translate(x, 0f, 0f);
		}
		if (depthTranslation.isActivated())
		{
			float z = UnityEngine.Input.GetAxis(mouseVerticalAxisName) * depthTranslation.sensitivity;
			base.transform.Translate(0f, 0f, z);
		}
		if (scroll.isActivated())
		{
			float z2 = UnityEngine.Input.GetAxis(scrollAxisName) * scroll.sensitivity;
			base.transform.Translate(0f, 0f, z2);
		}
	}
}
