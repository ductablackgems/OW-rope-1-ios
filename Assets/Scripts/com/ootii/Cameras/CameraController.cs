using com.ootii.Actors;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Input;
using com.ootii.Messages;
using com.ootii.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Cameras
{
	[AddComponentMenu("ootii/Camera Rigs/Camera Controller")]
	public class CameraController : BaseCameraRig
	{
		public const float EPSILON = 0.0001f;

		public const float MIN_PITCH = -87.4f;

		public const float MAX_PITCH = 87.4f;

		protected ICharacterController mCharacterController;

		public GameObject _InputSourceOwner;

		public bool _AutoFindInputSource = true;

		public Vector3 _AnchorOffset = new Vector3(0f, 2f, 0f);

		public bool _RotateAnchorOffset = true;

		public bool _InvertPitch = true;

		public int _ActiveMotorIndex = -1;

		public MessageEvent MotorTestActivateEvent;

		public MessageEvent MotorActivatedEvent;

		public MessageEvent MotorDeactivatedEvent;

		public MessageEvent ActionTriggeredEvent;

		[NonSerialized]
		public CameraMotorEvent MotorActivated;

		[NonSerialized]
		public CameraMotorEvent MotorUpdated;

		[NonSerialized]
		public CameraMotorEvent MotorArrived;

		[NonSerialized]
		public CameraMotorEvent MotorDeactivated;

		protected Quaternion mTilt = Quaternion.identity;

		public bool _IsCollisionsEnabled = true;

		public int _CollisionLayers = 1;

		public float _CollisionRadius = 0.2f;

		public float _MinCollisionDistance;

		public float _CollisionRecoverySpeed = 5f;

		protected bool mHadCollided;

		protected float mActualDistance;

		public bool _IsZoomEnabled = true;

		public string _ZoomActionAlias = "Camera Zoom";

		public bool _ZoomResetOnRelease = true;

		public float _ZoomSpeed = 25f;

		public float _ZoomSmoothing = 0.1f;

		public float _ZoomMin = 20f;

		public float _ZoomMax;

		protected float mOriginalFOV = 60f;

		protected float mTargetFOV = float.MaxValue;

		private float mZoomVelocity;

		public bool _IsFadeEnabled = true;

		public float _FadeDistance = 0.4f;

		public float _FadeSpeed = 0.25f;

		public bool _DisableRenderers;

		protected float mAlpha = 1f;

		protected float mAlphaStart;

		protected float mAlphaEnd = 1f;

		protected float mAlphaElapsed;

		public AnimationCurve _ShakeStrength = AnimationCurve.Linear(0f, 0f, 1f, 0f);

		protected float mShakeElapsed;

		protected float mShakeDuration;

		protected float mShakeSpeedFactor = 1f;

		protected float mShakeRange = 0.05f;

		protected float mShakeStrengthX = 1f;

		protected float mShakeStrengthY = 1f;

		[NonSerialized]
		public List<CameraMotor> Motors = new List<CameraMotor>();

		protected IInputSource mInputSource;

		public List<string> MotorDefinitions = new List<string>();

		public List<Transform> StoredTransforms = new List<Transform>();

		protected Vector3 mLastPosition = Vector3.zero;

		protected Quaternion mLastRotation = Quaternion.identity;

		public ICharacterController CharacterController => mCharacterController;

		public GameObject InputSourceOwner
		{
			get
			{
				return _InputSourceOwner;
			}
			set
			{
				_InputSourceOwner = value;
				if (_InputSourceOwner != null)
				{
					mInputSource = InterfaceHelper.GetComponent<IInputSource>(_InputSourceOwner);
				}
			}
		}

		public bool AutoFindInputSource
		{
			get
			{
				return _AutoFindInputSource;
			}
			set
			{
				_AutoFindInputSource = value;
			}
		}

		public override Transform Anchor
		{
			get
			{
				return _Anchor;
			}
			set
			{
				if (_Anchor != null)
				{
					ICharacterController component = InterfaceHelper.GetComponent<ICharacterController>(_Anchor.gameObject);
					if (component != null)
					{
						ICharacterController characterController = component;
						characterController.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(characterController.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
					}
					else
					{
						IBaseCameraAnchor component2 = _Anchor.GetComponent<IBaseCameraAnchor>();
						if (component2 != null)
						{
							IBaseCameraAnchor baseCameraAnchor = component2;
							baseCameraAnchor.OnAnchorPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(baseCameraAnchor.OnAnchorPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
						}
					}
				}
				bool flag = _Anchor == value;
				_Anchor = value;
				if (!(_Anchor != null) || !base.enabled)
				{
					return;
				}
				ICharacterController component3 = InterfaceHelper.GetComponent<ICharacterController>(_Anchor.gameObject);
				if (component3 == null)
				{
					IBaseCameraAnchor component4 = _Anchor.GetComponent<IBaseCameraAnchor>();
					if (component4 != null)
					{
						IBaseCameraAnchor baseCameraAnchor2 = component4;
						baseCameraAnchor2.OnAnchorPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Combine(baseCameraAnchor2.OnAnchorPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
					}
					else
					{
						base.IsInternalUpdateEnabled = true;
					}
				}
				else
				{
					base.IsInternalUpdateEnabled = false;
					IsFixedUpdateEnabled = false;
					ICharacterController characterController2 = component3;
					characterController2.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Combine(characterController2.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				}
				if (Application.isPlaying)
				{
					if (!flag)
					{
						InstantiateMotors();
					}
					if (!flag && ActiveMotor != null)
					{
						ActiveMotor.Initialize();
					}
				}
			}
		}

		public Vector3 AnchorOffset
		{
			get
			{
				return _AnchorOffset;
			}
			set
			{
				_AnchorOffset = value;
			}
		}

		public bool RotateAnchorOffset
		{
			get
			{
				return _RotateAnchorOffset;
			}
			set
			{
				_RotateAnchorOffset = value;
			}
		}

		public Vector3 AnchorPosition
		{
			get
			{
				if (_Anchor == null)
				{
					return _AnchorOffset;
				}
				return _Anchor.position + _Anchor.rotation * _AnchorOffset;
			}
		}

		public bool InvertPitch
		{
			get
			{
				return _InvertPitch;
			}
			set
			{
				_InvertPitch = value;
			}
		}

		public override int Mode
		{
			get
			{
				return _ActiveMotorIndex;
			}
			set
			{
				ActivateMotor(value);
			}
		}

		public int ActiveMotorIndex => _ActiveMotorIndex;

		public CameraMotor ActiveMotor
		{
			get
			{
				if (_ActiveMotorIndex < 0 || _ActiveMotorIndex >= Motors.Count)
				{
					return null;
				}
				return Motors[_ActiveMotorIndex];
			}
		}

		public Quaternion Tilt => mTilt;

		public bool IsCollisionsEnabled
		{
			get
			{
				return _IsCollisionsEnabled;
			}
			set
			{
				_IsCollisionsEnabled = value;
			}
		}

		public int CollisionLayers
		{
			get
			{
				return _CollisionLayers;
			}
			set
			{
				_CollisionLayers = value;
			}
		}

		public float CollisionRadius
		{
			get
			{
				return _CollisionRadius;
			}
			set
			{
				_CollisionRadius = value;
			}
		}

		public float MinCollisionDistance
		{
			get
			{
				return _MinCollisionDistance;
			}
			set
			{
				_MinCollisionDistance = value;
			}
		}

		public float CollisionRecoverySpeed
		{
			get
			{
				return _CollisionRecoverySpeed;
			}
			set
			{
				_CollisionRecoverySpeed = value;
			}
		}

		public bool IsZoomEnabled
		{
			get
			{
				return _IsZoomEnabled;
			}
			set
			{
				_IsZoomEnabled = value;
			}
		}

		public string ZoomActionAlias
		{
			get
			{
				return _ZoomActionAlias;
			}
			set
			{
				_ZoomActionAlias = value;
			}
		}

		public bool ZoomResetOnRelease
		{
			get
			{
				return _ZoomResetOnRelease;
			}
			set
			{
				_ZoomResetOnRelease = value;
			}
		}

		public float ZoomSpeed
		{
			get
			{
				return _ZoomSpeed;
			}
			set
			{
				_ZoomSpeed = value;
			}
		}

		public float ZoomSmoothing
		{
			get
			{
				return _ZoomSmoothing;
			}
			set
			{
				_ZoomSmoothing = value;
			}
		}

		public float ZoomMin
		{
			get
			{
				return _ZoomMin;
			}
			set
			{
				_ZoomMin = value;
			}
		}

		public float ZoomMax
		{
			get
			{
				return _ZoomMax;
			}
			set
			{
				_ZoomMax = value;
			}
		}

		public float OriginalFOV => mOriginalFOV;

		public float TargetFOV
		{
			get
			{
				return mTargetFOV;
			}
			set
			{
				mTargetFOV = value;
			}
		}

		public bool IsFadeEnabed
		{
			get
			{
				return _IsFadeEnabled;
			}
			set
			{
				_IsFadeEnabled = value;
				if (!_IsFadeEnabled)
				{
					SetAnchorAlpha(1f);
				}
			}
		}

		public float FadeDistance
		{
			get
			{
				return _FadeDistance;
			}
			set
			{
				_FadeDistance = value;
			}
		}

		public float FadeSpeed
		{
			get
			{
				return _FadeSpeed;
			}
			set
			{
				_FadeSpeed = value;
			}
		}

		public bool DisableRenderers
		{
			get
			{
				return _DisableRenderers;
			}
			set
			{
				_DisableRenderers = value;
			}
		}

		public AnimationCurve ShakeStrength
		{
			get
			{
				return _ShakeStrength;
			}
			set
			{
				_ShakeStrength = value;
			}
		}

		public IInputSource InputSource
		{
			get
			{
				return mInputSource;
			}
			set
			{
				mInputSource = value;
			}
		}

		public Vector3 LastPosition => mLastPosition;

		public Quaternion LastRotation => mLastRotation;

		protected override void Awake()
		{
			base.Awake();
			mLastPosition = base.transform.position;
			mLastRotation = base.transform.rotation;
			if (_InputSourceOwner != null)
			{
				mInputSource = InterfaceHelper.GetComponent<IInputSource>(_InputSourceOwner);
			}
			if (_AutoFindInputSource && mInputSource == null)
			{
				mInputSource = InterfaceHelper.GetComponent<IInputSource>(base.gameObject);
			}
			if (_AutoFindInputSource && mInputSource == null)
			{
				IInputSource[] components = InterfaceHelper.GetComponents<IInputSource>();
				for (int i = 0; i < components.Length; i++)
				{
					GameObject gameObject = ((MonoBehaviour)components[i]).gameObject;
					if (gameObject.activeSelf && components[i].IsEnabled)
					{
						mInputSource = components[i];
						_InputSourceOwner = gameObject;
					}
				}
			}
			if (_Anchor != null && base.enabled)
			{
				ICharacterController component = InterfaceHelper.GetComponent<ICharacterController>(_Anchor.gameObject);
				if (component == null)
				{
					IBaseCameraAnchor component2 = _Anchor.GetComponent<IBaseCameraAnchor>();
					if (component2 != null)
					{
						base.IsInternalUpdateEnabled = false;
						IsFixedUpdateEnabled = false;
						IBaseCameraAnchor baseCameraAnchor = component2;
						baseCameraAnchor.OnAnchorPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Combine(baseCameraAnchor.OnAnchorPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
					}
				}
				else
				{
					base.IsInternalUpdateEnabled = false;
					IsFixedUpdateEnabled = false;
					ICharacterController characterController = component;
					characterController.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Combine(characterController.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				}
				mTilt = QuaternionExt.FromToRotation(Vector3.up, _Anchor.up);
			}
			if (_Camera != null)
			{
				mOriginalFOV = _Camera.fieldOfView;
			}
			if (_ShakeStrength.keys.Length == 2 && _ShakeStrength.keys[0].value == 0f && _ShakeStrength.keys[0].value == 0f)
			{
				_ShakeStrength.AddKey(0.5f, 1f);
			}
			InstantiateMotors();
		}

		protected override void Start()
		{
			if (Anchor == null)
			{
				GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
				if (gameObject != null)
				{
					Anchor = gameObject.transform;
				}
			}
		}

		protected void OnEnable()
		{
			if (!(_Anchor != null))
			{
				return;
			}
			ICharacterController component = InterfaceHelper.GetComponent<ICharacterController>(_Anchor.gameObject);
			if (component != null)
			{
				if (component.OnControllerPostLateUpdate != null)
				{
					ICharacterController characterController = component;
					characterController.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(characterController.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				}
				ICharacterController characterController2 = component;
				characterController2.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Combine(characterController2.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				return;
			}
			IBaseCameraAnchor component2 = _Anchor.GetComponent<IBaseCameraAnchor>();
			if (component2 != null)
			{
				if (component2.OnAnchorPostLateUpdate != null)
				{
					IBaseCameraAnchor baseCameraAnchor = component2;
					baseCameraAnchor.OnAnchorPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(baseCameraAnchor.OnAnchorPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				}
				IBaseCameraAnchor baseCameraAnchor2 = component2;
				baseCameraAnchor2.OnAnchorPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Combine(baseCameraAnchor2.OnAnchorPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
			}
		}

		protected void OnDisable()
		{
			if (!(_Anchor != null))
			{
				return;
			}
			ICharacterController component = InterfaceHelper.GetComponent<ICharacterController>(_Anchor.gameObject);
			if (component != null && component.OnControllerPostLateUpdate != null)
			{
				ICharacterController characterController = component;
				characterController.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(characterController.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				return;
			}
			IBaseCameraAnchor component2 = _Anchor.GetComponent<IBaseCameraAnchor>();
			if (component2 != null)
			{
				IBaseCameraAnchor baseCameraAnchor = component2;
				baseCameraAnchor.OnAnchorPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(baseCameraAnchor.OnAnchorPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
			}
		}

		public override void ExtrapolateAnchorPosition(out Vector3 rPosition, out Quaternion rRotation)
		{
			rPosition = _Anchor.position;
			rRotation = _Anchor.rotation;
			CameraMotor activeMotor = ActiveMotor;
			if (activeMotor != null)
			{
				float d = (mActualDistance > 0f) ? mActualDistance : activeMotor.Distance;
				Vector3 a = _Transform.position + _Transform.forward * d;
				YawPitchMotor yawPitchMotor = activeMotor as YawPitchMotor;
				if (yawPitchMotor != null)
				{
					Quaternion quaternion = _Transform.rotation * Quaternion.Inverse(Quaternion.Euler(yawPitchMotor.LocalPitch, yawPitchMotor.LocalYaw, 0f));
					a = a + _Transform.right * (0f - activeMotor.Offset.x) + _Transform.forward * (0f - activeMotor.Offset.z);
					Vector3 vector = rPosition = a - quaternion * activeMotor.AnchorOffset;
					rRotation = quaternion;
				}
			}
		}

		public void EnableMotor(string rName, bool rEnable)
		{
			for (int i = 0; i < Motors.Count; i++)
			{
				CameraMotor cameraMotor = Motors[i];
				if (cameraMotor.Name == rName)
				{
					cameraMotor.IsEnabled = rEnable;
				}
			}
		}

		public void EnableMotor<T>(bool rEnable, string rName = null) where T : CameraMotor
		{
			Type typeFromHandle = typeof(T);
			for (int i = 0; i < Motors.Count; i++)
			{
				CameraMotor cameraMotor = Motors[i];
				if (ReflectionHelper.IsSubclassOf(cameraMotor.GetType(), typeFromHandle) && (rName == null || cameraMotor.Name == rName))
				{
					cameraMotor.IsEnabled = rEnable;
				}
			}
		}

		public void ActivateMotor(int rIndex)
		{
			if (_ActiveMotorIndex == rIndex || rIndex < 0 || rIndex >= Motors.Count || !Motors[rIndex].IsEnabled)
			{
				return;
			}
			CameraMessage cameraMessage = null;
			CameraMotor cameraMotor = null;
			if (_ActiveMotorIndex >= 0 && _ActiveMotorIndex < Motors.Count)
			{
				cameraMotor = Motors[_ActiveMotorIndex];
				cameraMotor.Deactivate(Motors[rIndex]);
				if (MotorDeactivated != null)
				{
					MotorDeactivated(cameraMotor);
				}
				cameraMessage = CameraMessage.Allocate();
				cameraMessage.ID = EnumMessageID.MSG_CAMERA_MOTOR_DEACTIVATE;
				cameraMessage.Motor = cameraMotor;
				if (MotorDeactivatedEvent != null)
				{
					MotorDeactivatedEvent.Invoke(cameraMessage);
				}
				CameraMessage.Release(cameraMessage);
			}
			Motors[rIndex].Activate(cameraMotor);
			_ActiveMotorIndex = rIndex;
			if (MotorActivated != null)
			{
				MotorActivated(Motors[_ActiveMotorIndex]);
			}
			cameraMessage = CameraMessage.Allocate();
			cameraMessage.ID = EnumMessageID.MSG_CAMERA_MOTOR_ACTIVATE;
			cameraMessage.Motor = Motors[_ActiveMotorIndex];
			if (MotorActivatedEvent != null)
			{
				MotorActivatedEvent.Invoke(cameraMessage);
			}
			CameraMessage.Release(cameraMessage);
		}

		public void ActivateMotor(CameraMotor rMotor)
		{
			if (rMotor == null || !rMotor.IsEnabled)
			{
				return;
			}
			int num = Motors.IndexOf(rMotor);
			if (num == _ActiveMotorIndex)
			{
				return;
			}
			CameraMessage cameraMessage = null;
			CameraMotor cameraMotor = null;
			if (_ActiveMotorIndex >= 0 && _ActiveMotorIndex < Motors.Count)
			{
				cameraMotor = Motors[_ActiveMotorIndex];
				cameraMotor.Deactivate(rMotor);
				if (MotorDeactivated != null)
				{
					MotorDeactivated(cameraMotor);
				}
				cameraMessage = CameraMessage.Allocate();
				cameraMessage.ID = EnumMessageID.MSG_CAMERA_MOTOR_DEACTIVATE;
				cameraMessage.Motor = Motors[_ActiveMotorIndex];
				if (MotorDeactivatedEvent != null)
				{
					MotorDeactivatedEvent.Invoke(cameraMessage);
				}
				CameraMessage.Release(cameraMessage);
			}
			rMotor.Activate(cameraMotor);
			_ActiveMotorIndex = num;
			if (MotorActivated != null)
			{
				MotorActivated(Motors[_ActiveMotorIndex]);
			}
			cameraMessage = CameraMessage.Allocate();
			cameraMessage.ID = EnumMessageID.MSG_CAMERA_MOTOR_ACTIVATE;
			cameraMessage.Motor = Motors[_ActiveMotorIndex];
			if (MotorActivatedEvent != null)
			{
				MotorActivatedEvent.Invoke(cameraMessage);
			}
			CameraMessage.Release(cameraMessage);
		}

		public void DeactivateMotor()
		{
			if (_ActiveMotorIndex >= 0 && _ActiveMotorIndex < Motors.Count)
			{
				Motors[_ActiveMotorIndex].Deactivate(null);
				if (MotorDeactivated != null)
				{
					MotorDeactivated(Motors[_ActiveMotorIndex]);
				}
				CameraMessage cameraMessage = CameraMessage.Allocate();
				cameraMessage.ID = EnumMessageID.MSG_CAMERA_MOTOR_DEACTIVATE;
				cameraMessage.Motor = Motors[_ActiveMotorIndex];
				if (MotorDeactivatedEvent != null)
				{
					MotorDeactivatedEvent.Invoke(cameraMessage);
				}
				CameraMessage.Release(cameraMessage);
			}
			_ActiveMotorIndex = -1;
		}

		public CameraMotor GetMotor(string rName)
		{
			for (int i = 0; i < Motors.Count; i++)
			{
				CameraMotor cameraMotor = Motors[i];
				if (cameraMotor.Name == rName)
				{
					return cameraMotor;
				}
			}
			return null;
		}

		public T GetMotor<T>(string rName = null) where T : CameraMotor
		{
			Type typeFromHandle = typeof(T);
			for (int i = 0; i < Motors.Count; i++)
			{
				CameraMotor cameraMotor = Motors[i];
				if (ReflectionHelper.IsSubclassOf(cameraMotor.GetType(), typeFromHandle) && (rName == null || cameraMotor.Name == rName))
				{
					return cameraMotor as T;
				}
			}
			return null;
		}

		public int GetMotorIndex(string rName)
		{
			for (int i = 0; i < Motors.Count; i++)
			{
				if (Motors[i].Name == rName)
				{
					return i;
				}
			}
			return -1;
		}

		public int GetMotorIndex<T>(string rName = null) where T : CameraMotor
		{
			Type typeFromHandle = typeof(T);
			for (int i = 0; i < Motors.Count; i++)
			{
				CameraMotor cameraMotor = Motors[i];
				if (ReflectionHelper.IsSubclassOf(cameraMotor.GetType(), typeFromHandle) && (rName == null || cameraMotor.Name == rName))
				{
					return i;
				}
			}
			return -1;
		}

		public void Shake(float rRange, float rDuration)
		{
			mShakeElapsed = 0f;
			mShakeSpeedFactor = 1f;
			mShakeStrengthX = 1f;
			mShakeStrengthY = 1f;
			mShakeRange = rRange;
			mShakeDuration = rDuration;
		}

		public void Shake(float rRange, float rStrengthX, float rStrengthY, float rDuration)
		{
			mShakeElapsed = 0f;
			mShakeSpeedFactor = 1f;
			mShakeStrengthX = rStrengthX;
			mShakeStrengthY = rStrengthY;
			mShakeRange = rRange;
			mShakeDuration = rDuration;
		}

		public override void ClearTargetYawPitch()
		{
			for (int i = 0; i < Motors.Count; i++)
			{
				(Motors[i] as YawPitchMotor)?.ClearTargetYawPitch();
			}
		}

		public override void SetTargetYawPitch(float rYaw, float rPitch, float rSpeed = -1f, bool rAutoClearTarget = true)
		{
			for (int i = 0; i < Motors.Count; i++)
			{
				YawPitchMotor yawPitchMotor = Motors[i] as YawPitchMotor;
				if (yawPitchMotor != null && yawPitchMotor.IsActive)
				{
					yawPitchMotor.SetTargetYawPitch(rYaw, rPitch, rSpeed, rAutoClearTarget);
				}
			}
		}

		public override void ClearTargetForward()
		{
			for (int i = 0; i < Motors.Count; i++)
			{
				(Motors[i] as YawPitchMotor)?.ClearTargetForward();
			}
		}

		public override void SetTargetForward(Vector3 rForward, float rSpeed = -1f, bool rAutoClearTarget = true)
		{
			for (int i = 0; i < Motors.Count; i++)
			{
				(Motors[i] as YawPitchMotor)?.SetTargetForward(rForward, rSpeed, rAutoClearTarget);
			}
		}

		protected override void InternalUpdate()
		{
			if (_IsInternalUpdateEnabled)
			{
				base.InternalUpdate();
				mLastPosition = _Transform.position;
				mLastRotation = _Transform.rotation;
			}
		}

		public override void RigLateUpdate(float rDeltaTime, int rUpdateIndex)
		{
			CameraMotor cameraMotor = ActiveMotor;
			float rTiltAngle = UpdateTilt();
			int num = -1;
			for (int i = 0; i < Motors.Count; i++)
			{
				if (i != _ActiveMotorIndex)
				{
					bool flag = Motors[i].TestActivate(cameraMotor);
					CameraMessage cameraMessage = CameraMessage.Allocate();
					cameraMessage.ID = EnumMessageID.MSG_CAMERA_MOTOR_TEST;
					cameraMessage.Motor = Motors[_ActiveMotorIndex];
					cameraMessage.Continue = true;
					if (flag && MotorTestActivateEvent != null)
					{
						MotorActivatedEvent.Invoke(cameraMessage);
						flag = cameraMessage.Continue;
					}
					CameraMessage.Release(cameraMessage);
					if (flag && (num < 0 || Motors[i].Priority >= Motors[num].Priority))
					{
						num = i;
					}
				}
			}
			if (num >= 0)
			{
				ActivateMotor(num);
				cameraMotor = Motors[num];
			}
			if (cameraMotor == null)
			{
				return;
			}
			CameraTransform rTransform = cameraMotor.RigLateUpdate(rDeltaTime, rUpdateIndex, rTiltAngle);
			UpdateCollisions(rDeltaTime, ref rTransform);
			_Transform.position = rTransform.Position;
			_Transform.rotation = rTransform.Rotation;
			UpdateZoom(rDeltaTime);
			UpdateFade(rDeltaTime);
			UpdateShake(rDeltaTime);
			if (MotorUpdated != null)
			{
				MotorUpdated(cameraMotor);
			}
			if (_IsInternalUpdateEnabled)
			{
				if (mOnPostLateUpdate != null)
				{
					mOnPostLateUpdate(rDeltaTime, mUpdateIndex, this);
				}
				cameraMotor.PostRigLateUpdate();
			}
		}

		protected void OnControllerLateUpdate(ICharacterController rController, float rDeltaTime, int rUpdateIndex)
		{
			mCharacterController = rController;
			RigLateUpdate(rDeltaTime, rUpdateIndex);
			if (mOnPostLateUpdate != null)
			{
				mOnPostLateUpdate(rDeltaTime, mUpdateIndex, this);
			}
			if (_ActiveMotorIndex >= 0 && _ActiveMotorIndex < Motors.Count)
			{
				Motors[_ActiveMotorIndex].PostRigLateUpdate();
			}
			_FrameForceToFollowAnchor = false;
			mLastPosition = _Transform.position;
			mLastRotation = _Transform.rotation;
		}

		public void InstantiateMotors()
		{
			int count = MotorDefinitions.Count;
			if (_Transform == null)
			{
				_Transform = base.gameObject.transform;
			}
			for (int num = Motors.Count - 1; num >= count; num--)
			{
				Motors.RemoveAt(num);
			}
			List<int> list = new List<int>();
			for (int i = 0; i < count; i++)
			{
				string text = MotorDefinitions[i];
				JSONNode jSONNode = JSONNode.Parse(text);
				if (jSONNode == null)
				{
					continue;
				}
				CameraMotor cameraMotor = null;
				string value = jSONNode["Type"].Value;
				bool rNameChanged;
				Type type = AssemblyHelper.ResolveType(value, out rNameChanged);
				if (type == null)
				{
					list.Add(i);
					continue;
				}
				if (Motors.Count <= i || value != Motors[i].GetType().AssemblyQualifiedName)
				{
					cameraMotor = (Activator.CreateInstance(type) as CameraMotor);
					cameraMotor.RigController = this;
					if (Motors.Count <= i)
					{
						Motors.Add(cameraMotor);
					}
					else
					{
						Motors[i] = cameraMotor;
					}
				}
				else
				{
					cameraMotor = Motors[i];
				}
				if (cameraMotor != null)
				{
					cameraMotor.DeserializeMotor(text);
					if (rNameChanged)
					{
						cameraMotor.Type = "";
						MotorDefinitions[i] = cameraMotor.SerializeMotor();
					}
				}
			}
			if (list.Count > 0)
			{
				for (int j = 0; j < list.Count; j++)
				{
					int index = list[j];
					MotorDefinitions.RemoveAt(index);
				}
			}
			for (int k = 0; k < Motors.Count; k++)
			{
				Motors[k].Awake();
			}
			if (Application.isPlaying && _ActiveMotorIndex >= 0 && _ActiveMotorIndex < Motors.Count)
			{
				Motors[_ActiveMotorIndex].Activate(null);
			}
		}

		protected virtual float UpdateTilt()
		{
			float result = 0f;
			if (_Anchor != null)
			{
				Vector3 vector = mTilt.Up();
				Quaternion quaternion = QuaternionExt.FromToRotation(vector, _Anchor.up);
				if (!quaternion.IsIdentity())
				{
					result = Vector3.Angle(vector, _Anchor.up);
					mTilt = quaternion * mTilt;
				}
				if (Vector3.Angle(mTilt.Up(), Vector3.up) < 0.0001f && !mTilt.IsIdentity())
				{
					mTilt = Quaternion.identity;
				}
			}
			return result;
		}

		protected virtual void UpdateZoom(float rDeltaTime)
		{
			if (_IsZoomEnabled && _ZoomActionAlias.Length > 0)
			{
				if (_ZoomResetOnRelease && mInputSource.IsJustReleased(_ZoomActionAlias))
				{
					mTargetFOV = mOriginalFOV;
				}
				else
				{
					float max = (_ZoomMax > 0f) ? _ZoomMax : mOriginalFOV;
					float num = (0f - mInputSource.GetValue(_ZoomActionAlias)) * _ZoomSpeed;
					mTargetFOV = Mathf.Clamp(mTargetFOV + num, mHadCollided ? mOriginalFOV : _ZoomMin, max);
				}
			}
			else if (mTargetFOV != mOriginalFOV)
			{
				mTargetFOV = mOriginalFOV;
			}
			if (_IsZoomEnabled && Mathf.Abs(mTargetFOV - _Camera.fieldOfView) > 0.001f)
			{
				_Camera.fieldOfView = Mathf.SmoothDampAngle(_Camera.fieldOfView, mTargetFOV, ref mZoomVelocity, _ZoomSmoothing);
			}
			else
			{
				mZoomVelocity = 0f;
			}
		}

		protected virtual void UpdateCollisions(float rDeltaTime, ref CameraTransform rTransform)
		{
			bool flag = false;
			CameraMotor activeMotor = ActiveMotor;
			float num = _CollisionRecoverySpeed * rDeltaTime;
			Vector3 rhs;
			Vector3 vector;
			if (activeMotor == null)
			{
				rhs = _Anchor.position + _Anchor.rotation * _AnchorOffset;
				vector = _Anchor.position + _Anchor.rotation * _AnchorOffset;
			}
			else
			{
				rhs = activeMotor.AnchorPosition;
				vector = activeMotor.GetFocusPosition(rTransform.Rotation);
			}
			bool flag2 = vector != rhs;
			float num2 = Vector3.Distance(_Transform.position, vector);
			Vector3 vector2 = rTransform.Position - vector;
			Vector3 normalized = vector2.normalized;
			float magnitude = vector2.magnitude;
			if (_IsCollisionsEnabled && (activeMotor == null || activeMotor.IsCollisionEnabled) && RaycastExt.SafeSphereCast(vector, normalized, _CollisionRadius, out RaycastHit rHitInfo, magnitude, _CollisionLayers, _Anchor))
			{
				mActualDistance = rHitInfo.distance;
				if (mActualDistance == 0f && RaycastExt.SafeRaycast(vector, normalized, out rHitInfo, magnitude, _CollisionLayers, _Anchor))
				{
					mActualDistance = rHitInfo.distance;
				}
				if (mActualDistance > 0f && mActualDistance - num2 <= num)
				{
					mActualDistance = Mathf.Max(mActualDistance, _MinCollisionDistance);
					mHadCollided = true;
					flag = true;
					rTransform.Position = vector + normalized * mActualDistance;
				}
			}
			if (!flag && mHadCollided)
			{
				if (magnitude - num2 - 0.0001f > num)
				{
					mActualDistance = num2 + _CollisionRecoverySpeed * rDeltaTime;
					rTransform.Position = vector + normalized * mActualDistance;
				}
				else
				{
					mHadCollided = false;
				}
			}
			if (mActualDistance == 0f)
			{
				mActualDistance = activeMotor.Distance;
			}
		}

		protected virtual void UpdateShake(float rDeltaTime)
		{
			Vector3 zero = Vector3.zero;
			if (mShakeDuration > 0f)
			{
				mShakeElapsed += rDeltaTime * mShakeSpeedFactor;
				float num = Mathf.Clamp01(mShakeElapsed / mShakeDuration);
				if (num < 1f)
				{
					float num2 = _ShakeStrength.Evaluate(num);
					zero.x = ((float)NumberHelper.Randomizer.NextDouble() * 2f - 1f) * mShakeRange * mShakeStrengthX * num2;
					zero.y = ((float)NumberHelper.Randomizer.NextDouble() * 2f - 1f) * mShakeRange * mShakeStrengthY * num2;
				}
				else
				{
					mShakeElapsed = 0f;
					mShakeDuration = 0f;
				}
				_Camera.transform.localPosition = zero;
			}
		}

		protected virtual void UpdateFade(float rDelta)
		{
			Vector3 position = _Transform.position;
			Vector3 b = ActiveMotor?.GetFocusPosition(_Transform.rotation) ?? (_Anchor.position + _Anchor.rotation * _AnchorOffset);
			if (Vector3.Distance(position, b) < _FadeDistance)
			{
				mAlphaStart = mAlpha;
				mAlphaEnd = 0f;
			}
			else
			{
				mAlphaStart = mAlpha;
				mAlphaEnd = 1f;
			}
			if (mAlpha != mAlphaEnd)
			{
				mAlphaElapsed += Time.deltaTime;
				mAlpha = NumberHelper.SmoothStep(mAlphaStart, mAlphaEnd, (_FadeSpeed > 0f) ? (mAlphaElapsed / _FadeSpeed) : 1f);
				if (_IsFadeEnabled)
				{
					SetAnchorAlpha(mAlpha);
				}
			}
			else
			{
				mAlphaElapsed = 0f;
				mAlphaStart = mAlpha;
			}
		}

		protected void SetAnchorAlpha(float rAlpha)
		{
			if (_Anchor == null || !Application.isPlaying || !_IsFadeEnabled || (ActiveMotor != null && !ActiveMotor.IsFadingEnabled))
			{
				return;
			}
			Renderer[] componentsInChildren = _Anchor.gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				float num = rAlpha;
				bool enabled = (num > 0f) ? true : false;
				Renderer renderer = componentsInChildren[i];
				if (!_DisableRenderers && !renderer.enabled)
				{
					continue;
				}
				if (ActiveMotor.SpecifyFadeRenderers && !ActiveMotor.IsFadeRenderer(renderer.transform))
				{
					num = 1f;
					enabled = true;
				}
				Material[] materials = renderer.materials;
				for (int j = 0; j < materials.Length; j++)
				{
					if (materials[j].HasProperty("_Color"))
					{
						Color color = materials[j].color;
						color.a = num;
						materials[j].color = color;
					}
				}
				if (_DisableRenderers)
				{
					renderer.enabled = enabled;
				}
			}
		}
	}
}
