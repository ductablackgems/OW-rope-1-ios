using App.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveDriveTo : GameplayObjective
	{
		[Header("Objective Drive To")]
		[SerializeField]
		private float reachDistance = 2f;

		private ObjectiveWaypoint currentWaypoint;

		private List<ObjectiveWaypoint> allWaypoints = new List<ObjectiveWaypoint>(16);

		private List<ObjectiveWaypoint> waypoints = new List<ObjectiveWaypoint>(16);

		protected override void OnInitialized()
		{
			base.OnInitialized();
			GetComponentsInChildren(includeInactive: true, allWaypoints);
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			waypoints.Clear();
			waypoints.AddRange(allWaypoints);
		}

		protected override void OnReset()
		{
			base.OnReset();
			foreach (ObjectiveWaypoint allWaypoint in allWaypoints)
			{
				allWaypoint.SetActive(isActive: false);
			}
			currentWaypoint = null;
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (data.IsVehicleKilled)
			{
				Fail();
			}
			else if (allWaypoints.Count > 0)
			{
				UpdateWaypoints();
			}
			else if (Vector3.Distance(base.Player.Transform.position, base.Position) < reachDistance)
			{
				Finish();
			}
		}

		protected override Vector3 GetNavigationPosition()
		{
			if (!(currentWaypoint != null))
			{
				return base.GetNavigationPosition();
			}
			return currentWaypoint.Position;
		}

		private bool GetIsInVehicle()
		{
			VehicleComponents vehicle = base.Player.PlayerMonitor.GetVehicle();
			if (vehicle == null)
			{
				return false;
			}
			if (vehicle.driver == null)
			{
				return false;
			}
			if (vehicle != data.Vehicle)
			{
				return false;
			}
			return true;
		}

		private void UpdateWaypoints()
		{
			if (GetIsInVehicle())
			{
				TrySetNextWaypoint();
				if (!(currentWaypoint == null) && !(Vector3.Distance(base.Player.Transform.position, currentWaypoint.Position) > reachDistance))
				{
					currentWaypoint.SetActive(isActive: false);
					currentWaypoint = null;
				}
			}
		}

		private void TrySetNextWaypoint()
		{
			if (!(currentWaypoint != null))
			{
				if (waypoints.Count == 0)
				{
					Finish();
					return;
				}
				currentWaypoint = waypoints[0];
				currentWaypoint.SetActive(isActive: true);
				waypoints.RemoveAt(0);
			}
		}
	}
}
