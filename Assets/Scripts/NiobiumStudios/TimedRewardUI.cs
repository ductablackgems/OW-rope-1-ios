using UnityEngine;
using UnityEngine.UI;

namespace NiobiumStudios
{
	public class TimedRewardUI : MonoBehaviour
	{
		[Header("UI Elements")]
		public Text textReward;

		public Text textUnit;

		public Image imageReward;

		public Button button;

		[Header("Internal")]
		public int index;

		[HideInInspector]
		public Reward reward;

		public void Initialize()
		{
			textUnit.text = reward.unit.ToString();
			if (reward.reward > 0)
			{
				textReward.text = reward.reward.ToString();
			}
			imageReward.sprite = reward.sprite;
		}
	}
}
