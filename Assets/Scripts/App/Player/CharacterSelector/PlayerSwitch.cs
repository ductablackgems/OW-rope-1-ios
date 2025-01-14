using App.SaveSystem;
using UnityEngine;

namespace App.Player.CharacterSelector
{
	public class PlayerSwitch : MonoBehaviour
	{
		public GameObject[] players;

		private PlayerModel playerModel;

		private PlayerSaveEntity playerSave;

		private int currentIndex;

		public void SaveCurrent()
		{
			playerSave.characterIndex = currentIndex;
			playerSave.colorIndex = playerModel.ColorManager.ColorIndex;
			playerSave.Save();
		}

		public void Revert()
		{
			Activate(playerSave.characterIndex);
			playerModel.ColorManager.SetColorIndex(playerSave.colorIndex);
		}

		public void MoveRight()
		{
			currentIndex++;
			if (currentIndex >= players.Length)
			{
				currentIndex = 0;
			}
			Activate(currentIndex);
			PrepareColor();
		}

		public void MoveLeft()
		{
			currentIndex--;
			if (currentIndex < 0)
			{
				currentIndex = players.Length - 1;
			}
			Activate(currentIndex);
			PrepareColor();
		}

		private void Awake()
		{
			playerModel = ServiceLocator.GetPlayerModel();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			Activate(playerSave.characterIndex);
			playerModel.ColorManager.SetColorIndex(playerSave.colorIndex);
		}

		private void Activate(int index)
		{
			for (int i = 0; i < players.Length; i++)
			{
				if (i != index)
				{
					players[i].SetActive(value: false);
				}
			}
			currentIndex = index;
			players[index].GetComponentSafe<PlayerController>().controlled = false;
			players[index].SetActive(value: true);
			playerModel.SetPlayer(players[index]);
		}

		private void PrepareColor()
		{
			if (currentIndex == playerSave.characterIndex)
			{
				playerModel.ColorManager.SetColorIndex(playerSave.colorIndex);
			}
			else
			{
				playerModel.ColorManager.SetColorIndex(0);
			}
		}
	}
}
