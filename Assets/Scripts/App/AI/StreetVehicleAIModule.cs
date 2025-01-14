using App.AI.Scanner;
using App.Player;
using App.Player.Definition;
using App.Util;
using App.Vehicles;
using App.Vehicles.Bicycle;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class StreetVehicleAIModule : AbstractAIScript
	{
		public VehicleComponents myVehicle;

		private NavMeshAgent agent;

		private Rigidbody _rigidbody;

		private PlayerAnimatorHandler animatorHandler;

		private Health health;

		private BicycleDriver bicycleDriver;

		private GyroboardDriver gyroboardDriver;

		private SkateboardDriver skateboardDriver;

		private RagdollHelper ragdollHelper;

		private NavmeshWalker walker;

		private AITransitions transitions;

		private AIScanner scanner;

		private PlayerModel player;

		private CrimeManager crimeManager;

		private DurationTimer getInVehicleTimer = new DurationTimer();

		private Health myVehicleHealth;

		private VehicleType myVehicleType;

		public event Action OnLooseVehicle;

		private void Awake()
		{
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			_rigidbody = base.ComponentsRoot.GetComponentSafe<Rigidbody>();
			animatorHandler = base.ComponentsRoot.GetComponentSafe<PlayerAnimatorHandler>();
			health = base.ComponentsRoot.GetComponentSafe<Health>();
			bicycleDriver = base.ComponentsRoot.GetComponentSafe<BicycleDriver>();
			gyroboardDriver = base.ComponentsRoot.GetComponentSafe<GyroboardDriver>();
			skateboardDriver = base.ComponentsRoot.GetComponentSafe<SkateboardDriver>();
			ragdollHelper = base.ComponentsRoot.GetComponentSafe<RagdollHelper>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			transitions = this.GetComponentSafe<AITransitions>();
			scanner = this.GetComponentSafe<AIScanner>();
			player = ServiceLocator.GetPlayerModel();
			crimeManager = ServiceLocator.Get<CrimeManager>();
		}

		private void OnEnable()
		{
			bicycleDriver.BeforeRun += BeforeVehicleControllerStart;
			bicycleDriver.AfterStop += AfterVehicleControllerStop;
			gyroboardDriver.BeforeRun += BeforeVehicleControllerStart;
			gyroboardDriver.AfterStop += AfterVehicleControllerStop;
			skateboardDriver.BeforeRun += BeforeVehicleControllerStart;
			skateboardDriver.AfterStop += AfterVehicleControllerStop;
		}

		private void OnDisable()
		{
			bicycleDriver.BeforeRun -= BeforeVehicleControllerStart;
			bicycleDriver.AfterStop -= AfterVehicleControllerStop;
			gyroboardDriver.BeforeRun -= BeforeVehicleControllerStart;
			gyroboardDriver.AfterStop -= AfterVehicleControllerStop;
			skateboardDriver.BeforeRun -= BeforeVehicleControllerStart;
			skateboardDriver.AfterStop -= AfterVehicleControllerStop;
			walker.Stop();
		}

		private void Update()
		{
			if (myVehicle == null)
			{
				base.enabled = false;
				if (this.OnLooseVehicle != null)
				{
					this.OnLooseVehicle();
				}
				return;
			}
			if (myVehicle.streetVehicleModesHelper.Mode == StreetVehicleMode.Free)
			{
				if (walker.TargetTransform != myVehicle.transform)
				{
					walker.FollowTransform(this, myVehicle.transform, NavmeshWalkerSpeed.Walk);
				}
			}
			else if (walker.TargetTransform != myVehicle.handleTrigger)
			{
				walker.FollowTransform(this, myVehicle.handleTrigger, NavmeshWalkerSpeed.Walk);
			}
			if (myVehicleHealth == null || myVehicleHealth.transform != myVehicle.transform)
			{
				myVehicleHealth = myVehicle.GetComponentSafe<Health>();
				myVehicleType = myVehicle.type;
			}
			AbstractVehicleDriver abstractVehicleDriver = (myVehicleType == VehicleType.Skateboard) ? skateboardDriver : ((myVehicleType != VehicleType.Gyroboard) ? ((AbstractVehicleDriver)bicycleDriver) : ((AbstractVehicleDriver)gyroboardDriver));
			bool flag = transitions.temperament == AITemperament.Policeman && crimeManager.StarCount > 0 && scanner.HasTrackToPlayer;
			float magnitude = (player.Transform.position - base.ComponentsRoot.position).magnitude;
			if (abstractVehicleDriver.Running())
			{
				IFollowingVehicle followingVehicle = myVehicle.GetFollowingVehicle();
				if ((myVehicleHealth.AttackedRecently(2f) || (flag && magnitude < 17f)) && (animatorHandler.UseBicycle || animatorHandler.UseGyroboard || animatorHandler.UseSkateboard))
				{
					abstractVehicleDriver.Stop();
					health.ApplyDamage(1f, 2);
				}
				else if (flag && !followingVehicle.FollowingPlayer)
				{
					followingVehicle.SetFollowingPlayer(followingPlayer: true);
				}
				else if (!flag && followingVehicle.FollowingPlayer)
				{
					followingVehicle.SetFollowingPlayer(followingPlayer: false);
				}
			}
			else if (!abstractVehicleDriver.Running() && !myVehicleHealth.AttackedRecently(5f) && !health.AttackedRecently(5f))
			{
				if (!ragdollHelper.Ragdolled && myVehicle.streetVehicleModesHelper.Mode == StreetVehicleMode.Free && (base.transform.position - myVehicle.transform.position).magnitude < 1f)
				{
					myVehicle.streetVehicleModesHelper.SetEmptyStanding();
					getInVehicleTimer.Run(0.3f);
				}
				if (((myVehicle.streetVehicleModesHelper.Mode == StreetVehicleMode.EmptyStanding && !getInVehicleTimer.InProgress()) || myVehicle.streetVehicleModesHelper.Mode == StreetVehicleMode.Player) && !ragdollHelper.Ragdolled && abstractVehicleDriver.Runnable() && abstractVehicleDriver.Vehicle.OpenableVelocity())
				{
					walker.Stop();
					abstractVehicleDriver.Run();
				}
			}
		}

		private void BeforeVehicleControllerStart()
		{
			agent.enabled = false;
		}

		private void AfterVehicleControllerStop()
		{
			agent.enabled = true;
		}
	}
}
