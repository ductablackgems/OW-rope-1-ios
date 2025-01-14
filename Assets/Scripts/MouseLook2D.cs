using UnityEngine;

public class MouseLook2D : MonoBehaviour
{
	public Camera CurrentCamera;

	public float MaxAimRange = 10000f;

	public Vector3 AimPoint;

	public bool LockX;

	public bool LockY;

	public bool LockZ;

	public GameObject AimObject;

	private void Start()
	{
	}

	private void Update()
	{
		if (CurrentCamera == null)
		{
			CurrentCamera = Camera.main;
			if (CurrentCamera == null)
			{
				CurrentCamera = Camera.current;
			}
		}
		Ray ray = CurrentCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, MaxAimRange))
		{
			AimPoint = hitInfo.point;
			AimPoint = AxisLock(AimPoint);
			AimObject = hitInfo.collider.gameObject;
		}
		else
		{
			AimPoint = ray.origin + ray.direction * MaxAimRange;
			AimPoint = AxisLock(AimPoint);
			AimObject = null;
		}
		if ((bool)AimObject)
		{
			base.gameObject.transform.LookAt(AimObject.transform.position + AimPoint);
		}
		else
		{
			base.gameObject.transform.LookAt(AimPoint);
		}
	}

	public Vector3 AxisLock(Vector3 axis)
	{
		if (LockX)
		{
			axis.x = base.transform.position.x;
		}
		if (LockY)
		{
			axis.y = base.transform.position.y;
		}
		if (LockZ)
		{
			axis.z = base.transform.position.z;
		}
		return axis;
	}
}
