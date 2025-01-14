using UnityEngine;

namespace App.Vehicles.Deformations
{
	public class WheelColliderCollision : MonoBehaviour
	{
		[Tooltip("force the wheel is deforming object below")]
		public float force = 0.3f;

		private WheelCollider wheelCollider;

		private ImpactDeformable impactDeformable;

		private void Start()
		{
			wheelCollider = GetComponent<WheelCollider>();
		}

		public void OnTriggerEnter(Collider other)
		{
			if (wheelCollider.isGrounded)
			{
				impactDeformable = other.GetComponent<ImpactDeformable>();
				if ((bool)impactDeformable)
				{
					Process(other, impactDeformable);
				}
			}
		}

		private void Process(Collider other, ImpactDeformable impactDeformable)
		{
			if (Physics.Raycast(base.transform.position, other.transform.position - base.transform.position, out RaycastHit hitInfo, 5f))
			{
				impactDeformable.Deform(hitInfo.point, Vector3.down * force);
			}
		}
	}
}
