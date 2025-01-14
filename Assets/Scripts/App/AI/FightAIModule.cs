using App.Player;
using App.Player.FightSystem;
using App.Util;
using App.Vehicles;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class FightAIModule : AbstractAIScript
	{
		public FightAILevel fightAILevel;

		private NavMeshAgent agent;

		private AdvancedFightController advancedFightController;

		private HitHandler hitHandler;

		private CarDriver carDriver;

		private BikeDriver bikeDriver;

		private BicycleDriver bicycleDriver;

		private GyroboardDriver gyroboardDriver;

		private SkateboardDriver skateboardDriver;

		private AttackZone attackZone;

		private VehicleHandleSensor vehicleHandleSensor;

		private NavmeshWalker walker;

		private GameObject player;

		private CarDriver playerCarDriver;

		private BikeDriver playerBikeDriver;

		private BicycleDriver playerBicycleDriver;

		private GyroboardDriver playerGyroboardDriver;

		private SkateboardDriver playerSkateboardDriver;

		private RagdollHelper playerRagdollHelper;

		private AdvancedFightController playerFightController;

		private HitHandler playerHitHandler;

		private DurationTimer openDoorsTimer = new DurationTimer();

		private float pausedFrom;

		private float pausedTo;

		private float nonPausedFrom;

		private float nonPausedTo;

		private DurationTimer pauseTimer = new DurationTimer();

		private DurationTimer nonPauseTimer = new DurationTimer();

		protected void Awake()
		{
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			advancedFightController = base.ComponentsRoot.GetComponentSafe<AdvancedFightController>();
			hitHandler = base.ComponentsRoot.GetComponentSafe<HitHandler>();
			carDriver = base.ComponentsRoot.GetComponentSafe<CarDriver>();
			bikeDriver = base.ComponentsRoot.GetComponentSafe<BikeDriver>();
			bicycleDriver = base.ComponentsRoot.GetComponentSafe<BicycleDriver>();
			gyroboardDriver = base.ComponentsRoot.GetComponentSafe<GyroboardDriver>();
			skateboardDriver = base.ComponentsRoot.GetComponentSafe<SkateboardDriver>();
			attackZone = base.ComponentsRoot.GetComponentInChildrenSafe<AttackZone>();
			vehicleHandleSensor = base.ComponentsRoot.GetComponentSafe<VehicleHandleSensor>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			player = ServiceLocator.GetGameObject("Player");
			playerCarDriver = player.GetComponentSafe<CarDriver>();
			playerBikeDriver = player.GetComponentSafe<BikeDriver>();
			playerBicycleDriver = player.GetComponentSafe<BicycleDriver>();
			playerGyroboardDriver = player.GetComponentSafe<GyroboardDriver>();
			playerSkateboardDriver = player.GetComponentSafe<SkateboardDriver>();
			playerRagdollHelper = player.GetComponentSafe<RagdollHelper>();
			playerFightController = player.GetComponentSafe<AdvancedFightController>();
			playerHitHandler = player.GetComponentSafe<HitHandler>();
			if (fightAILevel == FightAILevel.Gangster)
			{
				pausedFrom = 0.15f;
				pausedTo = 0.3f;
				nonPausedFrom = 1.5f;
				nonPausedTo = 3f;
			}
			else
			{
				pausedFrom = 0.6f;
				pausedTo = 0.9f;
				nonPausedFrom = 0.3f;
				nonPausedTo = 1.3f;
			}
		}

		protected void OnEnable()
		{
			walker.FollowTransform(this, player.transform, NavmeshWalkerSpeed.Run);
			nonPauseTimer.Run(Random.Range(nonPausedFrom, nonPausedTo));
			advancedFightController.OnStartCombo += OnStartCombo;
		}

		protected void OnDisable()
		{
			walker.Stop();
			advancedFightController.Stop();
			nonPauseTimer.Stop();
			pauseTimer.Stop();
			advancedFightController.OnStartCombo -= OnStartCombo;
		}

		protected void Update()
		{
			if (nonPauseTimer.Done())
			{
				nonPauseTimer.Stop();
				pauseTimer.Run(Random.Range(pausedFrom, pausedTo));
			}
			else if (pauseTimer.Done())
			{
				nonPauseTimer.Run(Random.Range(nonPausedFrom, nonPausedTo));
				pauseTimer.Stop();
			}
			if (playerCarDriver.SittingInVehicle || playerBikeDriver.SittingInVehicle || playerBicycleDriver.SittingInVehicle || playerGyroboardDriver.SittingInVehicle || playerSkateboardDriver.SittingInVehicle)
			{
				AbstractVehicleDriver abstractVehicleDriver;
				AbstractVehicleDriver abstractVehicleDriver2;
				Transform handleTrigger;
				if (playerBicycleDriver.SittingInVehicle)
				{
					abstractVehicleDriver = bicycleDriver;
					abstractVehicleDriver2 = bicycleDriver;
					handleTrigger = playerBicycleDriver.Vehicle.handleTrigger;
				}
				else if (playerSkateboardDriver.SittingInVehicle)
				{
					abstractVehicleDriver = skateboardDriver;
					abstractVehicleDriver2 = playerSkateboardDriver;
					handleTrigger = playerSkateboardDriver.Vehicle.handleTrigger;
				}
				else if (playerCarDriver.SittingInVehicle)
				{
					abstractVehicleDriver = carDriver;
					abstractVehicleDriver2 = playerCarDriver;
					handleTrigger = playerCarDriver.Vehicle.handleTrigger;
				}
				else if (playerBikeDriver.SittingInVehicle)
				{
					abstractVehicleDriver = bikeDriver;
					abstractVehicleDriver2 = playerBikeDriver;
					handleTrigger = playerBikeDriver.Vehicle.handleTrigger;
				}
				else
				{
					abstractVehicleDriver = gyroboardDriver;
					abstractVehicleDriver2 = playerGyroboardDriver;
					handleTrigger = playerGyroboardDriver.Vehicle.handleTrigger;
				}
				if (abstractVehicleDriver.Running())
				{
					return;
				}
				if (walker.TargetTransform != handleTrigger)
				{
					walker.FollowTransform(this, handleTrigger, NavmeshWalkerSpeed.Run);
				}
				if (vehicleHandleSensor.GetTargetHandle() != handleTrigger)
				{
					vehicleHandleSensor.SetTargetHandle(handleTrigger);
				}
				if (abstractVehicleDriver.Runnable(useTargetTrigger: true) && abstractVehicleDriver2.Vehicle.OpenableVelocity())
				{
					if (openDoorsTimer.Done())
					{
						walker.Stop();
						advancedFightController.Stop();
						abstractVehicleDriver.Run(onlyThrowOutDriver: true, useTargetTrigger: true);
						openDoorsTimer.Stop();
					}
					else if (!openDoorsTimer.Running())
					{
						if (abstractVehicleDriver2.Vehicle.type == VehicleType.Bicycle || abstractVehicleDriver2.Vehicle.type == VehicleType.Gyroboard || abstractVehicleDriver2.Vehicle.type == VehicleType.Skateboard)
						{
							openDoorsTimer.Run(0.4f);
						}
						else
						{
							openDoorsTimer.Run(0.8f);
						}
					}
				}
				else
				{
					openDoorsTimer.Stop();
				}
			}
			else
			{
				if (hitHandler.RunningKinematic())
				{
					return;
				}
				openDoorsTimer.Stop();
				bool attackPressed = !pauseTimer.Running() && attackZone.IsIn && !playerRagdollHelper.Ragdolled && !playerHitHandler.WillRagdoll() && !playerFightController.RunningCustomMovement();
				float num = Vector3.Distance(player.transform.position, base.ComponentsRoot.transform.position);
				if (attackZone.IsIn)
				{
					advancedFightController.Run();
					walker.Stop();
					Vector3 vector = new Vector3(player.transform.position.x, base.ComponentsRoot.position.y, player.transform.position.z);
					if (!playerRagdollHelper.Ragdolled)
					{
						Quaternion to = Quaternion.LookRotation(vector - new Vector3(base.ComponentsRoot.position.x, vector.y, base.ComponentsRoot.position.z));
						base.ComponentsRoot.rotation = Quaternion.RotateTowards(base.ComponentsRoot.rotation, to, 300f * Time.deltaTime);
					}
				}
				else
				{
					if (walker.TargetTransform != player.transform)
					{
						walker.FollowTransform(this, player.transform, NavmeshWalkerSpeed.Run);
					}
					if (playerRagdollHelper.Ragdolled && num < 2f)
					{
						walker.Stop();
					}
				}
				advancedFightController.Control(attackPressed);
			}
		}

		private void OnStartCombo()
		{
			nonPauseTimer.Run(Random.Range(2.5f, 3.5f));
			pauseTimer.Stop();
		}
	}
}
