using UnityEngine;

namespace App.Util
{
	public static class GizmoUtils
	{
		public static void DrawBounds(Transform transform, Bounds bounds, Color color)
		{
			Color color2 = Gizmos.color;
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Gizmos.DrawCube(bounds.center, bounds.size);
			Gizmos.color = color2;
		}
	}
}
