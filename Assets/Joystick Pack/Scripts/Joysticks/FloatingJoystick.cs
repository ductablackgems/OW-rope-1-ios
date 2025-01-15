using System.Collections;
using System.Collections.Generic;
using _0.OW.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    Vector2 startPosition;
    protected override void Start()
    {
        base.Start();
        //background.gameObject.SetActive(false);
        startPosition = background.anchoredPosition;
        background.gameObject.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //background.gameObject.SetActive(false);
        background.anchoredPosition = startPosition;
        OWManager.instance.playerController.playerMovement.OnJoyEnd();
        base.OnPointerUp(eventData);
    }

    public void JoyUp()
    {
        background.anchoredPosition = startPosition;
        Up();
    }
}