using App.GUI;
using App.Player;
using App.Player.Definition;
using App.Rewards;
using App.SaveSystem;
using App.Util;
using System;
using UnityEngine;

namespace App.Spawn
{
	public class PlayerRespawner : MonoBehaviour
	{
		public enum RespawnType
		{
			Busted,
			Dead
		}

		public int MoneyDeadPenalty = 200;

		private Transform hospitalRespawnPosition;

		private Transform policeRespawnPosition;

		private GameObject player;

		private Health health;

		private PlayerAnimatorHandler animatorHandler;

		private Resetor playeResetor;

		private PanelsManager panelsManager;

		private CrimeManager crimeManager;

		private DurationTimer showDeathScreenTimer = new DurationTimer();

		private DurationTimer respawnTimer = new DurationTimer();

		private SaveEntities saveEntities;

		private WeaponInventory weaponInventory;

		private RewardManager rewardManager;

		private int reklamaCount;

		public int LostMoney
		{
			get;
			private set;
		}

		public float LostArmor
		{
			get;
			private set;
		}

		public GunType LostWeapon
		{
			get;
			private set;
		}

		public RespawnType RespawnReason
		{
			get;
			private set;
		}

		public event Action AfterRespawn;

		public void ReturnLostItems()
		{
			ReturnLostWeaponAndArmor();
			ReturnLostMoney();
		}

		private void Awake()
		{
			hospitalRespawnPosition = ServiceLocator.GetGameObject("HospitalSpawnPosition").transform;
			policeRespawnPosition = ServiceLocator.GetGameObject("PoliceSpawnPosition").transform;
			player = ServiceLocator.GetGameObject("Player");
			health = player.GetComponentSafe<Health>();
			animatorHandler = player.GetComponentSafe<PlayerAnimatorHandler>();
			playeResetor = player.GetComponentSafe<Resetor>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
			crimeManager = ServiceLocator.Get<CrimeManager>();
			saveEntities = ServiceLocator.Get<SaveEntities>();
			weaponInventory = ServiceLocator.Get<WeaponInventory>();
			rewardManager = ServiceLocator.Get<RewardManager>();
			health.OnDie += OnDie;
		}

		private void OnDestroy()
		{
			health.OnDie -= OnDie;
		}

		private void Update()
		{
			if (crimeManager.Busted && !respawnTimer.Running())
			{
				respawnTimer.Run(3f);
			}
			if (showDeathScreenTimer.Done())
			{
				showDeathScreenTimer.Stop();
				respawnTimer.Run(3f);
				panelsManager.ShowPanel(PanelType.Dead);
			}
			if (respawnTimer.Done())
			{
				respawnTimer.Stop();
				LostMoney = 0;
				LostArmor = 0f;
				LostWeapon = GunType.Unknown;
				if (crimeManager.Busted)
				{
					crimeManager.SetBusted(busted: false);
					DropWeaponAndArmor();
					player.transform.position = policeRespawnPosition.position;
					RespawnReason = RespawnType.Busted;
				}
				else
				{
					DropMoney();
					player.transform.position = hospitalRespawnPosition.position;
					RespawnReason = RespawnType.Dead;
				}
				playeResetor.ResetStates();
				crimeManager.ResetStates();
				if (this.AfterRespawn != null)
				{
					this.AfterRespawn();
				}
				rewardManager.ResetRewardStates();
				panelsManager.ShowPanel(PanelType.Respawn);
				ServiceLocator.SendMessage(MessageID.Player.Respawned, this);
			}
		}

		private void OnDie()
		{
			showDeathScreenTimer.Run(3f);
			ServiceLocator.SendMessage(MessageID.Player.Died, this);
		}

		private void DropWeaponAndArmor()
		{
			PlayerSaveEntity playerSave = saveEntities.PlayerSave;
			GunType gunType = ServiceLocator.GetPlayerModel().ShotController.gunType;
			if (playerSave.armor > 0f)
			{
				LostArmor = playerSave.armor;
				playerSave.armor = 0f;
				playerSave.Save();
			}
			if (gunType == GunType.Unknown)
			{
				DropMoney();
				return;
			}
			LostWeapon = gunType;
			weaponInventory.SwitchGun();
			GunSaveEntity gunSave = playerSave.GetGunSave(gunType);
			gunSave.buyed = false;
			gunSave.Save();
		}

		private void DropMoney()
		{
			PlayerSaveEntity playerSave = saveEntities.PlayerSave;
			int num = Mathf.Max(0, playerSave.score - MoneyDeadPenalty);
			if (num != playerSave.score)
			{
				LostMoney = MoneyDeadPenalty;
				playerSave.score = num;
				playerSave.Save();
			}
		}

		private void ReturnLostWeaponAndArmor()
		{
			PlayerSaveEntity playerSave = saveEntities.PlayerSave;
			if (LostArmor > 0f)
			{
				playerSave.armor = LostArmor;
				playerSave.Save();
				LostArmor = 0f;
			}
			if (LostWeapon != 0)
			{
				GunSaveEntity gunSave = playerSave.GetGunSave(LostWeapon);
				gunSave.buyed = true;
				gunSave.Save();
				LostWeapon = GunType.Unknown;
			}
		}

		private void ReturnLostMoney()
		{
			if (LostMoney > 0)
			{
				PlayerSaveEntity playerSave = saveEntities.PlayerSave;
				playerSave.score += LostMoney;
				playerSave.Save();
				LostMoney = 0;
			}
		}
	}
}
