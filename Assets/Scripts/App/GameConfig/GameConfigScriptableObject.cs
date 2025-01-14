using UnityEngine;

namespace App.GameConfig
{
	public class GameConfigScriptableObject : ScriptableObject
	{
		public GameTitle title;

		public int killMoneyPercentage;

		public int minKillMoney;

		public int maxKillMoney;
	}
}
