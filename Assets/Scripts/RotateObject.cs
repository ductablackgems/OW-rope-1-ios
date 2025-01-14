using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public Vector3 axis;

	public float rate;

	protected void Update()
	{
		base.transform.Rotate(axis * Time.deltaTime * rate);
	}
}
