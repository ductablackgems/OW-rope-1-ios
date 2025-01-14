using App.GUI;
using App.Missions;
using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Vehicles.Car
{
	public class TaxiDispatching : MonoBehaviour
	{
		public TaxiType type;

		private VehicleModesHandler vehicleModesHandler;

		private VehicleComponents vehicleComponents;

		private Rigidbody _rigidbody;

		private TaxiSpots taxiSpots;

		private MissionManager missionManager;

		private DurationTimer orderTimer = new DurationTimer();

		private DurationTimer setDestinationTimer = new DurationTimer();

		private int missionKey = -1;

		private void Awake()
		{
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			vehicleComponents = this.GetComponentSafe<VehicleComponents>();
			taxiSpots = ServiceLocator.Get<TaxiSpots>();
			missionManager = ServiceLocator.Get<MissionManager>();
		}

		private void Update()
		{
			if (vehicleModesHandler.mode != VehicleMode.Player)
			{
				orderTimer.Stop();
				setDestinationTimer.Stop();
				return;
			}
			if (orderTimer.Done())
			{
				orderTimer.Stop();
				string jobText = GetJobText(isStart: true);
				missionKey = missionManager.StartMission(jobText, delegate
				{
					taxiSpots.ReleaseTarget();
				});
				TaxiSpot spot = taxiSpots.ActivateRandomWaiter(missionKey, _rigidbody, vehicleComponents.passengerHandleTrigger);
				SetMapTarget(missionKey, spot);
				missionManager.SetRemainTime(missionKey, 120f);
			}
			if (!missionManager.CompareMission(missionKey) && !orderTimer.Running())
			{
				orderTimer.Run(4f);
			}
			if (missionManager.CompareMission(missionKey) && vehicleComponents.passenger != null && !setDestinationTimer.Running() && !orderTimer.Running() && taxiSpots.TargetSpot.isWaiter)
			{
				setDestinationTimer.Run(2f);
			}
			if (setDestinationTimer.Done())
			{
				setDestinationTimer.Stop();
				if (missionManager.SetJobText(missionKey, GetJobText(isStart: false)))
				{
					TaxiSpot targetSpot = taxiSpots.TargetSpot;
					TaxiSpot taxiSpot = taxiSpots.ActivateRandomDestination(type);
					SetMapTarget(missionKey, taxiSpot);
					float magnitude = (targetSpot.transform.position - taxiSpot.transform.position).magnitude;
					missionManager.SetRewards(missionKey, Mathf.CeilToInt(magnitude / 5f));
					int num = Mathf.CeilToInt(magnitude / 100f * 0.8f) * 30 + 30;
					missionManager.SetRemainTime(missionKey, num);
				}
			}
		}

		private void SetMapTarget(int missionKey, TaxiSpot spot)
		{
			TargetCursorType targetCursorType = TargetCursorType.None;
			switch (type)
			{
			case TaxiType.Ambulance:
				targetCursorType = TargetCursorType.Ambulance;
				break;
			case TaxiType.Taxi:
				targetCursorType = TargetCursorType.Taxi;
				break;
			case TaxiType.Bus:
				targetCursorType = TargetCursorType.BusStop;
				break;
			}
			missionManager.SetMapTarget(missionKey, spot.transform.position, targetCursorType);
		}

		private string GetJobText(bool isStart)
		{
			int key = 0;
			switch (type)
			{
			case TaxiType.Ambulance:
				key = (isStart ? 5012 : 5010);
				break;
			case TaxiType.Taxi:
				key = (isStart ? 5013 : 5011);
				break;
			case TaxiType.Bus:
				key = (isStart ? 5018 : 5011);
				break;
			}
			return LocalizationManager.Instance.GetText(key);
		}
	}
}
