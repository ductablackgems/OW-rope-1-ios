using System;
using UnityEngine;

namespace App.Player.FightSystem.Definition
{
	[Serializable]
	public class AnimationDefinition
	{
		[NonSerialized]
		public int hash;

		public string name;

		public float maxRemainExitTime;

		public bool preventYRotation;

		[Space]
		public bool kinematic;

		[Space]
		public bool useCustomTransition;

		public float customTransitionDuration;
	}
}
