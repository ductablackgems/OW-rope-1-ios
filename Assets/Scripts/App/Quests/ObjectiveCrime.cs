using App.Player;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveCrime : GameplayObjective
	{
		[Header("Objective Crime")]
		[SerializeField]
		private int numberOfStars;

		private CrimeManager crimeManager;

		private float crime;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			crimeManager = ServiceLocator.Get<CrimeManager>();
			int num = Mathf.Clamp(numberOfStars - 1, 0, crimeManager.criminalityTresholds.Length - 1);
			crime = ((numberOfStars > 0) ? crimeManager.criminalityTresholds[num] : 0f);
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			crimeManager.SetCrime(crime + crime * 0.2f);
			Finish();
		}
	}
}
