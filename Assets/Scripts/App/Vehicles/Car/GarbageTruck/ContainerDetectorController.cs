using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class ContainerDetectorController : MonoBehaviour
	{
		private GarbageTruckManager garbagetruckManager;

		public ContainerParameters ActualContainer
		{
			get;
			set;
		}

		private void Awake()
		{
			garbagetruckManager = base.gameObject.GetComponentInParent<GarbageTruckManager>();
		}

		private void OnTriggerEnter(Collider coll)
		{
			if (!garbagetruckManager || !coll.gameObject.CompareTag("Dumpster") || !(coll.gameObject.name == "ContactCollider"))
			{
				return;
			}
			ContainerParameters componentInParent = coll.gameObject.GetComponentInParent<ContainerParameters>();
			if ((bool)componentInParent)
			{
				float num = Quaternion.Angle(garbagetruckManager.transform.rotation, componentInParent.transform.rotation);
				if (num >= 85f && num <= 95f)
				{
					ActualContainer = componentInParent;
				}
			}
		}

		private void OnTriggerExit(Collider coll)
		{
			if (coll.gameObject.CompareTag("Dumpster") && coll.gameObject.name == "ContactCollider")
			{
				ContainerParameters componentInParent = coll.gameObject.GetComponentInParent<ContainerParameters>();
				if ((bool)componentInParent && (bool)ActualContainer && componentInParent.gameObject.GetInstanceID() == ActualContainer.gameObject.GetInstanceID())
				{
					ActualContainer = null;
				}
			}
		}
	}
}
