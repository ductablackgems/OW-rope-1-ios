using UnityEngine;

public class GizmoObject : MonoBehaviour
{
	public GizmoShape gizmoShape;

	public Color gizmoColor = Color.white;

	public float gizmoSize = 1f;

	public bool wireMode;

	public bool drawRay;

	public float rayLength = 2f;

	private void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		if (drawRay)
		{
			Vector3 a = base.transform.TransformDirection(Vector3.fwd);
			Gizmos.DrawRay(base.transform.position, a * rayLength);
		}
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
		switch (gizmoShape)
		{
		case GizmoShape.Cube:
			if (wireMode)
			{
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one * gizmoSize);
			}
			else
			{
				Gizmos.DrawCube(Vector3.zero, Vector3.one * gizmoSize);
			}
			break;
		case GizmoShape.Sphere:
			if (wireMode)
			{
				Gizmos.DrawWireSphere(Vector3.zero, gizmoSize);
			}
			else
			{
				Gizmos.DrawSphere(Vector3.zero, gizmoSize);
			}
			break;
		}
	}
}
