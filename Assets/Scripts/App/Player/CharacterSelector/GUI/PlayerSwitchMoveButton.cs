using UnityEngine;

namespace App.Player.CharacterSelector.GUI
{
	public class PlayerSwitchMoveButton : MonoBehaviour
	{
		public bool right;

		private PlayerSwitch playerSwitch;

		private void Awake()
		{
			playerSwitch = ServiceLocator.Get<PlayerSwitch>();
		}

		private void OnClick()
		{
			if (right)
			{
				playerSwitch.MoveRight();
			}
			else
			{
				playerSwitch.MoveLeft();
			}
		}
	}
}
