using UnityEngine;

namespace App.Player.ColorManager.GUI
{
	public class PlayerColorButton : MonoBehaviour
	{
		public int colorIndex;

		private PlayerModel player;

		private void Awake()
		{
			player = ServiceLocator.GetPlayerModel();
		}

		private void OnClick()
		{
			player.ColorManager.SetColorIndex(colorIndex);
		}
	}
}
