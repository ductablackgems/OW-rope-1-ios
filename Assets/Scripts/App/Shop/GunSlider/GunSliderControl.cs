using App.Shop.Slider;
using UnityEngine;

namespace App.Shop.GunSlider
{
	public class GunSliderControl : MonoBehaviour
	{
		public Vector3 cameraOffset;

		private ShopSounds shopSounds;

		private AbstractShopItem[] items;

		private AbstractShopItemView itemView;

		private AbstractShopItem focusedItem;

		public void Run()
		{
			if (!focusedItem.Focused)
			{
				focusedItem = items[0];
				focusedItem.Focused = true;
			}
			itemView.Focus(focusedItem);
			itemView.UpdateView();
		}

		public void Stop()
		{
			focusedItem.Focused = false;
		}

		public void MoveRight()
		{
			shopSounds.Play(shopSounds.slideSound);
			AbstractShopItem abstractShopItem = null;
			AbstractShopItem[] array = items;
			foreach (AbstractShopItem abstractShopItem2 in array)
			{
				if (abstractShopItem != null)
				{
					focusedItem = abstractShopItem2;
					focusedItem.Focused = true;
					itemView.Focus(focusedItem);
					itemView.UpdateView();
					return;
				}
				if (abstractShopItem2.Focused)
				{
					abstractShopItem = abstractShopItem2;
					abstractShopItem.Focused = false;
				}
			}
			focusedItem = items[0];
			focusedItem.Focused = true;
			itemView.Focus(focusedItem);
			itemView.UpdateView();
		}

		public void MoveLeft()
		{
			shopSounds.Play(shopSounds.slideSound);
			AbstractShopItem x = null;
			AbstractShopItem[] array = items;
			foreach (AbstractShopItem abstractShopItem in array)
			{
				if (abstractShopItem.Focused)
				{
					abstractShopItem.Focused = false;
					if (x != null)
					{
						focusedItem = x;
						focusedItem.Focused = true;
					}
					else
					{
						focusedItem = items[items.Length - 1];
						focusedItem.Focused = true;
					}
					itemView.Focus(focusedItem);
					itemView.UpdateView();
					return;
				}
				x = abstractShopItem;
			}
			focusedItem = x;
			focusedItem.Focused = true;
			itemView.Focus(focusedItem);
			itemView.UpdateView();
		}

		public Vector3 GetTargetCameraPosition()
		{
			return focusedItem.transform.parent.position - base.transform.TransformDirection(cameraOffset);
		}

		public Quaternion GetTargetCameraRotation()
		{
			return Quaternion.LookRotation(base.transform.TransformDirection(cameraOffset));
		}

		protected virtual AbstractShopItemView GetView()
		{
			return ServiceLocator.Get<GunShopView>();
		}

		private void Awake()
		{
			shopSounds = ServiceLocator.Get<ShopSounds>();
			itemView = GetView();
			items = GetComponentsInChildren<AbstractShopItem>();
			focusedItem = items[0];
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawLine(base.transform.position, base.transform.position - base.transform.TransformDirection(cameraOffset));
		}
	}
}
