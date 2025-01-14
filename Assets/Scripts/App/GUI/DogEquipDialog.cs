using App.Dogs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class DogEquipDialog : MonoBehaviour
	{
		[SerializeField]
		private int emptyTextID;

		[SerializeField]
		private Button buttonBack;

		private List<DogEquipButton> buttons = new List<DogEquipButton>(3);

		private Action<DogManager.DogSlot> result;

		private string emptyText;

		public void Initialize(Action<DogManager.DogSlot> resultCallback)
		{
			if (result == null)
			{
				result = resultCallback;
				emptyText = LocalizationManager.Instance.GetText(emptyTextID);
				GetComponentsInChildren(includeInactive: true, buttons);
				for (int i = 0; i < buttons.Count; i++)
				{
					DogEquipButton dogEquipButton = buttons[i];
					dogEquipButton.Slot = IndexToDogSlot(i);
					dogEquipButton.Click = OnEquipButtonClick;
				}
				buttonBack.onClick.AddListener(OnButtonBackClick);
				Close();
			}
		}

		public void Show(Func<DogManager.DogSlot, string> getSlotName)
		{
			base.gameObject.SetActive(value: true);
			foreach (DogEquipButton button in buttons)
			{
				string text = getSlotName(button.Slot);
				button.Description = (string.IsNullOrEmpty(text) ? emptyText : text);
			}
		}

		public void Close()
		{
			base.gameObject.SetActive(value: false);
		}

		private void OnEquipButtonClick(DogEquipButton button)
		{
			int index = buttons.IndexOf(button);
			DogManager.DogSlot obj = IndexToDogSlot(index);
			result(obj);
		}

		private void OnButtonBackClick()
		{
			result(DogManager.DogSlot.None);
		}

		private DogManager.DogSlot IndexToDogSlot(int index)
		{
			return (DogManager.DogSlot)(index + 1);
		}
	}
}
