using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick Twist")]
	public class QuickTwist : QuickBase
	{
		[Serializable]
		public class OnTwistAction : UnityEvent<Gesture>
		{
		}

		public enum ActionTiggering
		{
			InProgress,
			End
		}

		public enum ActionRotationDirection
		{
			All,
			Clockwise,
			Counterclockwise
		}

		[SerializeField]
		public OnTwistAction onTwistAction;

		public bool isGestureOnMe;

		public ActionTiggering actionTriggering;

		public ActionRotationDirection rotationDirection;

		private float axisActionValue;

		public bool enableSimpleAction;

		public QuickTwist()
		{
			quickActionName = "QuickTwist" + Guid.NewGuid().ToString().Substring(0, 7);
		}

		public override void OnEnable()
		{
			EasyTouch.On_Twist += On_Twist;
			EasyTouch.On_TwistEnd += On_TwistEnd;
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
			EasyTouch.On_Twist -= On_Twist;
			EasyTouch.On_TwistEnd -= On_TwistEnd;
		}

		private void On_Twist(Gesture gesture)
		{
			if (actionTriggering == ActionTiggering.InProgress && IsRightRotation(gesture))
			{
				DoAction(gesture);
			}
		}

		private void On_TwistEnd(Gesture gesture)
		{
			if (actionTriggering == ActionTiggering.End && IsRightRotation(gesture))
			{
				DoAction(gesture);
			}
		}

		private bool IsRightRotation(Gesture gesture)
		{
			axisActionValue = 0f;
			float num = 1f;
			if (inverseAxisValue)
			{
				num = -1f;
			}
			switch (rotationDirection)
			{
			case ActionRotationDirection.All:
				axisActionValue = gesture.twistAngle * sensibility * num;
				return true;
			case ActionRotationDirection.Clockwise:
				if (gesture.twistAngle < 0f)
				{
					axisActionValue = gesture.twistAngle * sensibility * num;
					return true;
				}
				break;
			case ActionRotationDirection.Counterclockwise:
				if (gesture.twistAngle > 0f)
				{
					axisActionValue = gesture.twistAngle * sensibility * num;
					return true;
				}
				break;
			}
			return false;
		}

		private void DoAction(Gesture gesture)
		{
			if (isGestureOnMe)
			{
				if (realType == GameObjectType.UI)
				{
					if (gesture.isOverGui && (gesture.pickedUIElement == base.gameObject || gesture.pickedUIElement.transform.IsChildOf(base.transform)))
					{
						onTwistAction.Invoke(gesture);
						if (enableSimpleAction)
						{
							DoDirectAction(axisActionValue);
						}
					}
				}
				else if (((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI) && gesture.GetCurrentPickedObject(isTwoFinger: true) == base.gameObject)
				{
					onTwistAction.Invoke(gesture);
					if (enableSimpleAction)
					{
						DoDirectAction(axisActionValue);
					}
				}
			}
			else if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI)
			{
				onTwistAction.Invoke(gesture);
				if (enableSimpleAction)
				{
					DoDirectAction(axisActionValue);
				}
			}
		}
	}
}
