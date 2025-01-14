using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class VehicleModesHandler : MonoBehaviour
	{
		public VehicleMode mode;

		private IPlayerVehicleController playerControl;

		private AIVehicleModesHandler aiModesHandler;

		private CarAudio carAudio;

		private bool initialized;

		public void SetMode(VehicleMode mode)
		{
			if (!initialized)
			{
				Init();
			}
			this.mode = mode;
			switch (mode)
			{
			case VehicleMode.Empty:
				playerControl.enabled = false;
				if (aiModesHandler != null)
				{
					aiModesHandler.enabled = false;
				}
				carAudio.enabled = false;
				break;
			case VehicleMode.Player:
				if (aiModesHandler != null)
				{
					aiModesHandler.enabled = false;
				}
				playerControl.enabled = true;
				carAudio.enabled = true;
				break;
			case VehicleMode.AI:
				playerControl.enabled = false;
				if (aiModesHandler != null)
				{
					aiModesHandler.enabled = true;
				}
				carAudio.enabled = true;
				break;
			}
		}

		private void Awake()
		{
			if (!initialized)
			{
				Init();
			}
			SetMode(mode);
		}

		private void Init()
		{
			initialized = true;
			playerControl = this.GetComponentSafe<IPlayerVehicleController>();
			aiModesHandler = GetComponentInChildren<AIVehicleModesHandler>(includeInactive: true);
			carAudio = this.GetComponentSafe<CarAudio>();
		}
	}
}
