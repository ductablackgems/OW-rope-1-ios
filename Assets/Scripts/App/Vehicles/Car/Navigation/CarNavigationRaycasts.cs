using App.Player.Climbing;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class CarNavigationRaycasts : MonoBehaviour
	{
		public LayerMask fullMask;

		public float distance;

		public float middleRayDistance;

		public float backRayDistance;

		public ClimbRaycast frontMiddle;

		public ClimbRaycast frontLeft0;

		public ClimbRaycast frontLeft1;

		public ClimbRaycast frontRight0;

		public ClimbRaycast frontRight1;

		public ClimbRaycast backMiddle;

		public void SetDelay(float delay)
		{
			frontMiddle.delay = delay;
			frontLeft0.delay = delay;
			frontLeft1.delay = delay;
			frontRight0.delay = delay;
			frontRight1.delay = delay;
			backMiddle.delay = delay;
		}

		private void Awake()
		{
			frontMiddle.mask = fullMask;
			frontMiddle.distance = middleRayDistance;
			frontLeft0.mask = fullMask;
			frontLeft0.distance = distance;
			frontLeft1.mask = fullMask;
			frontLeft1.distance = distance;
			frontRight0.mask = fullMask;
			frontRight0.distance = distance;
			frontRight1.mask = fullMask;
			frontRight1.distance = distance;
			backMiddle.mask = fullMask;
			backMiddle.distance = backRayDistance;
		}

		private void OnDrawGizmos()
		{
			DrawRayGizmo(frontMiddle, middleRayDistance);
			DrawRayGizmo(frontLeft0, distance);
			DrawRayGizmo(frontLeft1, distance);
			DrawRayGizmo(frontRight0, distance);
			DrawRayGizmo(frontRight1, distance);
			DrawRayGizmo(backMiddle, backRayDistance);
		}

		private void DrawRayGizmo(ClimbRaycast raycast, float distance)
		{
			if (raycast.transform != null)
			{
				Color color = Color.white;
				if (Application.isPlaying && raycast.Hit(keepOldValue: true))
				{
					color = (raycast.HitClimbable(keepOldValue: true) ? Color.yellow : Color.blue);
					distance = raycast.GetHitDistance(keepOldValue: true);
				}
				DrawArrow.ForGizmo(raycast.transform.position, raycast.transform.forward * distance, color, Mathf.Clamp(distance / 3f, 0.1f, 0.4f));
			}
		}
	}
}
