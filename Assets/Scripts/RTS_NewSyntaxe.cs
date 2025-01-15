using HedgehogTeam.EasyTouch;
using UnityEngine;

public class RTS_NewSyntaxe : MonoBehaviour
{
	private GameObject cube;

	private void Start()
	{
		cube = null;
	}

	private void Update()
	{
		Gesture current = EasyTouch.current;
		if (current.type == EasyTouch.EvtType.On_SimpleTap && current.pickedObject != null && current.pickedObject.name == "Cube")
		{
			ResteColor();
			cube = current.pickedObject;
			cube.GetComponent<Renderer>().material.color = Color.red;
			base.transform.Translate(Vector2.up, Space.World);
		}
		if (current.type == EasyTouch.EvtType.On_Swipe && current.touchCount == 1)
		{
			base.transform.Translate(Vector3.left * current.deltaPosition.x / Screen.width);
			base.transform.Translate(Vector3.back * current.deltaPosition.y / Screen.height);
		}
		if (current.type == EasyTouch.EvtType.On_Pinch)
		{
			Camera.main.fieldOfView += current.deltaPinch * 10f * Time.deltaTime;
		}
		if (current.type == EasyTouch.EvtType.On_Twist)
		{
			base.transform.Rotate(Vector3.up * current.twistAngle);
		}
	}

	private void ResteColor()
	{
		if (cube != null)
		{
			cube.GetComponent<Renderer>().material.color = new Color(0.235294119f, 143f / 255f, 67f / 85f);
		}
	}
}
