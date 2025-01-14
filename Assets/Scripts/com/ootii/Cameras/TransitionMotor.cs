using com.ootii.Base;
using com.ootii.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("Transition")]
	[BaseDescription("Motor that transitions from one motor to another. The 'Transition' motor does the transition while the 'End' motor is the result.")]
	public class TransitionMotor : CameraMotor
	{
		private static string[] ActionAliasEventTypes = new string[2]
		{
			"Key down",
			"Key up"
		};

		public static string[] BlendTypes = new string[4]
		{
			"Active Start",
			"Static Start",
			"Start Only",
			"End Only"
		};

		public static string[] NumericComparisonTypes = new string[3]
		{
			"<",
			">",
			"="
		};

		public int _StartMotorIndex;

		public int _EndMotorIndex;

		public int _EndMotorIndexOverride = -1;

		public int _PositionBlend;

		public int _RotationBlend;

		public float _TransitionTime = 0.5f;

		public int _ActionAliasEventType;

		public bool _LimitToStart = true;

		public bool _TestDistance;

		public string _ActorStances = "";

		public bool _InvertVerifyActorStances;

		public int _DistanceCompareType;

		public float _DistanceValue = 0.5f;

		protected CameraMotor mStartMotor;

		protected CameraMotor mEndMotor;

		protected Vector3 mStartPosition = Vector3.zero;

		protected Quaternion mStartRotation = Quaternion.identity;

		protected bool mIsActionAliasInUse;

		protected bool mWasActionAliasInUse;

		protected List<int> mActorStances;

		protected float mTransitionElapsedTime;

		public int StartMotorIndex
		{
			get
			{
				return _StartMotorIndex;
			}
			set
			{
				_StartMotorIndex = value;
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

		public int EndMotorIndexOverride
		{
			get
			{
				return _EndMotorIndexOverride;
			}
			set
			{
				_EndMotorIndexOverride = value;
			}
		}

		public int PositionBlend
		{
			get
			{
				return _PositionBlend;
			}
			set
			{
				_PositionBlend = value;
			}
		}

		public int RotationBlend
		{
			get
			{
				return _RotationBlend;
			}
			set
			{
				_RotationBlend = value;
			}
		}

		public float TransitionTime
		{
			get
			{
				return _TransitionTime;
			}
			set
			{
				_TransitionTime = value;
			}
		}

		public int ActionAliasEventType
		{
			get
			{
				return _ActionAliasEventType;
			}
			set
			{
				_ActionAliasEventType = value;
			}
		}

		public bool LimitToStart
		{
			get
			{
				return _LimitToStart;
			}
			set
			{
				_LimitToStart = value;
			}
		}

		public bool TestDistance
		{
			get
			{
				return _TestDistance;
			}
			set
			{
				_TestDistance = value;
			}
		}

		public string ActorStances
		{
			get
			{
				return _ActorStances;
			}
			set
			{
				_ActorStances = value;
				if (_ActorStances.Length == 0)
				{
					if (mActorStances != null)
					{
						mActorStances.Clear();
					}
					return;
				}
				if (mActorStances == null)
				{
					mActorStances = new List<int>();
				}
				mActorStances.Clear();
				int result = 0;
				string[] array = _ActorStances.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					if (int.TryParse(array[i], out result) && !mActorStances.Contains(result))
					{
						mActorStances.Add(result);
					}
				}
			}
		}

		public bool InvertVerifyActorStances
		{
			get
			{
				return _InvertVerifyActorStances;
			}
			set
			{
				_InvertVerifyActorStances = value;
			}
		}

		public int DistanceCompareType
		{
			get
			{
				return _DistanceCompareType;
			}
			set
			{
				_DistanceCompareType = value;
			}
		}

		public float DistanceValue
		{
			get
			{
				return _DistanceValue;
			}
			set
			{
				_DistanceValue = value;
			}
		}

		public float TransitionElapsedTime => mTransitionElapsedTime;

		public override void Awake()
		{
			base.Awake();
		}

		public override bool Initialize()
		{
			mStartMotor = null;
			if (_StartMotorIndex < 0)
			{
				mStartMotor = RigController.ActiveMotor;
			}
			else if (_StartMotorIndex < RigController.Motors.Count)
			{
				mStartMotor = RigController.Motors[_StartMotorIndex];
			}
			if (mStartMotor == this)
			{
				mStartMotor = null;
			}
			mEndMotor = null;
			if (_EndMotorIndex >= 0 && _EndMotorIndex < RigController.Motors.Count)
			{
				mEndMotor = RigController.Motors[_EndMotorIndex];
			}
			if (mEndMotor == this)
			{
				mEndMotor = null;
			}
			float num = 1f;
			if (RigController.ActiveMotor is TransitionMotor)
			{
				TransitionMotor transitionMotor = RigController.ActiveMotor as TransitionMotor;
				num = ((transitionMotor.TransitionTime > 0f) ? (transitionMotor.TransitionElapsedTime / transitionMotor.TransitionTime) : 1f);
			}
			else if (mStartMotor is YawPitchMotor)
			{
				YawPitchMotor yawPitchMotor = mStartMotor as YawPitchMotor;
				num = ((yawPitchMotor.MaxDistance != 0f) ? NumberHelper.SmoothStepTime(0f, yawPitchMotor.MaxDistance, yawPitchMotor.Distance) : 1f);
			}
			else if (mEndMotor is YawPitchMotor)
			{
				YawPitchMotor yawPitchMotor2 = mEndMotor as YawPitchMotor;
				num = ((yawPitchMotor2.MaxDistance != 0f) ? NumberHelper.SmoothStepTime(0f, yawPitchMotor2.MaxDistance, yawPitchMotor2.Distance) : 1f);
			}
			mTransitionElapsedTime = _TransitionTime * (1f - num);
			if (mStartMotor != null && mEndMotor != null)
			{
				mStartMotor.Initialize();
				mEndMotor.Initialize();
				mStartPosition = RigController._Transform.position;
				mStartRotation = RigController._Transform.rotation;
				return base.Initialize();
			}
			return false;
		}

		public override bool TestActivate(CameraMotor rActiveMotor)
		{
			UpdateToggle();
			if (!_IsEnabled)
			{
				return false;
			}
			if (_LimitToStart && RigController.ActiveMotorIndex != _StartMotorIndex)
			{
				return false;
			}
			bool flag = false;
			if (_ActionAlias.Length > 0 && RigController.InputSource != null)
			{
				if (_ActionAliasEventType == 0)
				{
					if (RigController.InputSource.IsJustPressed(_ActionAlias) || (mIsActionAliasInUse && !mWasActionAliasInUse))
					{
						flag = true;
					}
					else if (_LimitToStart && RigController.InputSource.IsPressed(_ActionAlias))
					{
						flag = true;
					}
				}
				else if (_ActionAliasEventType == 1 && (RigController.InputSource.IsJustReleased(_ActionAlias) || (!mIsActionAliasInUse && mWasActionAliasInUse)))
				{
					flag = true;
				}
			}
			if (!flag && TestDistance && RigController.ActiveMotor != null)
			{
				if (DistanceCompareType == 0)
				{
					if (RigController.ActiveMotor.Distance < DistanceValue)
					{
						flag = true;
					}
				}
				else if (DistanceCompareType == 1)
				{
					if (RigController.ActiveMotor.Distance > DistanceValue)
					{
						flag = true;
					}
				}
				else if (Mathf.Abs(RigController.ActiveMotor.Distance - DistanceValue) < 0.001f)
				{
					flag = true;
				}
			}
			return flag;
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			UpdateToggle();
			if (mStartMotor == null || mEndMotor == null)
			{
				return mRigTransform;
			}
			mTransitionElapsedTime += rDeltaTime;
			float t = mTransitionElapsedTime / _TransitionTime;
			CameraTransform cameraTransform = mStartMotor.RigLateUpdate(rDeltaTime, rUpdateIndex, rTiltAngle);
			CameraTransform cameraTransform2 = mEndMotor.RigLateUpdate(rDeltaTime, rUpdateIndex, rTiltAngle);
			if (_PositionBlend == 1)
			{
				mRigTransform.Position = Vector3.Lerp(mStartPosition, cameraTransform2.Position, t);
			}
			else if (_PositionBlend == 2)
			{
				mRigTransform.Position = cameraTransform.Position;
			}
			else if (_PositionBlend == 3)
			{
				mRigTransform.Position = cameraTransform2.Position;
			}
			else
			{
				mRigTransform.Position = Vector3.Lerp(cameraTransform.Position, cameraTransform2.Position, t);
			}
			if (_RotationBlend == 1)
			{
				mRigTransform.Rotation = Quaternion.Slerp(mStartRotation, cameraTransform2.Rotation, t);
			}
			else if (_RotationBlend == 2)
			{
				mRigTransform.Rotation = cameraTransform.Rotation;
			}
			else if (_RotationBlend == 3)
			{
				mRigTransform.Rotation = cameraTransform2.Rotation;
			}
			else
			{
				mRigTransform.Rotation = Quaternion.Slerp(cameraTransform.Rotation, cameraTransform2.Rotation, t);
			}
			return mRigTransform;
		}

		public override void PostRigLateUpdate()
		{
			base.PostRigLateUpdate();
			if (mStartMotor != null)
			{
				mStartMotor.PostRigLateUpdate();
			}
			if (mEndMotor != null)
			{
				mEndMotor.PostRigLateUpdate();
			}
			if (mTransitionElapsedTime >= _TransitionTime)
			{
				int rIndex = (_EndMotorIndexOverride >= 0 && _EndMotorIndex < RigController.Motors.Count) ? _EndMotorIndexOverride : _EndMotorIndex;
				RigController.ActivateMotor(rIndex);
			}
		}

		protected virtual void UpdateToggle()
		{
			if (_ActionAlias.Length > 0 && RigController.InputSource != null)
			{
				mWasActionAliasInUse = mIsActionAliasInUse;
				mIsActionAliasInUse = (RigController.InputSource.GetValue(_ActionAlias) != 0f);
			}
		}

		public override void DeserializeMotor(string rDefinition)
		{
			base.DeserializeMotor(rDefinition);
			ActorStances = _ActorStances;
		}
	}
}
