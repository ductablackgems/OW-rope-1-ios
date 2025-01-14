using System;
using UnityEngine;

namespace App.Shop.Slider
{
	[RequireComponent(typeof(SliderControl))]
	public class SliderController : MonoBehaviour
	{
		public float gluedSpeed = 2f;

		private SliderControl slider;

		private SlideGestureController slideGestureController;

		private TouchDistanceController touchDistanceController;

		private bool glued;

		protected void Awake()
		{
			slider = this.GetComponentSafe<SliderControl>();
			slideGestureController = ServiceLocator.Get<SlideGestureController>();
			touchDistanceController = ServiceLocator.Get<TouchDistanceController>();
		}

		protected void Update()
		{
			if (!glued && touchDistanceController.Touched())
			{
				glued = true;
				slider.StopMove();
			}
			if (glued)
			{
				if (touchDistanceController.Touched())
				{
					slider.MoveIndex(touchDistanceController.GetDistance().x / (float)Screen.width * gluedSpeed);
				}
				else
				{
					glued = false;
				}
			}
		}

		protected void OnEnable()
		{
			SlideGestureController obj = slideGestureController;
			obj.OnGesture = (SlideGestureController.GestureEventHandler)Delegate.Combine(obj.OnGesture, new SlideGestureController.GestureEventHandler(OnGesture));
		}

		protected void OnDisable()
		{
			SlideGestureController obj = slideGestureController;
			obj.OnGesture = (SlideGestureController.GestureEventHandler)Delegate.Remove(obj.OnGesture, new SlideGestureController.GestureEventHandler(OnGesture));
		}

		private void OnGlue()
		{
			slider.StopMove();
		}

		private void OnGesture(SlideGestureController.Gesture gesture)
		{
			switch (gesture)
			{
			case SlideGestureController.Gesture.SlideLeft:
				slider.MoveTo(ItemSliderDirection.Right);
				break;
			case SlideGestureController.Gesture.SlideRight:
				slider.MoveTo(ItemSliderDirection.Left);
				break;
			case SlideGestureController.Gesture.Stay:
				slider.MoveTo(ItemSliderDirection.Auto);
				break;
			}
		}
	}
}
