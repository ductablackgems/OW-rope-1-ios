using UnityEngine;

namespace App.Vehicles.Motorbike
{
	public class MotorbikeModesHelper : MonoBehaviour
	{
		private PlayerMotorbikeController playerMotorbikeController;

		public MotorbikMode Mode
		{
			get;
			private set;
		}

		public void SetPlayerMode()
		{
			playerMotorbikeController.enabled = true;
		}

		private void Awake()
		{
			playerMotorbikeController = this.GetComponentSafe<PlayerMotorbikeController>();
		}
	}
}
