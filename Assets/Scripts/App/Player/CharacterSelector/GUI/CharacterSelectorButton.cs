using App.Camera;
using App.GUI;
using UnityEngine;

namespace App.Player.CharacterSelector.GUI
{
	public class CharacterSelectorButton : MonoBehaviour
	{
		private PanelsManager panelsManager;

		private CameraManager cameraManager;

		private void Awake()
		{
			panelsManager = ServiceLocator.Get<PanelsManager>();
			cameraManager = ServiceLocator.Get<CameraManager>();
		}

		private void OnClick()
		{
			panelsManager.ShowPanel(PanelType.CharacterSelector);
			cameraManager.SetClothesShopCamera();
		}
	}
}
