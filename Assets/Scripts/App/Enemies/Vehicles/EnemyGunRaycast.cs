using UnityEngine;

namespace App.Enemies.Vehicles
{
	public class EnemyGunRaycast : MonoBehaviour
	{
		public bool HitPlayer(float distance)
		{
			Vector3 vector = base.transform.TransformDirection(Vector3.forward);
			UnityEngine.Debug.DrawRay(base.transform.position, vector, Color.red);
			int layerMask = 1063937;
			if (Physics.Raycast(base.transform.position, vector, out RaycastHit hitInfo, distance, layerMask))
			{
				if (hitInfo.transform.gameObject.layer != 11)
				{
					return WhoIs.Compare(hitInfo.collider, WhoIs.Entities.Player);
				}
				return true;
			}
			return false;
		}
	}
}
