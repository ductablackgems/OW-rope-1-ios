using UnityEngine;
using UnityEngine.UI;

namespace NiobiumStudios
{
	public class DailyRewardUI : MonoBehaviour
	{
		public enum DailyRewardState
		{
			UNCLAIMED_AVAILABLE,
			UNCLAIMED_UNAVAILABLE,
			CLAIMED
		}

		public bool showRewardName;

		[Header("UI Elements")]
		public Text textDay;

		public Text textReward;

		public Image imageRewardBackground;

		public Image imageReward;

		public Color colorClaim;

		private Color colorUnclaimed;

		[Header("Internal")]
		public int day;

		[HideInInspector]
		public Reward reward;

		public DailyRewardState state;

		private void Awake()
		{
			colorUnclaimed = imageReward.color;
		}

		public void Initialize()
		{
			textDay.text = $"Day {day.ToString()}";
			if (reward.reward > 0)
			{
				if (showRewardName)
				{
					textReward.text = reward.reward + " " + reward.unit;
				}
				else
				{
					textReward.text = reward.reward.ToString();
				}
			}
			else
			{
				textReward.text = reward.unit.ToString();
			}
			imageReward.sprite = reward.sprite;
		}

		public void Refresh()
		{
			switch (state)
			{
			case DailyRewardState.UNCLAIMED_AVAILABLE:
				imageRewardBackground.color = colorClaim;
				break;
			case DailyRewardState.UNCLAIMED_UNAVAILABLE:
				imageRewardBackground.color = colorUnclaimed;
				break;
			case DailyRewardState.CLAIMED:
				imageRewardBackground.color = colorClaim;
				break;
			}
		}
	}
}
