using App.SaveSystem;
using UnityEngine;

namespace App.Player
{
	public class Armor : MonoBehaviour
	{
		public GameObject armorMesh;

		public float damageCapacity = 200f;

		private PlayerSaveEntity playerSave;

		private MagicShield magicShield;

		public float CurrentArmor
		{
			get;
			private set;
		}

		public float AbsorbDamage(float damage)
		{
			if ((bool)magicShield && magicShield.DefenseRunning)
			{
				return 0f;
			}
			if (CurrentArmor == 0f)
			{
				return damage;
			}
			float num = damage / damageCapacity;
			if (num >= CurrentArmor)
			{
				damage = (num - CurrentArmor) * damageCapacity;
				CurrentArmor = 0f;
				playerSave.armor = 0f;
				playerSave.Save(includeChildren: false);
				armorMesh.SetActive(value: false);
				return damage;
			}
			CurrentArmor -= num;
			playerSave.armor = CurrentArmor;
			playerSave.Save(includeChildren: false);
			return 0f;
		}

		private void Awake()
		{
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			magicShield = GetComponent<MagicShield>();
			CurrentArmor = playerSave.armor;
			playerSave.OnSave += OnPlayerSave;
		}

		private void Start()
		{
			armorMesh.SetActive(CurrentArmor > 0f);
		}

		private void OnDestroy()
		{
			playerSave.OnSave -= OnPlayerSave;
		}

		private void OnPlayerSave(AbstractSaveEntity saveEntity)
		{
			if (CurrentArmor != playerSave.armor)
			{
				CurrentArmor = playerSave.armor;
				armorMesh.SetActive(CurrentArmor > 0f);
			}
		}
	}
}
