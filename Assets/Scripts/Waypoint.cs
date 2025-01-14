using UnityEngine;

public class Waypoint : MonoBehaviour
{
	private void Start()
	{
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = false;
		}
	}
}
