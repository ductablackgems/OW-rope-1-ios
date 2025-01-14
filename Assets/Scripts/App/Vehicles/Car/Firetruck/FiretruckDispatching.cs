using App.GUI;
using App.Missions;
using App.Spawn;
using App.Util;
using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Vehicles.Car.Firetruck
{
	public class FiretruckDispatching : MonoBehaviour
	{
		private VehicleModesHandler vehicleModesHandler;

		private VehicleSpawner vehicleSpawner;

		private MissionManager missionManager;

		private Health playerHealth;

		private DurationTimer orderTimer = new DurationTimer();

		private FireManager targetFireManager;

		private bool isTargeted;

		private int missionKey = -1;

		private void Awake()
		{
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			vehicleSpawner = ServiceLocator.Get<VehicleSpawner>();
			missionManager = ServiceLocator.Get<MissionManager>();
			playerHealth = ServiceLocator.GetGameObject("Player").GetComponentSafe<Health>();
		}

		private void OnDisable()
		{
			AbortMission();
		}

		private void Update()
		{
			if (isTargeted && (targetFireManager == null || playerHealth.Dead()))
			{
				AbortMission();
			}
			if (isTargeted && (playerHealth.transform.position - base.transform.position).magnitude > 30f)
			{
				AbortMission();
			}
			if (isTargeted && !missionManager.CompareMission(missionKey))
			{
				AbortMission();
			}
			if (vehicleModesHandler.mode != VehicleMode.Player)
			{
				orderTimer.Stop();
				return;
			}
			if (orderTimer.Done())
			{
				GameObject gameObject = vehicleSpawner.SpawnCarInFire(base.transform.position, 200f);
				if (gameObject == null)
				{
					orderTimer.Run(4f);
				}
				else
				{
					isTargeted = true;
					orderTimer.Stop();
					targetFireManager = gameObject.GetComponentSafe<FireManager>();
					missionKey = missionManager.StartMission(LocalizationManager.Instance.GetText(5009), delegate
					{
						if (targetFireManager != null)
						{
							targetFireManager.GetComponentSafe<DestroyGameObject>().enabled = true;
							targetFireManager = null;
						}
					});
					missionManager.SetMapTarget(missionKey, gameObject.transform.position, TargetCursorType.Fire);
					float magnitude = (base.transform.position - gameObject.transform.position).magnitude;
					missionManager.SetRewards(missionKey, Mathf.CeilToInt(magnitude / 5f * 0.8f));
					int num = Mathf.CeilToInt(magnitude / 100f * 0.6f) * 30 + 30;
					missionManager.SetRemainTime(missionKey, num);
				}
			}
			if (!orderTimer.Running() && targetFireManager == null)
			{
				orderTimer.Run(4f);
			}
			if (targetFireManager != null && !targetFireManager.IsInFire())
			{
				missionManager.FinishMission(missionKey, success: true);
				isTargeted = false;
			}
		}

		private void AbortMission()
		{
			missionManager.FinishMission(missionKey, success: false);
			isTargeted = false;
		}
	}
}
