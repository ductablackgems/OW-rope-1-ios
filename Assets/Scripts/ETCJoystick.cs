using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class ETCJoystick : ETCBase, IPointerEnterHandler, IEventSystemHandler, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
	[Serializable]
	public class OnMoveStartHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnMoveSpeedHandler : UnityEvent<Vector2>
	{
	}

	[Serializable]
	public class OnMoveHandler : UnityEvent<Vector2>
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
	public class OnTouchUpHandler : UnityEvent
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

	public enum JoystickArea
	{
		UserDefined,
		FullScreen,
		Left,
		Right,
		Top,
		Bottom,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	public enum JoystickType
	{
		Dynamic,
		Static
	}

	public enum RadiusBase
	{
		Width,
		Height,
		UserDefined
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
	public OnTouchUpHandler onTouchUp;

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

	public JoystickType joystickType;

	public bool allowJoystickOverTouchPad;

	public RadiusBase radiusBase;

	public float radiusBaseValue;

	public ETCAxis axisX;

	public ETCAxis axisY;

	public RectTransform thumb;

	public JoystickArea joystickArea;

	public RectTransform userArea;

	public bool isTurnAndMove;

	public float tmSpeed = 10f;

	public float tmAdditionnalRotation;

	public AnimationCurve tmMoveCurve;

	public bool tmLockInJump;

	private Vector3 tmLastMove;

	private Vector2 thumbPosition;

	private bool isDynamicActif;

	private Vector2 tmpAxis;

	private Vector2 OldTmpAxis;

	private bool isOnTouch;

	[SerializeField]
	private bool isNoReturnThumb;

	private Vector2 noReturnPosition;

	private Vector2 noReturnOffset;

	[SerializeField]
	private bool isNoOffsetThumb;

	public bool IsNoReturnThumb
	{
		get
		{
			return isNoReturnThumb;
		}
		set
		{
			isNoReturnThumb = value;
		}
	}

	public bool IsNoOffsetThumb
	{
		get
		{
			return isNoOffsetThumb;
		}
		set
		{
			isNoOffsetThumb = value;
		}
	}

	public ETCJoystick()
	{
		joystickType = JoystickType.Static;
		allowJoystickOverTouchPad = false;
		radiusBase = RadiusBase.Width;
		axisX = new ETCAxis("Horizontal");
		axisY = new ETCAxis("Vertical");
		_visible = true;
		_activated = true;
		joystickArea = JoystickArea.FullScreen;
		isDynamicActif = false;
		isOnDrag = false;
		isOnTouch = false;
		axisX.unityAxis = "Horizontal";
		axisY.unityAxis = "Vertical";
		enableKeySimulation = true;
		isNoReturnThumb = false;
		showPSInspector = false;
		showAxesInspector = false;
		showEventInspector = false;
		showSpriteInspector = false;
	}

	protected override void Awake()
	{
		base.Awake();
		if (joystickType == JoystickType.Dynamic)
		{
			this.rectTransform().anchorMin = new Vector2(0.5f, 0.5f);
			this.rectTransform().anchorMax = new Vector2(0.5f, 0.5f);
			this.rectTransform().SetAsLastSibling();
			base.visible = false;
		}
		if (allowSimulationStandalone && enableKeySimulation && !Application.isEditor && joystickType != 0)
		{
			SetVisible(visibleOnStandalone);
		}
	}

	public override void Start()
	{
		axisX.InitAxis();
		axisY.InitAxis();
		if (enableCamera)
		{
			InitCameraLookAt();
		}
		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;
		noReturnPosition = thumb.position;
		pointId = -1;
		if (joystickType == JoystickType.Dynamic)
		{
			base.visible = false;
		}
		base.Start();
		if (enableCamera && cameraMode == CameraMode.SmoothFollow && (bool)cameraTransform && (bool)cameraLookAt)
		{
			cameraTransform.position = cameraLookAt.TransformPoint(new Vector3(0f, followHeight, 0f - followDistance));
			cameraTransform.LookAt(cameraLookAt);
		}
		if (enableCamera && cameraMode == CameraMode.Follow && (bool)cameraTransform && (bool)cameraLookAt)
		{
			cameraTransform.position = cameraLookAt.position + followOffset;
			cameraTransform.LookAt(cameraLookAt.position);
		}
	}

	public override void Update()
	{
		base.Update();
		if (joystickType == JoystickType.Dynamic && !_visible && _activated)
		{
			Vector2 localPosition = Vector2.zero;
			Vector2 screenPosition = Vector2.zero;
			if (isTouchOverJoystickArea(ref localPosition, ref screenPosition))
			{
				GameObject firstUIElement = GetFirstUIElement(screenPosition);
				if (firstUIElement == null || (allowJoystickOverTouchPad && (bool)firstUIElement.GetComponent<ETCTouchPad>()) || (firstUIElement != null && (bool)firstUIElement.GetComponent<ETCArea>()))
				{
					cachedRectTransform.anchoredPosition = localPosition;
					base.visible = true;
				}
			}
		}
		if (joystickType == JoystickType.Dynamic && _visible && GetTouchCount() == 0)
		{
			base.visible = false;
		}
	}

	public override void LateUpdate()
	{
		if (enableCamera && !cameraLookAt)
		{
			InitCameraLookAt();
		}
		base.LateUpdate();
	}

	private void InitCameraLookAt()
	{
		if (cameraTargetMode == CameraTargetMode.FromDirectActionAxisX)
		{
			cameraLookAt = axisX.directTransform;
		}
		else if (cameraTargetMode == CameraTargetMode.FromDirectActionAxisY)
		{
			cameraLookAt = axisY.directTransform;
			if (isTurnAndMove)
			{
				cameraLookAt = axisX.directTransform;
			}
		}
		else if (cameraTargetMode == CameraTargetMode.LinkOnTag)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag(camTargetTag);
			if ((bool)gameObject)
			{
				cameraLookAt = gameObject.transform;
			}
		}
		if ((bool)cameraLookAt)
		{
			cameraLookAtCC = cameraLookAt.GetComponent<CharacterController>();
		}
	}

	protected override void UpdateControlState()
	{
		if (_visible)
		{
			UpdateJoystick();
		}
		else if (joystickType == JoystickType.Dynamic)
		{
			OnUp(real: false);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (joystickType == JoystickType.Dynamic && !isDynamicActif && _activated && pointId == -1)
		{
			eventData.pointerDrag = base.gameObject;
			eventData.pointerPress = base.gameObject;
			isDynamicActif = true;
			pointId = eventData.pointerId;
		}
		if (joystickType == JoystickType.Dynamic && !eventData.eligibleForClick)
		{
			OnPointerUp(eventData);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		onTouchStart.Invoke();
		pointId = eventData.pointerId;
		if (isNoOffsetThumb)
		{
			OnDrag(eventData);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (pointId != eventData.pointerId)
		{
			return;
		}
		isOnDrag = true;
		isOnTouch = true;
		float radius = GetRadius();
		if (!isNoReturnThumb)
		{
			thumbPosition = eventData.position - eventData.pressPosition;
		}
		else
		{
			thumbPosition = (eventData.position - noReturnPosition) / cachedRootCanvas.rectTransform().localScale.x + noReturnOffset;
		}
		if (isNoOffsetThumb)
		{
			thumbPosition = (eventData.position - (Vector2)cachedRectTransform.position) / cachedRootCanvas.rectTransform().localScale.x;
		}
		thumbPosition.x = Mathf.FloorToInt(thumbPosition.x);
		thumbPosition.y = Mathf.FloorToInt(thumbPosition.y);
		if (!axisX.enable)
		{
			thumbPosition.x = 0f;
		}
		if (!axisY.enable)
		{
			thumbPosition.y = 0f;
		}
		if (thumbPosition.magnitude > radius)
		{
			if (!isNoReturnThumb)
			{
				thumbPosition = thumbPosition.normalized * radius;
			}
			else
			{
				thumbPosition = thumbPosition.normalized * radius;
			}
		}
		thumb.anchoredPosition = thumbPosition;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (pointId == eventData.pointerId)
		{
			OnUp();
		}
	}

	private void OnUp(bool real = true)
	{
		isOnDrag = false;
		isOnTouch = false;
		if (isNoReturnThumb)
		{
			noReturnPosition = thumb.position;
			noReturnOffset = thumbPosition;
		}
		if (!isNoReturnThumb)
		{
			thumbPosition = Vector2.zero;
			thumb.anchoredPosition = Vector2.zero;
			axisX.axisState = ETCAxis.AxisState.None;
			axisY.axisState = ETCAxis.AxisState.None;
		}
		if (!axisX.isEnertia && !axisY.isEnertia)
		{
			axisX.ResetAxis();
			axisY.ResetAxis();
			tmpAxis = Vector2.zero;
			OldTmpAxis = Vector2.zero;
			if (real)
			{
				onMoveEnd.Invoke();
			}
		}
		if (joystickType == JoystickType.Dynamic)
		{
			base.visible = false;
			isDynamicActif = false;
		}
		pointId = -1;
		if (real)
		{
			onTouchUp.Invoke();
		}
	}

	protected override void DoActionBeforeEndOfFrame()
	{
		axisX.DoGravity();
		axisY.DoGravity();
	}

	private void UpdateJoystick()
	{
		if (enableKeySimulation && !isOnTouch && _activated && _visible)
		{
			float axis = UnityEngine.Input.GetAxis(axisX.unityAxis);
			float axis2 = UnityEngine.Input.GetAxis(axisY.unityAxis);
			if (!isNoReturnThumb)
			{
				thumb.localPosition = Vector2.zero;
			}
			isOnDrag = false;
			if (axis != 0f)
			{
				isOnDrag = true;
				thumb.localPosition = new Vector2(GetRadius() * axis, thumb.localPosition.y);
			}
			if (axis2 != 0f)
			{
				isOnDrag = true;
				thumb.localPosition = new Vector2(thumb.localPosition.x, GetRadius() * axis2);
			}
			thumbPosition = thumb.localPosition;
		}
		OldTmpAxis.x = axisX.axisValue;
		OldTmpAxis.y = axisY.axisValue;
		tmpAxis = thumbPosition / GetRadius();
		axisX.UpdateAxis(tmpAxis.x, isOnDrag, ControlType.Joystick);
		axisY.UpdateAxis(tmpAxis.y, isOnDrag, ControlType.Joystick);
		if ((axisX.axisValue != 0f || axisY.axisValue != 0f) && OldTmpAxis == Vector2.zero)
		{
			onMoveStart.Invoke();
		}
		if (axisX.axisValue != 0f || axisY.axisValue != 0f)
		{
			if (!isTurnAndMove)
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
			}
			else
			{
				DoTurnAndMove();
			}
			onMove.Invoke(new Vector2(axisX.axisValue, axisY.axisValue));
			onMoveSpeed.Invoke(new Vector2(axisX.axisSpeedValue, axisY.axisSpeedValue));
		}
		else if (axisX.axisValue == 0f && axisY.axisValue == 0f && OldTmpAxis != Vector2.zero)
		{
			onMoveEnd.Invoke();
		}
		if (!isTurnAndMove)
		{
			if (axisX.axisValue == 0f && (bool)axisX.directCharacterController && !axisX.directCharacterController.isGrounded && axisX.isLockinJump)
			{
				axisX.DoDirectAction();
			}
			if (axisY.axisValue == 0f && (bool)axisY.directCharacterController && !axisY.directCharacterController.isGrounded && axisY.isLockinJump)
			{
				axisY.DoDirectAction();
			}
		}
		else if (axisX.axisValue == 0f && axisY.axisValue == 0f && (bool)axisX.directCharacterController && !axisX.directCharacterController.isGrounded && tmLockInJump)
		{
			DoTurnAndMove();
		}
		float num = 1f;
		if (axisX.invertedAxis)
		{
			num = -1f;
		}
		if (Mathf.Abs(OldTmpAxis.x) < axisX.axisThreshold && Mathf.Abs(axisX.axisValue) >= axisX.axisThreshold)
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
		if (Mathf.Abs(OldTmpAxis.y) < axisY.axisThreshold && Mathf.Abs(axisY.axisValue) >= axisY.axisThreshold)
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

	private bool isTouchOverJoystickArea(ref Vector2 localPosition, ref Vector2 screenPosition)
	{
		bool flag = false;
		bool flag2 = false;
		screenPosition = Vector2.zero;
		int touchCount = GetTouchCount();
		for (int i = 0; i < touchCount; i++)
		{
			if (flag)
			{
				break;
			}
			if (UnityEngine.Input.GetTouch(i).phase == TouchPhase.Began)
			{
				screenPosition = UnityEngine.Input.GetTouch(i).position;
				flag2 = true;
			}
			if (flag2 && isScreenPointOverArea(screenPosition, ref localPosition))
			{
				flag = true;
			}
		}
		return flag;
	}

	private bool isScreenPointOverArea(Vector2 screenPosition, ref Vector2 localPosition)
	{
		bool result = false;
		if (joystickArea != 0)
		{
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedRootCanvas.rectTransform(), screenPosition, null, out localPosition))
			{
				switch (joystickArea)
				{
				case JoystickArea.Left:
					if (localPosition.x < 0f)
					{
						result = true;
					}
					break;
				case JoystickArea.Right:
					if (localPosition.x > 0f)
					{
						result = true;
					}
					break;
				case JoystickArea.FullScreen:
					result = true;
					break;
				case JoystickArea.TopLeft:
					if (localPosition.y > 0f && localPosition.x < 0f)
					{
						result = true;
					}
					break;
				case JoystickArea.Top:
					if (localPosition.y > 0f)
					{
						result = true;
					}
					break;
				case JoystickArea.TopRight:
					if (localPosition.y > 0f && localPosition.x > 0f)
					{
						result = true;
					}
					break;
				case JoystickArea.BottomLeft:
					if (localPosition.y < 0f && localPosition.x < 0f)
					{
						result = true;
					}
					break;
				case JoystickArea.Bottom:
					if (localPosition.y < 0f)
					{
						result = true;
					}
					break;
				case JoystickArea.BottomRight:
					if (localPosition.y < 0f && localPosition.x > 0f)
					{
						result = true;
					}
					break;
				}
			}
		}
		else if (RectTransformUtility.RectangleContainsScreenPoint(userArea, screenPosition, cachedRootCanvas.worldCamera))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedRootCanvas.rectTransform(), screenPosition, cachedRootCanvas.worldCamera, out localPosition);
			result = true;
		}
		return result;
	}

	private int GetTouchCount()
	{
		return UnityEngine.Input.touchCount;
	}

	public float GetRadius()
	{
		float result = 0f;
		switch (radiusBase)
		{
		case RadiusBase.Width:
			result = cachedRectTransform.sizeDelta.x * 0.5f;
			break;
		case RadiusBase.Height:
			result = cachedRectTransform.sizeDelta.y * 0.5f;
			break;
		case RadiusBase.UserDefined:
			result = radiusBaseValue;
			break;
		}
		return result;
	}

	protected override void SetActivated()
	{
		GetComponent<CanvasGroup>().blocksRaycasts = _activated;
		if (!_activated)
		{
			OnUp(real: false);
		}
	}

	protected override void SetVisible(bool visible = true)
	{
		bool enabled = _visible;
		if (!visible)
		{
			enabled = visible;
		}
		GetComponent<Image>().enabled = enabled;
		thumb.GetComponent<Image>().enabled = enabled;
		GetComponent<CanvasGroup>().blocksRaycasts = _activated;
	}

	private void DoTurnAndMove()
	{
		float num = Mathf.Atan2(axisX.axisValue, axisY.axisValue) * 57.29578f;
		float d = tmMoveCurve.Evaluate(new Vector2(axisX.axisValue, axisY.axisValue).magnitude) * tmSpeed;
		if (!(axisX.directTransform != null))
		{
			return;
		}
		axisX.directTransform.rotation = Quaternion.Euler(new Vector3(0f, num + tmAdditionnalRotation, 0f));
		if (axisX.directCharacterController != null)
		{
			if (axisX.directCharacterController.isGrounded || !tmLockInJump)
			{
				Vector3 a = axisX.directCharacterController.transform.TransformDirection(Vector3.forward) * d;
				axisX.directCharacterController.Move(a * Time.deltaTime);
				tmLastMove = a;
			}
			else
			{
				axisX.directCharacterController.Move(tmLastMove * Time.deltaTime);
			}
		}
		else
		{
			axisX.directTransform.Translate(Vector3.forward * d * Time.deltaTime, Space.Self);
		}
	}

	public void InitCurve()
	{
		axisX.InitDeadCurve();
		axisY.InitDeadCurve();
		InitTurnMoveCurve();
	}

	public void InitTurnMoveCurve()
	{
		tmMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		tmMoveCurve.postWrapMode = WrapMode.PingPong;
		tmMoveCurve.preWrapMode = WrapMode.PingPong;
	}
}
