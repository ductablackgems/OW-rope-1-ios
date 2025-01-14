using App.Player;
using App.Vehicles;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public static class AIUtils
	{
		private static readonly int mask = LayerMask.GetMask("Default", "Player", "Enemy", "Impact", "Climbable", "Ground");

		private static readonly NavMeshPath path = new NavMeshPath();

		private static NavMeshHit hit = default(NavMeshHit);

		private static RaycastHit[] hits = new RaycastHit[16];

		public static bool CanSeePlayer(Vector3 origin, PlayerModel player, float distance, float verticalOffset, out bool vehicleHit)
		{
			vehicleHit = false;
			VehicleComponents vehicle = player.PlayerMonitor.GetVehicle();
			RaycastHit raycastHit = default(RaycastHit);
			raycastHit.distance = float.MaxValue;
			RaycastHit raycastHit2 = raycastHit;
			int num = Physics.SphereCastNonAlloc(origin, 0.2f, (player.Transform.position + Vector3.up * verticalOffset - origin).normalized, hits, distance, mask, QueryTriggerInteraction.Ignore);
			if (num == 0)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit3 = hits[i];
				if (!(raycastHit3.distance > raycastHit2.distance))
				{
					raycastHit2 = raycastHit3;
				}
			}
			if (raycastHit2.collider == null)
			{
				return false;
			}
			WhoIsResult whoIsResult = WhoIs.Resolve(raycastHit2.collider, WhoIs.Masks.AIScanner);
			if (whoIsResult.IsEmpty)
			{
				return false;
			}
			if (whoIsResult.gameObject.CompareTag("Player"))
			{
				return true;
			}
			if (whoIsResult.Compare(WhoIs.Masks.AllVehicles) && vehicle != null && vehicle.transform == whoIsResult.transform)
			{
				vehicleHit = true;
			}
			return true;
		}

		public static bool GetSafeNavmeshPosition(Vector3 origin, Vector3 targetPosition, int areaMask, out Vector3 result, float maxDistance = 2f)
		{
			path.ClearCorners();
			result = Vector3.zero;
			bool flag = NavMesh.SamplePosition(targetPosition, out hit, maxDistance, areaMask);
			if (!flag)
			{
				return false;
			}
			if (!NavMesh.CalculatePath(origin, hit.position, areaMask, path))
			{
				return false;
			}
			switch (path.status)
			{
			case NavMeshPathStatus.PathComplete:
				result = hit.position;
				break;
			case NavMeshPathStatus.PathPartial:
				result = path.corners[path.corners.Length - 1];
				break;
			case NavMeshPathStatus.PathInvalid:
				return false;
			}
			return flag;
		}
	}
}
