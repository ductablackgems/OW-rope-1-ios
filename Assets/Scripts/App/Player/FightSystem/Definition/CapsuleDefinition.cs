using System;
using UnityEngine;

namespace App.Player.FightSystem.Definition
{
	[Serializable]
	public class CapsuleDefinition
	{
		public Vector3 start;

		public Vector3 end;

		public float radius = 1f;
	}
}
