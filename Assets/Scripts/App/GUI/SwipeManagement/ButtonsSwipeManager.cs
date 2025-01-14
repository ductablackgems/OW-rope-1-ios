using App.Util;
using UnityEngine;

namespace App.GUI.SwipeManagement
{
	public class ButtonsSwipeManager : MonoBehaviour
	{
		public SwipeButtonGroup[] buttonGroups;

		private SwipeButtonGroup swipedGroup;

		private bool swiped;

		private void Update()
		{
			if (InputUtils.ControlMode == ControlMode.keyboard)
			{
				return;
			}
			if (swiped && !swipedGroup.Pressed())
			{
				swipedGroup.Show();
				swiped = false;
				swipedGroup = null;
				SwipeOut();
			}
			for (int i = 0; i < buttonGroups.Length; i++)
			{
				SwipeButtonGroup swipeButtonGroup = buttonGroups[i];
				if (swipeButtonGroup != swipedGroup && swipeButtonGroup.GameObjectsActive())
				{
					swipeButtonGroup.Show();
				}
			}
			int num = 0;
			SwipeButtonGroup swipeButtonGroup2;
			while (true)
			{
				if (num < buttonGroups.Length)
				{
					swipeButtonGroup2 = buttonGroups[num];
					if (swipeButtonGroup2.PressedDown())
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (swiped)
			{
				swipedGroup.Show();
			}
			swiped = true;
			swipedGroup = swipeButtonGroup2;
			swipeButtonGroup2.Hide();
			SwipeIn();
		}

		private void SwipeIn()
		{
			ETCInput.SetControlSwipeIn("CameraPad", value: true);
		}

		private void SwipeOut()
		{
			ETCInput.SetControlSwipeIn("CameraPad", value: false);
		}
	}
}
