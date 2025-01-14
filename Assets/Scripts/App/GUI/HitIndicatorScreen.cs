using App.Player;
using App.Util;
using App.Vehicles;
using UnityEngine;

namespace App.GUI
{
	public class HitIndicatorScreen : MonoBehaviour
	{
		private HitIndicator[] indicators;

		private Health playerHealth;

		private Health vehicleHealth;

		private PlayerModel player;

		private MechDriver mechDriver;

		private void Awake()
		{
			Initialize();
		}

		private void Update()
		{
			Vector3 forward = UnityEngine.Camera.main.transform.forward;
			for (int i = 0; i < indicators.Length; i++)
			{
				UpdateIndicator(indicators[i], forward, playerHealth.transform.position);
			}
		}

		private void OnDamageTaken(float damage, int damageType, GameObject agressor)
		{
			if (!(agressor == null))
			{
				Transform transform = agressor.transform;
				HitIndicator indicator = GetIndicator(transform);
				if (!(indicator == null))
				{
					indicator.Show(transform);
				}
			}
		}

		private void OnDriverAfterRun()
		{
			VehicleComponents vehicle = mechDriver.Vehicle;
			if (!(vehicle == null))
			{
				vehicleHealth = vehicle.GetComponent<Health>();
				if (!(vehicleHealth == null))
				{
					vehicleHealth.OnDamage += OnDamageTaken;
				}
			}
		}

		private void OnDriverAfterStop()
		{
			if (!(vehicleHealth == null))
			{
				vehicleHealth.OnDamage -= OnDamageTaken;
				vehicleHealth = null;
			}
		}

		private void Initialize()
		{
			player = ServiceLocator.GetPlayerModel();
			indicators = GetComponentsInChildren<HitIndicator>(includeInactive: true);
			mechDriver = player.Transform.GetComponent<MechDriver>();
			HitIndicator[] array = indicators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
			playerHealth = player.Transform.GetComponent<Health>();
			playerHealth.OnDamage += OnDamageTaken;
			mechDriver.AfterRun += OnDriverAfterRun;
			mechDriver.AfterStop += OnDriverAfterStop;
		}

		private HitIndicator GetIndicator(Transform target)
		{
			HitIndicator hitIndicator = null;
			for (int i = 0; i < indicators.Length; i++)
			{
				HitIndicator hitIndicator2 = indicators[i];
				if (hitIndicator2.Target == target)
				{
					return hitIndicator2;
				}
				if (hitIndicator == null && hitIndicator2.IsReady)
				{
					hitIndicator = hitIndicator2;
				}
			}
			return hitIndicator;
		}

		private void UpdateIndicator(HitIndicator indicator, Vector3 lookDir, Vector3 playerPos)
		{
			if (!(indicator.Target == null))
			{
				Vector3 postion = indicator.Target.position - playerPos;
				float num = Vector3.SignedAngle(To2D(lookDir), To2D(postion), Vector3.up);
				indicator.transform.localRotation = Quaternion.Euler(0f, 0f, num * -1f);
			}
		}

		private static Vector3 To2D(Vector3 postion)
		{
			postion.y = 0f;
			return postion;
		}
	}
}
