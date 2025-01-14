using App.Player;
using App.Rewards;
using App.SaveSystem;
using UnityEngine;

namespace App.ATM
{
	public class ATMManager : MonoBehaviour
	{
		private PlayerModel player;

		private PlayerSaveEntity playerSave;

		private CrimeManager crimeManager;

		private RewardManager rewardManager;

		public ATMInteractiveObject[] Terminals
		{
			get;
			private set;
		}

		public int GetDeposit()
		{
			return playerSave.bankDeposit;
		}

		public int GetInventoryMoney()
		{
			return playerSave.score;
		}

		public void DepositMoney()
		{
			playerSave.bankDeposit += playerSave.score;
			playerSave.score = 0;
			playerSave.Save();
		}

		public void WithdrawMoney()
		{
			playerSave.score += playerSave.bankDeposit;
			playerSave.bankDeposit = 0;
			playerSave.Save();
		}

		public void HackingFailed(ATMInteractiveObject atm)
		{
			float num = crimeManager.criminalityTresholds[0] * 1.2f;
			crimeManager.SetCrime(crimeManager.CriminalityValue + num);
		}

		public void HackingSucceded(int reward, int hackcount)
		{
			rewardManager.AssignHackingReward(reward,hackcount);
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			player = ServiceLocator.GetPlayerModel();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			crimeManager = ServiceLocator.Get<CrimeManager>();
			rewardManager = ServiceLocator.Get<RewardManager>();
			Terminals = GetComponentsInChildren<ATMInteractiveObject>();
			ATMInteractiveObject[] terminals = Terminals;
			foreach (ATMInteractiveObject aTMInteractiveObject in terminals)
			{
				player.PlayerMonitor.InteractiveObjectSensor.Register(aTMInteractiveObject);
				aTMInteractiveObject.Initialize(this);
			}
		}
	}
}
