using UnityEngine;

namespace App.Shop.Slider
{
	public class SlideGestureController : MonoBehaviour
	{
		public enum Gesture
		{
			SlideLeft,
			Stay,
			SlideRight
		}

		public delegate void GestureEventHandler(Gesture gesture);

		public float minimalSpeed = 5f;

		public GestureEventHandler OnGesture;

		private TouchEventDispatcher touchEventDispatcher;

		private float lastSpeed;

		protected void Awake()
		{
			touchEventDispatcher = ServiceLocator.Get<TouchEventDispatcher>();
		}

		protected void OnEnable()
		{
			touchEventDispatcher.onTouchMove += OnTouchMove;
			touchEventDispatcher.onTouchUp += OnTouchUp;
		}

		protected void OnDisable()
		{
			touchEventDispatcher.onTouchMove -= OnTouchMove;
			touchEventDispatcher.onTouchUp -= OnTouchUp;
			lastSpeed = 0f;
		}

		private void OnTouchMove(Touch touch)
		{
			lastSpeed = touch.deltaPosition.x / Time.deltaTime / Screen.dpi;
		}

		private void OnTouchUp(Touch touch)
		{
			if (OnGesture != null)
			{
				if (lastSpeed >= minimalSpeed)
				{
					OnGesture(Gesture.SlideRight);
				}
				else if (0f - lastSpeed >= minimalSpeed)
				{
					OnGesture(Gesture.SlideLeft);
				}
				else
				{
					OnGesture(Gesture.Stay);
				}
			}
			lastSpeed = 0f;
		}
	}
}
