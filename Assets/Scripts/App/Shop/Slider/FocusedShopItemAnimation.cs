using UnityEngine;

namespace App.Shop.Slider
{
	[RequireComponent(typeof(AbstractShopItem))]
	public class FocusedShopItemAnimation : MonoBehaviour
	{
		public float rotationSpeed = 20f;

		public float unfocusedRotationSpeed = 100f;

		private AbstractShopItem item;

		private Quaternion initialRotation;

		private bool running;

		protected void Awake()
		{
			item = this.GetComponentSafe<AbstractShopItem>();
			initialRotation = base.transform.localRotation;
		}

		protected void Update()
		{
			if (running)
			{
				if (item.Focused)
				{
					base.transform.Rotate(new Vector3(0f, rotationSpeed * Time.deltaTime, 0f));
				}
				else
				{
					running = false;
				}
			}
			else if (item.Focused)
			{
				running = true;
			}
			else if (base.transform.localRotation != initialRotation)
			{
				base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, initialRotation, Time.deltaTime * unfocusedRotationSpeed);
			}
		}
	}
}
