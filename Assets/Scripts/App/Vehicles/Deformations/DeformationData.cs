using UnityEngine;

namespace App.Vehicles.Deformations
{
	internal class DeformationData
	{
		public int uid
		{
			get;
			set;
		}

		public Collider collider
		{
			get;
			set;
		}

		public ImpactDeformable impactDeformable
		{
			get;
			set;
		}

		public float colliderVolume
		{
			get;
			set;
		}

		public DeformationData(ImpactDeformable impactDeformable)
		{
			uid = impactDeformable.GetInstanceID();
			if (impactDeformable.GetComponent<Collider>() != null)
			{
				collider = impactDeformable.GetComponent<Collider>();
				colliderVolume = collider.bounds.size.x * collider.bounds.size.y * collider.bounds.size.z;
			}
			this.impactDeformable = impactDeformable;
		}
	}
}
