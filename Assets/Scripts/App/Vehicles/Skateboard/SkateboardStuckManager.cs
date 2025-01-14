using App.Player.Definition;
using App.Vehicles.Bicycle;
using UnityEngine;

namespace App.Vehicles.Skateboard
{
	public class SkateboardStuckManager : MonoBehaviour
	{
		public float stuckSpeed = 0.01f;

		public float stuckDelay = 0.3f;

		private PlayerSkateboardController skateboardController;

		private Rigidbody _rigidbody;

		private StreetVehicleCrasher crasher;

		private PlayerAnimatorHandler animatorHandler;

		private DurationTimer stuckTimer = new DurationTimer();

		private void Awake()
		{
			skateboardController = this.GetComponentSafe<PlayerSkateboardController>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			crasher = this.GetComponentSafe<StreetVehicleCrasher>();
			animatorHandler = ServiceLocator.GetGameObject("Player").GetComponentSafe<PlayerAnimatorHandler>();
		}

		private void OnDisable()
		{
			stuckTimer.Stop();
		}

		private void Update()
		{
			if (skateboardController.InAir && Mathf.Abs(_rigidbody.velocity.y) < stuckSpeed)
			{
				if (stuckTimer.Done())
				{
					stuckTimer.Stop();
					crasher.Crash();
				}
				else if (!stuckTimer.Running())
				{
					stuckTimer.Run(stuckDelay);
				}
			}
			else
			{
				stuckTimer.Run(stuckDelay);
			}
		}
	}
}
