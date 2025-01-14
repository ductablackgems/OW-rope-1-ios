using App.Camera;
using UnityEngine;

namespace App.GUI
{
	public class DogShopButton : MonoBehaviour
	{
		private CameraManager cameraManager;

		private PanelsManager panels;

		private void Awake()
		{
			cameraManager = ServiceLocator.Get<CameraManager>();
			panels = ServiceLocator.Get<PanelsManager>();
		}

		private void OnClick()
		{
			cameraManager.SetDogShopCamera();
			(panels.ShowPanel(PanelType.DogShop) as DogShopPanel).Show();
		}
	}
}
