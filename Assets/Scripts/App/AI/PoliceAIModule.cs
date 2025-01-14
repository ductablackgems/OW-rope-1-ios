using App.AI.Monitor;
using App.Player;
using App.Player.Definition;
using App.Spawn;
using App.Vehicles;
using System;
using UnityEngine;

namespace App.AI
{
	public class PoliceAIModule : AbstractAIScript, IResetable
	{
		[NonSerialized]
		public VehicleComponents myVehicle;

		private PlayerAnimatorHandler animatorHandler;

		private CarDriver carDriver;

		private BikeDriver bikeDriver;

		private VehicleHandleSensor vehicleHandleSensor;

		private NavmeshWalker walker;

		private DeathEnemiesMonitor deathEnemiesMonitor;

		private DurationTimer openDoorsTimer = new DurationTimer();

		private DurationTimer checkDeathsTimer = new DurationTimer();

		private DurationTimer blendInvestigateTimer = new DurationTimer();

		public DeathTarget DeathTarget
		{
			get;
			set;
		}

		public void ResetStates()
		{
			animatorHandler.Investigate = false;
			myVehicle = null;
			DeathTarget = default(DeathTarget);
			openDoorsTimer.Stop();
			checkDeathsTimer.Stop();
			blendInvestigateTimer.Stop();
		}

		private void Awake()
		{
			animatorHandler = base.ComponentsRoot.GetComponentSafe<PlayerAnimatorHandler>();
			carDriver = base.ComponentsRoot.GetComponentSafe<CarDriver>();
			bikeDriver = base.ComponentsRoot.GetComponentSafe<BikeDriver>();
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
					walker.FollowTransform(this, DeathTarget.TargetTransform, NavmeshWalkerSpeed.Walk);
				}
			}
		}

		private void OnDisable()
		{
			walker.Stop();
			animatorHandler.Investigate = false;
			DeathTarget = default(DeathTarget);
			openDoorsTimer.Stop();
			checkDeathsTimer.Stop();
		}

		private void Update()
		{
			if (DeathTarget.IsValid() && !animatorHandler.Investigate && (DeathTarget.TargetTransform.position - base.ComponentsRoot.position).magnitude <= 2f)
			{
				animatorHandler.InvestigateBlend.ForceValue(UnityEngine.Random.Range(0, 2));
				animatorHandler.Investigate = true;
				walker.Stop();
				blendInvestigateTimer.Run(UnityEngine.Random.Range(4f, 6f));
			}
			if (!DeathTarget.IsValid() && animatorHandler.Investigate)
			{
				animatorHandler.Investigate = false;
				DeathTarget = default(DeathTarget);
				blendInvestigateTimer.Stop();
			}
			if (blendInvestigateTimer.Done())
			{
				blendInvestigateTimer.Run(UnityEngine.Random.Range(4f, 6f));
				animatorHandler.InvestigateBlend.BlendTo((animatorHandler.InvestigateBlend.TargetValue == 0f) ? 1 : 0);
			}
			if (!animatorHandler.InvestigateBlend.Done)
			{
				animatorHandler.InvestigateBlend.Update(Time.deltaTime);
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
					DeathTarget = deathEnemiesMonitor.FindAnyTarget(base.ComponentsRoot.position, 30f);
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
			AbstractVehicleDriver abstractVehicleDriver = (myVehicle.type == VehicleType.Bike) ? ((AbstractVehicleDriver)bikeDriver) : ((AbstractVehicleDriver)carDriver);
			if (abstractVehicleDriver.Runnable(useTargetTrigger: true) && myVehicle.OpenableVelocity())
			{
				if (openDoorsTimer.Done())
				{
					walker.Stop();
					abstractVehicleDriver.Run(onlyThrowOutDriver: true, useTargetTrigger: true);
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
