using UnityEngine;

namespace App.Abilities
{
	public class AbilityShockWave : Ability
	{
		private ProgressiveExplosion explosion;

		private Transform effect;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			explosion = GetComponentInChildren<ProgressiveExplosion>(includeInactive: true);
			effect = base.transform.Find("Effect");
			ActivateEffects(activate: false, base.Owner.transform.position);
		}

		protected override void OnActivated(Vector3 position)
		{
			base.OnActivated(position);
			ActivateEffects(activate: true, position);
			position = new Vector3(position.x, base.Owner.transform.position.y, position.z);
			explosion.Explode(position, base.Owner);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			ActivateEffects(activate: false, base.Owner.transform.position);
		}

		private void ActivateEffects(bool activate, Vector3 position)
		{
			Transform parent = activate ? null : base.Owner.transform;
			explosion.transform.SetParent(parent);
			effect.transform.SetParent(parent);
			explosion.gameObject.SetActive(activate);
			effect.gameObject.SetActive(activate);
			explosion.transform.position = position;
			effect.transform.position = position;
		}
	}
}
