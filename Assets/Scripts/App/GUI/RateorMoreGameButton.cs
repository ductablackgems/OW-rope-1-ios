using UnityEngine;

namespace App.GUI
{
	public class RateorMoreGameButton : MonoBehaviour
	{
		public string googlePlay = "market://search?q=pub:HGames-ArtWorks";

		public string itunes = "itms-apps://itunes.apple.com/us/developer/id1416036753";

		public string amazon = "https://www.amazon.com/s?rh=n%3A2350149011%2Cp_4%3AHGames&_encoding=UTF8&ref=bl_sr_mobile-apps";

		public string tizenStore = "tizenstore://SellerApps/wo8jcczr7o";

		public bool rateOn;

		private void OnClick()
		{
			if (rateOn)
			{
				googlePlay += Application.identifier;
				tizenStore += Application.identifier;
			}
			Application.OpenURL(googlePlay);
		}
	}
}
