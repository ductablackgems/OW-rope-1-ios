using System;
using UnityEngine;

namespace App.Player.FightSystem.Definition
{
	[Serializable]
	public class FightMovementDefinition
	{
		public string tid;

		public AnimationDefinition[] animations = new AnimationDefinition[1];

		public FightHitDefinition[] hits = new FightHitDefinition[1];

		[Space]
		public bool useCustomHit;

		public string customHitTid;

		public bool testSpace;

		public CapsuleDefinition testSpaceCapsule;

		[Space]
		public float ragdollTime;

		public bool alwaysRagdollize;

		public bool useCustomRagdollSpeed;

		public Vector3 ragdollSpeed;
	}
}
