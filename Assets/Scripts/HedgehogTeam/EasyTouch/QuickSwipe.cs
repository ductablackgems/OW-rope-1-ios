using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick Swipe")]
	public class QuickSwipe : QuickBase
	{
		[Serializable]
		public class OnSwipeAction : UnityEvent<Gesture>
		{
		}

		public enum ActionTriggering
		{
			InProgress,
			End
		}

		public enum SwipeDirection
		{
			Vertical,
			Horizontal,
			DiagonalRight,
			DiagonalLeft,
			Up,
			UpRight,
			Right,
			DownRight,
			Down,
			DownLeft,
			Left,
			UpLeft,
			All
		}

		[SerializeField]
		public OnSwipeAction onSwipeAction;

		public bool allowSwipeStartOverMe = true;

		public ActionTriggering actionTriggering;

		public SwipeDirection swipeDirection = SwipeDirection.All;

		private float axisActionValue;

		public bool enableSimpleAction;

		public QuickSwipe()
		{
			quickActionName = "QuickSwipe" + Guid.NewGuid().ToString().Substring(0, 7);
		}

		public override void OnEnable()
		{
			base.OnEnable();
			EasyTouch.On_Drag += On_Drag;
			EasyTouch.On_Swipe += On_Swipe;
			EasyTouch.On_DragEnd += On_DragEnd;
			EasyTouch.On_SwipeEnd += On_SwipeEnd;
		}

		public override void OnDisable()
		{
			base.OnDisable();
			UnsubscribeEvent();
		}

		private void OnDestroy()
		{
			UnsubscribeEvent();
		}

		private void UnsubscribeEvent()
		{
			EasyTouch.On_Drag -= On_Drag;
			EasyTouch.On_Swipe -= On_Swipe;
			EasyTouch.On_DragEnd -= On_DragEnd;
			EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		}

		private void On_Swipe(Gesture gesture)
		{
			if (gesture.touchCount != 1 || ((!(gesture.pickedObject != base.gameObject) || allowSwipeStartOverMe) && !allowSwipeStartOverMe))
			{
				return;
			}
			fingerIndex = gesture.fingerIndex;
			if (actionTriggering == ActionTriggering.InProgress && isRightDirection(gesture))
			{
				onSwipeAction.Invoke(gesture);
				if (enableSimpleAction)
				{
					DoAction(gesture);
				}
			}
		}

		private void On_SwipeEnd(Gesture gesture)
		{
			if (actionTriggering == ActionTriggering.End && isRightDirection(gesture))
			{
				onSwipeAction.Invoke(gesture);
				if (enableSimpleAction)
				{
					DoAction(gesture);
				}
			}
			if (fingerIndex == gesture.fingerIndex)
			{
				fingerIndex = -1;
			}
		}

		private void On_DragEnd(Gesture gesture)
		{
			if (gesture.pickedObject == base.gameObject && allowSwipeStartOverMe)
			{
				On_SwipeEnd(gesture);
			}
		}

		private void On_Drag(Gesture gesture)
		{
			if (gesture.pickedObject == base.gameObject && allowSwipeStartOverMe)
			{
				On_Swipe(gesture);
			}
		}

		private bool isRightDirection(Gesture gesture)
		{
			float num = -1f;
			if (inverseAxisValue)
			{
				num = 1f;
			}
			axisActionValue = 0f;
			switch (swipeDirection)
			{
			case SwipeDirection.All:
				axisActionValue = gesture.deltaPosition.magnitude * num;
				return true;
			case SwipeDirection.Horizontal:
				if (gesture.swipe == EasyTouch.SwipeDirection.Left || gesture.swipe == EasyTouch.SwipeDirection.Right)
				{
					axisActionValue = gesture.deltaPosition.x * num;
					return true;
				}
				break;
			case SwipeDirection.Vertical:
				if (gesture.swipe == EasyTouch.SwipeDirection.Up || gesture.swipe == EasyTouch.SwipeDirection.Down)
				{
					axisActionValue = gesture.deltaPosition.y * num;
					return true;
				}
				break;
			case SwipeDirection.DiagonalLeft:
				if (gesture.swipe == EasyTouch.SwipeDirection.UpLeft || gesture.swipe == EasyTouch.SwipeDirection.DownRight)
				{
					axisActionValue = gesture.deltaPosition.magnitude * num;
					return true;
				}
				break;
			case SwipeDirection.DiagonalRight:
				if (gesture.swipe == EasyTouch.SwipeDirection.UpRight || gesture.swipe == EasyTouch.SwipeDirection.DownLeft)
				{
					axisActionValue = gesture.deltaPosition.magnitude * num;
					return true;
				}
				break;
			case SwipeDirection.Left:
				if (gesture.swipe == EasyTouch.SwipeDirection.Left)
				{
					axisActionValue = gesture.deltaPosition.x * num;
					return true;
				}
				break;
			case SwipeDirection.Right:
				if (gesture.swipe == EasyTouch.SwipeDirection.Right)
				{
					axisActionValue = gesture.deltaPosition.x * num;
					return true;
				}
				break;
			case SwipeDirection.DownLeft:
				if (gesture.swipe == EasyTouch.SwipeDirection.DownLeft)
				{
					axisActionValue = gesture.deltaPosition.magnitude * num;
					return true;
				}
				break;
			case SwipeDirection.DownRight:
				if (gesture.swipe == EasyTouch.SwipeDirection.DownRight)
				{
					axisActionValue = gesture.deltaPosition.magnitude * num;
					return true;
				}
				break;
			case SwipeDirection.UpLeft:
				if (gesture.swipe == EasyTouch.SwipeDirection.UpLeft)
				{
					axisActionValue = gesture.deltaPosition.magnitude * num;
					return true;
				}
				break;
			case SwipeDirection.UpRight:
				if (gesture.swipe == EasyTouch.SwipeDirection.UpRight)
				{
					axisActionValue = gesture.deltaPosition.magnitude * num;
					return true;
				}
				break;
			case SwipeDirection.Up:
				if (gesture.swipe == EasyTouch.SwipeDirection.Up)
				{
					axisActionValue = gesture.deltaPosition.y * num;
					return true;
				}
				break;
			case SwipeDirection.Down:
				if (gesture.swipe == EasyTouch.SwipeDirection.Down)
				{
					axisActionValue = gesture.deltaPosition.y * num;
					return true;
				}
				break;
			}
			axisActionValue = 0f;
			return false;
		}

		private void DoAction(Gesture gesture)
		{
			switch (directAction)
			{
			case DirectAction.Rotate:
			case DirectAction.RotateLocal:
				axisActionValue *= sensibility;
				break;
			case DirectAction.Translate:
			case DirectAction.TranslateLocal:
			case DirectAction.Scale:
				axisActionValue /= Screen.dpi;
				axisActionValue *= sensibility;
				break;
			}
			DoDirectAction(axisActionValue);
		}
	}
}
