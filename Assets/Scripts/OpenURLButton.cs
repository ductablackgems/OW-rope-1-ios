using UnityEngine;

public class OpenURLButton : MonoBehaviour
{
	public string link = "https://www.facebook.com/hgamesart/?fref=ts";

	public void url()
	{
		Application.OpenURL(link);
	}
}
