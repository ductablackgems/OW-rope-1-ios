using App.Levels;
using UnityEngine;

namespace App.GUI
{
	public class PlayGameButton : MonoBehaviour
	{
		public GameObject panelInternet;
		private SceneLoadManager sceneLoadManager;

		private void Awake()
		{
			sceneLoadManager = ServiceLocator.Get<SceneLoadManager>();
		}

		private void OnClick()
		{
			if (PlayerPrefs.GetInt("daylogin") >= 4)
			{
				if (CallAdsManager.CheckInternet())
				{
					CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.menu_play, () =>
					{
						sceneLoadManager.LoadGame();
					});
				}
				else
				{
					panelInternet.SetActive(true);
				}
			}
			else
			{
				CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.menu_play, () =>
				{
					sceneLoadManager.LoadGame();
				});
			}
				
			// CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.menu_play, () =>
			// {
			// 	sceneLoadManager.LoadGame();
			// });                                                                                                                                                    
		}
	}
}
