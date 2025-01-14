using App.GUI.Panels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class QuestNotificationPanel : AbstractPanel
	{
		[SerializeField]
		private Text textHeader;

		[SerializeField]
		private Text textContent;

		[SerializeField]
		private Button buttonContinue;

		[SerializeField]
		private Button buttonExit;

		[Header("Typing")]
		[SerializeField]
		private int symbolsPerSecond = 20;

		private readonly string[] separators = new string[1]
		{
			"\\n"
		};

		private string currentPage;

		private Coroutine coroutine;

		private Action close;

		private List<string> pages = new List<string>(8);

		public string Content
		{
			get
			{
				return textContent.text;
			}
			set
			{
				textContent.text = value;
			}
		}

		public void SetText(string name, string content, Action close)
		{
			textHeader.text = name;
			SetContent(content);
			ShowNextPage();
			this.close = close;
		}

		public override PanelType GetPanelType()
		{
			return PanelType.QuestNotification;
		}

		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}

		private void OnButtonContinueClicked()
		{
			if (!StopTyping() && !ShowNextPage())
			{
				CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_continue, () =>
				{
					OnCloseRequest();
				});
			}
		}

		private void OnButtonExit()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_quest_exit, () =>
			{
				OnCloseRequest();
			});
		}

		private void Initialize()
		{
			buttonContinue.onClick.AddListener(OnButtonContinueClicked);
			buttonExit.onClick.AddListener(OnButtonExit);
			textHeader.text = string.Empty;
			textContent.text = string.Empty;
		}

		private void SetContent(string text)
		{
			textContent.text = string.Empty;
			currentPage = string.Empty;
			pages.Clear();
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace("\n", "\\n");
				pages.AddRange(text.Split(separators, StringSplitOptions.RemoveEmptyEntries));
			}
		}

		private bool ShowNextPage()
		{
			if (pages.Count == 0)
			{
				return false;
			}
			currentPage = pages[0];
			coroutine = StartCoroutine(TypeText_Coroutine(currentPage));
			pages.RemoveAt(0);
			return true;
		}

		private IEnumerator TypeText_Coroutine(string text)
		{
			char[] symbols = text.ToCharArray();
			Content = string.Empty;
			float speed = 1f / (float)symbolsPerSecond;
			for (int idx = 0; idx < symbols.Length; idx++)
			{
				Content += symbols[idx].ToString();
				float waitTime = speed;
				while (waitTime > 0f)
				{
					waitTime -= Time.unscaledDeltaTime;
					yield return null;
				}
			}
			coroutine = null;
		}

		private void OnCloseRequest()
		{
			StopTyping();
			close();
		}

		private bool StopTyping()
		{
			if (coroutine == null)
			{
				return false;
			}
			StopCoroutine(coroutine);
			coroutine = null;
			Content = currentPage;
			return true;
		}
	}
}
