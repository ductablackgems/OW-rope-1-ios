using App.Util;

namespace App.Quests
{
	public class ObjectiveText : GameplayObjective
	{
		private Pauser pauser;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			pauser = ServiceLocator.Get<Pauser>();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (!pauser.IsGamePaused)
			{
				Finish();
			}
		}
	}
}
