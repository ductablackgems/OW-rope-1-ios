using com.ootii.Actors;
using com.ootii.Helpers;
using System;
using UnityEngine;

namespace com.ootii.Cameras
{
	public class BaseCameraAnchor : MonoBehaviour, IBaseCameraAnchor
	{
		[NonSerialized]
		public Transform _Transform;

		public bool _IsFollowingEnabled = true;

		public bool _RotateWithTarget = true;

		public Transform _Root;

		public Transform _RotationRoot;

		public Vector3 _RootOffset = new Vector3(0f, 0f, 0f);

		public float _MovementLerp = 1f;

		public bool _FreezePositionX;

		public bool _FreezePositionY;

		public bool _FreezePositionZ;

		public bool _FreezeRotationX;

		public bool _FreezeRotationY;

		public bool _FreezeRotationZ;

		protected ControllerLateUpdateDelegate mOnAnchorPostLateUpdate;

		protected bool mIsAttachedToCharacterController;

		protected bool mIsTargetPostionSet;

		protected Transform mTarget;

		protected Vector3 mTargetOffset = Vector3.zero;

		protected float mTargetSpeed = 1f;

		protected float mTargetLerp;

		protected bool mTargetClear = true;

		protected bool mTargetRoot;

		public virtual Transform Transform => _Transform;

		public virtual bool IsFollowingEnabled
		{
			get
			{
				return _IsFollowingEnabled;
			}
			set
			{
				_IsFollowingEnabled = value;
			}
		}

		public virtual bool RotateWithTarget
		{
			get
			{
				return _RotateWithTarget;
			}
			set
			{
				_RotateWithTarget = value;
			}
		}

		public virtual Transform Root
		{
			get
			{
				return _Root;
			}
			set
			{
				if (_Root != null)
				{
					OnDisable();
				}
				_Root = value;
				if (_Root != null && base.enabled)
				{
					OnEnable();
				}
			}
		}

		public virtual Transform RotationRoot
		{
			get
			{
				return _RotationRoot;
			}
			set
			{
				_RotationRoot = value;
			}
		}

		public virtual Vector3 RootOffset
		{
			get
			{
				return _RootOffset;
			}
			set
			{
				_RootOffset = value;
			}
		}

		public float MovementLerp
		{
			get
			{
				return _MovementLerp;
			}
			set
			{
				_MovementLerp = Mathf.Clamp01(value);
			}
		}

		public bool FreezePositionX
		{
			get
			{
				return _FreezePositionX;
			}
			set
			{
				_FreezePositionX = value;
			}
		}

		public bool FreezePositionY
		{
			get
			{
				return _FreezePositionY;
			}
			set
			{
				_FreezePositionY = value;
			}
		}

		public bool FreezePositionZ
		{
			get
			{
				return _FreezePositionZ;
			}
			set
			{
				_FreezePositionZ = value;
			}
		}

		public bool FreezeRotationX
		{
			get
			{
				return _FreezeRotationX;
			}
			set
			{
				_FreezeRotationX = value;
			}
		}

		public bool FreezeRotationY
		{
			get
			{
				return _FreezeRotationY;
			}
			set
			{
				_FreezeRotationY = value;
			}
		}

		public bool FreezeRotationZ
		{
			get
			{
				return _FreezeRotationZ;
			}
			set
			{
				_FreezeRotationZ = value;
			}
		}

		public virtual ControllerLateUpdateDelegate OnAnchorPostLateUpdate
		{
			get
			{
				return mOnAnchorPostLateUpdate;
			}
			set
			{
				mOnAnchorPostLateUpdate = value;
			}
		}

		protected virtual void Awake()
		{
			_Transform = base.gameObject.transform;
			if (_Root != null && base.enabled)
			{
				OnEnable();
			}
		}

