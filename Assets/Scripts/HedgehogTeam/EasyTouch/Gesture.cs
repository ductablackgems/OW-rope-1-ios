using System;
using UnityEngine;

namespace HedgehogTeam.EasyTouch
{
	public class Gesture : BaseFinger, ICloneable
	{
		public EasyTouch.SwipeDirection swipe;

		public float swipeLength;

		public Vector2 swipeVector;

		public float deltaPinch;

		public float twistAngle;

		public float twoFingerDistance;

		public EasyTouch.EvtType type;

		public object Clone()
		{
			return MemberwiseClone();
		}

		public Vector3 GetTouchToWorldPoint(float z)
		{
			return Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, z));
		}

		public Vector3 GetTouchToWorldPoint(Vector3 position3D)
		{
			return Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, Camera.main.transform.InverseTransformPoint(position3D).z));
		}

		public float GetSwipeOrDragAngle()
		{
			return Mathf.Atan2(swipeVector.normalized.y, swipeVector.normalized.x) * 57.29578f;
		}

		public Vector2 NormalizedPosition()
		{
			return new Vector2(100f / (float)Screen.width * position.x / 100f, 100f / (float)Screen.height * position.y / 100f);
		}

		public bool IsOverUIElement()
		{
			return EasyTouch.IsFingerOverUIElement(fingerIndex);
		}

		public bool IsOverRectTransform(RectTransform tr, Camera camera = null)
		{
			if (camera == null)
			{
				return RectTransformUtility.RectangleContainsScreenPoint(tr, position, null);
			}
			return RectTransformUtility.RectangleContainsScreenPoint(tr, position, camera);
		}

		public GameObject GetCurrentFirstPickedUIElement(bool isTwoFinger = false)
		{
			return EasyTouch.GetCurrentPickedUIElement(fingerIndex, isTwoFinger);
		}

		public GameObject GetCurrentPickedObject(bool isTwoFinger = false)
		{
			return EasyTouch.GetCurrentPickedObject(fingerIndex, isTwoFinger);
		}
	}
}
