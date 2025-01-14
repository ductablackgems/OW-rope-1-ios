using App.Util;
using UnityEngine;

namespace App.Player
{
	public class PlayerControlDeactivator : MonoBehaviour
	{
		private HelicopterController controller;

		private TiltController tiltController;

		private Health health;

		public void Deactive()
		{
			controller.enabled = false;
			tiltController.enabled = false;
		}

		protected void Awake()
		{
			controller = this.GetComponentSafe<HelicopterController>();
			tiltController = this.GetComponentInChildrenSafe<TiltController>();
			health = this.GetComponentInChildrenSafe<Health>();
		}

		protected void Update()
		{
			if (health.GetCurrentHealth() == 0f && (controller.enabled || tiltController.enabled))
			{
				Deactive();
			}
		}
	}
}
