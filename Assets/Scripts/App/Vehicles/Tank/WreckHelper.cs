using App.Camera;
using App.GUI;
using UnityEngine;

namespace App.Vehicles.Tank
{
	public class WreckHelper : MonoBehaviour
	{
		public Transform cameraTarget;

		public Transform tankTower;

		private CameraManager cameraManager;

		private PanelsManager panelsManager;

		private DurationTimer playerCameraTimer = new DurationTimer();

		public void FixTankTower(Transform cameraTarget, Transform tankTower)
		{
			this.tankTower.localRotation = tankTower.localRotation;
			this.cameraTarget.parent.localRotation = cameraTarget.parent.localRotation;
		}

		private void Awake()
		{
			cameraManager = ServiceLocator.Get<CameraManager>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}

		private void Update()
		{
			if (!cameraManager.RunningWreckCamera())
			{
				base.enabled = false;
			}
			else if (!playerCameraTimer.Running())
			{
				playerCameraTimer.Run(1.2f);
			}
			else if (playerCameraTimer.Done())
			{
				cameraManager.SetPlayerCamera();
				if (panelsManager.ActivePanel.GetPanelType() == PanelType.Tank)
				{
					panelsManager.ShowPanel(PanelType.Game);
				}
				base.enabled = false;
			}
		}
	}
}
