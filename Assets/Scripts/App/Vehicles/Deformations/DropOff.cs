using UnityEngine;

namespace App.Vehicles.Deformations
{
	[RequireComponent(typeof(ImpactDeformable))]
	public class DropOff : MonoBehaviour
	{
		public float impactLimit = 0.1f;

		private bool isDroppedOff;

		private void Awake()
		{
			GetComponent<ImpactDeformable>().OnDeform += OnDeform;
		}

		public void OnDisable()
		{
			GetComponent<ImpactDeformable>().OnDeform -= OnDeform;
		}

		public void Now()
		{
			if (!isDroppedOff)
			{
				Process();
			}
		}

		private void OnDeform(ImpactDeformable deformable)
		{
			if (!isDroppedOff && deformable.StructuralDamage > impactLimit)
			{
				Process();
			}
		}

		private void Process()
		{
			base.gameObject.tag = "FallenVehiclePart";
			base.transform.parent = base.transform.root.parent;
			Rigidbody rigidbody = (!base.gameObject.GetComponent<Rigidbody>()) ? base.gameObject.AddComponent<Rigidbody>() : base.gameObject.GetComponent<Rigidbody>();
			rigidbody.mass = 0.1f;
			if (!GetComponent<Destroy>())
			{
				base.gameObject.AddComponent<Destroy>();
			}
			isDroppedOff = true;
		}
	}
}
