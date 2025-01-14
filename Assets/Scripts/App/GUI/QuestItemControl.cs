using App.Quests;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class QuestItemControl : MonoBehaviour
	{
		private Button button;

		private Image image;

		private Text text;

		private Color colorBackup;

		private Quest quest;

		public Quest Quest
		{
			get
			{
				return quest;
			}
			set
			{
				SetQuest(value);
			}
		}

		public string Text
		{
			get
			{
				return text.text;
			}
			set
			{
				text.text = value;
			}
		}

		public event Action<QuestItemControl> Selected;


		public void Select()
		{
			button.OnSelect(null);
			button.Select();
			OnButtonClicked();
		}

		public void Mark(bool isMark)
		{
			image.color = (isMark ? Color.green : colorBackup);
		}

		public void Initialize()
		{
			button = GetComponentInChildren<Button>();
			text = GetComponentInChildren<Text>();
			image = button.GetComponent<Image>();
			colorBackup = image.color;
			button.onClick.AddListener(OnButtonClicked);
		}

		private void OnButtonClicked()
		{
			if (this.Selected != null)
			{
				this.Selected(this);
			}
		}

		private void SetQuest(Quest quest)
		{
			this.quest = quest;
			if (!(quest == null))
			{
				text.text = LocalizationManager.Instance.GetText(quest.Settings.NameID);
			}
		}
	}
}
