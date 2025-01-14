using UnityEngine;

namespace App.Quests
{
	public class ObjectiveWaypoint : MonoBehaviour
	{
		public Vector3 Position => base.transform.position;

		public void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		private void OnDrawGizmos()
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(base.transform.position, 0.5f);
			Gizmos.color = color;
		}
	}
}
