using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class ETCButton : ETCBase, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	[Serializable]
	public class OnDownHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnPressedHandler : UnityEvent
	{
	}

	[Serializable]
	public class OnPressedValueandler : UnityEvent<float>
	{
	}

	[Serializable]
	public class OnUPHandler : UnityEvent
	{
	}

	[SerializeField]
	public OnDownHandler onDown;

	[SerializeField]
	public OnPressedHandler onPressed;

	[SerializeField]
	public OnPressedValueandler onPressedValue;

	[SerializeField]
	public OnUPHandler onUp;

	public ETCAxis axis;

	public Sprite normalSprite;

	public Color normalColor;

	public Sprite pressedSprite;

	public Color pressedColor;

	public bool updateNonVisible;

	private Image cachedImage;

	private bool isOnPress;

	private GameObject previousDargObject;

	private bool isOnTouch;

	public ETCButton()
	{
		axis = new ETCAxis("Button");
		_visible = true;
		_activated = true;
		isOnTouch = false;
		enableKeySimulation = true;
		axis.unityAxis = "Jump";
		showPSInspector = true;
		showSpriteInspector = false;
		showBehaviourInspector = false;
		showEventInspector = false;
	}

	protected override void Awake()
	{
		base.Awake();
		cachedImage = GetComponent<Image>();
	}

	public override void Start()
	{
		axis.InitAxis();
		base.Start();
		isOnPress = false;
		if (allowSimulationStandalone && enableKeySimulation && !Application.isEditor)
		{
			SetVisible(visibleOnStandalone);
		}
	}

	protected override void UpdateControlState()
	{
		UpdateButton();
	}

	protected override void DoActionBeforeEndOfFrame()
	{
		axis.DoGravity();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isSwipeIn && !isOnTouch)
		{
			if (eventData.pointerDrag != null && (bool)eventData.pointerDrag.GetComponent<ETCBase>() && eventData.pointerDrag != base.gameObject)
			{
				previousDargObject = eventData.pointerDrag;
			}
			eventData.pointerDrag = base.gameObject;
			eventData.pointerPress = base.gameObject;
			OnPointerDown(eventData);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		
		if (_activated && !isOnTouch)
		{
			pointId = eventData.pointerId;
			axis.ResetAxis();
			axis.axisState = ETCAxis.AxisState.Down;
			isOnPress = false;
			isOnTouch = true;
			onDown.Invoke();
			ApllyState();
			axis.UpdateButton();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (pointId == eventData.pointerId)
		{
			isOnPress = false;
			isOnTouch = false;
			axis.axisState = ETCAxis.AxisState.Up;
			axis.axisValue = 0f;
			onUp.Invoke();
			ApllyState();
			if ((bool)previousDargObject)
			{
				ExecuteEvents.Execute(previousDargObject, eventData, ExecuteEvents.pointerUpHandler);
				previousDargObject = null;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (pointId == eventData.pointerId && axis.axisState == ETCAxis.AxisState.Press && !isSwipeOut)
		{
			OnPointerUp(eventData);
		}
	}

	private void UpdateButton()
	{
		if (axis.axisState == ETCAxis.AxisState.Down)
		{
			isOnPress = true;
			axis.axisState = ETCAxis.AxisState.Press;
		}
		if (isOnPress)
		{
			axis.UpdateButton();
			onPressed.Invoke();
			onPressedValue.Invoke(axis.axisValue);
		}
		if (axis.axisState == ETCAxis.AxisState.Up)
		{
			isOnPress = false;
			axis.axisState = ETCAxis.AxisState.None;
		}
		if (enableKeySimulation && _activated && (_visible || updateNonVisible) && !isOnTouch)
		{
			if (Input.GetButton(axis.unityAxis) && axis.axisState == ETCAxis.AxisState.None)
			{
				axis.ResetAxis();
				onDown.Invoke();
				axis.axisState = ETCAxis.AxisState.Down;
			}
			if (!Input.GetButton(axis.unityAxis) && axis.axisState == ETCAxis.AxisState.Press)
			{
				axis.axisState = ETCAxis.AxisState.Up;
				axis.axisValue = 0f;
				onUp.Invoke();
			}
			axis.UpdateButton();
			ApllyState();
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

	private void ApllyState()
	{
		if ((bool)cachedImage)
		{
			ETCAxis.AxisState axisState = axis.axisState;
			if ((uint)(axisState - 1) <= 1u)
			{
				cachedImage.sprite = pressedSprite;
				cachedImage.color = pressedColor;
			}
			else
			{
				cachedImage.sprite = normalSprite;
				cachedImage.color = normalColor;
			}
		}
	}

	protected override void SetActivated()
	{
		if (!_activated)
		{
			isOnPress = false;
			isOnTouch = false;
			axis.axisState = ETCAxis.AxisState.None;
			axis.axisValue = 0f;
			ApllyState();
		}
	}
}
