using App.Util;

namespace App.Player
{
	public class PlayerEnergyBar : EnergyBar
	{
		protected void Start()
		{
			energy = ServiceLocator.GetGameObject("Player").GetComponentSafe<EnergyScript>();
		}
	}
}
