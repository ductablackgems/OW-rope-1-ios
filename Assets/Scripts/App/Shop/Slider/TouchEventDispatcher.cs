using UnityEngine;

namespace App.Shop.Slider
{
	public class TouchEventDispatcher : MonoBehaviour
	{
		public delegate void TouchEventHandler(Touch touch);

		private UILayerTester uiLayerTester;

		private bool isPressed;

		private int touchId;

		public event TouchEventHandler onTouchDown;

		public event TouchEventHandler onTouchMove;

		public event TouchEventHandler onTouchUp;

		protected void Awake()
		{
			uiLayerTester = GetComponent<UILayerTester>();
		}

		protected void Update()
		{
			int touchCount = UnityEngine.Input.touchCount;
			Touch touch;
			if (isPressed)
			{
				for (int i = 0; i < touchCount; i++)
				{
					touch = UnityEngine.Input.GetTouch(i);
					if (touch.phase == TouchPhase.Ended && touch.fingerId == touchId)
					{
						isPressed = false;
						if (this.onTouchUp != null)
						{
							this.onTouchUp(touch);
						}
						break;
					}
				}
			}
			int num = 0;
			while (true)
			{
				if (num >= touchCount)
				{
					return;
				}
				touch = UnityEngine.Input.GetTouch(num);
				if (isPressed && touch.fingerId == touchId)
				{
					if (touch.phase == TouchPhase.Moved)
					{
						if (this.onTouchMove != null)
						{
							this.onTouchMove(touch);
						}
						return;
					}
					if (touch.phase == TouchPhase.Stationary)
					{
						return;
					}
				}
				if (touch.phase == TouchPhase.Began && !isPressed && (uiLayerTester == null || !uiLayerTester.CollideWithUI(touch)))
				{
					break;
				}
				num++;
			}
			isPressed = true;
			touchId = touch.fingerId;
			if (this.onTouchDown != null)
			{
				this.onTouchDown(touch);
			}
		}
	}
}
