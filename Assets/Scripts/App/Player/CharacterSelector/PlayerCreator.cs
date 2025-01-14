using App.SaveSystem;
using UnityEngine;

namespace App.Player.CharacterSelector
{
	public class PlayerCreator : MonoBehaviour
	{
		public GameObject[] playerPrefabs;

		private void Awake()
		{
			GameObject gameObject = ServiceLocator.GetGameObject("Player", showError: false);
			if (!(gameObject != null))
			{
				PlayerSaveEntity playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
				Transform transform = ServiceLocator.GetGameObject("PlayerInitPosition").transform;
				PlayerModel playerModel = ServiceLocator.GetPlayerModel();
				gameObject = UnityEngine.Object.Instantiate(playerPrefabs[playerSave.characterIndex], transform.position, transform.rotation);
				playerModel.SetPlayer(gameObject);
				playerModel.ColorManager.SetColorIndex(playerSave.colorIndex);
			}
		}
	}
}
