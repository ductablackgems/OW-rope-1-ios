using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick Drag")]
	public class QuickDrag : QuickBase
	{
		[Serializable]
		public class OnDragStart : UnityEvent<Gesture>
		{
		}

		[Serializable]
		public class OnDrag : UnityEvent<Gesture>
		{
		}

		[Serializable]
		public class OnDragEnd : UnityEvent<Gesture>
		{
		}

		[SerializeField]
		public OnDragStart onDragStart;

		[SerializeField]
		public OnDrag onDrag;

		[SerializeField]
		public OnDragEnd onDragEnd;

		public bool isStopOncollisionEnter;

		private Vector3 deltaPosition;

		private bool isOnDrag;

		private Gesture lastGesture;

		public QuickDrag()
		{
			quickActionName = "QuickDrag" + Guid.NewGuid().ToString().Substring(0, 7);
			axesAction = AffectedAxesAction.XY;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			EasyTouch.On_TouchStart += On_TouchStart;
			EasyTouch.On_TouchDown += On_TouchDown;
			EasyTouch.On_TouchUp += On_TouchUp;
			EasyTouch.On_Drag += On_Drag;
			EasyTouch.On_DragStart += On_DragStart;
			EasyTouch.On_DragEnd += On_DragEnd;
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
			EasyTouch.On_TouchStart -= On_TouchStart;
			EasyTouch.On_TouchDown -= On_TouchDown;
			EasyTouch.On_TouchUp -= On_TouchUp;
			EasyTouch.On_Drag -= On_Drag;
			EasyTouch.On_DragStart -= On_DragStart;
			EasyTouch.On_DragEnd -= On_DragEnd;
		}

		private void OnCollisionEnter()
		{
			if (isStopOncollisionEnter && isOnDrag)
			{
				StopDrag();
			}
		}

		private void On_TouchStart(Gesture gesture)
		{
			if (realType == GameObjectType.UI && gesture.isOverGui && (gesture.pickedUIElement == base.gameObject || gesture.pickedUIElement.transform.IsChildOf(base.transform)) && fingerIndex == -1)
			{
				fingerIndex = gesture.fingerIndex;
				base.transform.SetAsLastSibling();
				onDragStart.Invoke(gesture);
				isOnDrag = true;
			}
		}

		private void On_TouchDown(Gesture gesture)
		{
			if (isOnDrag && fingerIndex == gesture.fingerIndex && realType == GameObjectType.UI && gesture.isOverGui && (gesture.pickedUIElement == base.gameObject || gesture.pickedUIElement.transform.IsChildOf(base.transform)))
			{
				base.transform.position += (Vector3)gesture.deltaPosition;
				if (gesture.deltaPosition != Vector2.zero)
				{
					onDrag.Invoke(gesture);
				}
				lastGesture = gesture;
			}
		}

		private void On_TouchUp(Gesture gesture)
		{
			if (fingerIndex == gesture.fingerIndex && realType == GameObjectType.UI)
			{
				lastGesture = gesture;
				StopDrag();
			}
		}

		private void On_DragStart(Gesture gesture)
		{
			if (realType == GameObjectType.UI || ((enablePickOverUI || !(gesture.pickedUIElement == null)) && !enablePickOverUI) || !(gesture.pickedObject == base.gameObject) || isOnDrag)
			{
				return;
			}
			isOnDrag = true;
			fingerIndex = gesture.fingerIndex;
			Vector3 touchToWorldPoint = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
			deltaPosition = touchToWorldPoint - base.transform.position;
			if (resetPhysic)
			{
				if ((bool)cachedRigidBody)
				{
					cachedRigidBody.isKinematic = true;
				}
				if ((bool)cachedRigidBody2D)
				{
					cachedRigidBody2D.isKinematic = true;
				}
			}
			onDragStart.Invoke(gesture);
		}

		private void On_Drag(Gesture gesture)
		{
			if (fingerIndex == gesture.fingerIndex && (realType == GameObjectType.Obj_2D || realType == GameObjectType.Obj_3D) && gesture.pickedObject == base.gameObject && fingerIndex == gesture.fingerIndex)
			{
				Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position) - deltaPosition;
				base.transform.position = GetPositionAxes(position);
				if (gesture.deltaPosition != Vector2.zero)
				{
					onDrag.Invoke(gesture);
				}
				lastGesture = gesture;
			}
		}

		private void On_DragEnd(Gesture gesture)
		{
			if (fingerIndex == gesture.fingerIndex)
			{
				lastGesture = gesture;
				StopDrag();
			}
		}

		private Vector3 GetPositionAxes(Vector3 position)
		{
			Vector3 result = position;
			switch (axesAction)
			{
			case AffectedAxesAction.X:
				result = new Vector3(position.x, base.transform.position.y, base.transform.position.z);
				break;
			case AffectedAxesAction.Y:
				result = new Vector3(base.transform.position.x, position.y, base.transform.position.z);
				break;
			case AffectedAxesAction.Z:
				result = new Vector3(base.transform.position.x, base.transform.position.y, position.z);
				break;
			case AffectedAxesAction.XY:
				result = new Vector3(position.x, position.y, base.transform.position.z);
				break;
			case AffectedAxesAction.XZ:
				result = new Vector3(position.x, base.transform.position.y, position.z);
				break;
			case AffectedAxesAction.YZ:
				result = new Vector3(base.transform.position.x, position.y, position.z);
				break;
			}
			return result;
		}

		public void StopDrag()
		{
			fingerIndex = -1;
			if (resetPhysic)
			{
				if ((bool)cachedRigidBody)
				{
					cachedRigidBody.isKinematic = isKinematic;
				}
				if ((bool)cachedRigidBody2D)
				{
					cachedRigidBody2D.isKinematic = isKinematic2D;
				}
			}
			isOnDrag = false;
			onDragEnd.Invoke(lastGesture);
		}
	}
}
