using UnityEngine;

namespace App.Quests
{
	public class ObjectiveGoTo : GameplayObjective
	{
		[Header("Objective Go To")]
		[SerializeField]
		private float minDistance = 2f;

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (!(Vector3.Distance(base.Player.Transform.position, base.Position) > minDistance))
			{
				Finish();
			}
		}
	}
}
