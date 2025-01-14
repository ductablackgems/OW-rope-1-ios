using App.Shop.Slider;
using UnityEngine;

namespace App.Shop.GunSlider
{
	public class FocusedGunShopItem : MonoBehaviour
	{
		public Vector3 focusOffset;

		public float speed = 1f;

		private AbstractShopItem item;

		private Vector3 initialPosition;

		private void Awake()
		{
			item = this.GetComponentSafe<AbstractShopItem>();
			initialPosition = base.transform.position;
		}

		private void Update()
		{
			if (item.Focused)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, initialPosition - base.transform.parent.TransformDirection(focusOffset), speed * Time.deltaTime);
			}
			else
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, initialPosition, speed * Time.deltaTime);
			}
		}
	}
}
