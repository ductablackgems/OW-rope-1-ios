using App.GUI.Panels;
using App.Spawn;
using App.Util;
using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Garage
{
	public class GarageControlZone : MonoBehaviour
	{
		public GarageDoors doors;

		private GamePanel gamePanel;

		private PlayerRespawner playerRespawner;

		private Transform Player;

		private bool distanceControl;

		private float dist;

		public bool IsPlayerIn
		{
			get;
			private set;
		}

		private void Awake()
		{
			gamePanel = ServiceLocator.Get<GamePanel>();
			playerRespawner = ServiceLocator.Get<PlayerRespawner>();
			playerRespawner.AfterRespawn += AfterPlayerRespawn;
		}

		private void OnDestroy()
		{
			playerRespawner.AfterRespawn -= AfterPlayerRespawn;
		}

		private void Update()
		{
			if (IsPlayerIn && InputUtils.GarageDoors.IsDown)
			{
				if (doors.State == GarageDoorsState.Closed || doors.State == GarageDoorsState.Closing)
				{
					CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_garage, () =>
					{
						doors.Open();
					});
				}
				else
				{
					CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_garage, () =>
					{
						doors.Close();
					});
				}
			}
			if (distanceControl)
			{
				dist = Vector3.Distance(Player.position, base.transform.position);
				if (dist > 20f)
				{
					doors.Close();
					distanceControl = false;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				gamePanel.garageControlZone = this;
				IsPlayerIn = true;
				Player = other.transform;
				distanceControl = true;
			}
			if (other.CompareTag("Vehicle"))
			{
				VehicleModesHandler component = other.GetComponent<VehicleModesHandler>();
				if (component != null && component.mode == VehicleMode.Player && doors.State == GarageDoorsState.Closed && doors.State != GarageDoorsState.Opened)
				{
					doors.Open();
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				IsPlayerIn = false;
			}
		}

		private void AfterPlayerRespawn()
		{
			IsPlayerIn = false;
		}
	}
}
