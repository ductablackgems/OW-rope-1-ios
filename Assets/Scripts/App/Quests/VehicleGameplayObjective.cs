using App.AI;
using App.Spawn;
using App.Util;
using App.Vehicles;
using App.Vehicles.Car;
using UnityEngine;

namespace App.Quests
{
	public abstract class VehicleGameplayObjective : GameplayObjective
	{
		[Header("Vehicle Spawn")]
		[SerializeField]
		private VehicleComponents inSceneVehicle;

		[SerializeField]
		private VehicleComponents vehicleToSpawn;

		[Tooltip("[Not Set] = Spawn position is obtained dynamically")]
		[SerializeField]
		private Transform spawnPosition;

		protected VehicleComponents vehicle;

		private VehicleSpawner carSpawner;

		private AIVehicleSpawner tankAISpawner;

		private AIVehicleSpawner mechAISpawner;

		private MechSpawner mechSpawner;

		private TankSpawner tankSpawner;

		private DestroyGameObject destroyer;

		private Health health;

		private int layerMask;

		protected virtual void OnVehicleDestroyed()
		{
			UnregisterListeners();
		}

		protected override void OnInitialized()
		{
			if (inSceneVehicle != null || vehicleToSpawn == null)
			{
				return;
			}
			layerMask = LayerMask.GetMask("Enemy", "Impact", "Player", "Climbable", "Ignore Raycast");
			AIController component = vehicleToSpawn.GetComponent<AIController>();
			if (vehicleToSpawn.type == VehicleType.Mech)
			{
				if (component != null)
				{
					mechAISpawner = ServiceLocator.Get<AIMechSpawner>();
				}
				else
				{
					mechSpawner = ServiceLocator.Get<MechSpawner>();
				}
			}
			else if (vehicleToSpawn.type == VehicleType.Tank)
			{
				if (component != null)
				{
					tankAISpawner = ServiceLocator.Get<AITankSpawner>();
				}
				else
				{
					tankSpawner = ServiceLocator.Get<TankSpawner>();
				}
			}
			else
			{
				carSpawner = ServiceLocator.Get<VehicleSpawner>();
			}
			base.OnInitialized();
		}

		protected VehicleComponents SpawnVehicle()
		{
			vehicle = ((inSceneVehicle != null) ? inSceneVehicle : SpawnVehicle_Internal());
			if (vehicle == null)
			{
				return null;
			}
			destroyer = vehicle.GetComponent<DestroyGameObject>();
			health = ((vehicle.health != null) ? vehicle.health : vehicle.GetComponent<Health>());
			health.OnDie += OnVehicleDestroyed;
			TaxiDispatching component = vehicle.GetComponent<TaxiDispatching>();
			if (component != null)
			{
				component.enabled = false;
			}
			return vehicle;
		}

		protected override void OnStateChanged()
		{
			base.OnStateChanged();
			if (base.CurrentState == State.Inactive)
			{
				UnregisterListeners();
			}
		}

		protected void EnableDestroyer(bool enable)
		{
			if (!(destroyer == null))
			{
				destroyer.enabled = enable;
			}
		}

		private void UnregisterListeners()
		{
			if (!(health == null))
			{
				health.OnDie -= OnVehicleDestroyed;
			}
		}

		private VehicleComponents SpawnVehicle_Internal()
		{
			if (vehicleToSpawn == null)
			{
				return null;
			}
			AIController component = vehicleToSpawn.GetComponent<AIController>();
			Vector3 vector = (spawnPosition != null) ? spawnPosition.position : Vector3.zero;
			if (component != null)
			{
				AIController aIController = null;
				switch (vehicleToSpawn.type)
				{
				case VehicleType.Mech:
					aIController = mechAISpawner.Spawn(component, vector);
					break;
				case VehicleType.Tank:
					aIController = tankAISpawner.Spawn(component, vector);
					break;
				default:
					UnityEngine.Debug.LogErrorFormat("Not supported Vehicle AI type {0}", vehicleToSpawn.type);
					break;
				}
				if (!(aIController != null))
				{
					return null;
				}
				return aIController.GetComponent<VehicleComponents>();
			}
			if (vector == Vector3.zero)
			{
				vector = PhysicsUtils.GetValidSpherePosition(base.Player.Transform.position, base.Player.Transform.forward, 6f, 6.5f, 30f, 30f, layerMask);
			}
			GameObject gameObject;
			switch (vehicleToSpawn.type)
			{
			case VehicleType.Mech:
				gameObject = mechSpawner.SpawnVehicle(vector, vehicleToSpawn.gameObject);
				break;
			case VehicleType.Tank:
				gameObject = tankSpawner.SpawnVehicle(vector, vehicleToSpawn.gameObject);
				break;
			default:
				gameObject = carSpawner.SpawnVehicle(base.Position, vehicleToSpawn.gameObject);
				break;
			}
			if (!(gameObject != null))
			{
				return null;
			}
			return gameObject.GetComponent<VehicleComponents>();
		}

		private void DestroyVehicle()
		{
			if (!(vehicle == null))
			{
				UnregisterListeners();
				UnityEngine.Object.DestroyImmediate(vehicle);
				health = null;
				vehicle = null;
			}
		}
	}
}
