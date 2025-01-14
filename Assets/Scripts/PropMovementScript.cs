using UnityEngine;

public class PropMovementScript : MonoBehaviour
{
	public float TrahsHold;

	public bool isMoving;

	public Vector3 targetPosition;

	public float speed;

	public bool isRotating;

	public Vector3 targetRotation;

	public float speedRotation;

	private Vector3 startPosition;

	private Vector3 startRotation;

	private void Start()
	{
		startPosition = base.gameObject.transform.position;
		startRotation = base.gameObject.transform.localEulerAngles;
		if (TrahsHold < 0.15f)
		{
			TrahsHold = 0.15f;
		}
	}

	private void Update()
	{
		if (isMoving)
		{
			Vector3 vector = Vector3.MoveTowards(base.gameObject.transform.position, targetPosition, speed * Time.deltaTime);
			if (Mathf.Abs(targetPosition.x - vector.x) < TrahsHold && Mathf.Abs(targetPosition.y - vector.y) < TrahsHold && Mathf.Abs(targetPosition.z - vector.z) < TrahsHold)
			{
				base.transform.position = startPosition;
			}
			else
			{
				base.transform.position = vector;
			}
		}
		if (isRotating)
		{
			Vector3 vector2 = Vector3.RotateTowards(base.gameObject.transform.localEulerAngles, targetRotation, speedRotation * Time.deltaTime, 1f);
			if (Mathf.Abs(targetRotation.x - vector2.x) < TrahsHold && Mathf.Abs(targetRotation.y - vector2.y) < TrahsHold && Mathf.Abs(targetRotation.z - vector2.z) < TrahsHold)
			{
				base.transform.localEulerAngles = startRotation;
			}
			else
			{
				base.transform.localEulerAngles = vector2;
			}
		}
	}
}
