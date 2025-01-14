using System;
using UnityEngine;

namespace com.ootii.Cameras
{
	public class BaseCameraRig : MonoBehaviour, IBaseCameraRig
	{
		public bool _UseFixedUpdate;

		[NonSerialized]
		public Transform _Transform;

		[NonSerialized]
		[HideInInspector]
		public Camera _Camera;

		public int _Mode;

		protected bool mLockMode;

		public Transform _Anchor;

		protected bool mFrameLockForward;

		public bool _FrameForceToFollowAnchor;

		public bool _IsInternalUpdateEnabled = true;

		public bool _IsFixedUpdateEnabled;

		public float _FixedUpdateFPS = 60f;

		[NonSerialized]
		public float _DeltaTime;

		protected CameraUpdateEvent mOnPostLateUpdate;

		protected bool mIsFirstUpdate = true;

		protected int mUpdateCount;

		protected int mUpdateIndex = 1;

		protected float mFixedElapsedTime;

		protected float mEditorLastTime;

		protected float mEditorDeltaTime;

		public bool UseFixedUpdate
		{
			get
			{
				return _UseFixedUpdate;
			}
			set
			{
				_UseFixedUpdate = value;
			}
		}

		public virtual Transform Transform => _Transform;

		public virtual Camera Camera => _Camera;

		public virtual int Mode
		{
			get
			{
				return _Mode;
			}
			set
			{
				_Mode = value;
			}
		}

		public virtual bool LockMode
		{
			get
			{
				return mLockMode;
			}
			set
			{
				mLockMode = value;
			}
		}

		public virtual Transform Anchor
		{
			get
			{
				return _Anchor;
			}
			set
			{
				_Anchor = value;
			}
		}

		public virtual bool FrameLockForward
		{
			get
			{
				return mFrameLockForward;
			}
			set
			{
				mFrameLockForward = value;
			}
		}

		public virtual bool FrameForceToFollowAnchor
		{
			get
			{
				return _FrameForceToFollowAnchor;
			}
			set
			{
				_FrameForceToFollowAnchor = value;
			}
		}

		public bool IsInternalUpdateEnabled
		{
			get
			{
				return _IsInternalUpdateEnabled;
			}
			set
			{
				_IsInternalUpdateEnabled = value;
			}
		}

		public virtual bool IsFixedUpdateEnabled
		{
			get
			{
				return _IsFixedUpdateEnabled;
			}
			set
			{
				_IsFixedUpdateEnabled = value;
			}
		}

		public virtual float FixedUpdateFPS
		{
			get
			{
				return _FixedUpdateFPS;
			}
			set
			{
				_FixedUpdateFPS = value;
			}
		}

		public float DeltaTime => _DeltaTime;

		public CameraUpdateEvent OnPostLateUpdate
		{
			get
			{
				return mOnPostLateUpdate;
			}
			set
			{
				mOnPostLateUpdate = value;
			}
		}

		protected virtual void Awake()
		{
			_Transform = base.gameObject.transform;
			if (!(_Camera == null))
			{
				return;
			}
			_Camera = base.gameObject.GetComponent<Camera>();
			if (!(_Camera == null))
			{
				return;
			}
			Camera[] componentsInChildren = base.gameObject.GetComponentsInChildren<Camera>();
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < componentsInChildren.Length)
				{
					if (componentsInChildren[num].enabled)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_Camera = componentsInChildren[num];
		}

		protected virtual void Start()
		{
		}

		public virtual void EnableMode(int rMode, bool rEnable)
		{
		}

		public virtual void ClearTargetYawPitch()
		{
		}

		public virtual void SetTargetYawPitch(float rYaw, float rPitch, float rSpeed = -1f, bool rAutoClearTarget = true)
		{
		}

		public virtual void ClearTargetForward()
		{
		}

		public virtual void SetTargetForward(Vector3 rForward, float rSpeed = -1f, bool rAutoClearTarget = true)
		{
		}

		public virtual void ExtrapolateAnchorPosition(out Vector3 rPosition, out Quaternion rRotation)
		{
			if (_Anchor != null)
			{
				rPosition = _Anchor.position;
				rRotation = _Anchor.rotation;
			}
			else
			{
				rPosition = Vector3.zero;
				rRotation = Quaternion.identity;
			}
		}

		protected virtual void Update()
		{
			if (!_IsInternalUpdateEnabled)
			{
				return;
			}
			mUpdateCount = 0;
			_DeltaTime = Time.deltaTime;
			if (!_IsFixedUpdateEnabled || _FixedUpdateFPS <= 0f)
			{
				mUpdateCount = 1;
			}
			else
			{
				_DeltaTime = 1f / _FixedUpdateFPS;
				if (Mathf.Abs(_DeltaTime - Time.deltaTime) < _DeltaTime * 0.1f)
				{
					mUpdateCount = 1;
				}
				else
				{
					mFixedElapsedTime += Time.deltaTime;
					while (mFixedElapsedTime >= _DeltaTime)
					{
						mUpdateCount++;
						mFixedElapsedTime -= _DeltaTime;
						if (mUpdateCount >= 5)
						{
							mFixedElapsedTime = 0f;
							break;
						}
					}
				}
			}
			mUpdateIndex = 1;
		}

		protected virtual void FixedUpdate()
		{
			if (_IsInternalUpdateEnabled && _UseFixedUpdate)
			{
				InternalUpdate();
			}
		}

		protected virtual void LateUpdate()
		{
			if (!_IsInternalUpdateEnabled || !_UseFixedUpdate)
			{
				InternalUpdate();
			}
		}

		public virtual void RigLateUpdate(float rDeltaTime, int rUpdateIndex)
		{
		}

		protected virtual void InternalUpdate()
		{
			if (!_IsInternalUpdateEnabled)
			{
				return;
			}
			if (mUpdateCount > 0)
			{
				for (mUpdateIndex = 1; mUpdateIndex <= mUpdateCount; mUpdateIndex++)
				{
					RigLateUpdate(_DeltaTime, mUpdateIndex);
					mIsFirstUpdate = false;
				}
			}
			else
			{
				mUpdateIndex = 0;
				RigLateUpdate(_DeltaTime, mUpdateIndex);
			}
			mUpdateIndex = 1;
			if (mOnPostLateUpdate != null)
			{
				mOnPostLateUpdate(_DeltaTime, mUpdateIndex, this);
			}
		}

		public static BaseCameraRig ExtractCameraRig(Transform rCamera)
		{
			if (rCamera == null)
			{
				return null;
			}
			Transform transform = rCamera;
			while (transform != null)
			{
				BaseCameraRig[] components = transform.gameObject.GetComponents<BaseCameraRig>();
				if (components != null && components.Length != 0)
				{
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i].enabled && components[i].gameObject.activeSelf)
						{
							return components[i];
						}
					}
				}
				transform = transform.parent;
			}
			return null;
		}
	}
}
