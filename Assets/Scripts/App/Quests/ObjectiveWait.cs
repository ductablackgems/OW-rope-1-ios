using UnityEngine;

namespace App.Quests
{
	public class ObjectiveWait : GameplayObjective
	{
		[Header("Objective Wait")]
		[SerializeField]
		private float maxDistance = 4f;

		[SerializeField]
		private float failDistance = 20f;

		protected override void OnTimeIsUp()
		{
			if (Vector3.Distance(base.Position, base.Player.Transform.position) > maxDistance)
			{
				Fail();
			}
			else
			{
				Finish();
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (failDistance > 0f && Vector3.Distance(base.Position, base.Player.Transform.position) > failDistance)
			{
				Fail();
			}
		}
	}
}
