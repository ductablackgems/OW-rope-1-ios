using App.Util;
using UnityEngine;

namespace App.Weapons
{
	[RequireComponent(typeof(Health))]
	public class Destroyer : MonoBehaviour
	{
		private Health health;

		private IExplosion explosion;

		protected void Awake()
		{
			health = this.GetComponentSafe<Health>();
			explosion = this.GetComponentSafe<IExplosion>();
		}

		protected void Update()
		{
			if (health.GetCurrentHealth() == 0f)
			{
				explosion.Explode();
			}
		}
	}
}
