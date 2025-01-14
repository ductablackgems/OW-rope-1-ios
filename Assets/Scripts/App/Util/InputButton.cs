using UnityEngine;

namespace App.Util
{
	public sealed class InputButton
	{
		public string ButtonName
		{
			get;
			private set;
		}

		public string InputName
		{
			get;
			private set;
		}

		public bool IsDown => GetButtonState();

		public bool IsPressed => GetButtonState(isPress: true);

		public InputButton(string buttonName, string inputName)
		{
			ButtonName = buttonName;
			InputName = inputName;
		}

		private bool GetButtonState(bool isPress = false)
		{
			if (InputUtils.ControlMode != ControlMode.keyboard)
			{
				return GetButton(isPress);
			}
			return GetInput(isPress);
		}

		private bool GetButton(bool isPress = false)
		{
			if (string.IsNullOrEmpty(ButtonName))
			{
				return false;
			}
			if (!isPress)
			{
				return ETCInput.GetButtonDown(ButtonName);
			}
			return ETCInput.GetButton(ButtonName);
		}

		private bool GetInput(bool isPress = false)
		{
			if (string.IsNullOrEmpty(InputName))
			{
				return false;
			}
			if (InputUtils.GetIsGamePaused())
			{
				return false;
			}
			if (!isPress)
			{
				return Input.GetButtonDown(InputName);
			}
			return Input.GetButton(InputName);
		}
	}
}
