using UnityEngine;

public class Example : MonoBehaviour
{
	public bool debugPoint;

	public Vector3 debugPoint_Position;

	public float debugPoint_Scale;

	public Color debugPoint_Color;

	public bool debugBounds;

	public Vector3 debugBounds_Position;

	public Vector3 debugBounds_Size;

	public Color debugBounds_Color;

	public bool debugCircle;

	public Vector3 debugCircle_Up;

	public float debugCircle_Radius;

	public Color debugCircle_Color;

	public bool debugWireSphere;

	public float debugWireSphere_Radius;

	public Color debugWireSphere_Color;

	public bool debugCylinder;

	public Vector3 debugCylinder_End;

	public float debugCylinder_Radius;

	public Color debugCylinder_Color;

	public bool debugCone;

	public Vector3 debugCone_Direction;

	public float debugCone_Angle;

	public Color debugCone_Color;

	public bool debugArrow;

	public Vector3 debugArrow_Direction;

	public Color debugArrow_Color;

	public bool debugCapsule;

	public Vector3 debugCapsule_End;

	public float debugCapsule_Radius;

	public Color debugCapsule_Color;

	private void OnDrawGizmos()
	{
		if (debugPoint)
		{
			Debug2.DrawPoint(debugPoint_Position, debugPoint_Color, debugPoint_Scale);
		}
		if (debugBounds)
		{
			Debug2.DrawBounds(new Bounds(new Vector3(10f, 0f, 0f), debugBounds_Size), debugBounds_Color);
		}
		if (debugCircle)
		{
			Debug2.DrawCircle(new Vector3(20f, 0f, 0f), debugCircle_Up, debugCircle_Color, debugCircle_Radius);
		}
		if (debugWireSphere)
		{
			Gizmos.color = debugWireSphere_Color;
			Gizmos.DrawWireSphere(new Vector3(30f, 0f, 0f), debugWireSphere_Radius);
		}
		if (debugCylinder)
		{
			Debug2.DrawCylinder(new Vector3(40f, 0f, 0f), debugCylinder_End, debugCylinder_Color, debugCylinder_Radius);
		}
		if (debugCone)
		{
			Debug2.DrawCone(new Vector3(50f, 0f, 0f), debugCone_Direction, debugCone_Color, debugCone_Angle);
		}
		if (debugArrow)
		{
			Debug2.DrawArrow(new Vector3(60f, 0f, 0f), debugArrow_Direction, debugArrow_Color);
		}
		if (debugCapsule)
		{
			Debug2.DrawCapsule(new Vector3(70f, 0f, 0f), debugCapsule_End, debugCapsule_Color, debugCapsule_Radius);
		}
	}

	private void Update()
	{
		Debug2.DebugPoint(debugPoint_Position, debugPoint_Color, debugPoint_Scale);
		Debug2.DebugBounds(new Bounds(new Vector3(10f, 0f, 0f), debugBounds_Size), debugBounds_Color);
		Debug2.DebugCircle(new Vector3(20f, 0f, 0f), debugCircle_Up, debugCircle_Color, debugCircle_Radius);
		Debug2.DebugWireSphere(new Vector3(30f, 0f, 0f), debugWireSphere_Color, debugWireSphere_Radius);
		Debug2.DebugCylinder(new Vector3(40f, 0f, 0f), debugCylinder_End, debugCylinder_Color, debugCylinder_Radius);
		Debug2.DebugCone(new Vector3(50f, 0f, 0f), debugCone_Direction, debugCone_Color, debugCone_Angle);
		Debug2.DebugArrow(new Vector3(60f, 0f, 0f), debugArrow_Direction, debugArrow_Color);
		Debug2.DebugCapsule(new Vector3(70f, 0f, 0f), debugCapsule_End, debugCapsule_Color, debugCapsule_Radius);
	}
}
