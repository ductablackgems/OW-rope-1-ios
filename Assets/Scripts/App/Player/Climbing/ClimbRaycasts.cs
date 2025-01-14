using UnityEngine;

namespace App.Player.Climbing
{
	public class ClimbRaycasts : MonoBehaviour
	{
		public LayerMask fullMask;

		public float distance;

		public ClimbRaycast headRaycast;

		public ClimbRaycast topRaycast;

		public ClimbRaycast bottomRaycast;

		public ClimbRaycast topEdgeRaycast;

		private void Awake()
		{
			headRaycast.mask = fullMask;
			headRaycast.distance = distance;
			topRaycast.mask = fullMask;
			topRaycast.distance = distance;
			bottomRaycast.mask = fullMask;
			bottomRaycast.distance = distance;
			topEdgeRaycast.mask = fullMask;
			topEdgeRaycast.distance = distance;
		}

		private void OnDrawGizmos()
		{
			DrawRayGizmo(headRaycast);
			DrawRayGizmo(topRaycast);
			DrawRayGizmo(bottomRaycast);
			DrawRayGizmo(topEdgeRaycast);
		}

		private void DrawRayGizmo(ClimbRaycast raycast)
		{
			if (raycast.transform != null)
			{
				DrawArrow.ForGizmo(raycast.transform.position, raycast.transform.forward * distance, distance / 3f);
			}
		}
	}
}
