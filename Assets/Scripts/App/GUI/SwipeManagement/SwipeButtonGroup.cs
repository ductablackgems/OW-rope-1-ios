using System;

namespace App.GUI.SwipeManagement
{
	[Serializable]
	public class SwipeButtonGroup
	{
		public ETCButton[] buttons;

		public void Show()
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				ETCButton eTCButton = buttons[i];
				if (!(eTCButton == null))
				{
					eTCButton.visible = true;
				}
			}
		}

		public void Hide()
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				ETCButton eTCButton = buttons[i];
				if (!(eTCButton == null))
				{
					eTCButton.visible = false;
				}
			}
		}

		public bool PressedDown()
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				ETCButton eTCButton = buttons[i];
				if (!(eTCButton == null) && eTCButton.gameObject.activeInHierarchy && ETCInput.GetButtonDown(eTCButton.gameObject.name))
				{
					return true;
				}
			}
			return false;
		}

		public bool Pressed()
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				ETCButton eTCButton = buttons[i];
				if (!(eTCButton == null) && eTCButton.gameObject.activeInHierarchy && ETCInput.GetButton(eTCButton.gameObject.name))
				{
					return true;
				}
			}
			return false;
		}

		public bool GameObjectsActive()
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				ETCButton eTCButton = buttons[i];
				if (!(eTCButton == null) && eTCButton.gameObject.activeInHierarchy)
				{
					return true;
				}
			}
			return false;
		}
	}
}
