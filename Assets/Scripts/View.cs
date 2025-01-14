using UnityEngine;

public class View : MonoBehaviour
{
	public Transform Target;

	private void Update()
	{
		base.transform.LookAt(Target, Vector3.up);
	}
}
