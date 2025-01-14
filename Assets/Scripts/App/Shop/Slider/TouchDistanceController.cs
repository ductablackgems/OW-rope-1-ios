using UnityEngine;

namespace App.Shop.Slider
{
	public class TouchDistanceController : MonoBehaviour
	{
		private TouchEventDispatcher touchEventDispatcher;

		private Vector3 touchDownPosition;

		private Vector3 currentTouchPosition;

		private bool touched;

		public bool Touched()
		{
			return touched;
		}

		public Vector3 GetDistance()
		{
			if (touched)
			{
				return currentTouchPosition - touchDownPosition;
			}
			return Vector3.zero;
		}

		protected void Awake()
		{
			touchEventDispatcher = ServiceLocator.Get<TouchEventDispatcher>();
		}

		protected void OnEnable()
		{
			touchEventDispatcher.onTouchDown += OnTouchDown;
			touchEventDispatcher.onTouchMove += OnTouchMove;
			touchEventDispatcher.onTouchUp += OnTouchUp;
		}

		protected void OnDisable()
		{
			touchEventDispatcher.onTouchDown -= OnTouchDown;
			touchEventDispatcher.onTouchMove -= OnTouchMove;
			touchEventDispatcher.onTouchUp -= OnTouchUp;
		}

		private void OnTouchDown(Touch touch)
		{
			touched = true;
			touchDownPosition = touch.position;
			currentTouchPosition = touch.position;
		}

		private void OnTouchMove(Touch touch)
		{
			currentTouchPosition = touch.position;
		}

		private void OnTouchUp(Touch touch)
		{
			touched = false;
		}
	}
}
