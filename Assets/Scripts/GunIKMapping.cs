using System;
using UnityEngine;

namespace App.Player.Definition
{
	[Serializable]
	public struct GunIKMapping
	{
		public GunType gunType;

		public bool useLeftHandIKOffset;

		public Vector3 leftHandOffset;

		public bool useRightHandIK;

		public Vector3 rightHandIKLocalPosition;

		public Quaternion rightHandIKLocalRotation;

		public GunIKMapping(GunType gunType, Transform rightHandIKTarget)
		{
			this.gunType = gunType;
			useLeftHandIKOffset = false;
			leftHandOffset = Vector3.zero;
			if (rightHandIKTarget == null)
			{
				useRightHandIK = false;
				rightHandIKLocalPosition = Vector3.zero;
				rightHandIKLocalRotation = Quaternion.identity;
			}
			else
			{
				useRightHandIK = true;
				rightHandIKLocalPosition = rightHandIKTarget.localPosition;
				rightHandIKLocalRotation = rightHandIKTarget.localRotation;
			}
		}
	}
}
