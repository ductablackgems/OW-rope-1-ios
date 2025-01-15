using System;
using System.Collections.Generic;
using UnityEngine;

namespace HedgehogTeam.EasyTouch
{
	[Serializable]
	[AddComponentMenu("EasyTouch/Trigger")]
	public class EasyTouchTrigger : MonoBehaviour
	{
		public enum ETTParameter
		{
			None,
			Gesture,
			Finger_Id,
			Touch_Count,
			Start_Position,
			Position,
			Delta_Position,
			Swipe_Type,
			Swipe_Length,
			Swipe_Vector,
			Delta_Pinch,
			Twist_Anlge,
			ActionTime,
			DeltaTime,
			PickedObject,
			PickedUIElement
		}

		public enum ETTType
		{
			Object3D,
			UI
		}

		[Serializable]
		public class EasyTouchReceiver
		{
			public bool enable;

			public ETTType triggerType;

			public string name;

			public bool restricted;

			public GameObject gameObject;

			public bool otherReceiver;

			public GameObject gameObjectReceiver;

			public EasyTouch.EvtType eventName;

			public string methodName;

			public ETTParameter parameter;
		}

		[SerializeField]
		public List<EasyTouchReceiver> receivers = new List<EasyTouchReceiver>();

		private void Start()
		{
			EasyTouch.SetEnableAutoSelect(value: true);
		}

		private void OnEnable()
		{
			SubscribeEasyTouchEvent();
		}

		private void OnDisable()
		{
			UnsubscribeEasyTouchEvent();
		}

		private void OnDestroy()
		{
			UnsubscribeEasyTouchEvent();
		}

