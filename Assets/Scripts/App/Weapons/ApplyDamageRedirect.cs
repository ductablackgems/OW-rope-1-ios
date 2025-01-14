using App.Util;
using UnityEngine;

namespace App.Weapons
{
	public class ApplyDamageRedirect : MonoBehaviour
	{
		public GameObject targetGO;

		private Health health;

		public Health GetHealth()
		{
			if (health == null)
			{
				health = targetGO.GetComponentSafe<Health>();
			}
			return health;
		}

		protected void ApplyDamage(float damage)
		{
			targetGO.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
		}
	}
}
