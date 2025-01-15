using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class ETCTouchPad : ETCBase, IBeginDragHandler, IEventSystemHandler, IDragHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
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

	public bool isDPI;

	private Image cachedImage;

	private Vector2 tmpAxis;

	private Vector2 OldTmpAxis;

	private GameObject previousDargObject;

	private bool isOut;

	private bool isOnTouch;

	private bool cachedVisible;

	public float intensity;

	public ETCTouchPad()
	{
		axisX = new ETCAxis("Horizontal");
		axisX.speed = 1f;
		axisY = new ETCAxis("Vertical");
		axisY.speed = 1f;
		_visible = true;
		_activated = true;
		showPSInspector = true;
		showSpriteInspector = false;
		showBehaviourInspector = false;
		showEventInspector = false;
		tmpAxis = Vector2.zero;
		isOnDrag = false;
		isOnTouch = false;
		axisX.unityAxis = "Horizontal";
		axisY.unityAxis = "Vertical";
		enableKeySimulation = true;
		enableKeySimulation = false;
		isOut = false;
		axisX.axisState = ETCAxis.AxisState.None;
		useFixedUpdate = false;
		isDPI = false;
	}

	protected override void Awake()
	{
		base.Awake();
		cachedVisible = _visible;
		cachedImage = GetComponent<Image>();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (!cachedVisible)
		{
			cachedImage.color = new Color(0f, 0f, 0f, 0f);
		}
		if (allowSimulationStandalone && enableKeySimulation && !Application.isEditor)
		{
			SetVisible(visibleOnStandalone);
		}
	}

	public override void Start()
	{
		base.Start();
		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;
		axisX.InitAxis();
		axisY.InitAxis();
	}

	protected override void UpdateControlState()
	{
		UpdateTouchPad();
	}

	protected override void DoActionBeforeEndOfFrame()
	{
		axisX.DoGravity();
		axisY.DoGravity();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isSwipeIn && axisX.axisState == ETCAxis.AxisState.None && _activated && !isOnTouch)
		{
			if (eventData.pointerDrag != null && eventData.pointerDrag != base.gameObject)
			{
				previousDargObject = eventData.pointerDrag;
			}
			else if (eventData.pointerPress != null && eventData.pointerPress != base.gameObject)
			{
				previousDargObject = eventData.pointerPress;
			}
			eventData.pointerDrag = base.gameObject;
			eventData.pointerPress = base.gameObject;
			OnPointerDown(eventData);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (pointId == eventData.pointerId)
		{
			onMoveStart.Invoke();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (base.activated && !isOut && pointId == eventData.pointerId)
		{
			isOnTouch = true;
			isOnDrag = true;
			if (isDPI)
			{
				tmpAxis = new Vector2(eventData.delta.x / Screen.dpi * 100f, eventData.delta.y / Screen.dpi * 100f);
			}
			else
			{
				tmpAxis = new Vector2(eventData.delta.x, eventData.delta.y);
			}
			if (!axisX.enable)
			{
				tmpAxis.x = 0f;
			}
			if (!axisY.enable)
			{
				tmpAxis.y = 0f;
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (_activated && !isOnTouch)
		{
			axisX.axisState = ETCAxis.AxisState.Down;
			tmpAxis = eventData.delta;
			isOut = false;
			isOnTouch = true;
			pointId = eventData.pointerId;
			onTouchStart.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (pointId == eventData.pointerId)
		{
			isOnDrag = false;
			isOnTouch = false;
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
			onTouchUp.Invoke();
			if ((bool)previousDargObject)
			{
				ExecuteEvents.Execute(previousDargObject, eventData, ExecuteEvents.pointerUpHandler);
				previousDargObject = null;
			}
			pointId = -1;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (pointId == eventData.pointerId && !isSwipeOut)
		{
			isOut = true;
			OnPointerUp(eventData);
		}
	}

	private void UpdateTouchPad()
	{
		if (enableKeySimulation && !isOnTouch && _activated && _visible)
		{
			isOnDrag = false;
			tmpAxis = Vector2.zero;
			float axis = UnityEngine.Input.GetAxis(axisX.unityAxis);
			float axis2 = UnityEngine.Input.GetAxis(axisY.unityAxis);
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
		// WayU Sensitive
		this.axisX.UpdateAxis(this.tmpAxis.x * intensity, this.isOnDrag, ETCBase.ControlType.DPad, true);
		this.axisY.UpdateAxis(this.tmpAxis.y * intensity, this.isOnDrag, ETCBase.ControlType.DPad, true);
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
		tmpAxis = Vector2.zero;
	}

	protected override void SetVisible(bool forceUnvisible = false)
	{
		if (Application.isPlaying)
		{
			if (!_visible)
			{
				cachedImage.color = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				cachedImage.color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}

	protected override void SetActivated()
	{
		if (!_activated)
		{
			isOnDrag = false;
			isOnTouch = false;
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
}
