using App.Prefabs;
using App.SaveSystem;
using System;
using UnityEngine;

namespace App.Garage
{
	public class GarageManager : MonoBehaviour
	{
		public const float CloseDoorsDistance = 20f;

		public Transform vehicleSpawnPosition;

		public int garageNum;

		private GarageDoors doors;

		private GarageInterior interior;

		private GarageSaveEntity garageSave;

		private VehiclePrefabsScriptableObject vehiclePrefabs;

		private Transform playerTransform;

		public event Action<VehiclePrefabId> VehicleGaraged;

		private void Awake()
		{
			doors = this.GetComponentInChildrenSafe<GarageDoors>();
			interior = this.GetComponentInChildrenSafe<GarageInterior>();
			garageSave = ServiceLocator.Get<SaveEntities>().PlayerSave.garages[garageNum];
			vehiclePrefabs = ServiceLocator.Get<PrefabsContainer>().vehiclePrefabs;
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
			doors.OnStartOpening += OnStartOpening;
			doors.OnClosed += OnClosed;
		}

		private void OnDestroy()
		{
			doors.OnStartOpening -= OnStartOpening;
			doors.OnClosed -= OnClosed;
		}

		private void Update()
		{
			if (doors.State == GarageDoorsState.Closed && interior.GetVehiclePrefabId() != null)
			{
				UnityEngine.Object.Destroy(interior.GetVehiclePrefabId().gameObject);
			}
			if (interior.IsPlayerIn() && (doors.State == GarageDoorsState.Closing || doors.State == GarageDoorsState.Closing))
			{
				doors.Open();
			}
			if (doors.State == GarageDoorsState.Opened && Vector3.Distance(base.transform.position, playerTransform.position) > 20f)
			{
				doors.Close();
			}
		}

		private void OnStartOpening()
		{
			if (garageSave.vehicleInside)
			{
				VehiclePrefabId vehiclePrefabId = vehiclePrefabs.Find(garageSave.prefabTid);
				if (!(vehiclePrefabId == null))
				{
					UnityEngine.Object.Instantiate(vehiclePrefabId.gameObject, vehicleSpawnPosition.position, vehicleSpawnPosition.rotation);
					garageSave.vehicleInside = false;
					garageSave.prefabTid = "";
					garageSave.Save();
				}
			}
		}

		private void OnClosed()
		{
			VehiclePrefabId vehiclePrefabId = interior.GetVehiclePrefabId();
			if (!(vehiclePrefabId == null))
			{
				if (this.VehicleGaraged != null)
				{
					this.VehicleGaraged(vehiclePrefabId);
				}
				garageSave.vehicleInside = true;
				garageSave.prefabTid = vehiclePrefabId.tid;
				garageSave.Save();
				UnityEngine.Object.Destroy(vehiclePrefabId.gameObject);
			}
		}
	}
}
