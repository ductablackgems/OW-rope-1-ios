using UnityEngine;

namespace App.Abilities
{
	public class AbilityAbsorbShield : Ability
	{
		[Range(0f, 10000f)]
		[SerializeField]
		private float m_Capacity = 3000f;

		private Transform effect;

		private float capacity;

		public bool AbsorbDamage(float damage)
		{
			if (!base.IsRunning)
			{
				return false;
			}
			if (capacity == 0f)
			{
				return false;
			}
			capacity = Mathf.Max(0f, capacity - damage);
			if (capacity == 0f)
			{
				Deactivate();
				return false;
			}
			return true;
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			capacity = m_Capacity;
			effect = base.transform.Find("Effect");
			if (!(effect == null))
			{
				effect.localScale = Vector3.one * Radius;
				effect.gameObject.SetActive(value: true);
			}
		}

		protected override bool OnCanActivate()
		{
			if (!base.OnCanActivate())
			{
				return false;
			}
			return capacity > 0f;
		}
	}
}
