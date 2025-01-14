using UnityEngine;
using UnityEngine.UI;

public class CFX_Demo_GTToggle : MonoBehaviour
{
	public Texture Normal;

	public Texture Hover;

	public Color NormalColor = new Color32(128, 128, 128, 128);

	public Color DisabledColor = new Color32(128, 128, 128, 48);

	public bool State = true;

	public string Callback;

	public GameObject Receiver;

	private bool Over;

	private Text Label;

	private void Awake()
	{
		Label = GetComponentInChildren<Text>();
		//UpdateTexture();
	}

	//private void Update()
	//{
	//	if (CollisionRect.Contains(UnityEngine.Input.mousePosition))
	//	{
	//		Over = true;
	//		if (Input.GetMouseButtonDown(0))
	//		{
	//			OnClick();
	//		}
	//	}
	//	else
	//	{
	//		Over = false;
	//		GetComponent<Texture>().color = NormalColor;
	//	}
	//	UpdateTexture();
	//}

	public void OnClick()
	{
		State = !State;
		Receiver.SendMessage(Callback);
	}

	//private void UpdateTexture()
	//{
	//	Color color = State ? NormalColor : DisabledColor;
	//	if (Over)
	//	{
	//		GetComponent<Texture>().texture = Hover;
	//	}
	//	else
	//	{
	//		GetComponent<Texture>().texture = Normal;
	//	}
	//	GetComponent<Texture>().color = color;
	//	if (Label != null)
	//	{
	//		Label.color = color * 1.75f;
	//	}
	//}
}
