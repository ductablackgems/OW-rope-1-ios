using App.AI;
using App.Base;
using App.Player;
using App.SaveSystem;
using App.Settings;
using App.Util;
using UnityEngine;

namespace App.Dogs
{
	public class Dog : BaseBehaviour
	{
		private PlayerModel player;

		private DogSaveEntity saveEntity;

		private DogAIController aiController;

		private Transform model;

		public DogSettingsItem Settings
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public float Damage
		{
			get;
			private set;
		}

		public float Speed
		{
			get;
			private set;
		}

		public Health Health
		{
			get;
			private set;
		}

		public GameObject Owner
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			private set;
		}

		public DogSpot Spot
		{
			get;
			private set;
		}

		public bool IsSpawned
		{
			get;
			private set;
		}

		public bool IsAlive => !Health.Dead();

		public bool IsPrimary => saveEntity.Slot == DogManager.DogSlot.Primary;

		public bool IsEquipped => saveEntity.Slot != DogManager.DogSlot.None;

		public bool IsOwned => saveEntity.isOwned;

		public DogManager.DogSlot Slot => saveEntity.Slot;

		public void Initialize(DogSettingsItem settings, DogSaveEntity dogSave, DogSpot spot, PlayerModel playerModel)
		{
			Level = -1;
			Settings = settings;
			Health = GetComponent<Health>();
			player = playerModel;
			saveEntity = dogSave;
			Owner = player.GameObject;
			Spot = spot;
			aiController = GetComponent<DogAIController>();
			model = GetComponentInChildren<Animator>().transform;
			Name = ((!string.IsNullOrEmpty(saveEntity.name)) ? saveEntity.name : Settings.Name);
			aiController.Initialize();
			SetLevel(saveEntity.level);
			if (saveEntity.isDead)
			{
				Health.Kill();
			}
			Physics.IgnoreCollision(GetComponent<Collider>(), playerModel.GameObject.GetComponent<Collider>());
			ServiceLocator.Messages.Subscribe(MessageID.Dog.Died, this, OnDogDied);
		}

		public void ResetScale()
		{
			model.localScale = Vector3.one;
		}

		public void Deinitialize()
		{
			ServiceLocator.Messages.Unsubscribe(MessageID.Dog.Died, this, OnDogDied);
		}

		public void Spawn()
		{
			base.transform.position = Spot.Position;
			SetActive(isActive: true);
			aiController.Spawn(Spot.Position);
			IsSpawned = true;
		}

		public void Despawn()
		{
			aiController.Despawn();
			SetActive(isActive: false);
			IsSpawned = false;
		}

		public void Respawn()
		{
			SetActive(isActive: true);
			aiController.Respawn();
			Revive();
		}

		public void RegisterKill()
		{
			DogSettings dogSettings = SettingsManager.DogSettings;
			saveEntity.experience += dogSettings.ExpPerKill;
			int level = dogSettings.GetLevel(saveEntity.experience);
			if (level > Level)
			{
				SetLevel(level);
				saveEntity.level = level;
			}
			saveEntity.Save();
		}

		public void AttackTargetInDirection()
		{
			aiController.AttackTargetsInDirection();
		}

		public void Heal()
		{
			if (IsAlive)
			{
				Health.ResetHealth();
			}
		}

		public void Revive()
		{
			saveEntity.isDead = false;
			saveEntity.Save();
			Health.ResetHealth();
			ServiceLocator.SendMessage(MessageID.Dog.Revived, this);
		}

		public void SetModelScale(float scale)
		{
			model.localScale = new Vector3(scale, scale, scale);
		}

		private void OnDogDied(object sender, object data)
		{
			if (!(data as Dog != this))
			{
				RegisterDeath();
			}
		}

		private void SetupStats()
		{
			Speed = Settings.WalkSpeed;
			Health.maxHealth = GetScaledValue(Settings.MinHealth, Settings.MaxHealth);
			Damage = GetScaledValue(Settings.MinDamage, Settings.MaxDamage);
			float scaledValue = GetScaledValue(Settings.MinScale, Settings.MaxScale);
			SetModelScale(scaledValue);
			UnityEngine.Debug.LogFormat("DogID [{0}] Level Set [{1}] Health [{2}] Damage [{3}] Scale [{4}]", Settings.ID, Level, Health.maxHealth, Damage, scaledValue);
		}

		private void SetLevel(int newLevel)
		{
			if (Level < newLevel && Level < SettingsManager.DogSettings.GetMaxLevel())
			{
				Level = newLevel;
				SetupStats();
				Heal();
			}
		}

		private void RegisterDeath()
		{
			float num = (float)Level * SettingsManager.DogSettings.DeathPenalty;
			int num2 = (int)((float)saveEntity.experience * num);
			saveEntity.experience -= num2;
			saveEntity.isDead = true;
			saveEntity.Save();
		}

		private float GetScaledValue(float minValue, float maxValue)
		{
			int maxLevel = SettingsManager.DogSettings.GetMaxLevel();
			float num = (maxValue - minValue) / (float)maxLevel;
			return minValue + num * (float)Level;
		}
	}
}
