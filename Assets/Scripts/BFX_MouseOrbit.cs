using System;
using UnityEngine;

public class BFX_MouseOrbit : MonoBehaviour
{
	public GameObject target;

	public float distance = 10f;

	public float xSpeed = 250f;

	public float ySpeed = 120f;

	public float yMinLimit = -20f;

	public float yMaxLimit = 80f;

	private float x;

	private float y;

	private float prevDistance;

	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
	}

	private void LateUpdate()
	{
		if (distance < 2f)
		{
			distance = 2f;
		}
		distance -= UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 2f;
		if ((bool)target && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
		{
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			float num = 1f;
			if (Screen.dpi < 1f)
			{
				num = 1f;
			}
			num = ((!(Screen.dpi < 200f)) ? (Screen.dpi / 200f) : 1f);
			if (mousePosition.x < 380f * num && (float)Screen.height - mousePosition.y < 250f * num)
			{
				return;
			}
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			x += UnityEngine.Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= UnityEngine.Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			Quaternion rotation = Quaternion.Euler(y, x, 0f);
			Vector3 position = rotation * new Vector3(0f, 0f, 0f - distance) + target.transform.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
		}
		else
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		if (Math.Abs(prevDistance - distance) > 0.001f)
		{
			prevDistance = distance;
			Quaternion rotation2 = Quaternion.Euler(y, x, 0f);
			Vector3 position2 = rotation2 * new Vector3(0f, 0f, 0f - distance) + target.transform.position;
			base.transform.rotation = rotation2;
			base.transform.position = position2;
		}
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
