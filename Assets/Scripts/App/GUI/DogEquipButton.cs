using App.Dogs;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace App.GUI
{
	public class DogEquipButton : Button
	{
		private Text textDescription;

		public string Description
		{
			get
			{
				return textDescription.text;
			}
			set
			{
				textDescription.text = value;
			}
		}

		public DogManager.DogSlot Slot
		{
			get;
			set;
		}

		public Action<DogEquipButton> Click
		{
			get;
			set;
		}

		protected override void Awake()
		{
			textDescription = this.GetComponentInChildren<Text>("TextDescription");
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			Click.SafeInvoke(this);
		}
	}
}
