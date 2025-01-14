using App.Dogs;
using System;
using UnityEngine;

namespace App.Settings
{
	[Serializable]
	public class DogSettingsItem : SettingsItem
	{
		[SerializeField]
		private Dog prefab;

		[SerializeField]
		private string name = "Dog";

		[SerializeField]
		private int price;

		[SerializeField]
		private float minHealth = 100f;

		[SerializeField]
		private float maxHealth = 1000f;

		[SerializeField]
		private float walkSpeed = 2f;

		[SerializeField]
		private float runSpeed = 6f;

		[SerializeField]
		private float minScale = 0.6f;

		[SerializeField]
		private float maxScale = 1.2f;

		[Header("Combat")]
		[SerializeField]
		private float attackRange = 1f;

		[SerializeField]
		private float minDamage = 50f;

		[SerializeField]
		private float maxDamage = 100f;

		public Dog Prefab => prefab;

		public string Name => name;

		public int Price => price;

		public float MinHealth => minHealth;

		public float MaxHealth => maxHealth;

		public float MinDamage => minDamage;

		public float MaxDamage => maxDamage;

		public float WalkSpeed => walkSpeed;

		public float RunSpeed => runSpeed;

		public float AttackRange => attackRange;

		public float MinScale => minScale;

		public float MaxScale => maxScale;
	}
}
