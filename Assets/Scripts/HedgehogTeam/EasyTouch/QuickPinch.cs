using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick Pinch")]
	public class QuickPinch : QuickBase
	{
		[Serializable]
		public class OnPinchAction : UnityEvent<Gesture>
		{
		}

		public enum ActionTiggering
		{
			InProgress,
			End
		}

		public enum ActionPinchDirection
		{
			All,
			PinchIn,
			PinchOut
		}

		[SerializeField]
		public OnPinchAction onPinchAction;

		public bool isGestureOnMe;

		public ActionTiggering actionTriggering;

		public ActionPinchDirection pinchDirection;

		private float axisActionValue;

		public bool enableSimpleAction;

		public QuickPinch()
		{
			quickActionName = "QuickPinch" + Guid.NewGuid().ToString().Substring(0, 7);
		}

		public override void OnEnable()
		{
			EasyTouch.On_Pinch += On_Pinch;
			EasyTouch.On_PinchIn += On_PinchIn;
			EasyTouch.On_PinchOut += On_PinchOut;
			EasyTouch.On_PinchEnd += On_PichEnd;
		}

		public override void OnDisable()
		{
			UnsubscribeEvent();
		}

		private void OnDestroy()
		{
			UnsubscribeEvent();
		}

		private void UnsubscribeEvent()
		{
			EasyTouch.On_Pinch -= On_Pinch;
			EasyTouch.On_PinchIn -= On_PinchIn;
			EasyTouch.On_PinchOut -= On_PinchOut;
			EasyTouch.On_PinchEnd -= On_PichEnd;
		}

		private void On_Pinch(Gesture gesture)
		{
			if (actionTriggering == ActionTiggering.InProgress && pinchDirection == ActionPinchDirection.All)
			{
				DoAction(gesture);
			}
		}

		private void On_PinchIn(Gesture gesture)
		{
			if ((actionTriggering == ActionTiggering.InProgress) & (pinchDirection == ActionPinchDirection.PinchIn))
			{
				DoAction(gesture);
			}
		}

		private void On_PinchOut(Gesture gesture)
		{
			if ((actionTriggering == ActionTiggering.InProgress) & (pinchDirection == ActionPinchDirection.PinchOut))
			{
				DoAction(gesture);
			}
		}

		private void On_PichEnd(Gesture gesture)
		{
			if (actionTriggering == ActionTiggering.End)
			{
				DoAction(gesture);
			}
		}

		private void DoAction(Gesture gesture)
		{
			axisActionValue = gesture.deltaPinch * sensibility * Time.deltaTime;
			if (isGestureOnMe)
			{
				if (realType == GameObjectType.UI)
				{
					if (gesture.isOverGui && (gesture.pickedUIElement == base.gameObject || gesture.pickedUIElement.transform.IsChildOf(base.transform)))
					{
						onPinchAction.Invoke(gesture);
						if (enableSimpleAction)
						{
							DoDirectAction(axisActionValue);
						}
					}
				}
				else if (((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI) && gesture.GetCurrentPickedObject(isTwoFinger: true) == base.gameObject)
				{
					onPinchAction.Invoke(gesture);
					if (enableSimpleAction)
					{
						DoDirectAction(axisActionValue);
					}
				}
			}
			else if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI)
			{
				onPinchAction.Invoke(gesture);
				if (enableSimpleAction)
				{
					DoDirectAction(axisActionValue);
				}
			}
		}
	}
}
