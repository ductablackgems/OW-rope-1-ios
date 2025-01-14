using System;
using UnityEngine;

namespace App.Player
{
	public class CharacterRotator : MonoBehaviour
	{
		public float flipRotationSpeed = 350f;

		public float standUpRotationSpeed = 350f;

		public float commonRotationSpeed = 180f;

		public float flipDownForce = 1000f;

		private Rigidbody _rigidbody;

		private DurationTimer rotationDelayTimer = new DurationTimer();

		private float speedCoeff = 1f;

		private Vector3 rotationAxis;

		private float rotationSpeed;

		public bool FixingRotation
		{
			get;
			private set;
		}

		public Transform RotationPoint
		{
			get;
			private set;
		}

		public Quaternion TargetRotation
		{
			get;
			private set;
		}

		public CharacterRotationType RotationType
		{
			get;
			private set;
		}

		public void StandUp(Quaternion targetRotation, Transform rotationPoint)
		{
			RotationType = CharacterRotationType.StandUp;
			rotationSpeed = standUpRotationSpeed;
			speedCoeff = ((base.transform.up.y < -0.1f) ? 1.5f : 1f);
			FixingRotation = true;
			TargetRotation = targetRotation;
			RotationPoint = rotationPoint;
		}

		public void Flip(Transform rotationPoint)
		{
			RotationPoint = rotationPoint;
			RotationType = CharacterRotationType.Flip;
			FixingRotation = true;
			Vector3 normalized = new Vector3(rotationPoint.right.x, 0f, rotationPoint.right.z).normalized;
			rotationAxis = (RotationPoint.right + normalized).normalized;
		}

		public void CommonRotate(Quaternion targetRotation, Transform rotationPoint)
		{
			RotationType = CharacterRotationType.LostWall;
			rotationSpeed = commonRotationSpeed;
			speedCoeff = ((base.transform.up.y >= 0f) ? 1 : 2);
			FixingRotation = true;
			TargetRotation = targetRotation;
			RotationPoint = rotationPoint;
		}

		public void Interrupt()
		{
			FixingRotation = false;
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
		}

		private void FixedUpdate()
		{
			if (!FixingRotation || rotationDelayTimer.InProgress())
			{
				return;
			}
			if (RotationType == CharacterRotationType.LostWall || RotationType == CharacterRotationType.StandUp)
			{
				Vector3 position = RotationPoint.position;
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, TargetRotation, Time.fixedDeltaTime * rotationSpeed * speedCoeff);
				base.transform.position = base.transform.position + position - RotationPoint.position;
				if (Quaternion.Angle(TargetRotation, base.transform.rotation) < 1f)
				{
					base.transform.rotation = TargetRotation;
					FixingRotation = false;
				}
			}
			else if (RotationType == CharacterRotationType.Flip)
			{
				base.transform.RotateAround(rotationAxis, Time.fixedDeltaTime * flipRotationSpeed * ((float)Math.PI / 180f));
				Quaternion quaternion = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
				if (Quaternion.Angle(quaternion, base.transform.rotation) < 15f)
				{
					base.transform.rotation = quaternion;
					FixingRotation = false;
				}
				_rigidbody.AddForce(Vector3.down * flipDownForce);
			}
		}
	}
}
