using UnityEngine;

public class FreeflightCamera : MonoBehaviour
{
	public float speedNormal = 10f;

	public float speedFast = 50f;

	public float mouseSensitivityX = 5f;

	public float mouseSensitivityY = 5f;

	private float rotY;

	private void Start()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	private void FixedUpdate()
	{
		float num = 1f;
		if (Input.GetMouseButton(1))
		{
			float y = base.transform.localEulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * mouseSensitivityX * num;
			rotY += UnityEngine.Input.GetAxis("Mouse Y") * mouseSensitivityY * num;
			rotY = Mathf.Clamp(rotY, -89.5f, 89.5f);
			base.transform.localEulerAngles = new Vector3(0f - rotY, y, 0f);
		}
		float axis = UnityEngine.Input.GetAxis("Vertical");
		float axis2 = UnityEngine.Input.GetAxis("Horizontal");
		if (axis != 0f)
		{
			float num2 = UnityEngine.Input.GetKey(KeyCode.LeftShift) ? speedFast : speedNormal;
			Vector3 point = new Vector3(0f, 0f, axis * num2 * Time.deltaTime);
			base.gameObject.transform.localPosition += base.gameObject.transform.localRotation * point;
		}
		if (axis2 != 0f)
		{
			float num3 = UnityEngine.Input.GetKey(KeyCode.LeftShift) ? speedFast : speedNormal;
			Vector3 point2 = new Vector3(axis2 * num3 * Time.deltaTime, 0f, 0f);
			base.gameObject.transform.localPosition += base.gameObject.transform.localRotation * point2;
		}
	}
}
