using App.Prefabs;
using App.Spawn;
using App.Util;
using App.Vehicles;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveFollowVehicle : GameplayObjective
	{
		[Header("Objective Floow Vehicle")]
		[SerializeField]
		private VehiclePrefabId vehiclePrefab;

		[SerializeField]
		private Transform spawnPoint;

		[SerializeField]
		private float successDistance = 50f;

		[SerializeField]
		private float autoFailDistance = 100f;

		private VehicleSpawner spawner;

		private int layerMask;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			spawner = ServiceLocator.Get<VehicleSpawner>();
			layerMask = LayerMask.GetMask("Enemy", "Impact", "Player", "Climbable", "Ignore Raycast");
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			Transform transform = (spawnPoint != null) ? spawnPoint : base.transform;
			Vector3 vector = transform.position;
			if (!PhysicsUtils.IsValidPosition(vector, 2.5f, layerMask))
			{
				vector = PhysicsUtils.GetValidSpherePosition(vector, transform.forward, 2.5f, 1f, 10f, 15f, layerMask);
			}
			VehiclePrefabId vehiclePrefabId = spawner.SpawnAIVehicle(vehiclePrefab, vector, transform.rotation);
			data.Vehicle = vehiclePrefabId.GetComponent<VehicleComponents>();
			vehiclePrefabId.GetComponent<DestroyGameObject>().enabled = false;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			VehicleComponents vehicle = data.Vehicle;
			if (!(vehicle == null))
			{
				DestroyGameObject component = vehicle.GetComponent<DestroyGameObject>();
				if (!(component == null))
				{
					component.enabled = true;
				}
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (data.IsVehicleKilled)
			{
				Fail();
			}
			else if (!(data.Vehicle == null))
			{
				if (data.Vehicle.driver == null)
				{
					Fail();
				}
				else if (Vector3.Distance(data.Vehicle.transform.position, base.Player.Transform.position) > autoFailDistance)
				{
					Fail();
				}
			}
		}

		protected override Vector3 GetNavigationPosition()
		{
			if (!(data.Vehicle != null))
			{
				return base.GetNavigationPosition();
			}
			return data.Vehicle.transform.position;
		}

		protected override void OnTimeIsUp()
		{
			if (Vector3.Distance(data.Vehicle.transform.position, base.Player.Transform.position) <= successDistance)
			{
				Finish();
			}
			else
			{
				Fail();
			}
		}
	}
}
