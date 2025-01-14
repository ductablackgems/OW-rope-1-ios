using App.SaveSystem;
using App.Util;
using System;
using UnityEngine;

namespace App.Player
{
	public class ItemsCollector : MonoBehaviour
	{
		public delegate void CollectHealBoxEventHandler(float amount);

		public AudioSource moneyAudioSource;

		public AudioSource healAudioSource;

		public AudioSource collectArmorAudioSource;

		private Health health;

		private PlayerSaveEntity playerSave;

		public event CollectHealBoxEventHandler OnCollectHealBox;

		public event Action OnCollectArmor;

		public event Action<CollectableItem> OnItemCollected;

		private void Awake()
		{
			health = this.GetComponentSafe<Health>();
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Money"))
			{
				CollectableMoney componentSafe = other.GetComponentSafe<CollectableMoney>();
				componentSafe.Collect();
				playerSave.score += componentSafe.value;
				playerSave.Save();
				UnityEngine.Object.Destroy(other.gameObject);
				moneyAudioSource.Play();
			}
			else if (other.CompareTag("HealBox"))
			{
				health.Heal(150f);
				other.gameObject.GetComponent<CollectableItem>().Collect();
				UnityEngine.Object.Destroy(other.gameObject);
				healAudioSource.Play();
				if (this.OnCollectHealBox != null)
				{
					this.OnCollectHealBox(150f);
				}
			}
			else if (other.CompareTag("Armor"))
			{
				playerSave.armor = 1f;
				playerSave.Save();
				other.gameObject.GetComponent<CollectableItem>().Collect();
				UnityEngine.Object.Destroy(other.gameObject);
				collectArmorAudioSource.Play();
				if (this.OnCollectArmor != null)
				{
					this.OnCollectArmor();
				}
			}
			else if (other.CompareTag("coin"))
			{
				UnityEngine.Object.Destroy(other.gameObject.transform.parent.gameObject, 1f);
				other.GetComponent<AudioSource>().Play();
				other.GetComponent<MeshRenderer>().enabled = false;
				other.GetComponent<Collider>().enabled = false;
			}
			else if (other.CompareTag("CollectableItem"))
			{
				CollectableItem component = other.gameObject.GetComponent<CollectableItem>();
				component.Collect();
				if (this.OnItemCollected != null)
				{
					this.OnItemCollected(component);
				}
				UnityEngine.Object.Destroy(other.gameObject);
			}
		}
	}
}
