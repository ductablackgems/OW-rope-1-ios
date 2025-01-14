using App.Prefabs;
using UnityEngine;

namespace App.Garage
{
	public class GarageInterior : MonoBehaviour
	{
		private TriggerMonitor vehicleTrigger = new TriggerMonitor();

		private TriggerMonitor playerTrigger = new TriggerMonitor();

		public VehiclePrefabId GetVehiclePrefabId()
		{
			return (VehiclePrefabId)vehicleTrigger.GetTrigger();
		}

		public bool IsPlayerIn()
		{
			return playerTrigger.IsTriggered();
		}

		private void FixedUpdate()
		{
			vehicleTrigger.FixedUpdate();
			playerTrigger.FixedUpdate();
		}

		private void OnTriggerStay(Collider other)
		{
			VehiclePrefabId vehiclePrefabId = (VehiclePrefabId)vehicleTrigger.GetTrigger();
			WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Masks.GarageInterior);
			if (whoIsResult.IsEmpty)
			{
				return;
			}
			if (whoIsResult.Compare(WhoIs.Entities.Vehicle))
			{
				if (vehiclePrefabId == null)
				{
					VehiclePrefabId component = whoIsResult.gameObject.GetComponent<VehiclePrefabId>();
					if ((bool)component)
					{
						vehicleTrigger.MarkTrigger(component);
					}
				}
				else if (vehiclePrefabId.gameObject.Equals(whoIsResult.gameObject))
				{
					vehicleTrigger.MarkTrigger(vehiclePrefabId);
				}
			}
			else if (whoIsResult.Compare(WhoIs.Entities.Player))
			{
				playerTrigger.MarkTrigger(whoIsResult.transform);
			}
		}
	}
}
