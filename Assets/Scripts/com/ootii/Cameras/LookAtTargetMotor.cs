using com.ootii.Base;
using System;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("Look At Motor")]
	[BaseDescription("Simple motor used to always look at a specific object.")]
	public class LookAtTargetMotor : CameraMotor
	{
		public bool _UseCurrentPosition = true;

		public Vector3 _Position = Vector3.zero;

		public int _TargetIndex = -1;

		[NonSerialized]
		public Transform _Target;

		public Vector3 _TargetOffset = Vector3.zero;

		protected bool mIsPositionSet;

		public bool UseCurrentPosition
		{
			get
			{
				return _UseCurrentPosition;
			}
			set
			{
				_UseCurrentPosition = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				return _Position;
			}
			set
			{
				_Position = value;
				mIsPositionSet = (_Position.sqrMagnitude > 0f);
			}
		}

		public virtual int TargetIndex
		{
			get
			{
				return _TargetIndex;
			}
			set
			{
				_TargetIndex = value;
				if (_TargetIndex >= 0 && RigController != null && _TargetIndex < RigController.StoredTransforms.Count)
				{
					_Target = RigController.StoredTransforms[_TargetIndex];
				}
			}
		}

		public virtual Transform Target
		{
			get
			{
				return _Target;
			}
			set
			{
				_Target = value;
			}
		}

		public virtual Vector3 TargetOffset
		{
			get
			{
				return _TargetOffset;
			}
			set
			{
				_TargetOffset = value;
			}
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			if (RigController == null)
			{
				return mRigTransform;
			}
			Vector3 up = Vector3.up;
			if (mIsPositionSet)
			{
				mRigTransform.Position = _Position;
			}
			else if (_UseCurrentPosition)
			{
				mRigTransform.Position = RigController._Transform.position;
			}
			else if (Anchor != null)
			{
				up = Anchor.up;
				mRigTransform.Position = AnchorPosition + Anchor.rotation * _Offset;
			}
			else
			{
				mRigTransform.Position = _Offset;
			}
			Vector3 forward = Vector3.forward;
			forward = ((!(_Target == null)) ? (_Target.position + _TargetOffset - mRigTransform.Position).normalized : (_TargetOffset - mRigTransform.Position).normalized);
			mRigTransform.Rotation = Quaternion.LookRotation(forward, up);
			return mRigTransform;
		}

		public override void DeserializeMotor(string rDefinition)
		{
			base.DeserializeMotor(rDefinition);
			if (_TargetIndex >= 0)
			{
				if (_TargetIndex < RigController.StoredTransforms.Count)
				{
					_Target = RigController.StoredTransforms[_TargetIndex];
				}
				else
				{
					_Target = null;
					_TargetIndex = -1;
				}
			}
			IsCollisionEnabled = false;
			IsFadingEnabled = false;
		}
	}
}
