using UnityEngine;

public class moveLimit : MonoBehaviour
{
	public Vector3 MinRange = new Vector3(-6f, -6f, -6f);

	public Vector3 MaxRange = new Vector3(6f, 6f, 6f);

	private void Update()
	{
		base.transform.localPosition = new Vector3(Mathf.Clamp(base.transform.localPosition.x, MinRange.x, MaxRange.x), Mathf.Clamp(base.transform.localPosition.y, MinRange.y, MaxRange.y), Mathf.Clamp(base.transform.localPosition.z, MinRange.z, MaxRange.z));
	}
}
