using App.Quests;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI.Controls
{
	public class QuestInfoControl : MonoBehaviour
	{
		[SerializeField]
		private Text textCountDown;

		[SerializeField]
		private Text textDescription;

		private GameplayObjective objective;

		private float updateTime;

		public void Show(GameplayObjective currentObjective)
		{
			objective = currentObjective;
			objective.TextChanged = OnTextChanged;
			textDescription.text = objective.ShortDecription;
			base.gameObject.SetActive(value: true);
		}

		public void Hide()
		{
			if (objective != null)
			{
				objective.TextChanged = null;
			}
			CleanText();
			objective = null;
			base.gameObject.SetActive(value: false);
		}

		private void OnTextChanged()
		{
			textDescription.text = objective.ShortDecription;
		}

		private void Awake()
		{
			CleanText();
		}

		private void Update()
		{
			if (!(objective == null))
			{
				UpdateRemainingTime();
			}
		}

		private void UpdateRemainingTime()
		{
			float remainingTime = objective.RemainingTime;
			if (remainingTime <= 0f)
			{
				textCountDown.gameObject.SetActive(value: false);
				return;
			}
			textCountDown.gameObject.SetActive(value: true);
			updateTime -= Time.deltaTime;
			if (!(updateTime > 0f))
			{
				updateTime = 0.25f;
				int num = Mathf.FloorToInt(remainingTime / 60f);
				int num2 = Mathf.FloorToInt(remainingTime - (float)(num * 60));
				textCountDown.text = $"{num:0}:{num2:00}";
			}
		}

		private void CleanText()
		{
			textCountDown.text = string.Empty;
			textDescription.text = string.Empty;
		}
	}
}
