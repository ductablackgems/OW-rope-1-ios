using App.Util;
using UnityEngine;

namespace App.Vehicles.Airplane.AI
{
	public class BoundsValidator : MonoBehaviour
	{
		[SerializeField]
		private Bounds bounds;

		[SerializeField]
		private Color color = Color.green;

		private Collider[] colliders = new Collider[10];

		public bool Validate(int layerMask)
		{
			return Physics.OverlapBoxNonAlloc(base.transform.position + bounds.center, bounds.extents, colliders, base.transform.rotation, layerMask, QueryTriggerInteraction.Ignore) == 0;
		}

		private void OnDrawGizmosSelected()
		{
			GizmoUtils.DrawBounds(base.transform, bounds, color);
		}
	}
}
