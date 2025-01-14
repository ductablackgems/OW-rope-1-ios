using App.AI;
using App.Player;
using App.Spawn;
using App.Spawn.Pooling;
using App.Util;
using App.Vehicles;
using UnityEngine;

namespace App.Quests
{
	public class ObjectivePickupPassenger : GameplayObjective
	{
		[Header("Objective Pickup Passenger")]
		[SerializeField]
		private EnemyType passengerType;

		[SerializeField]
		private Transform passengerPosition;

		[SerializeField]
		private bool runToVehicle;

		[SerializeField]
		private float stopDistance = 5f;

		private CarDriver passengerDriver;

		private NavmeshWalker passengerWalker;

		private DestroyGameObject passengerDestroyer;

		private HumanSpawner humanSpawner;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			humanSpawner = ServiceLocator.Get<HumanSpawner>();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			GameObject gameObject = humanSpawner.Spawn(passengerType, passengerPosition.position, isWalking: false, force: true).gameObject;
			gameObject.transform.rotation = passengerPosition.rotation;
			passengerDriver = gameObject.GetComponent<CarDriver>();
			passengerWalker = passengerDriver.GetComponentInChildren<NavmeshWalker>();
			passengerDestroyer = passengerDriver.GetComponent<DestroyGameObject>();
			VehicleHandleSensor component = passengerDriver.GetComponent<VehicleHandleSensor>();
			VehicleComponents vehicle = base.Player.PlayerMonitor.GetVehicle();
			if (passengerDestroyer != null)
			{
				passengerDestroyer.enabled = false;
			}
			component.SetTargetHandle(vehicle.passengerHandleTrigger);
			data.NPC = gameObject.GetComponent<Health>();
			passengerDriver.enabled = true;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			if (passengerDestroyer != null)
			{
				passengerDestroyer.enabled = true;
			}
		}

		protected override void OnReset()
		{
			base.OnReset();
			passengerDriver = null;
			passengerWalker = null;
			passengerDestroyer = null;
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (data.IsNPCKilled)
			{
				Fail();
			}
			else if (data.IsVehicleKilled)
			{
				Fail();
			}
			else
			{
				UpdatePickup();
			}
		}

		private void UpdatePickup()
		{
			VehicleComponents vehicle = GetVehicle();
			if (vehicle == null)
			{
				passengerWalker.Stop();
			}
			else if (vehicle.passenger == passengerDriver.transform && passengerDriver.Running())
			{
				Finish();
			}
			else if (passengerDriver.Runnable(useTargetTrigger: true))
			{
				passengerDriver.RunPassenger(stayInCar: true);
			}
			else if (!(passengerWalker.TargetTransform == vehicle.passengerHandleTrigger))
			{
				NavmeshWalkerSpeed speed = (!runToVehicle) ? NavmeshWalkerSpeed.Walk : NavmeshWalkerSpeed.RunFast;
				passengerWalker.FollowTransform(this, vehicle.passengerHandleTrigger, speed);
			}
		}

		private VehicleComponents GetVehicle()
		{
			if (!(Vector3.Distance(base.Position, base.Player.Transform.position) < stopDistance))
			{
				return null;
			}
			VehicleComponents vehicle = base.Player.PlayerMonitor.GetVehicle();
			if (vehicle == null)
			{
				return null;
			}
			if (vehicle.driver != base.Player.Transform)
			{
				return null;
			}
			if (!vehicle.OpenableVelocity())
			{
				return null;
			}
			return vehicle;
		}
	}
}