		private void SubscribeEasyTouchEvent()
		{
			if (IsRecevier4(EasyTouch.EvtType.On_Cancel))
			{
				EasyTouch.On_Cancel += On_Cancel;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_TouchStart))
			{
				EasyTouch.On_TouchStart += On_TouchStart;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_TouchDown))
			{
				EasyTouch.On_TouchDown += On_TouchDown;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_TouchUp))
			{
				EasyTouch.On_TouchUp += On_TouchUp;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_SimpleTap))
			{
				EasyTouch.On_SimpleTap += On_SimpleTap;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_LongTapStart))
			{
				EasyTouch.On_LongTapStart += On_LongTapStart;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_LongTap))
			{
				EasyTouch.On_LongTap += On_LongTap;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_LongTapEnd))
			{
				EasyTouch.On_LongTapEnd += On_LongTapEnd;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_DoubleTap))
			{
				EasyTouch.On_DoubleTap += On_DoubleTap;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_DragStart))
			{
				EasyTouch.On_DragStart += On_DragStart;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_Drag))
			{
				EasyTouch.On_Drag += On_Drag;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_DragEnd))
			{
				EasyTouch.On_DragEnd += On_DragEnd;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_SwipeStart))
			{
				EasyTouch.On_SwipeStart += On_SwipeStart;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_Swipe))
			{
				EasyTouch.On_Swipe += On_Swipe;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_SwipeEnd))
			{
				EasyTouch.On_SwipeEnd += On_SwipeEnd;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_TouchStart2Fingers))
			{
				EasyTouch.On_TouchStart2Fingers += On_TouchStart2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_TouchDown2Fingers))
			{
				EasyTouch.On_TouchDown2Fingers += On_TouchDown2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_TouchUp2Fingers))
			{
				EasyTouch.On_TouchUp2Fingers += On_TouchUp2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_SimpleTap2Fingers))
			{
				EasyTouch.On_SimpleTap2Fingers += On_SimpleTap2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_LongTapStart2Fingers))
			{
				EasyTouch.On_LongTapStart2Fingers += On_LongTapStart2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_LongTap2Fingers))
			{
				EasyTouch.On_LongTap2Fingers += On_LongTap2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_LongTapEnd2Fingers))
			{
				EasyTouch.On_LongTapEnd2Fingers += On_LongTapEnd2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_DoubleTap2Fingers))
			{
				EasyTouch.On_DoubleTap2Fingers += On_DoubleTap2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_SwipeStart2Fingers))
			{
				EasyTouch.On_SwipeStart2Fingers += On_SwipeStart2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_Swipe2Fingers))
			{
				EasyTouch.On_Swipe2Fingers += On_Swipe2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_SwipeEnd2Fingers))
			{
				EasyTouch.On_SwipeEnd2Fingers += On_SwipeEnd2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_DragStart2Fingers))
			{
				EasyTouch.On_DragStart2Fingers += On_DragStart2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_Drag2Fingers))
			{
				EasyTouch.On_Drag2Fingers += On_Drag2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_DragEnd2Fingers))
			{
				EasyTouch.On_DragEnd2Fingers += On_DragEnd2Fingers;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_Pinch))
			{
				EasyTouch.On_Pinch += On_Pinch;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_PinchIn))
			{
				EasyTouch.On_PinchIn += On_PinchIn;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_PinchOut))
			{
				EasyTouch.On_PinchOut += On_PinchOut;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_PinchEnd))
			{
				EasyTouch.On_PinchEnd += On_PinchEnd;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_Twist))
			{
				EasyTouch.On_Twist += On_Twist;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_TwistEnd))
			{
				EasyTouch.On_TwistEnd += On_TwistEnd;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_OverUIElement))
			{
				EasyTouch.On_OverUIElement += On_OverUIElement;
			}
			if (IsRecevier4(EasyTouch.EvtType.On_UIElementTouchUp))
			{
				EasyTouch.On_UIElementTouchUp += On_UIElementTouchUp;
			}
		}

		private void UnsubscribeEasyTouchEvent()
		{
			EasyTouch.On_Cancel -= On_Cancel;
			EasyTouch.On_TouchStart -= On_TouchStart;
			EasyTouch.On_TouchDown -= On_TouchDown;
			EasyTouch.On_TouchUp -= On_TouchUp;
			EasyTouch.On_SimpleTap -= On_SimpleTap;
			EasyTouch.On_LongTapStart -= On_LongTapStart;
			EasyTouch.On_LongTap -= On_LongTap;
			EasyTouch.On_LongTapEnd -= On_LongTapEnd;
			EasyTouch.On_DoubleTap -= On_DoubleTap;
			EasyTouch.On_DragStart -= On_DragStart;
			EasyTouch.On_Drag -= On_Drag;
			EasyTouch.On_DragEnd -= On_DragEnd;
			EasyTouch.On_SwipeStart -= On_SwipeStart;
			EasyTouch.On_Swipe -= On_Swipe;
			EasyTouch.On_SwipeEnd -= On_SwipeEnd;
			EasyTouch.On_TouchStart2Fingers -= On_TouchStart2Fingers;
			EasyTouch.On_TouchDown2Fingers -= On_TouchDown2Fingers;
			EasyTouch.On_TouchUp2Fingers -= On_TouchUp2Fingers;
			EasyTouch.On_SimpleTap2Fingers -= On_SimpleTap2Fingers;
			EasyTouch.On_LongTapStart2Fingers -= On_LongTapStart2Fingers;
			EasyTouch.On_LongTap2Fingers -= On_LongTap2Fingers;
			EasyTouch.On_LongTapEnd2Fingers -= On_LongTapEnd2Fingers;
			EasyTouch.On_DoubleTap2Fingers -= On_DoubleTap2Fingers;
			EasyTouch.On_SwipeStart2Fingers -= On_SwipeStart2Fingers;
			EasyTouch.On_Swipe2Fingers -= On_Swipe2Fingers;
			EasyTouch.On_SwipeEnd2Fingers -= On_SwipeEnd2Fingers;
			EasyTouch.On_DragStart2Fingers -= On_DragStart2Fingers;
			EasyTouch.On_Drag2Fingers -= On_Drag2Fingers;
			EasyTouch.On_DragEnd2Fingers -= On_DragEnd2Fingers;
			EasyTouch.On_Pinch -= On_Pinch;
			EasyTouch.On_PinchIn -= On_PinchIn;
			EasyTouch.On_PinchOut -= On_PinchOut;
			EasyTouch.On_PinchEnd -= On_PinchEnd;
			EasyTouch.On_Twist -= On_Twist;
			EasyTouch.On_TwistEnd -= On_TwistEnd;
			EasyTouch.On_OverUIElement += On_OverUIElement;
			EasyTouch.On_UIElementTouchUp += On_UIElementTouchUp;
		}

		private void On_TouchStart(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_TouchStart, gesture);
		}

		private void On_TouchDown(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_TouchDown, gesture);
		}

		private void On_TouchUp(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_TouchUp, gesture);
		}

		private void On_SimpleTap(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_SimpleTap, gesture);
		}

		private void On_DoubleTap(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_DoubleTap, gesture);
		}

		private void On_LongTapStart(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_LongTapStart, gesture);
		}

		private void On_LongTap(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_LongTap, gesture);
		}

		private void On_LongTapEnd(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_LongTapEnd, gesture);
		}

		private void On_SwipeStart(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_SwipeStart, gesture);
		}

		private void On_Swipe(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_Swipe, gesture);
		}

		private void On_SwipeEnd(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_SwipeEnd, gesture);
		}

		private void On_DragStart(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_DragStart, gesture);
		}

		private void On_Drag(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_Drag, gesture);
		}

		private void On_DragEnd(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_DragEnd, gesture);
		}

		private void On_Cancel(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_Cancel, gesture);
		}

		private void On_TouchStart2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_TouchStart2Fingers, gesture);
		}

		private void On_TouchDown2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_TouchDown2Fingers, gesture);
		}

		private void On_TouchUp2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_TouchUp2Fingers, gesture);
		}

		private void On_LongTapStart2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_LongTapStart2Fingers, gesture);
		}

		private void On_LongTap2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_LongTap2Fingers, gesture);
		}

		private void On_LongTapEnd2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_LongTapEnd2Fingers, gesture);
		}

		private void On_DragStart2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_DragStart2Fingers, gesture);
		}

		private void On_Drag2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_Drag2Fingers, gesture);
		}

		private void On_DragEnd2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_DragEnd2Fingers, gesture);
		}

		private void On_SwipeStart2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_SwipeStart2Fingers, gesture);
		}

		private void On_Swipe2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_Swipe2Fingers, gesture);
		}

		private void On_SwipeEnd2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_SwipeEnd2Fingers, gesture);
		}

		private void On_Twist(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_Twist, gesture);
		}

		private void On_TwistEnd(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_TwistEnd, gesture);
		}

		private void On_Pinch(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_Pinch, gesture);
		}

		private void On_PinchOut(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_PinchOut, gesture);
		}

		private void On_PinchIn(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_PinchIn, gesture);
		}

		private void On_PinchEnd(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_PinchEnd, gesture);
		}

		private void On_SimpleTap2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_SimpleTap2Fingers, gesture);
		}

		private void On_DoubleTap2Fingers(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_DoubleTap2Fingers, gesture);
		}

		private void On_UIElementTouchUp(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_UIElementTouchUp, gesture);
		}

		private void On_OverUIElement(Gesture gesture)
		{
			TriggerScheduler(EasyTouch.EvtType.On_OverUIElement, gesture);
		}

		public void AddTrigger(EasyTouch.EvtType ev)
		{
			EasyTouchReceiver easyTouchReceiver = new EasyTouchReceiver();
			easyTouchReceiver.enable = true;
			easyTouchReceiver.restricted = true;
			easyTouchReceiver.eventName = ev;
			easyTouchReceiver.gameObject = null;
			easyTouchReceiver.otherReceiver = false;
			easyTouchReceiver.name = "New trigger";
			receivers.Add(easyTouchReceiver);
			if (Application.isPlaying)
			{
				UnsubscribeEasyTouchEvent();
				SubscribeEasyTouchEvent();
			}
		}

		public bool SetTriggerEnable(string triggerName, bool value)
		{
			EasyTouchReceiver trigger = GetTrigger(triggerName);
			if (trigger != null)
			{
				trigger.enable = value;
				return true;
			}
			return false;
		}

		public bool GetTriggerEnable(string triggerName)
		{
			return GetTrigger(triggerName)?.enable ?? false;
		}

		private void TriggerScheduler(EasyTouch.EvtType evnt, Gesture gesture)
		{
			foreach (EasyTouchReceiver receiver in receivers)
			{
				if (receiver.enable && receiver.eventName == evnt && ((receiver.restricted && ((gesture.pickedObject == base.gameObject && receiver.triggerType == ETTType.Object3D) || (gesture.pickedUIElement == base.gameObject && receiver.triggerType == ETTType.UI))) || (!receiver.restricted && (receiver.gameObject == null || (receiver.gameObject == gesture.pickedObject && receiver.triggerType == ETTType.Object3D) || (gesture.pickedUIElement == receiver.gameObject && receiver.triggerType == ETTType.UI)))))
				{
					GameObject gameObject = base.gameObject;
					if (receiver.otherReceiver && receiver.gameObjectReceiver != null)
					{
						gameObject = receiver.gameObjectReceiver;
					}
					switch (receiver.parameter)
					{
					case ETTParameter.None:
						gameObject.SendMessage(receiver.methodName, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.ActionTime:
						gameObject.SendMessage(receiver.methodName, gesture.actionTime, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Delta_Pinch:
						gameObject.SendMessage(receiver.methodName, gesture.deltaPinch, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Delta_Position:
						gameObject.SendMessage(receiver.methodName, gesture.deltaPosition, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.DeltaTime:
						gameObject.SendMessage(receiver.methodName, gesture.deltaTime, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Finger_Id:
						gameObject.SendMessage(receiver.methodName, gesture.fingerIndex, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Gesture:
						gameObject.SendMessage(receiver.methodName, gesture, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.PickedObject:
						if (gesture.pickedObject != null)
						{
							gameObject.SendMessage(receiver.methodName, gesture.pickedObject, SendMessageOptions.DontRequireReceiver);
						}
						break;
					case ETTParameter.PickedUIElement:
						if (gesture.pickedUIElement != null)
						{
							gameObject.SendMessage(receiver.methodName, gesture.pickedObject, SendMessageOptions.DontRequireReceiver);
						}
						break;
					case ETTParameter.Position:
						gameObject.SendMessage(receiver.methodName, gesture.position, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Start_Position:
						gameObject.SendMessage(receiver.methodName, gesture.startPosition, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Swipe_Length:
						gameObject.SendMessage(receiver.methodName, gesture.swipeLength, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Swipe_Type:
						gameObject.SendMessage(receiver.methodName, gesture.swipe, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Swipe_Vector:
						gameObject.SendMessage(receiver.methodName, gesture.swipeVector, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Touch_Count:
						gameObject.SendMessage(receiver.methodName, gesture.touchCount, SendMessageOptions.DontRequireReceiver);
						break;
					case ETTParameter.Twist_Anlge:
						gameObject.SendMessage(receiver.methodName, gesture.twistAngle, SendMessageOptions.DontRequireReceiver);
						break;
					}
				}
			}
		}

		private bool IsRecevier4(EasyTouch.EvtType evnt)
		{
			if (receivers.FindIndex((EasyTouchReceiver e) => e.eventName == evnt) > -1)
			{
				return true;
			}
			return false;
		}

		private EasyTouchReceiver GetTrigger(string triggerName)
		{
			return receivers.Find((EasyTouchReceiver n) => n.name == triggerName);
		}
	}
}
