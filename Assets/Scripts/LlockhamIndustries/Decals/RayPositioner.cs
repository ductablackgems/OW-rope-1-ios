using UnityEngine;

namespace LlockhamIndustries.Decals
{
	public class RayPositioner : Positioner
	{
		public Transform rayTransform;

		public Vector3 positionOffset;

		public Vector3 rotationOffset;

		public float castLength = 100f;

		private void LateUpdate()
		{
			Transform obj = (rayTransform != null) ? rayTransform : base.transform;
			Quaternion rotation = obj.rotation * Quaternion.Euler(rotationOffset);
			Vector3 origin = obj.position + rotation * positionOffset;
			Ray ray = new Ray(origin, rotation * Vector3.forward);
			Reproject(ray, castLength, rotation * Vector3.up);
		}

		private void OnDrawGizmosSelected()
		{
			Transform obj = (rayTransform != null) ? rayTransform : base.transform;
			Quaternion rotation = obj.rotation * Quaternion.Euler(rotationOffset);
			Vector3 from = obj.position + rotation * positionOffset;
			Gizmos.color = Color.black;
			Gizmos.DrawRay(from, rotation * Vector3.up * 0.4f);
			Gizmos.color = Color.white;
			Gizmos.DrawRay(from, rotation * Vector3.forward * castLength);
		}
	}
}
