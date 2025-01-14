using UnityEngine;

namespace App.Spawn
{
	public class SpawnPoint : MonoBehaviour
	{
		public Vector3 Position => base.transform.position;

		private void OnDrawGizmos()
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(Position, 0.25f);
			Gizmos.color = color;
		}
	}
}
