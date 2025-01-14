using UnityEngine;

namespace App.Shop.Slider
{
	[RequireComponent(typeof(MoveToAnimation))]
	public class SliderControl : MonoBehaviour
	{
		public float speed = 2f;

		public Vector3 itemDistance = new Vector3(20f, 0f, 0f);

		public float overEdgeMotionMultiplier = 0.25f;

		private MoveToAnimation moveToAnimation;

		private AbstractShopItem[] items;

		private AbstractShopItemView itemView;

		private int currentIndex;

		private AbstractShopItem focusedItem;

		private Vector3 initPosition;

		private float lastFloatIndex;

		private int updateCount;

		public void MoveTo(ItemSliderDirection direction)
		{
			currentIndex = GetTargetIndex(direction);
			moveToAnimation.MoveTo(GetTargetPosition(currentIndex), speed * itemDistance.magnitude);
		}

		public void MoveLeft()
		{
			MoveIndex(0.01f);
			MoveTo(ItemSliderDirection.Left);
		}

		public void MoveRight()
		{
			MoveIndex(-0.01f);
			MoveTo(ItemSliderDirection.Right);
		}

		public AbstractShopItem GetFocusedItem()
		{
			return focusedItem;
		}

		public void FixTo(int targetIndex)
		{
			base.transform.position = GetTargetPosition(targetIndex);
			currentIndex = targetIndex;
			FocusItem(items[targetIndex]);
		}

		public void FixToCurrent()
		{
			base.transform.position = GetTargetPosition(currentIndex);
			FocusItem(items[currentIndex]);
		}

		public void MoveIndex(float indexMotion)
		{
			lastFloatIndex = (float)currentIndex - indexMotion;
			base.transform.position = GetTargetPosition(lastFloatIndex);
		}

		public void StopMove()
		{
			moveToAnimation.Stop();
		}

		public int GetTargetIndex(ItemSliderDirection direction)
		{
			int num;
			switch (direction)
			{
			case ItemSliderDirection.Right:
				num = Mathf.CeilToInt(lastFloatIndex);
				break;
			case ItemSliderDirection.Left:
				num = Mathf.FloorToInt(lastFloatIndex);
				break;
			default:
				num = Mathf.RoundToInt(lastFloatIndex);
				break;
			}
			if (num < 0)
			{
				return 0;
			}
			if (num >= items.Length)
			{
				return items.Length - 1;
			}
			return num;
		}

		protected void Awake()
		{
			moveToAnimation = this.GetComponentSafe<MoveToAnimation>();
			items = GetComponentsInChildren<AbstractShopItem>();
			itemView = ServiceLocator.Get<AbstractShopItemView>();
			initPosition = base.transform.position;
		}

		protected void Start()
		{
			FixToCurrent();
		}

		protected void Update()
		{
			float num = itemDistance.magnitude / 2f;
			for (int i = 0; i < items.Length; i++)
			{
				if ((initPosition - i * itemDistance - base.transform.position).magnitude < num)
				{
					FocusItem(items[i]);
					break;
				}
			}
			if (updateCount >= 2)
			{
				return;
			}
			if (updateCount == 1)
			{
				for (int j = 0; j < items.Length; j++)
				{
					if (items[j].IsSelected())
					{
						FixTo(j);
						break;
					}
				}
				itemView.UpdateView();
			}
			updateCount++;
		}

		private void FocusItem(AbstractShopItem newItem)
		{
			AbstractShopItem abstractShopItem = focusedItem;
			focusedItem = newItem;
			newItem.Focused = true;
			if (!focusedItem.Equals(abstractShopItem))
			{
				if (abstractShopItem != null)
				{
					abstractShopItem.Focused = false;
				}
				itemView.Focus(focusedItem);
			}
		}

		private void OnUpdateValues(AbstractShopItem item)
		{
			itemView.UpdateView();
		}

		private Vector3 GetTargetPosition(float targetPanelIndex)
		{
			if (targetPanelIndex < 0f)
			{
				targetPanelIndex *= overEdgeMotionMultiplier;
			}
			else if (targetPanelIndex > (float)(items.Length - 1))
			{
				int num = items.Length - 1;
				targetPanelIndex = (targetPanelIndex - (float)num) * overEdgeMotionMultiplier + (float)num;
			}
			return initPosition - targetPanelIndex * itemDistance;
		}
	}
}
