using UnityEngine;

public static class DrawArrow
{
	public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Gizmos.DrawRay(pos, direction);
		Vector3 a = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		Vector3 a2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		Gizmos.DrawRay(pos + direction, a * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, a2 * arrowHeadLength);
	}

	public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Gizmos.color = color;
		Gizmos.DrawRay(pos, direction);
		Vector3 a = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		Vector3 a2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		Gizmos.DrawRay(pos + direction, a * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, a2 * arrowHeadLength);
	}

	public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		UnityEngine.Debug.DrawRay(pos, direction);
		Vector3 a = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		Vector3 a2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		UnityEngine.Debug.DrawRay(pos + direction, a * arrowHeadLength);
		UnityEngine.Debug.DrawRay(pos + direction, a2 * arrowHeadLength);
	}

	public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		UnityEngine.Debug.DrawRay(pos, direction, color);
		Vector3 a = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		Vector3 a2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
		UnityEngine.Debug.DrawRay(pos + direction, a * arrowHeadLength, color);
		UnityEngine.Debug.DrawRay(pos + direction, a2 * arrowHeadLength, color);
	}
}
