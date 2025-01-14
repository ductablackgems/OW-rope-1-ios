using UnityEngine;

public class RotatingScript : MonoBehaviour
{
	public float xRotation;

	public float yRotation;

	public float zRotation;

	public float SpeedRotation = 5f;

	private Vector3 _currentRotation;

	private void Start()
	{
		_currentRotation = base.gameObject.transform.rotation.eulerAngles;
	}

	private void Update()
	{
		base.transform.Rotate(new Vector3(xRotation, yRotation, zRotation), SpeedRotation);
	}
}
