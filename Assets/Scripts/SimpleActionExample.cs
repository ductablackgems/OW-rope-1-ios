using HedgehogTeam.EasyTouch;
using UnityEngine;

public class SimpleActionExample : MonoBehaviour
{
	private TextMesh textMesh;

	private Vector3 startScale;

	private void Start()
	{
		textMesh = GetComponentInChildren<TextMesh>();
		startScale = base.transform.localScale;
	}

	public void ChangeColor(Gesture gesture)
	{
		RandomColor();
	}

	public void TimePressed(Gesture gesture)
	{
		textMesh.text = "Down since :" + gesture.actionTime.ToString("f2");
	}

	public void DisplaySwipeAngle(Gesture gesture)
	{
		float swipeOrDragAngle = gesture.GetSwipeOrDragAngle();
		textMesh.text = swipeOrDragAngle.ToString("f2") + " / " + gesture.swipe.ToString();
	}

	public void ChangeText(string text)
	{
		textMesh.text = text;
	}

	public void ResetScale()
	{
		base.transform.localScale = startScale;
	}

	private void RandomColor()
	{
		base.gameObject.GetComponent<Renderer>().material.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
	}
}
