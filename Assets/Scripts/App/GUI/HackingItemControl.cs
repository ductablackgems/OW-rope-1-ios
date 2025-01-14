using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
	public class HackingItemControl : MonoBehaviour
	{
		private Button button;

		private Image image;

		private Image imageBG;

		public Sprite Sprite
		{
			get
			{
				return image.sprite;
			}
			set
			{
				SetSprite(value);
			}
		}

		public event Action<HackingItemControl> OnClick;

		public void Initialize(bool isReadOnly)
		{
			button = GetComponentInChildren<Button>(includeInactive: true);
			image = button.GetComponentInChildren<Image>("Image", includeInactive: true);
			imageBG = button.GetComponentInChildren<Image>("BG", includeInactive: true);
			base.transform.localScale = Vector3.one;
			if (isReadOnly)
			{
				button.enabled = false;
				imageBG.gameObject.SetActive(value: false);
			}
			button.onClick.AddListener(OnButtonClicked);
		}

		private void OnButtonClicked()
		{
			if (this.OnClick != null)
			{
				CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
				{
					this.OnClick(this);
				});
			}
		}

		private void SetSprite(Sprite sprite)
		{
			image.sprite = sprite;
			image.gameObject.SetActive(sprite != null);
		}
	}
}
