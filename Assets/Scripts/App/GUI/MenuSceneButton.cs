using App.Levels;
using UnityEngine;

namespace App.GUI
{
	public class MenuSceneButton : MonoBehaviour
	{
		private SceneLoadManager sceneLoadManager;

		protected void Awake()
		{
			sceneLoadManager = ServiceLocator.Get<SceneLoadManager>();
		}

		protected void OnClick()
		{
			CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_pause, () =>
			{
				Time.timeScale = 1f;
				sceneLoadManager.LoadMainMenu();
			});
		}
	}
}
