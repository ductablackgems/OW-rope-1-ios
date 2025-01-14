using UnityEngine;

namespace LlockhamIndustries.Decals
{
	public abstract class Positioner : MonoBehaviour
	{
		public ProjectionRenderer projection;

		public LayerMask layers = -1;

		public bool alwaysVisible;

		private ProjectionRenderer proj;

		public ProjectionRenderer Active => proj;

		private void OnDisable()
		{
			if (proj != null)
			{
				proj.gameObject.SetActive(value: false);
			}
		}

		protected virtual void Start()
		{
			if (projection != null)
			{
				proj = UnityEngine.Object.Instantiate(projection.gameObject, DynamicDecals.System.DefaultPool.Parent).GetComponent<ProjectionRenderer>();
				proj.name = "Positioned Projection";
			}
			else
			{
				UnityEngine.Debug.LogWarning("Positioner has no projection to position.");
			}
		}

		protected void Reproject(Ray Ray, float CastLength, Vector3 ReferenceUp)
		{
			if (proj != null)
			{
				if (Physics.Raycast(Ray, out RaycastHit hitInfo, CastLength, layers.value))
				{
					proj.gameObject.SetActive(value: true);
					proj.transform.rotation = Quaternion.LookRotation(-hitInfo.normal, ReferenceUp);
					proj.transform.position = hitInfo.point;
				}
				else if (!alwaysVisible)
				{
					proj.gameObject.SetActive(value: false);
				}
			}
		}

		private Vector3 Divide(Vector3 A, Vector3 B)
		{
			return new Vector3(A.x / B.x, A.y / B.y, A.z / B.z);
		}
	}
}
