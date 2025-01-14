using App.Player.FightSystem.Definition;
using UnityEngine;

namespace App.Player.FightSystem
{
	public class FightMovementDefinitions : ScriptableObject
	{
		public FightMovementDefinition[] definitions;

		public FightMovementDefinition[] grabDefinitions;

		public FightMovementDefinition[] hitDefinitions;

		public FightMovementDefinition[] customHitDefinitions;
	}
}
