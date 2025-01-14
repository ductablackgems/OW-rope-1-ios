using App.Player;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI.Player
{
	public class PlayerStickyRocketLabel : MonoBehaviour
	{
		private StickyRocketController stickyRocketController;

		private int lastCount = int.MinValue;

		private Text countText;

		private void Awake()
		{
			countText = GetComponent<Text>();
			stickyRocketController = ServiceLocator.GetPlayerModel().GameObject.GetComponent<StickyRocketController>();
		}

		private void Update()
		{
			if (stickyRocketController.AvailableRocketCount != lastCount)
			{
				countText.text = stickyRocketController.AvailableRocketCount.ToString();
				lastCount = stickyRocketController.AvailableRocketCount;
			}
		}
	}
}
