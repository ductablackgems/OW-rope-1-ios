using UnityEngine;

public class PlayerControl : CarControl
{
	private void Update()
	{
		ControlCar(UnityEngine.Input.GetAxis("Vertical"), UnityEngine.Input.GetAxis("Horizontal"));
	}
}
