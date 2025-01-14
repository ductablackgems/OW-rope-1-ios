using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	public Transform target;

	public float distance = 10f;

	public float height = 5f;

	public float heightDamping = 2f;

	public float rotationDamping = 3f;

	[AddComponentMenu("Camera-Control/Smooth Follow")]
	private void LateUpdate()
	{
		if ((bool)target)
		{
			float y = target.eulerAngles.y;
			float b = target.position.y + height;
			float y2 = base.transform.eulerAngles.y;
			float y3 = base.transform.position.y;
			y2 = Mathf.LerpAngle(y2, y, rotationDamping * Time.deltaTime);
			y3 = Mathf.Lerp(y3, b, heightDamping * Time.deltaTime);
			Quaternion rotation = Quaternion.Euler(0f, y2, 0f);
			base.transform.position = target.position;
			base.transform.position -= rotation * Vector3.forward * distance;
			base.transform.position = new Vector3(base.transform.position.x, y3, base.transform.position.z);
			base.transform.LookAt(target);
		}
	}
}
