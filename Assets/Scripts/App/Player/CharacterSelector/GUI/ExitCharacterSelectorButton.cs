using App.Camera;
using App.GUI;
using UnityEngine;

namespace App.Player.CharacterSelector.GUI
{
	public class ExitCharacterSelectorButton : MonoBehaviour
	{
		private PlayerSwitch playerSwitch;

		private PanelsManager panelsManager;

		private CameraManager cameraManager;

		private void Awake()
		{
			playerSwitch = ServiceLocator.Get<PlayerSwitch>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
			cameraManager = ServiceLocator.Get<CameraManager>();
		}

		private void OnClick()
		{
			playerSwitch.Revert();
			panelsManager.ShowPanel(PanelType.MainMenu);
			cameraManager.SetMenuCamera();
		}
	}
}
