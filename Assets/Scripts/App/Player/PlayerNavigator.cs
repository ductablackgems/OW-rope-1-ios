using App.AI;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.Player
{
	public class PlayerNavigator : MonoBehaviour
	{
		private NavMeshAgent agent;

		private NavMeshObstacle obstacle;

		private NavmeshWalker walker;

		private VehicleHandleSensor vehicleHandleSensor;

		private VehicleSensor playerCarSensor;

		private Transform target;

		private DurationTimer preventInterruptTimer = new DurationTimer();

		private DurationTimer interruptTimer = new DurationTimer();

		public bool Navigating
		{
			get;
			private set;
		}

		public event Action OnReachTargetHandle;

		public void NavigateTo(Transform target)
		{
			Navigating = true;
			this.target = target;
			walker.FollowTransform(this, target, NavmeshWalkerSpeed.RunFast);
			walker.enabled = true;
			agent.enabled = true;
			obstacle.enabled = false;
			base.enabled = true;
			preventInterruptTimer.Run(0.5f);
			interruptTimer.Run(20f);
		}

		public void NavigateToVehicle()
		{
			if (playerCarSensor.Triggered)
			{
				NavigateTo(playerCarSensor.Components.handleTrigger);
				vehicleHandleSensor.SetTargetHandle(playerCarSensor.Components.handleTrigger);
			}
		}

		public void Interrupt(bool force = false)
		{
			if (Navigating && (force || !preventInterruptTimer.InProgress()))
			{
				interruptTimer.Stop();
				Navigating = false;
				target = null;
				walker.enabled = false;
				agent.enabled = false;
				obstacle.enabled = true;
				base.enabled = false;
			}
		}

		private void Awake()
		{
			agent = this.GetComponentSafe<NavMeshAgent>();
			obstacle = this.GetComponentSafe<NavMeshObstacle>();
			walker = this.GetComponentInChildrenSafe<NavmeshWalker>();
			vehicleHandleSensor = this.GetComponentSafe<VehicleHandleSensor>();
			playerCarSensor = this.GetComponentInChildrenSafe<VehicleSensor>();
		}

		private void Update()
		{
			if (!interruptTimer.InProgress() || (!(target == null) && vehicleHandleSensor.TargetTriggered()))
			{
				Interrupt(force: true);
				if (this.OnReachTargetHandle != null)
				{
					this.OnReachTargetHandle();
				}
			}
		}
	}
}
