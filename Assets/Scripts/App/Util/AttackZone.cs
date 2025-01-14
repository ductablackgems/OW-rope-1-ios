using App.Weapons;
using System.Collections;
using UnityEngine;

namespace App.Util
{
	public class AttackZone : MonoBehaviour
	{
		public string[] targetTags = new string[1]
		{
			"Player"
		};

		private Health targetHealth;

		private TriggerMonitor targetTrigger = new TriggerMonitor();

		public Health TargetHealth
		{
			get
			{
				if (!targetTrigger.IsTriggered())
				{
					return null;
				}
				return targetHealth;
			}
		}

		public bool IsIn => targetTrigger.IsTriggered();

		protected void FixedUpdate()
		{
			targetTrigger.FixedUpdate();
		}

		protected void OnTriggerStay(Collider other)
		{
			Vector3 a = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			Component marketTrigger = targetTrigger.GetMarketTrigger();
			float num = (marketTrigger == null) ? 0f : Vector3.Distance(a, new Vector3(marketTrigger.transform.position.x, 0f, marketTrigger.transform.position.z));
			string[] array = targetTags;
			int num2 = 0;
			Health otherHealth;
			while (true)
			{
				if (num2 >= array.Length)
				{
					return;
				}
				string tag = array[num2];
				if (other.CompareTag(tag))
				{
					otherHealth = GetOtherHealth(other);
					if (otherHealth != null && !otherHealth.Dead())
					{
						break;
					}
				}
				num2++;
			}
			if (!(marketTrigger != null) || !(Vector3.Distance(a, new Vector3(otherHealth.transform.position.x, 0f, otherHealth.transform.position.z)) > num))
			{
				targetHealth = otherHealth;
				targetTrigger.MarkTrigger(targetHealth);
			}
		}

		private Health GetOtherHealth(Collider other)
		{
			Health health = other.GetComponent<Health>();
			if (health == null)
			{
				ApplyDamageRedirect component = other.GetComponent<ApplyDamageRedirect>();
				if (component != null)
				{
					health = component.GetHealth();
				}
			}
			return health;
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(3f);
			if (base.transform.parent != base.transform.root)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
