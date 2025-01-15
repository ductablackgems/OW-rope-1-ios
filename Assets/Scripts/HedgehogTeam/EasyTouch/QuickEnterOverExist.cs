using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick Enter-Over-Exit")]
	public class QuickEnterOverExist : QuickBase
	{
		[Serializable]
		public class OnTouchEnter : UnityEvent<Gesture>
		{
		}

		[Serializable]
		public class OnTouchOver : UnityEvent<Gesture>
		{
		}

		[Serializable]
		public class OnTouchExit : UnityEvent<Gesture>
		{
		}

		[SerializeField]
		public OnTouchEnter onTouchEnter;

		[SerializeField]
		public OnTouchOver onTouchOver;

		[SerializeField]
		public OnTouchExit onTouchExit;

		private bool[] fingerOver = new bool[100];

		public QuickEnterOverExist()
		{
			quickActionName = "QuickEnterOverExit" + Guid.NewGuid().ToString().Substring(0, 7);
		}

		private void Awake()
		{
			for (int i = 0; i < 100; i++)
			{
				fingerOver[i] = false;
			}
		}

		public override void OnEnable()
		{
			base.OnEnable();
			EasyTouch.On_TouchDown += On_TouchDown;
			EasyTouch.On_TouchUp += On_TouchUp;
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
			EasyTouch.On_TouchDown -= On_TouchDown;
			EasyTouch.On_TouchUp -= On_TouchUp;
		}

		private void On_TouchDown(Gesture gesture)
		{
			if (realType != GameObjectType.UI)
			{
				if ((!enablePickOverUI && gesture.GetCurrentFirstPickedUIElement() == null) || enablePickOverUI)
				{
					if (gesture.GetCurrentPickedObject() == base.gameObject)
					{
						if (!fingerOver[gesture.fingerIndex] && ((!isOnTouch && !isMultiTouch) || isMultiTouch))
						{
							fingerOver[gesture.fingerIndex] = true;
							onTouchEnter.Invoke(gesture);
							isOnTouch = true;
						}
						else if (fingerOver[gesture.fingerIndex])
						{
							onTouchOver.Invoke(gesture);
						}
					}
					else if (fingerOver[gesture.fingerIndex])
					{
						fingerOver[gesture.fingerIndex] = false;
						onTouchExit.Invoke(gesture);
						if (!isMultiTouch)
						{
							isOnTouch = false;
						}
					}
				}
				else if (gesture.GetCurrentPickedObject() == base.gameObject && !enablePickOverUI && gesture.GetCurrentFirstPickedUIElement() != null && fingerOver[gesture.fingerIndex])
				{
					fingerOver[gesture.fingerIndex] = false;
					onTouchExit.Invoke(gesture);
					if (!isMultiTouch)
					{
						isOnTouch = false;
					}
				}
			}
			else if (gesture.GetCurrentFirstPickedUIElement() == base.gameObject)
			{
				if (!fingerOver[gesture.fingerIndex] && ((!isOnTouch && !isMultiTouch) || isMultiTouch))
				{
					fingerOver[gesture.fingerIndex] = true;
					onTouchEnter.Invoke(gesture);
					isOnTouch = true;
				}
				else if (fingerOver[gesture.fingerIndex])
				{
					onTouchOver.Invoke(gesture);
				}
			}
			else if (fingerOver[gesture.fingerIndex])
			{
				fingerOver[gesture.fingerIndex] = false;
				onTouchExit.Invoke(gesture);
				if (!isMultiTouch)
				{
					isOnTouch = false;
				}
			}
		}

		private void On_TouchUp(Gesture gesture)
		{
			if (fingerOver[gesture.fingerIndex])
			{
				fingerOver[gesture.fingerIndex] = false;
				onTouchExit.Invoke(gesture);
				if (!isMultiTouch)
				{
					isOnTouch = false;
				}
			}
		}
	}
}
