using App.Spawn;
using UnityEngine;

namespace App.Util
{
	public class DeathBloodManager : MonoBehaviour, IResetable
	{
		public GameObject bloodObject;

		private Health health;

		private DurationTimer hideBloodTimer = new DurationTimer();

		public void ResetStates()
		{
			hideBloodTimer.Stop();
			base.enabled = false;
			bloodObject.SetActive(value: false);
		}

		private void Awake()
		{
			health = this.GetComponentSafe<Health>();
			health.OnDie += OnDie;
			hideBloodTimer.Stop();
			base.enabled = false;
			bloodObject.SetActive(value: false);
		}

		private void OnDestroy()
		{
			health.OnDie -= OnDie;
		}

		private void Update()
		{
			if (!hideBloodTimer.InProgress())
			{
				hideBloodTimer.Stop();
				base.enabled = false;
				bloodObject.SetActive(value: false);
			}
		}

		private void OnDie()
		{
			hideBloodTimer.Run(3f);
			base.enabled = true;
			bloodObject.SetActive(value: true);
		}
	}
}
