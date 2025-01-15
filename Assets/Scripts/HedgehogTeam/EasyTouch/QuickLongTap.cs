using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick LongTap")]
	public class QuickLongTap : QuickBase
	{
		[Serializable]
		public class OnLongTap : UnityEvent<Gesture>
		{
		}

		public enum ActionTriggering
		{
			Start,
			InProgress,
			End
		}

		[SerializeField]
		public OnLongTap onLongTap;

		public ActionTriggering actionTriggering;

		private Gesture currentGesture;

		public QuickLongTap()
		{
			quickActionName = "QuickLongTap" + Guid.NewGuid().ToString().Substring(0, 7);
		}

		private void Update()
		{
			currentGesture = EasyTouch.current;
			if (currentGesture == null)
			{
				return;
			}
			if (!is2Finger)
			{
				if (currentGesture.type == EasyTouch.EvtType.On_TouchStart && fingerIndex == -1 && IsOverMe(currentGesture))
				{
					fingerIndex = currentGesture.fingerIndex;
				}
				if (currentGesture.type == EasyTouch.EvtType.On_LongTapStart && actionTriggering == ActionTriggering.Start && (currentGesture.fingerIndex == fingerIndex || isMultiTouch))
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_LongTap && actionTriggering == ActionTriggering.InProgress && (currentGesture.fingerIndex == fingerIndex || isMultiTouch))
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_LongTapEnd && actionTriggering == ActionTriggering.End && (currentGesture.fingerIndex == fingerIndex || isMultiTouch))
				{
					DoAction(currentGesture);
					fingerIndex = -1;
				}
			}
			else
			{
				if (currentGesture.type == EasyTouch.EvtType.On_LongTapStart2Fingers && actionTriggering == ActionTriggering.Start)
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_LongTap2Fingers && actionTriggering == ActionTriggering.InProgress)
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_LongTapEnd2Fingers && actionTriggering == ActionTriggering.End)
				{
					DoAction(currentGesture);
				}
			}
		}

		private void DoAction(Gesture gesture)
		{
			if (IsOverMe(gesture))
			{
				onLongTap.Invoke(gesture);
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
