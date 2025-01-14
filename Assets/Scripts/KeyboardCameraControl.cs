using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Keyboard")]
public class KeyboardCameraControl : MonoBehaviour
{
	public enum KeyboardAxis
	{
		Horizontal = 0,
		Vertical = 1,
		None = 3
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
	public class KeyboardControlConfiguration
	{
		public bool activate;

		public KeyboardAxis keyboardAxis;

		public Modifiers modifiers;

		public float sensitivity;

		public bool isActivated()
		{
			if (activate && keyboardAxis != KeyboardAxis.None)
			{
				return modifiers.checkModifiers();
			}
			return false;
		}
	}

	public KeyboardControlConfiguration yaw = new KeyboardControlConfiguration
	{
		keyboardAxis = KeyboardAxis.Horizontal,
		modifiers = new Modifiers
		{
			leftAlt = true
		},
		sensitivity = 1f
	};

	public KeyboardControlConfiguration pitch = new KeyboardControlConfiguration
	{
		keyboardAxis = KeyboardAxis.Vertical,
		modifiers = new Modifiers
		{
			leftAlt = true
		},
		sensitivity = 1f
	};

	public KeyboardControlConfiguration roll = new KeyboardControlConfiguration
	{
		keyboardAxis = KeyboardAxis.Horizontal,
		modifiers = new Modifiers
		{
			leftAlt = true,
			leftControl = true
		},
		sensitivity = 1f
	};

	public KeyboardControlConfiguration verticalTranslation = new KeyboardControlConfiguration
	{
		keyboardAxis = KeyboardAxis.Vertical,
		modifiers = new Modifiers
		{
			leftControl = true
		},
		sensitivity = 0.5f
	};

	public KeyboardControlConfiguration horizontalTranslation = new KeyboardControlConfiguration
	{
		keyboardAxis = KeyboardAxis.Horizontal,
		sensitivity = 0.5f
	};

	public KeyboardControlConfiguration depthTranslation = new KeyboardControlConfiguration
	{
		keyboardAxis = KeyboardAxis.Vertical,
		sensitivity = 0.5f
	};

	public string keyboardHorizontalAxisName = "Horizontal";

	public string keyboardVerticalAxisName = "Vertical";

	private string[] keyboardAxesNames;

	private void Start()
	{
		keyboardAxesNames = new string[2]
		{
			keyboardHorizontalAxisName,
			keyboardVerticalAxisName
		};
	}

	private void LateUpdate()
	{
		if (yaw.isActivated())
		{
			float yAngle = UnityEngine.Input.GetAxis(keyboardAxesNames[(int)yaw.keyboardAxis]) * yaw.sensitivity;
			base.transform.Rotate(0f, yAngle, 0f);
		}
		if (pitch.isActivated())
		{
			float num = UnityEngine.Input.GetAxis(keyboardAxesNames[(int)pitch.keyboardAxis]) * pitch.sensitivity;
			base.transform.Rotate(0f - num, 0f, 0f);
		}
		if (roll.isActivated())
		{
			float zAngle = UnityEngine.Input.GetAxis(keyboardAxesNames[(int)roll.keyboardAxis]) * roll.sensitivity;
			base.transform.Rotate(0f, 0f, zAngle);
		}
		if (verticalTranslation.isActivated())
		{
			float y = UnityEngine.Input.GetAxis(keyboardAxesNames[(int)verticalTranslation.keyboardAxis]) * verticalTranslation.sensitivity;
			base.transform.Translate(0f, y, 0f);
		}
		if (horizontalTranslation.isActivated())
		{
			float x = UnityEngine.Input.GetAxis(keyboardAxesNames[(int)horizontalTranslation.keyboardAxis]) * horizontalTranslation.sensitivity;
			base.transform.Translate(x, 0f, 0f);
		}
		if (depthTranslation.isActivated())
		{
			float z = UnityEngine.Input.GetAxis(keyboardAxesNames[(int)depthTranslation.keyboardAxis]) * depthTranslation.sensitivity;
			base.transform.Translate(0f, 0f, z);
		}
	}
}
