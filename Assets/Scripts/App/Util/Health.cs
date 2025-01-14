using App.Player;
using App.Spawn;
using System;
using UnityEngine;

namespace App.Util
{
	public class Health : MonoBehaviour, IResetable
	{
		public delegate void DamageEventHandler(float damage, int damageType, GameObject agressor);

		public float maxHealth = 500f;

		private IDamageBlocker damageBlocker;

		private Armor armor;

		private Transform targetBone;

		private float currentHealth;

		private float lastDamageTime = -1f;

		private bool initialized;

		public int DeathDamageType
		{
			get;
			private set;
		}

		public bool IsLaserImmunity
		{
			get;
			set;
		}

		public bool IsSquashed
		{
			get;
			private set;
		}

		public event DamageEventHandler OnDamage;

		public event Action OnDie;

		public void ResetStates()
		{
			currentHealth = maxHealth;
			lastDamageTime = -1f;
			DeathDamageType = 0;
		}

		public void ResetHealth(float ammount = 1f)
		{
			currentHealth = maxHealth * ammount;
		}

		public float GetCurrentHealth()
		{
			if (!initialized)
			{
				Init();
			}
			return currentHealth / maxHealth;
		}

		public float GetCurrentHealthNumeric()
		{
			if (!initialized)
			{
				Init();
			}
			return currentHealth;
		}

		public void Squash(bool isCritical, int damageType = 0)
		{
			IsSquashed = true;
			float damage = currentHealth;
			if (!isCritical)
			{
				float num = maxHealth * 0.4f;
				damage = ((currentHealth > num) ? (currentHealth - num) : 0f);
			}
			ApplyDamage(damage, damageType);
		}

		public void Kill(int deathDamageType = 0)
		{
			if (!initialized)
			{
				Init();
			}
			if (!Dead())
			{
				currentHealth = 0f;
				DeathDamageType = deathDamageType;
				if (this.OnDie != null)
				{
					this.OnDie();
				}
			}
		}

		public bool Dead()
		{
			if (!initialized)
			{
				Init();
			}
			return currentHealth == 0f;
		}

		public void ApplyDamage(float damage, int damageType = 0, GameObject agressor = null)
		{
			if ((IsLaserImmunity && damageType == 4) || (damageBlocker != null && damageBlocker.IsDamageBlocked(damage)))
			{
				return;
			}
			if (!initialized)
			{
				Init();
			}
			if (Dead())
			{
				return;
			}
			if (armor != null)
			{
				damage = armor.AbsorbDamage(damage);
			}
			currentHealth -= damage;
			lastDamageTime = Time.time;
			if (!base.enabled)
			{
				base.enabled = true;
			}
			if (this.OnDamage != null)
			{
				this.OnDamage(damage, damageType, agressor);
			}
			if (currentHealth <= 0f)
			{
				currentHealth = 0f;
				DeathDamageType = damageType;
				if (this.OnDie != null)
				{
					this.OnDie();
				}
			}
		}

		public bool AttackedRecently(float interval)
		{
			if (lastDamageTime != -1f)
			{
				return Time.time - interval < lastDamageTime;
			}
			return false;
		}

		public void Heal(float amount)
		{
			if (!initialized)
			{
				Init();
			}
			if (!Dead())
			{
				currentHealth += amount;
				if (currentHealth > maxHealth)
				{
					currentHealth = maxHealth;
				}
			}
		}

		public Transform GetTargetBone()
		{
			if (!initialized)
			{
				Init();
			}
			return targetBone;
		}

		protected void Start()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void Init()
		{
			armor = GetComponent<Armor>();
			initialized = true;
			currentHealth = maxHealth;
			Animator component = GetComponent<Animator>();
			damageBlocker = GetComponent<IDamageBlocker>();
			if (component == null)
			{
				targetBone = base.transform;
			}
			else
			{
				// targetBone = component.GetBoneTransform(HumanBodyBones.Hips);
			}
		}
	}
}
