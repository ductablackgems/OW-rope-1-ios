using UnityEngine;

public class CameraFollower : MonoBehaviour
{
	public GameObject Target;

	public Vector3 Offset;

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)Target)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Target.transform.position + Offset, Time.deltaTime * 10f);
		}
	}
}
