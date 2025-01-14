using com.ootii.Base;
using com.ootii.Geometry;
using System;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("Spline Motor")]
	[BaseDescription("Rig Motor will follow along a spline and look at a target or forward.")]
	public class SplineMotor : CameraMotor
	{
		public bool _UseAnchor = true;

		public int _PathOwnerIndex = -1;

		[NonSerialized]
		public Transform _PathOwner;

		private float mPathLength;

		public AnimationCurve _Speed = AnimationCurve.Linear(0f, 5f, 1f, 5f);

		public bool _Loop;

		public bool _RotateToMovementDirection;

		public bool _RotateToAnchor;

		public bool _ActivateMotorOnComplete;

		public int _EndMotorIndex;

		public float mDistanceTravelled;

		public bool mAutoStart = true;

		private bool mHasStarted;

		private bool mIsPaused;

		private Vector3 mLastPosition = Vector3.zero;

		protected BezierSpline mPath;

		public bool UseAnchor
		{
			get
			{
				return _UseAnchor;
			}
			set
			{
				_UseAnchor = value;
			}
		}

		public virtual int PathOwnerIndex
		{
			get
			{
				return _PathOwnerIndex;
			}
			set
			{
				_PathOwnerIndex = value;
				if (_PathOwnerIndex >= 0 && RigController != null && _PathOwnerIndex < RigController.StoredTransforms.Count)
				{
					_PathOwner = RigController.StoredTransforms[_PathOwnerIndex];
				}
			}
		}

		public virtual Transform PathOwner
		{
			get
			{
				return _PathOwner;
			}
			set
			{
				_PathOwner = value;
				if (!(RigController != null))
				{
					return;
				}
				if (_PathOwner == null)
				{
					if (_PathOwnerIndex >= 0 && _PathOwnerIndex < RigController.StoredTransforms.Count)
					{
						RigController.StoredTransforms[_PathOwnerIndex] = null;
						if (_PathOwnerIndex == RigController.StoredTransforms.Count - 1)
						{
							RigController.StoredTransforms.RemoveAt(_PathOwnerIndex);
							_PathOwnerIndex = -1;
						}
					}
					mPath = null;
					mPathLength = 0f;
				}
				else
				{
					if (_PathOwnerIndex == -1)
					{
						_PathOwnerIndex = RigController.StoredTransforms.Count;
						RigController.StoredTransforms.Add(null);
					}
					RigController.StoredTransforms[_PathOwnerIndex] = _PathOwner;
					mPath = _PathOwner.gameObject.GetComponent<BezierSpline>();
					mPathLength = ((mPath != null) ? mPath.Length : 0f);
				}
			}
		}

		public float PathLength => mPathLength;

		public AnimationCurve Speed
		{
			get
			{
				return _Speed;
			}
			set
			{
				_Speed = value;
			}
		}

		public bool Loop
		{
			get
			{
				return _Loop;
			}
			set
			{
				_Loop = value;
			}
		}

		public bool RotateToMovementDirection
		{
			get
			{
				return _RotateToMovementDirection;
			}
			set
			{
				_RotateToMovementDirection = value;
			}
		}

		public bool RotateToAnchor
		{
			get
			{
				return _RotateToAnchor;
			}
			set
			{
				_RotateToAnchor = value;
			}
		}

		public bool ActivateMotorOnComplete
		{
			get
			{
				return _ActivateMotorOnComplete;
			}
			set
			{
				_ActivateMotorOnComplete = value;
			}
		}

		public int EndMotorIndex
		{
			get
			{
				return _EndMotorIndex;
			}
			set
			{
				_EndMotorIndex = value;
			}
		}

		public float DistanceTravelled
		{
			get
			{
				return mDistanceTravelled;
			}
			set
			{
				mDistanceTravelled = value;
			}
		}

		public float DistanceTravelledNormalized
		{
			get
			{
				if (mPathLength == 0f)
				{
					return 0f;
				}
				return mDistanceTravelled / mPathLength;
			}
			set
			{
				mDistanceTravelled = mPathLength * value;
			}
		}

		public bool AutoStart
		{
			get
			{
				return mAutoStart;
			}
			set
			{
				mAutoStart = value;
			}
		}

		public bool IsPaused
		{
			get
			{
				return mIsPaused;
			}
			set
			{
				mIsPaused = value;
			}
		}

		public override void Awake()
		{
			base.Awake();
			if (PathOwner != null)
			{
				mPath = PathOwner.gameObject.GetComponent<BezierSpline>();
				mPathLength = ((mPath != null) ? mPath.Length : 0f);
			}
			if (Application.isPlaying && Anchor == null)
			{
				mRigTransform.Position = RigController._Transform.position;
				mRigTransform.Rotation = RigController._Transform.rotation;
			}
		}

		public void Start(float rNormalizedDistance = 0f)
		{
			mHasStarted = true;
			mIsPaused = false;
			if (mPath != null)
			{
				mDistanceTravelled = rNormalizedDistance * mPathLength;
				Vector3 point = mPath.GetPoint(rNormalizedDistance);
				if (mLastPosition == Vector3.zero)
				{
					mLastPosition = point;
				}
				mRigTransform.Position = point;
			}
		}

		public void Stop()
		{
			mHasStarted = false;
		}

		public override void Activate(CameraMotor rOldMotor)
		{
			base.Activate(rOldMotor);
			if (mAutoStart)
			{
				Start();
			}
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			if (mPath == null)
			{
				return mRigTransform;
			}
			if (RigController == null)
			{
				return mRigTransform;
			}
			bool flag = false;
			if (mHasStarted && !mIsPaused)
			{
				float num = _Speed.Evaluate(DistanceTravelledNormalized);
				mDistanceTravelled += num * rDeltaTime;
			}
			if (mDistanceTravelled >= mPathLength)
			{
				if (_Loop)
				{
					mDistanceTravelled -= mPathLength;
				}
				else
				{
					mDistanceTravelled = mPathLength;
				}
				flag = true;
			}
			float rPercent = mDistanceTravelled / mPathLength;
			Vector3 point = mPath.GetPoint(rPercent);
			if (mLastPosition == Vector3.zero)
			{
				mLastPosition = point;
			}
			mRigTransform.Position = point;
			if (_RotateToAnchor)
			{
				mRigTransform.Rotation = Quaternion.LookRotation(AnchorPosition - point, Vector3.up);
			}
			else if (_RotateToMovementDirection && (point - mLastPosition).sqrMagnitude != 0f)
			{
				mRigTransform.Rotation = Quaternion.LookRotation(point - mLastPosition, Vector3.up);
			}
			mLastPosition = point;
			if (flag && mHasStarted)
			{
				if (_ActivateMotorOnComplete && _EndMotorIndex >= 0 && _EndMotorIndex < RigController.Motors.Count)
				{
					RigController.ActivateMotor(_EndMotorIndex);
				}
				if (RigController.MotorArrived != null)
				{
					RigController.MotorArrived(this);
				}
				CameraMessage cameraMessage = CameraMessage.Allocate();
				cameraMessage.ID = 204;
				cameraMessage.Motor = this;
				if (RigController.ActionTriggeredEvent != null)
				{
					RigController.ActionTriggeredEvent.Invoke(cameraMessage);
				}
				CameraMessage.Release(cameraMessage);
				if (!_Loop)
				{
					mHasStarted = false;
				}
			}
			return mRigTransform;
		}

		public override void DeserializeMotor(string rDefinition)
		{
			base.DeserializeMotor(rDefinition);
			if (_PathOwnerIndex >= 0)
			{
				if (_PathOwnerIndex < RigController.StoredTransforms.Count)
				{
					_PathOwner = RigController.StoredTransforms[_PathOwnerIndex];
				}
				else
				{
					_PathOwner = null;
					_PathOwnerIndex = -1;
				}
			}
			if (_PathOwner != null)
			{
				mPath = _PathOwner.gameObject.GetComponent<BezierSpline>();
				mPathLength = ((mPath != null) ? mPath.Length : 0f);
			}
			_IsCollisionEnabled = false;
		}
	}
}
