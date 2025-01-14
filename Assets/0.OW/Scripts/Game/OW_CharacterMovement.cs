using System;
using _0.DucLib.Scripts.Common;
using HedgehogTeam.EasyTouch;
using UnityEngine;


namespace _0.OW.Scripts.Game
{
    public class OW_CharacterMovement : MonoBehaviour
    {
        public Transform Camera;
        Vector2 lastTouch;
        private void Awake()
        {
            EasyTouch.On_TouchDown += EasyTouch_On_TouchDown;
            EasyTouch.On_TouchUp += EasyTouch_On_TouchUp;
            EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
        }

        private void EasyTouch_On_TouchStart(Gesture gesture)
        {
            lastTouch = gesture.position;
        }

        private void EasyTouch_On_TouchUp(Gesture gesture)
        {
        }
        
        private void EasyTouch_On_TouchDown(Gesture gesture)
        {
            if (gesture.position.x > Screen.width * 0.3f)
            {
                var sensitivity = 0.5f;
                Vector2 deltaPosition = gesture.deltaPosition;
                Camera.transform.eulerAngles += Vector3.up * (deltaPosition.x / Screen.width) * 580 * sensitivity;

                float x = Camera.transform.eulerAngles.x;
                if (x > 180) x = x - 360;
                x = Mathf.Clamp(x - (deltaPosition.y / Screen.height) * 450 * sensitivity, -30, 60);
                Camera.transform.localEulerAngles = new Vector3(x, Camera.transform.localEulerAngles.y, 0);
            }
        }
    }
}