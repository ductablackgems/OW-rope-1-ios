using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ETCDPad : ETCBase, IDragHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler
{
	[Serializable]
	public class OnMoveStartHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnMoveHandler : UnityEvent<Vector2>
	{
	}

	[Serializable]
	public class OnMoveSpeedHandler : UnityEvent<Vector2>
	{
	}

	[Serializable]
	public class OnMoveEndHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnTouchStartHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnTouchUPHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnDownUpHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnDownDownHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnDownLeftHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnDownRightHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnPressUpHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnPressDownHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnPressLeftHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnPressRightHandler : UnityEvent
	{
	}

	[SerializeField]
	public OnMoveStartHandler onMoveStart;

	[SerializeField]
	public OnMoveHandler onMove;

	[SerializeField]
	public OnMoveSpeedHandler onMoveSpeed;

	[SerializeField]
	public OnMoveEndHandler onMoveEnd;

	[SerializeField]
	public OnTouchStartHandler onTouchStart;

	[SerializeField]
	public OnTouchUPHandler onTouchUp;

	[SerializeField]
	public OnDownUpHandler OnDownUp;

	[SerializeField]
	public OnDownDownHandler OnDownDown;

	[SerializeField]
	public OnDownLeftHandler OnDownLeft;

	[SerializeField]
	public OnDownRightHandler OnDownRight;

	[SerializeField]
	public OnDownUpHandler OnPressUp;

	[SerializeField]
	public OnDownDownHandler OnPressDown;

	[SerializeField]
	public OnDownLeftHandler OnPressLeft;

	[SerializeField]
	public OnDownRightHandler OnPressRight;

	public ETCAxis axisX;

	public ETCAxis axisY;

	public Sprite normalSprite;

	public Color normalColor;

	public Sprite pressedSprite;

	public Color pressedColor;

	private Vector2 tmpAxis;

	private Vector2 OldTmpAxis;

	private bool isOnTouch;

	private Image cachedImage;

	public float buttonSizeCoef = 3f;

	public ETCDPad()
	{
		axisX = new ETCAxis("Horizontal");
		axisY = new ETCAxis("Vertical");
		_visible = true;
		_activated = true;
		dPadAxisCount = DPadAxis.Two_Axis;
		tmpAxis = Vector2.zero;
		showPSInspector = true;
		showSpriteInspector = false;
		showBehaviourInspector = false;
		showEventInspector = false;
		isOnDrag = false;
		isOnTouch = false;
		axisX.unityAxis = "Horizontal";
		axisY.unityAxis = "Vertical";
		enableKeySimulation = true;
	}

	public override void Start()
	{
		base.Start();
		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;
		axisX.InitAxis();
		axisY.InitAxis();
		if (allowSimulationStandalone && enableKeySimulation && !Application.isEditor)
		{
			SetVisible(visibleOnStandalone);
		}
	}

	protected override void UpdateControlState()
	{
		UpdateDPad();
	}

	protected override void DoActionBeforeEndOfFrame()
	{
		axisX.DoGravity();
		axisY.DoGravity();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (_activated && !isOnTouch)
		{
			onTouchStart.Invoke();
			GetTouchDirection(eventData.position, eventData.pressEventCamera);
			isOnTouch = true;
			isOnDrag = true;
			pointId = eventData.pointerId;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (_activated && pointId == eventData.pointerId)
		{
			isOnTouch = true;
			isOnDrag = true;
			GetTouchDirection(eventData.position, eventData.pressEventCamera);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (pointId == eventData.pointerId)
		{
			isOnTouch = false;
			isOnDrag = false;
			tmpAxis = Vector2.zero;
			OldTmpAxis = Vector2.zero;
			axisX.axisState = ETCAxis.AxisState.None;
			axisY.axisState = ETCAxis.AxisState.None;
			if (!axisX.isEnertia && !axisY.isEnertia)
			{
				axisX.ResetAxis();
				axisY.ResetAxis();
				onMoveEnd.Invoke();
			}
			pointId = -1;
			onTouchUp.Invoke();
		}
	}

	private void UpdateDPad()
	{
		if (enableKeySimulation && !isOnTouch && _activated && _visible)
		{
			float axis = UnityEngine.Input.GetAxis(axisX.unityAxis);
			float axis2 = UnityEngine.Input.GetAxis(axisY.unityAxis);
			isOnDrag = false;
			tmpAxis = Vector2.zero;
			if (axis != 0f)
			{
				isOnDrag = true;
				tmpAxis = new Vector2(1f * Mathf.Sign(axis), tmpAxis.y);
			}
			if (axis2 != 0f)
			{
				isOnDrag = true;
				tmpAxis = new Vector2(tmpAxis.x, 1f * Mathf.Sign(axis2));
			}
		}
		OldTmpAxis.x = axisX.axisValue;
		OldTmpAxis.y = axisY.axisValue;
		axisX.UpdateAxis(tmpAxis.x, isOnDrag, ControlType.DPad);
		axisY.UpdateAxis(tmpAxis.y, isOnDrag, ControlType.DPad);
		if ((axisX.axisValue != 0f || axisY.axisValue != 0f) && OldTmpAxis == Vector2.zero)
		{
			onMoveStart.Invoke();
		}
		if (axisX.axisValue != 0f || axisY.axisValue != 0f)
		{
			if (axisX.actionOn == ETCAxis.ActionOn.Down && (axisX.axisState == ETCAxis.AxisState.DownLeft || axisX.axisState == ETCAxis.AxisState.DownRight))
			{
				axisX.DoDirectAction();
			}
			else if (axisX.actionOn == ETCAxis.ActionOn.Press)
			{
				axisX.DoDirectAction();
			}
			if (axisY.actionOn == ETCAxis.ActionOn.Down && (axisY.axisState == ETCAxis.AxisState.DownUp || axisY.axisState == ETCAxis.AxisState.DownDown))
			{
				axisY.DoDirectAction();
			}
			else if (axisY.actionOn == ETCAxis.ActionOn.Press)
			{
				axisY.DoDirectAction();
			}
			onMove.Invoke(new Vector2(axisX.axisValue, axisY.axisValue));
			onMoveSpeed.Invoke(new Vector2(axisX.axisSpeedValue, axisY.axisSpeedValue));
		}
		else if (axisX.axisValue == 0f && axisY.axisValue == 0f && OldTmpAxis != Vector2.zero)
		{
			onMoveEnd.Invoke();
		}
		float num = 1f;
		if (axisX.invertedAxis)
		{
			num = -1f;
		}
		if (OldTmpAxis.x == 0f && Mathf.Abs(axisX.axisValue) > 0f)
		{
			if (axisX.axisValue * num > 0f)
			{
				axisX.axisState = ETCAxis.AxisState.DownRight;
				OnDownRight.Invoke();
			}
			else if (axisX.axisValue * num < 0f)
			{
				axisX.axisState = ETCAxis.AxisState.DownLeft;
				OnDownLeft.Invoke();
			}
			else
			{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisX.axisState != 0)
		{
			if (axisX.axisValue * num > 0f)
			{
				axisX.axisState = ETCAxis.AxisState.PressRight;
				OnPressRight.Invoke();
			}
			else if (axisX.axisValue * num < 0f)
			{
				axisX.axisState = ETCAxis.AxisState.PressLeft;
				OnPressLeft.Invoke();
			}
			else
			{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}
		num = 1f;
		if (axisY.invertedAxis)
		{
			num = -1f;
		}
		if (OldTmpAxis.y == 0f && Mathf.Abs(axisY.axisValue) > 0f)
		{
			if (axisY.axisValue * num > 0f)
			{
				axisY.axisState = ETCAxis.AxisState.DownUp;
				OnDownUp.Invoke();
			}
			else if (axisY.axisValue * num < 0f)
			{
				axisY.axisState = ETCAxis.AxisState.DownDown;
				OnDownDown.Invoke();
			}
			else
			{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisY.axisState != 0)
		{
			if (axisY.axisValue * num > 0f)
			{
				axisY.axisState = ETCAxis.AxisState.PressUp;
				OnPressUp.Invoke();
			}
			else if (axisY.axisValue * num < 0f)
			{
				axisY.axisState = ETCAxis.AxisState.PressDown;
				OnPressDown.Invoke();
			}
			else
			{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}
	}

	protected override void SetVisible(bool forceUnvisible = false)
	{
		bool visible = _visible;
		if (!base.visible)
		{
			visible = base.visible;
		}
		GetComponent<Image>().enabled = visible;
	}

	protected override void SetActivated()
	{
		if (!_activated)
		{
			isOnTouch = false;
			isOnDrag = false;
			tmpAxis = Vector2.zero;
			OldTmpAxis = Vector2.zero;
			axisX.axisState = ETCAxis.AxisState.None;
			axisY.axisState = ETCAxis.AxisState.None;
			if (!axisX.isEnertia && !axisY.isEnertia)
			{
				axisX.ResetAxis();
				axisY.ResetAxis();
			}
			pointId = -1;
		}
	}

	private void GetTouchDirection(Vector2 position, Camera cam)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedRectTransform, position, cam, out Vector2 localPoint);
		Vector2 vector = this.rectTransform().sizeDelta / buttonSizeCoef;
		tmpAxis = Vector2.zero;
		if ((localPoint.x < (0f - vector.x) / 2f && localPoint.y > (0f - vector.y) / 2f && localPoint.y < vector.y / 2f && dPadAxisCount == DPadAxis.Two_Axis) || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.x < (0f - vector.x) / 2f))
		{
			tmpAxis.x = -1f;
		}
		if ((localPoint.x > vector.x / 2f && localPoint.y > (0f - vector.y) / 2f && localPoint.y < vector.y / 2f && dPadAxisCount == DPadAxis.Two_Axis) || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.x > vector.x / 2f))
		{
			tmpAxis.x = 1f;
		}
		if ((localPoint.y > vector.y / 2f && localPoint.x > (0f - vector.x) / 2f && localPoint.x < vector.x / 2f && dPadAxisCount == DPadAxis.Two_Axis) || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.y > vector.y / 2f))
		{
			tmpAxis.y = 1f;
		}
		if ((localPoint.y < (0f - vector.y) / 2f && localPoint.x > (0f - vector.x) / 2f && localPoint.x < vector.x / 2f && dPadAxisCount == DPadAxis.Two_Axis) || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.y < (0f - vector.y) / 2f))
		{
			tmpAxis.y = -1f;
		}
	}
}
