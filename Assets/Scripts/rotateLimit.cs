using UnityEngine;

public class rotateLimit : MonoBehaviour
{
	public Vector3 MinRange = new Vector3(0f, 0f, 0f);

	public Vector3 MaxRange = new Vector3(360f, 360f, 360f);

	private void Update()
	{
		base.transform.localEulerAngles = new Vector3(Mathf.Clamp(base.transform.localEulerAngles.x, MinRange.x, MaxRange.x), Mathf.Clamp(base.transform.localEulerAngles.y, MinRange.y, MaxRange.y), Mathf.Clamp(base.transform.localEulerAngles.z, MinRange.z, MaxRange.z));
	}
}
