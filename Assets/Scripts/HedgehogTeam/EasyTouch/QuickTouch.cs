using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick Touch")]
	public class QuickTouch : QuickBase
	{
		[Serializable]
		public class OnTouch : UnityEvent<Gesture>
		{
		}

		[Serializable]
		public class OnTouchNotOverMe : UnityEvent<Gesture>
		{
		}

		public enum ActionTriggering
		{
			Start,
			Down,
			Up
		}

		[SerializeField]
		public OnTouch onTouch;

		public OnTouchNotOverMe onTouchNotOverMe;

		public ActionTriggering actionTriggering;

		private Gesture currentGesture;

		public QuickTouch()
		{
			quickActionName = "QuickTouch" + Guid.NewGuid().ToString().Substring(0, 7);
		}

		private void Update()
		{
			currentGesture = EasyTouch.current;
			if (!is2Finger && currentGesture != null)
			{
				if (currentGesture.type == EasyTouch.EvtType.On_TouchStart && fingerIndex == -1 && IsOverMe(currentGesture))
				{
					fingerIndex = currentGesture.fingerIndex;
				}
				if (currentGesture.type == EasyTouch.EvtType.On_TouchStart && actionTriggering == ActionTriggering.Start && (currentGesture.fingerIndex == fingerIndex || isMultiTouch))
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_TouchDown && actionTriggering == ActionTriggering.Down && (currentGesture.fingerIndex == fingerIndex || isMultiTouch))
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type != EasyTouch.EvtType.On_TouchUp)
				{
					return;
				}
				if (actionTriggering == ActionTriggering.Up && (currentGesture.fingerIndex == fingerIndex || isMultiTouch))
				{
					if (IsOverMe(currentGesture))
					{
						onTouch.Invoke(currentGesture);
					}
					else
					{
						onTouchNotOverMe.Invoke(currentGesture);
					}
				}
				if (currentGesture.fingerIndex == fingerIndex)
				{
					fingerIndex = -1;
				}
			}
			else if (currentGesture != null)
			{
				if (currentGesture.type == EasyTouch.EvtType.On_TouchStart2Fingers && actionTriggering == ActionTriggering.Start)
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_TouchDown2Fingers && actionTriggering == ActionTriggering.Down)
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_TouchUp2Fingers && actionTriggering == ActionTriggering.Up)
				{
					DoAction(currentGesture);
				}
			}
		}

		private void DoAction(Gesture gesture)
		{
			if (IsOverMe(gesture))
			{
				onTouch.Invoke(gesture);
			}
		}

		private bool IsOverMe(Gesture gesture)
		{
			bool result = false;
			if (realType == GameObjectType.UI)
			{
				if (gesture.isOverGui && (gesture.pickedUIElement == base.gameObject || gesture.pickedUIElement.transform.IsChildOf(base.transform)))
				{
					result = true;
				}
			}
			else if (((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI) && EasyTouch.GetGameObjectAt(gesture.position, is2Finger) == base.gameObject)
			{
				result = true;
			}
			return result;
		}
	}
}
