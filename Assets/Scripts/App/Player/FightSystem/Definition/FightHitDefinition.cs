using System;

namespace App.Player.FightSystem.Definition
{
	[Serializable]
	public class FightHitDefinition
	{
		public float damage;

		public FightHitHeight height;

		public FightHitPower power;

		public float yAngleOffset;
	}
}
