using UnityEngine;

namespace O3DWB
{
	public static class ObjectHierarchySnap
	{
		public static void Snap(GameObject root, Vector3 pivotPoint, Vector3 destPoint)
		{
			Transform transform = root.transform;
			Vector3 b = transform.position - pivotPoint;
			transform.position = destPoint + b;
		}
	}
}
