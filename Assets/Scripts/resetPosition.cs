using UnityEngine;

public class resetPosition : MonoBehaviour
{
	public Vector3 position;

	public void resetPos()
	{
		base.transform.position = position;
	}
}
