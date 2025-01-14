using App.Util;
using UnityEngine;

namespace App.Vehicles.Airplane
{
	public class AirplaneBordersChecker : MonoBehaviour
	{
		[SerializeField]
		private int timeLimit = 10;

		private AirplaneController controller;

		private SceneBorders borders;

		private float outOfBordersTimer;

		private void Awake()
		{
			borders = ServiceLocator.Get<SceneBorders>();
			ServiceLocator.Messages.Subscribe(MessageID.Airplane.Enter, this, OnAirplaneEnter);
			ServiceLocator.Messages.Subscribe(MessageID.Airplane.Leave, this, OnAirplaneLeave);
		}

		private void Update()
		{
			if (controller == null || !controller.IsActive)
			{
				return;
			}
			if (GetIsValid())
			{
				TryReportBorderEnter();
				return;
			}
			TryReportBorderLeave();
			UpdateTimer();
			if (outOfBordersTimer == 0f)
			{
				controller.Respawn();
				outOfBordersTimer = timeLimit;
			}
		}

		private void OnAirplaneEnter(object sender, object data)
		{
			VehicleComponents vehicleComponents = data as VehicleComponents;
			controller = vehicleComponents.GetComponent<AirplaneController>();
		}

		private void OnAirplaneLeave(object sender, object data)
		{
			controller = null;
			outOfBordersTimer = 0f;
		}

		private bool GetIsValid()
		{
			if (controller == null)
			{
				return true;
			}
			if (!controller.IsActive)
			{
				return true;
			}
			return borders.BorderBounds.Contains(controller.transform.position);
		}

		private void UpdateTimer()
		{
			if (outOfBordersTimer != 0f)
			{
				outOfBordersTimer = Mathf.Max(0f, outOfBordersTimer - Time.deltaTime);
			}
		}

		private void TryReportBorderEnter()
		{
			if (outOfBordersTimer != 0f)
			{
				outOfBordersTimer = 0f;
				ServiceLocator.Messages.Send(MessageID.Airplane.BorderEnter, this);
			}
		}

		private void TryReportBorderLeave()
		{
			if (!(outOfBordersTimer > 0f))
			{
				outOfBordersTimer = timeLimit;
				ServiceLocator.Messages.Send(MessageID.Airplane.BorderLeave, this, outOfBordersTimer);
			}
		}
	}
}