		protected virtual void OnEnable()
		{
			if (!(_Root != null))
			{
				return;
			}
			ICharacterController component = InterfaceHelper.GetComponent<ICharacterController>(_Root.gameObject);
			if (component != null)
			{
				mIsAttachedToCharacterController = true;
				if (component.OnControllerPostLateUpdate != null)
				{
					ICharacterController characterController = component;
					characterController.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(characterController.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				}
				ICharacterController characterController2 = component;
				characterController2.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Combine(characterController2.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
			}
		}

		public virtual void ClearTarget(bool rFollowRoot = false)
		{
			mTarget = null;
			mTargetOffset = Vector3.zero;
			mTargetSpeed = 0f;
			mTargetLerp = 0f;
			mTargetClear = false;
			mTargetRoot = false;
			mIsTargetPostionSet = false;
			IsFollowingEnabled = rFollowRoot;
		}

		public virtual void ClearTarget(float rSpeed = 0f, float rLerp = 0f)
		{
			mTarget = null;
			mTargetOffset = Vector3.zero;
			mTargetSpeed = rSpeed;
			mTargetLerp = rLerp;
			mTargetClear = true;
			mTargetRoot = true;
			mIsTargetPostionSet = (rSpeed > 0f);
		}

		public virtual void SetTargetPosition(Transform rTarget, Vector3 rPosition, float rSpeed, float rLerp = 0f, bool rClearTargetOnArrival = true)
		{
			IsFollowingEnabled = false;
			mTarget = rTarget;
			mTargetOffset = rPosition;
			mTargetSpeed = rSpeed;
			mTargetLerp = rLerp;
			mTargetClear = rClearTargetOnArrival;
			mTargetRoot = false;
			mIsTargetPostionSet = true;
		}

		protected virtual void OnDisable()
		{
			if (_Root != null)
			{
				ICharacterController component = InterfaceHelper.GetComponent<ICharacterController>(_Root.gameObject);
				if (component != null && component.OnControllerPostLateUpdate != null)
				{
					ICharacterController characterController = component;
					characterController.OnControllerPostLateUpdate = (ControllerLateUpdateDelegate)Delegate.Remove(characterController.OnControllerPostLateUpdate, new ControllerLateUpdateDelegate(OnControllerLateUpdate));
				}
			}
		}

		protected virtual void LateUpdate()
		{
			if (!mIsAttachedToCharacterController)
			{
				AnchorLateUpdate(Time.deltaTime, 1);
				if (OnAnchorPostLateUpdate != null)
				{
					OnAnchorPostLateUpdate(null, Time.deltaTime, 1);
				}
			}
		}

		protected virtual void AnchorLateUpdate(float rDeltaTime, int rUpdateIndex)
		{
			if (_Root == null)
			{
				return;
			}
			if (mIsTargetPostionSet)
			{
				Vector3 a = mTargetOffset;
				if (mTargetRoot)
				{
					a = _Root.position + ((_RotationRoot != null) ? _RotationRoot.right : _Root.right) * _RootOffset.x;
				}
				else if (mTarget != null)
				{
					a = mTarget.position + mTarget.rotation * mTargetOffset;
				}
				Vector3 vector = a - _Transform.position;
				float num = vector.magnitude;
				if (num <= 0.001f)
				{
					if (mTargetClear)
					{
						ClearTarget(mTargetRoot);
					}
					if (mTargetRoot)
					{
						IsFollowingEnabled = true;
					}
				}
				else
				{
					if (mTargetSpeed > 0f)
					{
						num = Mathf.Min(num, mTargetSpeed * Time.deltaTime);
					}
					Vector3 b = vector.normalized * num;
					if (_FreezePositionX)
					{
						b.x = 0f;
					}
					if (_FreezePositionY)
					{
						b.y = 0f;
					}
					if (_FreezePositionZ)
					{
						b.z = 0f;
					}
					a = _Transform.position + b;
					if (mTargetLerp <= 0f || mTargetLerp >= 1f)
					{
						_Transform.position = a;
					}
					else
					{
						_Transform.position = Vector3.Lerp(_Transform.position, a, mTargetLerp);
					}
				}
			}
			if (_IsFollowingEnabled)
			{
				Vector3 b2 = _Root.position + ((_RotationRoot != null) ? _RotationRoot.right : _Root.right) * _RootOffset.x - _Transform.position;
				if (_FreezePositionX)
				{
					b2.x = 0f;
				}
				if (_FreezePositionY)
				{
					b2.y = 0f;
				}
				if (_FreezePositionZ)
				{
					b2.z = 0f;
				}
				_Transform.position += Vector3.Lerp(Vector3.zero, b2, _MovementLerp);
				if (_RotateWithTarget)
				{
					_Transform.rotation = _Root.rotation;
				}
			}
		}

		protected virtual void OnControllerLateUpdate(ICharacterController rController, float rDeltaTime, int rUpdateIndex)
		{
			AnchorLateUpdate(rDeltaTime, rUpdateIndex);
			if (OnAnchorPostLateUpdate != null)
			{
				OnAnchorPostLateUpdate(rController, rDeltaTime, rUpdateIndex);
			}
		}
	}
}
