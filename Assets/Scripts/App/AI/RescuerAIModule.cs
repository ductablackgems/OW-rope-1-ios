using App.AI.Monitor;
using App.Player;
using App.Player.Definition;
using App.Spawn;
using App.Vehicles;
using System;

namespace App.AI
{
	public class RescuerAIModule : AbstractAIScript, IResetable
	{
		[NonSerialized]
		public VehicleComponents myVehicle;

		private PlayerAnimatorHandler animatorHandler;

		private CarDriver carDriver;

		private VehicleHandleSensor vehicleHandleSensor;

		private NavmeshWalker walker;

		private DeathEnemiesMonitor deathEnemiesMonitor;

		private DurationTimer healTimer = new DurationTimer();

		private DurationTimer openDoorsTimer = new DurationTimer();

		private DurationTimer checkDeathsTimer = new DurationTimer();

		public DeathTarget DeathTarget
		{
			get;
			set;
		}

		public void ResetStates()
		{
			animatorHandler.Rescue = false;
			myVehicle = null;
			DeathTarget = default(DeathTarget);
			healTimer.Stop();
			openDoorsTimer.Stop();
			checkDeathsTimer.Stop();
		}

		private void Awake()
		{
			animatorHandler = base.ComponentsRoot.GetComponentSafe<PlayerAnimatorHandler>();
			carDriver = base.ComponentsRoot.GetComponentSafe<CarDriver>();
			vehicleHandleSensor = base.ComponentsRoot.GetComponentSafe<VehicleHandleSensor>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			deathEnemiesMonitor = ServiceLocator.Get<DeathEnemiesMonitor>();
		}

		private void OnEnable()
		{
			if (myVehicle != null)
			{
				vehicleHandleSensor.SetTargetHandle(myVehicle.handleTrigger);
				if (DeathTarget.IsValid())
				{
					walker.FollowTransform(this, DeathTarget.TargetTransform, NavmeshWalkerSpeed.Run);
				}
			}
		}

		private void OnDisable()
		{
			walker.Stop();
			animatorHandler.Rescue = false;
			DeathTarget = default(DeathTarget);
			healTimer.Stop();
			openDoorsTimer.Stop();
			checkDeathsTimer.Stop();
		}

		private void Update()
		{
			if (DeathTarget.IsValid() && !animatorHandler.Rescue && (double)(DeathTarget.TargetTransform.position - base.ComponentsRoot.position).magnitude <= 0.8)
			{
				animatorHandler.Rescue = true;
				walker.Stop();
				healTimer.Run(5f);
			}
			if (DeathTarget.IsValid() && healTimer.GetProgress() > 0.5f)
			{
				DeathTarget.Health.ResetHealth(0.5f);
			}
			if (healTimer.Done())
			{
				healTimer.Stop();
				animatorHandler.Rescue = false;
				DeathTarget = deathEnemiesMonitor.FindHealthTarget(base.ComponentsRoot.position, 30f);
				if (DeathTarget.IsValid())
				{
					walker.FollowTransform(this, DeathTarget.TargetTransform, NavmeshWalkerSpeed.Run);
				}
			}
			if (DeathTarget.HasHealth && !DeathTarget.IsValid())
			{
				DeathTarget = default(DeathTarget);
			}
			if (!DeathTarget.IsValid())
			{
				if (!checkDeathsTimer.Running())
				{
					checkDeathsTimer.Run(3f);
				}
				if (checkDeathsTimer.Done())
				{
					checkDeathsTimer.Run(3f);
					DeathTarget = deathEnemiesMonitor.FindHealthTarget(base.ComponentsRoot.position, 30f);
					if (DeathTarget.IsValid())
					{
						walker.FollowTransform(this, DeathTarget.TargetTransform, NavmeshWalkerSpeed.Run);
					}
				}
			}
			if (DeathTarget.IsValid())
			{
				return;
			}
			if (myVehicle != null && walker.TargetTransform != myVehicle.handleTrigger)
			{
				walker.FollowTransform(this, myVehicle.handleTrigger, NavmeshWalkerSpeed.Walk);
			}
			if (carDriver.Runnable(useTargetTrigger: true) && myVehicle.OpenableVelocity())
			{
				if (openDoorsTimer.Done())
				{
					walker.Stop();
					carDriver.Run(onlyThrowOutDriver: true, useTargetTrigger: true);
					openDoorsTimer.Stop();
				}
				else if (!openDoorsTimer.Running())
				{
					walker.FollowTransform(this, myVehicle.handleTrigger, NavmeshWalkerSpeed.Walk);
					openDoorsTimer.Run(0.8f);
				}
				else
				{
					walker.FollowTransform(this, myVehicle.handleTrigger, NavmeshWalkerSpeed.Walk);
				}
			}
		}
	}
}
