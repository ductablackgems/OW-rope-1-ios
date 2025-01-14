using App.Util;

namespace App.Player
{
	public class PlayerHealthBar : HealthBar
	{
		protected void Start()
		{
			health = ServiceLocator.GetGameObject("Player").GetComponentSafe<Health>();
		}
	}
}
