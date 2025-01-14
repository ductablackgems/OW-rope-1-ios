using UnityEngine;
using UnityEngine.UI;

public class CFX_Demo_GTButton : MonoBehaviour
{
	public string Callback;

	public GameObject Receiver;

	public void OnClick()
	{
		Receiver.SendMessage(Callback);
	}
}
