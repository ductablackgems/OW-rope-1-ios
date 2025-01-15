using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HedgehogTeam.EasyTouch
{
	public class EasyTouch : MonoBehaviour
	{
		[Serializable]
		private class DoubleTap
		{
			public bool inDoubleTap;

			public bool inWait;

			public float time;

			public int count;

			public Finger finger;

			public void Stop()
			{
				inDoubleTap = false;
				inWait = false;
				time = 0f;
				count = 0;
			}
		}

		private class PickedObject
		{
			public GameObject pickedObj;

			public Camera pickedCamera;

			public bool isGUI;
		}

		public delegate void TouchCancelHandler(Gesture gesture);

		public delegate void Cancel2FingersHandler(Gesture gesture);

		public delegate void TouchStartHandler(Gesture gesture);

		public delegate void TouchDownHandler(Gesture gesture);

		public delegate void TouchUpHandler(Gesture gesture);

		public delegate void SimpleTapHandler(Gesture gesture);

		public delegate void DoubleTapHandler(Gesture gesture);

		public delegate void LongTapStartHandler(Gesture gesture);

		public delegate void LongTapHandler(Gesture gesture);

		public delegate void LongTapEndHandler(Gesture gesture);

		public delegate void DragStartHandler(Gesture gesture);

		public delegate void DragHandler(Gesture gesture);

		public delegate void DragEndHandler(Gesture gesture);

		public delegate void SwipeStartHandler(Gesture gesture);

		public delegate void SwipeHandler(Gesture gesture);

		public delegate void SwipeEndHandler(Gesture gesture);

		public delegate void TouchStart2FingersHandler(Gesture gesture);

		public delegate void TouchDown2FingersHandler(Gesture gesture);

		public delegate void TouchUp2FingersHandler(Gesture gesture);

		public delegate void SimpleTap2FingersHandler(Gesture gesture);

		public delegate void DoubleTap2FingersHandler(Gesture gesture);

		public delegate void LongTapStart2FingersHandler(Gesture gesture);

		public delegate void LongTap2FingersHandler(Gesture gesture);

		public delegate void LongTapEnd2FingersHandler(Gesture gesture);

		public delegate void TwistHandler(Gesture gesture);

		public delegate void TwistEndHandler(Gesture gesture);

		public delegate void PinchInHandler(Gesture gesture);

		public delegate void PinchOutHandler(Gesture gesture);

		public delegate void PinchEndHandler(Gesture gesture);

		public delegate void PinchHandler(Gesture gesture);

		public delegate void DragStart2FingersHandler(Gesture gesture);

		public delegate void Drag2FingersHandler(Gesture gesture);

		public delegate void DragEnd2FingersHandler(Gesture gesture);

		public delegate void SwipeStart2FingersHandler(Gesture gesture);

		public delegate void Swipe2FingersHandler(Gesture gesture);

		public delegate void SwipeEnd2FingersHandler(Gesture gesture);

		public delegate void EasyTouchIsReadyHandler();

		public delegate void OverUIElementHandler(Gesture gesture);

		public delegate void UIElementTouchUpHandler(Gesture gesture);

		public enum GesturePriority
		{
			Tap,
			Slips
		}

		public enum DoubleTapDetection
		{
			BySystem,
			ByTime
		}

		public enum GestureType
		{
			Tap,
			Drag,
			Swipe,
			None,
			LongTap,
			Pinch,
			Twist,
			Cancel,
			Acquisition
		}

		public enum SwipeDirection
		{
			None,
			Left,
			Right,
			Up,
			Down,
			UpLeft,
			UpRight,
			DownLeft,
			DownRight,
			Other,
			All
		}

		public enum TwoFingerPickMethod
		{
			Finger,
			Average
		}

		public enum EvtType
		{
			None,
			On_TouchStart,
			On_TouchDown,
			On_TouchUp,
			On_SimpleTap,
			On_DoubleTap,
			On_LongTapStart,
			On_LongTap,
			On_LongTapEnd,
			On_DragStart,
			On_Drag,
			On_DragEnd,
			On_SwipeStart,
			On_Swipe,
			On_SwipeEnd,
			On_TouchStart2Fingers,
			On_TouchDown2Fingers,
			On_TouchUp2Fingers,
			On_SimpleTap2Fingers,
			On_DoubleTap2Fingers,
			On_LongTapStart2Fingers,
			On_LongTap2Fingers,
			On_LongTapEnd2Fingers,
			On_Twist,
			On_TwistEnd,
			On_Pinch,
			On_PinchIn,
			On_PinchOut,
			On_PinchEnd,
			On_DragStart2Fingers,
			On_Drag2Fingers,
			On_DragEnd2Fingers,
			On_SwipeStart2Fingers,
			On_Swipe2Fingers,
			On_SwipeEnd2Fingers,
			On_EasyTouchIsReady,
			On_Cancel,
			On_Cancel2Fingers,
			On_OverUIElement,
			On_UIElementTouchUp
		}

		private static EasyTouch _instance;

		private Gesture _currentGesture = new Gesture();

		private List<Gesture> _currentGestures = new List<Gesture>();

		public bool enable;

		public bool enableRemote;

		public GesturePriority gesturePriority;

		public float StationaryTolerance;

		public float longTapTime;

		public float swipeTolerance;

		public float minPinchLength;

		public float minTwistAngle;

		public DoubleTapDetection doubleTapDetection;

		public float doubleTapTime;

		public bool alwaysSendSwipe;

		public bool enable2FingersGesture;

		public bool enableTwist;

		public bool enablePinch;

		public bool enable2FingersSwipe;

		public TwoFingerPickMethod twoFingerPickMethod;

		public List<ECamera> touchCameras;

		public bool autoSelect;

		public LayerMask pickableLayers3D;

		public bool enable2D;

		public LayerMask pickableLayers2D;

		public bool autoUpdatePickedObject;

		public bool allowUIDetection;

		public bool enableUIMode;

		public bool autoUpdatePickedUI;

		public bool enabledNGuiMode;

		public LayerMask nGUILayers;

		public List<Camera> nGUICameras;

		public bool enableSimulation;

		public KeyCode twistKey;

		public KeyCode swipeKey;

		public bool showGuiInspector;

		public bool showSelectInspector;

		public bool showGestureInspector;

		public bool showTwoFingerInspector;

		public bool showSecondFingerInspector;

		private EasyTouchInput input = new EasyTouchInput();

		private Finger[] fingers = new Finger[100];

		public Texture secondFingerTexture;

		private TwoFingerGesture twoFinger = new TwoFingerGesture();

		private int oldTouchCount;

		private DoubleTap[] singleDoubleTap = new DoubleTap[100];

		private Finger[] tmpArray = new Finger[100];

		private PickedObject pickedObject = new PickedObject();

		private List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();

		private PointerEventData uiPointerEventData;

		private EventSystem uiEventSystem;

		public static EasyTouch instance
		{
			get
			{
				if (!_instance)
				{
					_instance = (UnityEngine.Object.FindObjectOfType(typeof(EasyTouch)) as EasyTouch);
					if (!_instance)
					{
						_instance = new GameObject("Easytouch").AddComponent<EasyTouch>();
					}
				}
				return _instance;
			}
		}

		public static Gesture current => instance._currentGesture;

		public static event TouchCancelHandler On_Cancel;

		public static event Cancel2FingersHandler On_Cancel2Fingers;

		public static event TouchStartHandler On_TouchStart;

		public static event TouchDownHandler On_TouchDown;

		public static event TouchUpHandler On_TouchUp;

		public static event SimpleTapHandler On_SimpleTap;

		public static event DoubleTapHandler On_DoubleTap;

		public static event LongTapStartHandler On_LongTapStart;

		public static event LongTapHandler On_LongTap;

		public static event LongTapEndHandler On_LongTapEnd;

		public static event DragStartHandler On_DragStart;

		public static event DragHandler On_Drag;

		public static event DragEndHandler On_DragEnd;

		public static event SwipeStartHandler On_SwipeStart;

		public static event SwipeHandler On_Swipe;

		public static event SwipeEndHandler On_SwipeEnd;

		public static event TouchStart2FingersHandler On_TouchStart2Fingers;

		public static event TouchDown2FingersHandler On_TouchDown2Fingers;

		public static event TouchUp2FingersHandler On_TouchUp2Fingers;

		public static event SimpleTap2FingersHandler On_SimpleTap2Fingers;

		public static event DoubleTap2FingersHandler On_DoubleTap2Fingers;

		public static event LongTapStart2FingersHandler On_LongTapStart2Fingers;

		public static event LongTap2FingersHandler On_LongTap2Fingers;

		public static event LongTapEnd2FingersHandler On_LongTapEnd2Fingers;

		public static event TwistHandler On_Twist;

		public static event TwistEndHandler On_TwistEnd;

		public static event PinchHandler On_Pinch;

		public static event PinchInHandler On_PinchIn;

		public static event PinchOutHandler On_PinchOut;

		public static event PinchEndHandler On_PinchEnd;

		public static event DragStart2FingersHandler On_DragStart2Fingers;

		public static event Drag2FingersHandler On_Drag2Fingers;

		public static event DragEnd2FingersHandler On_DragEnd2Fingers;

		public static event SwipeStart2FingersHandler On_SwipeStart2Fingers;

		public static event Swipe2FingersHandler On_Swipe2Fingers;

		public static event SwipeEnd2FingersHandler On_SwipeEnd2Fingers;

		public static event EasyTouchIsReadyHandler On_EasyTouchIsReady;

		public static event OverUIElementHandler On_OverUIElement;

		public static event UIElementTouchUpHandler On_UIElementTouchUp;

		public EasyTouch()
		{
			enable = true;
			allowUIDetection = true;
			enableUIMode = true;
			autoUpdatePickedUI = false;
			enabledNGuiMode = false;
			nGUICameras = new List<Camera>();
			autoSelect = true;
			touchCameras = new List<ECamera>();
			pickableLayers3D = 1;
			enable2D = false;
			pickableLayers2D = 1;
			gesturePriority = GesturePriority.Tap;
			StationaryTolerance = 15f;
			longTapTime = 1f;
			doubleTapDetection = DoubleTapDetection.BySystem;
			doubleTapTime = 0.3f;
			swipeTolerance = 0.85f;
			alwaysSendSwipe = false;
			enable2FingersGesture = true;
			twoFingerPickMethod = TwoFingerPickMethod.Finger;
			enable2FingersSwipe = true;
			enablePinch = true;
			minPinchLength = 0f;
			enableTwist = true;
			minTwistAngle = 0f;
			enableSimulation = true;
			twistKey = KeyCode.LeftAlt;
			swipeKey = KeyCode.LeftControl;
		}

		private void OnEnable()
		{
			if (Application.isPlaying && Application.isEditor)
			{
				Init();
			}
		}

		private void Awake()
		{
			Init();
		}

		private void Start()
		{
			for (int i = 0; i < 100; i++)
			{
				singleDoubleTap[i] = new DoubleTap();
			}
			if (touchCameras.FindIndex((ECamera c) => c.camera == Camera.main) < 0)
			{
				touchCameras.Add(new ECamera(Camera.main, gui: false));
			}
			if (EasyTouch.On_EasyTouchIsReady != null)
			{
				EasyTouch.On_EasyTouchIsReady();
			}
			_currentGestures.Add(new Gesture());
		}

		private void Init()
		{
		}

		private void OnDrawGizmos()
		{
		}

		private void Update()
		{
			if (!enable || !(instance == this))
			{
				return;
			}
			if (Application.isPlaying && UnityEngine.Input.touchCount > 0)
			{
				enableRemote = true;
			}
			if (Application.isPlaying && UnityEngine.Input.touchCount == 0)
			{
				enableRemote = false;
			}
			int num = input.TouchCount();
			if (oldTouchCount == 2 && num != 2 && num > 0)
			{
				CreateGesture2Finger(EvtType.On_Cancel2Fingers, Vector2.zero, Vector2.zero, Vector2.zero, 0f, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, 0f);
			}
			UpdateTouches(realTouch: true, num);
			twoFinger.oldPickedObject = twoFinger.pickedObject;
			if (enable2FingersGesture && num == 2)
			{
				TwoFinger();
			}
			for (int i = 0; i < 100; i++)
			{
				if (fingers[i] != null)
				{
					OneFinger(i);
				}
			}
			oldTouchCount = num;
		}

		private void LateUpdate()
		{
			if (_currentGestures.Count > 1)
			{
				_currentGestures.RemoveAt(0);
			}
			else
			{
				_currentGestures[0] = null;
			}
			_currentGesture = _currentGestures[0];
		}

		private void UpdateTouches(bool realTouch, int touchCount)
		{
			fingers.CopyTo(tmpArray, 0);
			if (realTouch || enableRemote)
			{
				ResetTouches();
				for (int i = 0; i < touchCount; i++)
				{
					Touch touch = UnityEngine.Input.GetTouch(i);
					for (int j = 0; j < 100; j++)
					{
						if (fingers[i] != null)
						{
							break;
						}
						if (tmpArray[j] != null && tmpArray[j].fingerIndex == touch.fingerId)
						{
							fingers[i] = tmpArray[j];
						}
					}
					if (fingers[i] == null)
					{
						fingers[i] = new Finger();
						fingers[i].fingerIndex = touch.fingerId;
						fingers[i].gesture = GestureType.None;
						fingers[i].phase = TouchPhase.Began;
					}
					else
					{
						fingers[i].phase = touch.phase;
					}
					if (fingers[i].phase != 0)
					{
						fingers[i].deltaPosition = touch.position - fingers[i].position;
					}
					else
					{
						fingers[i].deltaPosition = Vector2.zero;
					}
					fingers[i].position = touch.position;
					fingers[i].tapCount = touch.tapCount;
					fingers[i].deltaTime = touch.deltaTime;
					fingers[i].touchCount = touchCount;
					fingers[i].altitudeAngle = touch.altitudeAngle;
					fingers[i].azimuthAngle = touch.azimuthAngle;
					fingers[i].maximumPossiblePressure = touch.maximumPossiblePressure;
					fingers[i].pressure = touch.pressure;
					fingers[i].radius = touch.radius;
					fingers[i].radiusVariance = touch.radiusVariance;
					fingers[i].touchType = touch.type;
				}
			}
			else
			{
				for (int k = 0; k < touchCount; k++)
				{
					fingers[k] = input.GetMouseTouch(k, fingers[k]);
					fingers[k].touchCount = touchCount;
				}
			}
		}

		private void ResetTouches()
		{
			for (int i = 0; i < 100; i++)
			{
				fingers[i] = null;
			}
		}

		private void OneFinger(int fingerIndex)
		{
			if (fingers[fingerIndex].gesture == GestureType.None)
			{
				if (!singleDoubleTap[fingerIndex].inDoubleTap)
				{
					singleDoubleTap[fingerIndex].inDoubleTap = true;
					singleDoubleTap[fingerIndex].time = 0f;
					singleDoubleTap[fingerIndex].count = 1;
				}
				fingers[fingerIndex].startTimeAction = Time.realtimeSinceStartup;
				fingers[fingerIndex].gesture = GestureType.Acquisition;
				fingers[fingerIndex].startPosition = fingers[fingerIndex].position;
				if (autoSelect && GetPickedGameObject(fingers[fingerIndex]))
				{
					fingers[fingerIndex].pickedObject = pickedObject.pickedObj;
					fingers[fingerIndex].isGuiCamera = pickedObject.isGUI;
					fingers[fingerIndex].pickedCamera = pickedObject.pickedCamera;
				}
				if (allowUIDetection)
				{
					fingers[fingerIndex].isOverGui = IsScreenPositionOverUI(fingers[fingerIndex].position);
					fingers[fingerIndex].pickedUIElement = GetFirstUIElementFromCache();
				}
				CreateGesture(fingerIndex, EvtType.On_TouchStart, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
			}
			if (singleDoubleTap[fingerIndex].inDoubleTap)
			{
				singleDoubleTap[fingerIndex].time += Time.deltaTime;
			}
			fingers[fingerIndex].actionTime = Time.realtimeSinceStartup - fingers[fingerIndex].startTimeAction;
			if (fingers[fingerIndex].phase == TouchPhase.Canceled)
			{
				fingers[fingerIndex].gesture = GestureType.Cancel;
			}
			if (fingers[fingerIndex].phase != TouchPhase.Ended && fingers[fingerIndex].phase != TouchPhase.Canceled)
			{
				if (fingers[fingerIndex].phase == TouchPhase.Stationary && fingers[fingerIndex].actionTime >= longTapTime && fingers[fingerIndex].gesture == GestureType.Acquisition)
				{
					fingers[fingerIndex].gesture = GestureType.LongTap;
					CreateGesture(fingerIndex, EvtType.On_LongTapStart, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
				}
				if (((fingers[fingerIndex].gesture == GestureType.Acquisition || fingers[fingerIndex].gesture == GestureType.LongTap) && fingers[fingerIndex].phase == TouchPhase.Moved && gesturePriority == GesturePriority.Slips) || ((fingers[fingerIndex].gesture == GestureType.Acquisition || fingers[fingerIndex].gesture == GestureType.LongTap) && !FingerInTolerance(fingers[fingerIndex]) && gesturePriority == GesturePriority.Tap))
				{
					if (fingers[fingerIndex].gesture == GestureType.LongTap)
					{
						fingers[fingerIndex].gesture = GestureType.Cancel;
						CreateGesture(fingerIndex, EvtType.On_LongTapEnd, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
						fingers[fingerIndex].gesture = GestureType.Acquisition;
					}
					else
					{
						fingers[fingerIndex].oldSwipeType = SwipeDirection.None;
						if ((bool)fingers[fingerIndex].pickedObject)
						{
							fingers[fingerIndex].gesture = GestureType.Drag;
							CreateGesture(fingerIndex, EvtType.On_DragStart, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
							if (alwaysSendSwipe)
							{
								CreateGesture(fingerIndex, EvtType.On_SwipeStart, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
							}
						}
						else
						{
							fingers[fingerIndex].gesture = GestureType.Swipe;
							CreateGesture(fingerIndex, EvtType.On_SwipeStart, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
						}
					}
				}
				EvtType evtType = EvtType.None;
				switch (fingers[fingerIndex].gesture)
				{
				case GestureType.LongTap:
					evtType = EvtType.On_LongTap;
					break;
				case GestureType.Drag:
					evtType = EvtType.On_Drag;
					break;
				case GestureType.Swipe:
					evtType = EvtType.On_Swipe;
					break;
				}
				SwipeDirection swipeDirection = SwipeDirection.None;
				swipeDirection = GetSwipe(new Vector2(0f, 0f), fingers[fingerIndex].deltaPosition);
				if (evtType != 0)
				{
					fingers[fingerIndex].oldSwipeType = swipeDirection;
					CreateGesture(fingerIndex, evtType, fingers[fingerIndex], swipeDirection, 0f, fingers[fingerIndex].deltaPosition);
					if (evtType == EvtType.On_Drag && alwaysSendSwipe)
					{
						CreateGesture(fingerIndex, EvtType.On_Swipe, fingers[fingerIndex], swipeDirection, 0f, fingers[fingerIndex].deltaPosition);
					}
				}
				CreateGesture(fingerIndex, EvtType.On_TouchDown, fingers[fingerIndex], swipeDirection, 0f, fingers[fingerIndex].deltaPosition);
				return;
			}
			switch (fingers[fingerIndex].gesture)
			{
			case GestureType.Acquisition:
				if (doubleTapDetection == DoubleTapDetection.BySystem)
				{
					if (FingerInTolerance(fingers[fingerIndex]))
					{
						if (fingers[fingerIndex].tapCount < 2)
						{
							CreateGesture(fingerIndex, EvtType.On_SimpleTap, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
						}
						else
						{
							CreateGesture(fingerIndex, EvtType.On_DoubleTap, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
						}
					}
				}
				else if (!singleDoubleTap[fingerIndex].inWait)
				{
					singleDoubleTap[fingerIndex].finger = fingers[fingerIndex];
					StartCoroutine(SingleOrDouble(fingerIndex));
				}
				else
				{
					singleDoubleTap[fingerIndex].count++;
				}
				break;
			case GestureType.LongTap:
				CreateGesture(fingerIndex, EvtType.On_LongTapEnd, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
				break;
			case GestureType.Drag:
				CreateGesture(fingerIndex, EvtType.On_DragEnd, fingers[fingerIndex], GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].startPosition - fingers[fingerIndex].position).magnitude, fingers[fingerIndex].position - fingers[fingerIndex].startPosition);
				if (alwaysSendSwipe)
				{
					CreateGesture(fingerIndex, EvtType.On_SwipeEnd, fingers[fingerIndex], GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].position - fingers[fingerIndex].startPosition).magnitude, fingers[fingerIndex].position - fingers[fingerIndex].startPosition);
				}
				break;
			case GestureType.Swipe:
				CreateGesture(fingerIndex, EvtType.On_SwipeEnd, fingers[fingerIndex], GetSwipe(fingers[fingerIndex].startPosition, fingers[fingerIndex].position), (fingers[fingerIndex].position - fingers[fingerIndex].startPosition).magnitude, fingers[fingerIndex].position - fingers[fingerIndex].startPosition);
				break;
			case GestureType.Cancel:
				CreateGesture(fingerIndex, EvtType.On_Cancel, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
				break;
			}
			CreateGesture(fingerIndex, EvtType.On_TouchUp, fingers[fingerIndex], SwipeDirection.None, 0f, Vector2.zero);
			fingers[fingerIndex] = null;
		}

		private IEnumerator SingleOrDouble(int fingerIndex)
		{
			singleDoubleTap[fingerIndex].inWait = true;
			float num = doubleTapTime - singleDoubleTap[fingerIndex].finger.actionTime;
			if (num < 0f)
			{
				num = 0f;
			}
			yield return new WaitForSeconds(num);
			if (singleDoubleTap[fingerIndex].count < 2)
			{
				CreateGesture(fingerIndex, EvtType.On_SimpleTap, singleDoubleTap[fingerIndex].finger, SwipeDirection.None, 0f, singleDoubleTap[fingerIndex].finger.deltaPosition);
			}
			else
			{
				CreateGesture(fingerIndex, EvtType.On_DoubleTap, singleDoubleTap[fingerIndex].finger, SwipeDirection.None, 0f, singleDoubleTap[fingerIndex].finger.deltaPosition);
			}
			singleDoubleTap[fingerIndex].Stop();
			StopCoroutine("SingleOrDouble");
		}

		private void CreateGesture(int touchIndex, EvtType message, Finger finger, SwipeDirection swipe, float swipeLength, Vector2 swipeVector)
		{
			bool flag = true;
			if (autoUpdatePickedUI && allowUIDetection)
			{
				finger.isOverGui = IsScreenPositionOverUI(finger.position);
				finger.pickedUIElement = GetFirstUIElementFromCache();
			}
			if (enabledNGuiMode && message == EvtType.On_TouchStart)
			{
				finger.isOverGui = (finger.isOverGui || IsTouchOverNGui(finger.position));
			}
			if (enableUIMode || enabledNGuiMode)
			{
				flag = !finger.isOverGui;
			}
			Gesture gesture = finger.GetGesture();
			if (autoUpdatePickedObject && autoSelect && message != EvtType.On_Drag && message != EvtType.On_DragEnd && message != EvtType.On_DragStart)
			{
				if (GetPickedGameObject(finger))
				{
					gesture.pickedObject = pickedObject.pickedObj;
					gesture.pickedCamera = pickedObject.pickedCamera;
					gesture.isGuiCamera = pickedObject.isGUI;
				}
				else
				{
					gesture.pickedObject = null;
					gesture.pickedCamera = null;
					gesture.isGuiCamera = false;
				}
			}
			gesture.swipe = swipe;
			gesture.swipeLength = swipeLength;
			gesture.swipeVector = swipeVector;
			gesture.deltaPinch = 0f;
			gesture.twistAngle = 0f;
			if (flag)
			{
				RaiseEvent(message, gesture);
			}
			else if (finger.isOverGui)
			{
				if (message == EvtType.On_TouchUp)
				{
					RaiseEvent(EvtType.On_UIElementTouchUp, gesture);
				}
				else
				{
					RaiseEvent(EvtType.On_OverUIElement, gesture);
				}
			}
		}

		private void TwoFinger()
		{
			bool flag = false;
			if (twoFinger.currentGesture == GestureType.None)
			{
				if (!singleDoubleTap[99].inDoubleTap)
				{
					singleDoubleTap[99].inDoubleTap = true;
					singleDoubleTap[99].time = 0f;
					singleDoubleTap[99].count = 1;
				}
				twoFinger.finger0 = GetTwoFinger(-1);
				twoFinger.finger1 = GetTwoFinger(twoFinger.finger0);
				twoFinger.startTimeAction = Time.realtimeSinceStartup;
				twoFinger.currentGesture = GestureType.Acquisition;
				fingers[twoFinger.finger0].startPosition = fingers[twoFinger.finger0].position;
				fingers[twoFinger.finger1].startPosition = fingers[twoFinger.finger1].position;
				fingers[twoFinger.finger0].oldPosition = fingers[twoFinger.finger0].position;
				fingers[twoFinger.finger1].oldPosition = fingers[twoFinger.finger1].position;
				twoFinger.oldFingerDistance = Mathf.Abs(Vector2.Distance(fingers[twoFinger.finger0].position, fingers[twoFinger.finger1].position));
				twoFinger.startPosition = new Vector2((fingers[twoFinger.finger0].position.x + fingers[twoFinger.finger1].position.x) / 2f, (fingers[twoFinger.finger0].position.y + fingers[twoFinger.finger1].position.y) / 2f);
				twoFinger.position = twoFinger.startPosition;
				twoFinger.oldStartPosition = twoFinger.startPosition;
				twoFinger.deltaPosition = Vector2.zero;
				twoFinger.startDistance = twoFinger.oldFingerDistance;
				if (autoSelect)
				{
					if (GetTwoFingerPickedObject())
					{
						twoFinger.pickedObject = pickedObject.pickedObj;
						twoFinger.pickedCamera = pickedObject.pickedCamera;
						twoFinger.isGuiCamera = pickedObject.isGUI;
					}
					else
					{
						twoFinger.ClearPickedObjectData();
					}
				}
				if (allowUIDetection)
				{
					if (GetTwoFingerPickedUIElement())
					{
						twoFinger.pickedUIElement = pickedObject.pickedObj;
						twoFinger.isOverGui = true;
					}
					else
					{
						twoFinger.ClearPickedUIData();
					}
				}
				CreateGesture2Finger(EvtType.On_TouchStart2Fingers, twoFinger.startPosition, twoFinger.startPosition, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.oldFingerDistance);
			}
			if (singleDoubleTap[99].inDoubleTap)
			{
				singleDoubleTap[99].time += Time.deltaTime;
			}
			twoFinger.timeSinceStartAction = Time.realtimeSinceStartup - twoFinger.startTimeAction;
			twoFinger.position = new Vector2((fingers[twoFinger.finger0].position.x + fingers[twoFinger.finger1].position.x) / 2f, (fingers[twoFinger.finger0].position.y + fingers[twoFinger.finger1].position.y) / 2f);
			twoFinger.deltaPosition = twoFinger.position - twoFinger.oldStartPosition;
			twoFinger.fingerDistance = Mathf.Abs(Vector2.Distance(fingers[twoFinger.finger0].position, fingers[twoFinger.finger1].position));
			if (fingers[twoFinger.finger0].phase == TouchPhase.Canceled || fingers[twoFinger.finger1].phase == TouchPhase.Canceled)
			{
				twoFinger.currentGesture = GestureType.Cancel;
			}
			if (fingers[twoFinger.finger0].phase != TouchPhase.Ended && fingers[twoFinger.finger1].phase != TouchPhase.Ended && twoFinger.currentGesture != GestureType.Cancel)
			{
				if (twoFinger.currentGesture == GestureType.Acquisition && twoFinger.timeSinceStartAction >= longTapTime && FingerInTolerance(fingers[twoFinger.finger0]) && FingerInTolerance(fingers[twoFinger.finger1]))
				{
					twoFinger.currentGesture = GestureType.LongTap;
					CreateGesture2Finger(EvtType.On_LongTapStart2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.fingerDistance);
				}
				if (((!FingerInTolerance(fingers[twoFinger.finger0]) || !FingerInTolerance(fingers[twoFinger.finger1])) && gesturePriority == GesturePriority.Tap) || ((fingers[twoFinger.finger0].phase == TouchPhase.Moved || fingers[twoFinger.finger1].phase == TouchPhase.Moved) && gesturePriority == GesturePriority.Slips))
				{
					flag = true;
				}
				if (flag && twoFinger.currentGesture != 0)
				{
					Vector2 currentDistance = fingers[twoFinger.finger0].position - fingers[twoFinger.finger1].position;
					Vector2 previousDistance = fingers[twoFinger.finger0].oldPosition - fingers[twoFinger.finger1].oldPosition;
					float currentDelta = currentDistance.magnitude - previousDistance.magnitude;
					if (enable2FingersSwipe)
					{
						float num = Vector2.Dot(fingers[twoFinger.finger0].deltaPosition.normalized, fingers[twoFinger.finger1].deltaPosition.normalized);
						if (num > 0f)
						{
							if (twoFinger.oldGesture == GestureType.LongTap)
							{
								CreateStateEnd2Fingers(twoFinger.currentGesture, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, realEnd: false, twoFinger.fingerDistance);
								twoFinger.startTimeAction = Time.realtimeSinceStartup;
							}
							if ((bool)twoFinger.pickedObject && !twoFinger.dragStart && !alwaysSendSwipe)
							{
								twoFinger.currentGesture = GestureType.Drag;
								CreateGesture2Finger(EvtType.On_DragStart2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.fingerDistance);
								CreateGesture2Finger(EvtType.On_SwipeStart2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.fingerDistance);
								twoFinger.dragStart = true;
							}
							else if (!twoFinger.pickedObject && !twoFinger.swipeStart)
							{
								twoFinger.currentGesture = GestureType.Swipe;
								CreateGesture2Finger(EvtType.On_SwipeStart2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.fingerDistance);
								twoFinger.swipeStart = true;
							}
						}
						else if (num < 0f)
						{
							twoFinger.dragStart = false;
							twoFinger.swipeStart = false;
						}
						if (twoFinger.dragStart)
						{
							CreateGesture2Finger(EvtType.On_Drag2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition, twoFinger.position), 0f, twoFinger.deltaPosition, 0f, 0f, twoFinger.fingerDistance);
							CreateGesture2Finger(EvtType.On_Swipe2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition, twoFinger.position), 0f, twoFinger.deltaPosition, 0f, 0f, twoFinger.fingerDistance);
						}
						if (twoFinger.swipeStart)
						{
							CreateGesture2Finger(EvtType.On_Swipe2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition, twoFinger.position), 0f, twoFinger.deltaPosition, 0f, 0f, twoFinger.fingerDistance);
						}
					}
					DetectPinch(currentDelta);
					DetecTwist(previousDistance, currentDistance, currentDelta);
				}
				else if (twoFinger.currentGesture == GestureType.LongTap)
				{
					CreateGesture2Finger(EvtType.On_LongTap2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.fingerDistance);
				}
				CreateGesture2Finger(EvtType.On_TouchDown2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, GetSwipe(twoFinger.oldStartPosition, twoFinger.position), 0f, twoFinger.deltaPosition, 0f, 0f, twoFinger.fingerDistance);
				fingers[twoFinger.finger0].oldPosition = fingers[twoFinger.finger0].position;
				fingers[twoFinger.finger1].oldPosition = fingers[twoFinger.finger1].position;
				twoFinger.oldFingerDistance = twoFinger.fingerDistance;
				twoFinger.oldStartPosition = twoFinger.position;
				twoFinger.oldGesture = twoFinger.currentGesture;
			}
			else if (twoFinger.currentGesture != GestureType.Acquisition && twoFinger.currentGesture != 0)
			{
				CreateStateEnd2Fingers(twoFinger.currentGesture, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, realEnd: true, twoFinger.fingerDistance);
				twoFinger.currentGesture = GestureType.None;
				twoFinger.pickedObject = null;
				twoFinger.swipeStart = false;
				twoFinger.dragStart = false;
			}
			else
			{
				twoFinger.currentGesture = GestureType.Tap;
				CreateStateEnd2Fingers(twoFinger.currentGesture, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, realEnd: true, twoFinger.fingerDistance);
			}
		}

		private void DetectPinch(float currentDelta)
		{
			if (!enablePinch)
			{
				return;
			}
			if ((Mathf.Abs(twoFinger.fingerDistance - twoFinger.startDistance) >= minPinchLength && twoFinger.currentGesture != GestureType.Pinch) || twoFinger.currentGesture == GestureType.Pinch)
			{
				if (currentDelta != 0f && twoFinger.oldGesture == GestureType.LongTap)
				{
					CreateStateEnd2Fingers(twoFinger.currentGesture, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, realEnd: false, twoFinger.fingerDistance);
					twoFinger.startTimeAction = Time.realtimeSinceStartup;
				}
				twoFinger.currentGesture = GestureType.Pinch;
				if (currentDelta > 0f)
				{
					CreateGesture2Finger(EvtType.On_PinchOut, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, GetSwipe(twoFinger.startPosition, twoFinger.position), 0f, Vector2.zero, 0f, Mathf.Abs(twoFinger.fingerDistance - twoFinger.oldFingerDistance), twoFinger.fingerDistance);
				}
				if (currentDelta < 0f)
				{
					CreateGesture2Finger(EvtType.On_PinchIn, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, GetSwipe(twoFinger.startPosition, twoFinger.position), 0f, Vector2.zero, 0f, Mathf.Abs(twoFinger.fingerDistance - twoFinger.oldFingerDistance), twoFinger.fingerDistance);
				}
				if (currentDelta < 0f || currentDelta > 0f)
				{
					CreateGesture2Finger(EvtType.On_Pinch, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, GetSwipe(twoFinger.startPosition, twoFinger.position), 0f, Vector2.zero, 0f, currentDelta, twoFinger.fingerDistance);
				}
			}
			twoFinger.lastPinch = ((currentDelta > 0f) ? currentDelta : twoFinger.lastPinch);
		}

		private void DetecTwist(Vector2 previousDistance, Vector2 currentDistance, float currentDelta)
		{
			if (!enableTwist)
			{
				return;
			}
			float num = Vector2.Angle(previousDistance, currentDistance);
			if (previousDistance == currentDistance)
			{
				num = 0f;
			}
			if ((Mathf.Abs(num) >= minTwistAngle && twoFinger.currentGesture != GestureType.Twist) || twoFinger.currentGesture == GestureType.Twist)
			{
				if (twoFinger.oldGesture == GestureType.LongTap)
				{
					CreateStateEnd2Fingers(twoFinger.currentGesture, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, realEnd: false, twoFinger.fingerDistance);
					twoFinger.startTimeAction = Time.realtimeSinceStartup;
				}
				twoFinger.currentGesture = GestureType.Twist;
				if (num != 0f)
				{
					num *= Mathf.Sign(Vector3.Cross(previousDistance, currentDistance).z);
				}
				CreateGesture2Finger(EvtType.On_Twist, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, num, 0f, twoFinger.fingerDistance);
			}
			twoFinger.lastTwistAngle = ((num != 0f) ? num : twoFinger.lastTwistAngle);
		}

		private void CreateStateEnd2Fingers(GestureType gesture, Vector2 startPosition, Vector2 position, Vector2 deltaPosition, float time, bool realEnd, float fingerDistance, float twist = 0f, float pinch = 0f)
		{
			switch (gesture)
			{
			case GestureType.Tap:
			case GestureType.Acquisition:
				if (doubleTapDetection == DoubleTapDetection.BySystem)
				{
					if (fingers[twoFinger.finger0].tapCount < 2 && fingers[twoFinger.finger1].tapCount < 2)
					{
						CreateGesture2Finger(EvtType.On_SimpleTap2Fingers, startPosition, position, deltaPosition, time, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
					}
					else
					{
						CreateGesture2Finger(EvtType.On_DoubleTap2Fingers, startPosition, position, deltaPosition, time, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
					}
					twoFinger.currentGesture = GestureType.None;
					twoFinger.pickedObject = null;
					twoFinger.swipeStart = false;
					twoFinger.dragStart = false;
					singleDoubleTap[99].Stop();
					StopCoroutine("SingleOrDouble2Fingers");
				}
				else if (!singleDoubleTap[99].inWait)
				{
					StartCoroutine("SingleOrDouble2Fingers");
				}
				else
				{
					singleDoubleTap[99].count++;
				}
				break;
			case GestureType.LongTap:
				CreateGesture2Finger(EvtType.On_LongTapEnd2Fingers, startPosition, position, deltaPosition, time, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
				break;
			case GestureType.Pinch:
				CreateGesture2Finger(EvtType.On_PinchEnd, startPosition, position, deltaPosition, time, SwipeDirection.None, 0f, Vector2.zero, 0f, twoFinger.lastPinch, fingerDistance);
				break;
			case GestureType.Twist:
				CreateGesture2Finger(EvtType.On_TwistEnd, startPosition, position, deltaPosition, time, SwipeDirection.None, 0f, Vector2.zero, twoFinger.lastTwistAngle, 0f, fingerDistance);
				break;
			}
			if (realEnd)
			{
				if (twoFinger.dragStart)
				{
					CreateGesture2Finger(EvtType.On_DragEnd2Fingers, startPosition, position, deltaPosition, time, GetSwipe(startPosition, position), (position - startPosition).magnitude, position - startPosition, 0f, 0f, fingerDistance);
				}
				if (twoFinger.swipeStart)
				{
					CreateGesture2Finger(EvtType.On_SwipeEnd2Fingers, startPosition, position, deltaPosition, time, GetSwipe(startPosition, position), (position - startPosition).magnitude, position - startPosition, 0f, 0f, fingerDistance);
				}
				CreateGesture2Finger(EvtType.On_TouchUp2Fingers, startPosition, position, deltaPosition, time, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, fingerDistance);
			}
		}

		private IEnumerator SingleOrDouble2Fingers()
		{
			singleDoubleTap[99].inWait = true;
			yield return new WaitForSeconds(doubleTapTime);
			if (singleDoubleTap[99].count < 2)
			{
				CreateGesture2Finger(EvtType.On_SimpleTap2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.fingerDistance);
			}
			else
			{
				CreateGesture2Finger(EvtType.On_DoubleTap2Fingers, twoFinger.startPosition, twoFinger.position, twoFinger.deltaPosition, twoFinger.timeSinceStartAction, SwipeDirection.None, 0f, Vector2.zero, 0f, 0f, twoFinger.fingerDistance);
			}
			twoFinger.currentGesture = GestureType.None;
			twoFinger.pickedObject = null;
			twoFinger.swipeStart = false;
			twoFinger.dragStart = false;
			singleDoubleTap[99].Stop();
			StopCoroutine("SingleOrDouble2Fingers");
		}

		private void CreateGesture2Finger(EvtType message, Vector2 startPosition, Vector2 position, Vector2 deltaPosition, float actionTime, SwipeDirection swipe, float swipeLength, Vector2 swipeVector, float twist, float pinch, float twoDistance)
		{
			bool flag = true;
			Gesture gesture = new Gesture();
			gesture.isOverGui = false;
			if (enabledNGuiMode && message == EvtType.On_TouchStart2Fingers)
			{
				gesture.isOverGui = (gesture.isOverGui || (IsTouchOverNGui(twoFinger.position) && IsTouchOverNGui(twoFinger.position)));
			}
			gesture.touchCount = 2;
			gesture.fingerIndex = -1;
			gesture.startPosition = startPosition;
			gesture.position = position;
			gesture.deltaPosition = deltaPosition;
			gesture.actionTime = actionTime;
			gesture.deltaTime = Time.deltaTime;
			gesture.swipe = swipe;
			gesture.swipeLength = swipeLength;
			gesture.swipeVector = swipeVector;
			gesture.deltaPinch = pinch;
			gesture.twistAngle = twist;
			gesture.twoFingerDistance = twoDistance;
			gesture.pickedObject = twoFinger.pickedObject;
			gesture.pickedCamera = twoFinger.pickedCamera;
			gesture.isGuiCamera = twoFinger.isGuiCamera;
			if (autoUpdatePickedObject && message != EvtType.On_Drag && message != EvtType.On_DragEnd && message != EvtType.On_Twist && message != EvtType.On_TwistEnd && message != EvtType.On_Pinch && message != EvtType.On_PinchEnd && message != EvtType.On_PinchIn && message != EvtType.On_PinchOut)
			{
				if (GetTwoFingerPickedObject())
				{
					gesture.pickedObject = pickedObject.pickedObj;
					gesture.pickedCamera = pickedObject.pickedCamera;
					gesture.isGuiCamera = pickedObject.isGUI;
				}
				else
				{
					twoFinger.ClearPickedObjectData();
				}
			}
			gesture.pickedUIElement = twoFinger.pickedUIElement;
			gesture.isOverGui = twoFinger.isOverGui;
			if (allowUIDetection && autoUpdatePickedUI && message != EvtType.On_Drag && message != EvtType.On_DragEnd && message != EvtType.On_Twist && message != EvtType.On_TwistEnd && message != EvtType.On_Pinch && message != EvtType.On_PinchEnd && message != EvtType.On_PinchIn && message != EvtType.On_PinchOut && message == EvtType.On_SimpleTap2Fingers)
			{
				if (GetTwoFingerPickedUIElement())
				{
					gesture.pickedUIElement = pickedObject.pickedObj;
					gesture.isOverGui = true;
				}
				else
				{
					twoFinger.ClearPickedUIData();
				}
			}
			if (enableUIMode || (enabledNGuiMode && allowUIDetection))
			{
				flag = !gesture.isOverGui;
			}
			if (flag)
			{
				RaiseEvent(message, gesture);
			}
			else if (gesture.isOverGui)
			{
				if (message == EvtType.On_TouchUp2Fingers)
				{
					RaiseEvent(EvtType.On_UIElementTouchUp, gesture);
				}
				else
				{
					RaiseEvent(EvtType.On_OverUIElement, gesture);
				}
			}
		}

		private int GetTwoFinger(int index)
		{
			int i = index + 1;
			bool flag = false;
			for (; i < 10; i++)
			{
				if (flag)
				{
					break;
				}
				if (fingers[i] != null && i >= index)
				{
					flag = true;
				}
			}
			return i - 1;
		}

		private bool GetTwoFingerPickedObject()
		{
			bool result = false;
			if (twoFingerPickMethod == TwoFingerPickMethod.Finger)
			{
				if (GetPickedGameObject(fingers[twoFinger.finger0]))
				{
					GameObject pickedObj = pickedObject.pickedObj;
					if (GetPickedGameObject(fingers[twoFinger.finger1]) && pickedObj == pickedObject.pickedObj)
					{
						result = true;
					}
				}
			}
			else if (GetPickedGameObject(fingers[twoFinger.finger0], isTowFinger: true))
			{
				result = true;
			}
			return result;
		}

		private bool GetTwoFingerPickedUIElement()
		{
			bool result = false;
			if (fingers[twoFinger.finger0] == null)
			{
				return false;
			}
			if (twoFingerPickMethod == TwoFingerPickMethod.Finger)
			{
				if (IsScreenPositionOverUI(fingers[twoFinger.finger0].position))
				{
					GameObject firstUIElementFromCache = GetFirstUIElementFromCache();
					if (IsScreenPositionOverUI(fingers[twoFinger.finger1].position))
					{
						GameObject firstUIElementFromCache2 = GetFirstUIElementFromCache();
						if (firstUIElementFromCache2 == firstUIElementFromCache || firstUIElementFromCache2.transform.IsChildOf(firstUIElementFromCache.transform) || firstUIElementFromCache.transform.IsChildOf(firstUIElementFromCache2.transform))
						{
							pickedObject.pickedObj = firstUIElementFromCache;
							pickedObject.isGUI = true;
							result = true;
						}
					}
				}
			}
			else if (IsScreenPositionOverUI(twoFinger.position))
			{
				pickedObject.pickedObj = GetFirstUIElementFromCache();
				pickedObject.isGUI = true;
				result = true;
			}
			return result;
		}

		private void RaiseEvent(EvtType evnt, Gesture gesture)
		{
			gesture.type = evnt;
			switch (evnt)
			{
			case EvtType.On_Cancel:
				if (EasyTouch.On_Cancel != null)
				{
					EasyTouch.On_Cancel(gesture);
				}
				break;
			case EvtType.On_Cancel2Fingers:
				if (EasyTouch.On_Cancel2Fingers != null)
				{
					EasyTouch.On_Cancel2Fingers(gesture);
				}
				break;
			case EvtType.On_TouchStart:
				if (EasyTouch.On_TouchStart != null)
				{
					EasyTouch.On_TouchStart(gesture);
				}
				break;
			case EvtType.On_TouchDown:
				if (EasyTouch.On_TouchDown != null)
				{
					EasyTouch.On_TouchDown(gesture);
				}
				break;
			case EvtType.On_TouchUp:
				if (EasyTouch.On_TouchUp != null)
				{
					EasyTouch.On_TouchUp(gesture);
				}
				break;
			case EvtType.On_SimpleTap:
				if (EasyTouch.On_SimpleTap != null)
				{
					EasyTouch.On_SimpleTap(gesture);
				}
				break;
			case EvtType.On_DoubleTap:
				if (EasyTouch.On_DoubleTap != null)
				{
					EasyTouch.On_DoubleTap(gesture);
				}
				break;
			case EvtType.On_LongTapStart:
				if (EasyTouch.On_LongTapStart != null)
				{
					EasyTouch.On_LongTapStart(gesture);
				}
				break;
			case EvtType.On_LongTap:
				if (EasyTouch.On_LongTap != null)
				{
					EasyTouch.On_LongTap(gesture);
				}
				break;
			case EvtType.On_LongTapEnd:
				if (EasyTouch.On_LongTapEnd != null)
				{
					EasyTouch.On_LongTapEnd(gesture);
				}
				break;
			case EvtType.On_DragStart:
				if (EasyTouch.On_DragStart != null)
				{
					EasyTouch.On_DragStart(gesture);
				}
				break;
			case EvtType.On_Drag:
				if (EasyTouch.On_Drag != null)
				{
					EasyTouch.On_Drag(gesture);
				}
				break;
			case EvtType.On_DragEnd:
				if (EasyTouch.On_DragEnd != null)
				{
					EasyTouch.On_DragEnd(gesture);
				}
				break;
			case EvtType.On_SwipeStart:
				if (EasyTouch.On_SwipeStart != null)
				{
					EasyTouch.On_SwipeStart(gesture);
				}
				break;
			case EvtType.On_Swipe:
				if (EasyTouch.On_Swipe != null)
				{
					EasyTouch.On_Swipe(gesture);
				}
				break;
			case EvtType.On_SwipeEnd:
				if (EasyTouch.On_SwipeEnd != null)
				{
					EasyTouch.On_SwipeEnd(gesture);
				}
				break;
			case EvtType.On_TouchStart2Fingers:
				if (EasyTouch.On_TouchStart2Fingers != null)
				{
					EasyTouch.On_TouchStart2Fingers(gesture);
				}
				break;
			case EvtType.On_TouchDown2Fingers:
				if (EasyTouch.On_TouchDown2Fingers != null)
				{
					EasyTouch.On_TouchDown2Fingers(gesture);
				}
				break;
			case EvtType.On_TouchUp2Fingers:
				if (EasyTouch.On_TouchUp2Fingers != null)
				{
					EasyTouch.On_TouchUp2Fingers(gesture);
				}
				break;
			case EvtType.On_SimpleTap2Fingers:
				if (EasyTouch.On_SimpleTap2Fingers != null)
				{
					EasyTouch.On_SimpleTap2Fingers(gesture);
				}
				break;
			case EvtType.On_DoubleTap2Fingers:
				if (EasyTouch.On_DoubleTap2Fingers != null)
				{
					EasyTouch.On_DoubleTap2Fingers(gesture);
				}
				break;
			case EvtType.On_LongTapStart2Fingers:
				if (EasyTouch.On_LongTapStart2Fingers != null)
				{
					EasyTouch.On_LongTapStart2Fingers(gesture);
				}
				break;
			case EvtType.On_LongTap2Fingers:
				if (EasyTouch.On_LongTap2Fingers != null)
				{
					EasyTouch.On_LongTap2Fingers(gesture);
				}
				break;
			case EvtType.On_LongTapEnd2Fingers:
				if (EasyTouch.On_LongTapEnd2Fingers != null)
				{
					EasyTouch.On_LongTapEnd2Fingers(gesture);
				}
				break;
			case EvtType.On_Twist:
				if (EasyTouch.On_Twist != null)
				{
					EasyTouch.On_Twist(gesture);
				}
				break;
			case EvtType.On_TwistEnd:
				if (EasyTouch.On_TwistEnd != null)
				{
					EasyTouch.On_TwistEnd(gesture);
				}
				break;
			case EvtType.On_Pinch:
				if (EasyTouch.On_Pinch != null)
				{
					EasyTouch.On_Pinch(gesture);
				}
				break;
			case EvtType.On_PinchIn:
				if (EasyTouch.On_PinchIn != null)
				{
					EasyTouch.On_PinchIn(gesture);
				}
				break;
			case EvtType.On_PinchOut:
				if (EasyTouch.On_PinchOut != null)
				{
					EasyTouch.On_PinchOut(gesture);
				}
				break;
			case EvtType.On_PinchEnd:
				if (EasyTouch.On_PinchEnd != null)
				{
					EasyTouch.On_PinchEnd(gesture);
				}
				break;
			case EvtType.On_DragStart2Fingers:
				if (EasyTouch.On_DragStart2Fingers != null)
				{
					EasyTouch.On_DragStart2Fingers(gesture);
				}
				break;
			case EvtType.On_Drag2Fingers:
				if (EasyTouch.On_Drag2Fingers != null)
				{
					EasyTouch.On_Drag2Fingers(gesture);
				}
				break;
			case EvtType.On_DragEnd2Fingers:
				if (EasyTouch.On_DragEnd2Fingers != null)
				{
					EasyTouch.On_DragEnd2Fingers(gesture);
				}
				break;
			case EvtType.On_SwipeStart2Fingers:
				if (EasyTouch.On_SwipeStart2Fingers != null)
				{
					EasyTouch.On_SwipeStart2Fingers(gesture);
				}
				break;
			case EvtType.On_Swipe2Fingers:
				if (EasyTouch.On_Swipe2Fingers != null)
				{
					EasyTouch.On_Swipe2Fingers(gesture);
				}
				break;
			case EvtType.On_SwipeEnd2Fingers:
				if (EasyTouch.On_SwipeEnd2Fingers != null)
				{
					EasyTouch.On_SwipeEnd2Fingers(gesture);
				}
				break;
			case EvtType.On_OverUIElement:
				if (EasyTouch.On_OverUIElement != null)
				{
					EasyTouch.On_OverUIElement(gesture);
				}
				break;
			case EvtType.On_UIElementTouchUp:
				if (EasyTouch.On_UIElementTouchUp != null)
				{
					EasyTouch.On_UIElementTouchUp(gesture);
				}
				break;
			}
			int num = _currentGestures.FindIndex((Gesture obj) => obj != null && obj.type == gesture.type && obj.fingerIndex == gesture.fingerIndex);
			if (num > -1)
			{
				_currentGestures[num].touchCount = gesture.touchCount;
				_currentGestures[num].position = gesture.position;
				_currentGestures[num].actionTime = gesture.actionTime;
				_currentGestures[num].pickedCamera = gesture.pickedCamera;
				_currentGestures[num].pickedObject = gesture.pickedObject;
				_currentGestures[num].pickedUIElement = gesture.pickedUIElement;
				_currentGestures[num].isOverGui = gesture.isOverGui;
				_currentGestures[num].isGuiCamera = gesture.isGuiCamera;
				_currentGestures[num].deltaPinch += gesture.deltaPinch;
				_currentGestures[num].deltaPosition += gesture.deltaPosition;
				_currentGestures[num].deltaTime += gesture.deltaTime;
				_currentGestures[num].twistAngle += gesture.twistAngle;
			}
			if (num == -1)
			{
				_currentGestures.Add((Gesture)gesture.Clone());
				if (_currentGestures.Count > 0)
				{
					_currentGesture = _currentGestures[0];
				}
			}
		}

		private bool GetPickedGameObject(Finger finger, bool isTowFinger = false)
		{
			if (finger == null && !isTowFinger)
			{
				return false;
			}
			pickedObject.isGUI = false;
			pickedObject.pickedObj = null;
			pickedObject.pickedCamera = null;
			if (touchCameras.Count > 0)
			{
				for (int i = 0; i < touchCameras.Count; i++)
				{
					if (touchCameras[i].camera != null && touchCameras[i].camera.enabled)
					{
						Vector2 zero = Vector2.zero;
						zero = (isTowFinger ? twoFinger.position : finger.position);
						if (GetGameObjectAt(zero, touchCameras[i].camera, touchCameras[i].guiCamera))
						{
							return true;
						}
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning("No camera is assigned to EasyTouch");
			}
			return false;
		}

		private bool GetGameObjectAt(Vector2 position, Camera cam, bool isGuiCam)
		{
			Ray ray = cam.ScreenPointToRay(position);
			if (enable2D)
			{
				LayerMask mask = pickableLayers2D;
				RaycastHit2D[] array = new RaycastHit2D[1];
				if (Physics2D.GetRayIntersectionNonAlloc(ray, array, float.PositiveInfinity, mask) > 0)
				{
					pickedObject.pickedCamera = cam;
					pickedObject.isGUI = isGuiCam;
					pickedObject.pickedObj = array[0].collider.gameObject;
					return true;
				}
			}
			LayerMask mask2 = pickableLayers3D;
			if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, mask2))
			{
				pickedObject.pickedCamera = cam;
				pickedObject.isGUI = isGuiCam;
				pickedObject.pickedObj = hitInfo.collider.gameObject;
				return true;
			}
			return false;
		}

		private SwipeDirection GetSwipe(Vector2 start, Vector2 end)
		{
			Vector2 normalized = (end - start).normalized;
			if (Vector2.Dot(normalized, Vector2.up) >= swipeTolerance)
			{
				return SwipeDirection.Up;
			}
			if (Vector2.Dot(normalized, -Vector2.up) >= swipeTolerance)
			{
				return SwipeDirection.Down;
			}
			if (Vector2.Dot(normalized, Vector2.right) >= swipeTolerance)
			{
				return SwipeDirection.Right;
			}
			if (Vector2.Dot(normalized, -Vector2.right) >= swipeTolerance)
			{
				return SwipeDirection.Left;
			}
			if (Vector2.Dot(normalized, new Vector2(0.5f, 0.5f).normalized) >= swipeTolerance)
			{
				return SwipeDirection.UpRight;
			}
			if (Vector2.Dot(normalized, new Vector2(0.5f, -0.5f).normalized) >= swipeTolerance)
			{
				return SwipeDirection.DownRight;
			}
			if (Vector2.Dot(normalized, new Vector2(-0.5f, 0.5f).normalized) >= swipeTolerance)
			{
				return SwipeDirection.UpLeft;
			}
			if (Vector2.Dot(normalized, new Vector2(-0.5f, -0.5f).normalized) >= swipeTolerance)
			{
				return SwipeDirection.DownLeft;
			}
			return SwipeDirection.Other;
		}

		private bool FingerInTolerance(Finger finger)
		{
			if ((finger.position - finger.startPosition).sqrMagnitude <= StationaryTolerance * StationaryTolerance)
			{
				return true;
			}
			return false;
		}

		private bool IsTouchOverNGui(Vector2 position, bool isTwoFingers = false)
		{
			bool flag = false;
			if (enabledNGuiMode)
			{
				LayerMask mask = nGUILayers;
				int num = 0;
				while (!flag && num < nGUICameras.Count)
				{
					Vector2 zero = Vector2.zero;
					zero = (isTwoFingers ? twoFinger.position : position);
					flag = Physics.Raycast(nGUICameras[num].ScreenPointToRay(zero), out RaycastHit _, float.MaxValue, mask);
					num++;
				}
			}
			return flag;
		}

		private Finger GetFinger(int finderId)
		{
			int i = 0;
			Finger finger = null;
			for (; i < 10; i++)
			{
				if (finger != null)
				{
					break;
				}
				if (fingers[i] != null && fingers[i].fingerIndex == finderId)
				{
					finger = fingers[i];
				}
			}
			return finger;
		}

		private bool IsScreenPositionOverUI(Vector2 position)
		{
			uiEventSystem = EventSystem.current;
			if (uiEventSystem != null)
			{
				uiPointerEventData = new PointerEventData(uiEventSystem);
				uiPointerEventData.position = position;
				uiEventSystem.RaycastAll(uiPointerEventData, uiRaycastResultCache);
				if (uiRaycastResultCache.Count > 0)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private GameObject GetFirstUIElementFromCache()
		{
			if (uiRaycastResultCache.Count > 0)
			{
				return uiRaycastResultCache[0].gameObject;
			}
			return null;
		}

		private GameObject GetFirstUIElement(Vector2 position)
		{
			if (IsScreenPositionOverUI(position))
			{
				return GetFirstUIElementFromCache();
			}
			return null;
		}

		public static bool IsFingerOverUIElement(int fingerIndex)
		{
			if (instance != null)
			{
				Finger finger = instance.GetFinger(fingerIndex);
				if (finger != null)
				{
					return instance.IsScreenPositionOverUI(finger.position);
				}
				return false;
			}
			return false;
		}

		public static GameObject GetCurrentPickedUIElement(int fingerIndex, bool isTwoFinger)
		{
			if (instance != null)
			{
				Finger finger = instance.GetFinger(fingerIndex);
				if ((finger != null) | isTwoFinger)
				{
					Vector2 zero = Vector2.zero;
					zero = (isTwoFinger ? instance.twoFinger.position : finger.position);
					return instance.GetFirstUIElement(zero);
				}
				return null;
			}
			return null;
		}

		public static GameObject GetCurrentPickedObject(int fingerIndex, bool isTwoFinger)
		{
			if (instance != null)
			{
				Finger finger = instance.GetFinger(fingerIndex);
				if (((finger != null) | isTwoFinger) && instance.GetPickedGameObject(finger, isTwoFinger))
				{
					return instance.pickedObject.pickedObj;
				}
				return null;
			}
			return null;
		}

		public static GameObject GetGameObjectAt(Vector2 position, bool isTwoFinger = false)
		{
			if (instance != null)
			{
				if (isTwoFinger)
				{
					position = instance.twoFinger.position;
				}
				if (instance.touchCameras.Count > 0)
				{
					for (int i = 0; i < instance.touchCameras.Count; i++)
					{
						if (instance.touchCameras[i].camera != null && instance.touchCameras[i].camera.enabled)
						{
							if (instance.GetGameObjectAt(position, instance.touchCameras[i].camera, instance.touchCameras[i].guiCamera))
							{
								return instance.pickedObject.pickedObj;
							}
							return null;
						}
					}
				}
			}
			return null;
		}

		public static int GetTouchCount()
		{
			if ((bool)instance)
			{
				return instance.input.TouchCount();
			}
			return 0;
		}

		public static void ResetTouch(int fingerIndex)
		{
			if ((bool)instance)
			{
				instance.GetFinger(fingerIndex).gesture = GestureType.None;
			}
		}

		public static void SetEnabled(bool enable)
		{
			instance.enable = enable;
			if (enable)
			{
				instance.ResetTouches();
			}
		}

		public static bool GetEnabled()
		{
			if ((bool)instance)
			{
				return instance.enable;
			}
			return false;
		}

		public static void SetEnableUIDetection(bool enable)
		{
			if (instance != null)
			{
				instance.allowUIDetection = enable;
			}
		}

		public static bool GetEnableUIDetection()
		{
			if ((bool)instance)
			{
				return instance.allowUIDetection;
			}
			return false;
		}

		public static void SetUICompatibily(bool value)
		{
			if (instance != null)
			{
				instance.enableUIMode = value;
			}
		}

		public static bool GetUIComptability()
		{
			if (instance != null)
			{
				return instance.enableUIMode;
			}
			return false;
		}

		public static void SetAutoUpdateUI(bool value)
		{
			if ((bool)instance)
			{
				instance.autoUpdatePickedUI = value;
			}
		}

		public static bool GetAutoUpdateUI()
		{
			if ((bool)instance)
			{
				return instance.autoUpdatePickedUI;
			}
			return false;
		}

		public static void SetNGUICompatibility(bool value)
		{
			if ((bool)instance)
			{
				instance.enabledNGuiMode = value;
			}
		}

		public static bool GetNGUICompatibility()
		{
			if ((bool)instance)
			{
				return instance.enabledNGuiMode;
			}
			return false;
		}

		public static void SetEnableAutoSelect(bool value)
		{
			if ((bool)instance)
			{
				instance.autoSelect = value;
			}
		}

		public static bool GetEnableAutoSelect()
		{
			if ((bool)instance)
			{
				return instance.autoSelect;
			}
			return false;
		}

		public static void SetAutoUpdatePickedObject(bool value)
		{
			if ((bool)instance)
			{
				instance.autoUpdatePickedObject = value;
			}
		}

		public static bool GetAutoUpdatePickedObject()
		{
			if ((bool)instance)
			{
				return instance.autoUpdatePickedObject;
			}
			return false;
		}

		public static void Set3DPickableLayer(LayerMask mask)
		{
			if ((bool)instance)
			{
				instance.pickableLayers3D = mask;
			}
		}

		public static LayerMask Get3DPickableLayer()
		{
			if ((bool)instance)
			{
				return instance.pickableLayers3D;
			}
			return LayerMask.GetMask("Default");
		}

		public static void AddCamera(Camera cam, bool guiCam = false)
		{
			if ((bool)instance)
			{
				instance.touchCameras.Add(new ECamera(cam, guiCam));
			}
		}

		public static void RemoveCamera(Camera cam)
		{
			if ((bool)instance)
			{
				int num = instance.touchCameras.FindIndex((ECamera c) => c.camera == cam);
				if (num > -1)
				{
					instance.touchCameras[num] = null;
					instance.touchCameras.RemoveAt(num);
				}
			}
		}

		public static Camera GetCamera(int index = 0)
		{
			if ((bool)instance)
			{
				if (index < instance.touchCameras.Count)
				{
					return instance.touchCameras[index].camera;
				}
				return null;
			}
			return null;
		}

		public static void SetEnable2DCollider(bool value)
		{
			if ((bool)instance)
			{
				instance.enable2D = value;
			}
		}

		public static bool GetEnable2DCollider()
		{
			if ((bool)instance)
			{
				return instance.enable2D;
			}
			return false;
		}

		public static void Set2DPickableLayer(LayerMask mask)
		{
			if ((bool)instance)
			{
				instance.pickableLayers2D = mask;
			}
		}

		public static LayerMask Get2DPickableLayer()
		{
			if ((bool)instance)
			{
				return instance.pickableLayers2D;
			}
			return LayerMask.GetMask("Default");
		}

		public static void SetGesturePriority(GesturePriority value)
		{
			if ((bool)instance)
			{
				instance.gesturePriority = value;
			}
		}

		public static GesturePriority GetGesturePriority()
		{
			if ((bool)instance)
			{
				return instance.gesturePriority;
			}
			return GesturePriority.Tap;
		}

		public static void SetStationaryTolerance(float tolerance)
		{
			if ((bool)instance)
			{
				instance.StationaryTolerance = tolerance;
			}
		}

		public static float GetStationaryTolerance()
		{
			if ((bool)instance)
			{
				return instance.StationaryTolerance;
			}
			return -1f;
		}

		public static void SetLongTapTime(float time)
		{
			if ((bool)instance)
			{
				instance.longTapTime = time;
			}
		}

		public static float GetlongTapTime()
		{
			if ((bool)instance)
			{
				return instance.longTapTime;
			}
			return -1f;
		}

		public static void SetDoubleTapTime(float time)
		{
			if ((bool)instance)
			{
				instance.doubleTapTime = time;
			}
		}

		public static float GetDoubleTapTime()
		{
			if ((bool)instance)
			{
				return instance.doubleTapTime;
			}
			return -1f;
		}

		public static void SetDoubleTapMethod(DoubleTapDetection detection)
		{
			if ((bool)instance)
			{
				instance.doubleTapDetection = detection;
			}
		}

		public static DoubleTapDetection GetDoubleTapMethod()
		{
			if ((bool)instance)
			{
				return instance.doubleTapDetection;
			}
			return DoubleTapDetection.BySystem;
		}

		public static void SetSwipeTolerance(float tolerance)
		{
			if ((bool)instance)
			{
				instance.swipeTolerance = tolerance;
			}
		}

		public static float GetSwipeTolerance()
		{
			if ((bool)instance)
			{
				return instance.swipeTolerance;
			}
			return -1f;
		}

		public static void SetEnable2FingersGesture(bool enable)
		{
			if ((bool)instance)
			{
				instance.enable2FingersGesture = enable;
			}
		}

		public static bool GetEnable2FingersGesture()
		{
			if ((bool)instance)
			{
				return instance.enable2FingersGesture;
			}
			return false;
		}

		public static void SetTwoFingerPickMethod(TwoFingerPickMethod pickMethod)
		{
			if ((bool)instance)
			{
				instance.twoFingerPickMethod = pickMethod;
			}
		}

		public static TwoFingerPickMethod GetTwoFingerPickMethod()
		{
			if ((bool)instance)
			{
				return instance.twoFingerPickMethod;
			}
			return TwoFingerPickMethod.Finger;
		}

		public static void SetEnablePinch(bool enable)
		{
			if ((bool)instance)
			{
				instance.enablePinch = enable;
			}
		}

		public static bool GetEnablePinch()
		{
			if ((bool)instance)
			{
				return instance.enablePinch;
			}
			return false;
		}

		public static void SetMinPinchLength(float length)
		{
			if ((bool)instance)
			{
				instance.minPinchLength = length;
			}
		}

		public static float GetMinPinchLength()
		{
			if ((bool)instance)
			{
				return instance.minPinchLength;
			}
			return -1f;
		}

		public static void SetEnableTwist(bool enable)
		{
			if ((bool)instance)
			{
				instance.enableTwist = enable;
			}
		}

		public static bool GetEnableTwist()
		{
			if ((bool)instance)
			{
				return instance.enableTwist;
			}
			return false;
		}

		public static void SetMinTwistAngle(float angle)
		{
			if ((bool)instance)
			{
				instance.minTwistAngle = angle;
			}
		}

		public static float GetMinTwistAngle()
		{
			if ((bool)instance)
			{
				return instance.minTwistAngle;
			}
			return -1f;
		}

		public static bool GetSecondeFingerSimulation()
		{
			if (instance != null)
			{
				return instance.enableSimulation;
			}
			return false;
		}

		public static void SetSecondFingerSimulation(bool value)
		{
			if (instance != null)
			{
				instance.enableSimulation = value;
			}
		}
	}
}
