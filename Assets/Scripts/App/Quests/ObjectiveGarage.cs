using App.Garage;
using App.Prefabs;
using App.Vehicles;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveGarage : GameplayObjective
	{
		[Header("Objective Garage")]
		[SerializeField]
		private GarageManager garage;

		protected override void OnActivated()
		{
			base.OnActivated();
			garage.VehicleGaraged += OnVehicleGaraged;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			garage.VehicleGaraged -= OnVehicleGaraged;
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (data.IsVehicleKilled)
			{
				Fail();
			}
		}

		private void OnVehicleGaraged(VehiclePrefabId prefabID)
		{
			if (!(prefabID.GetComponent<VehicleComponents>() != data.Vehicle))
			{
				Finish();
			}
		}
	}
}
