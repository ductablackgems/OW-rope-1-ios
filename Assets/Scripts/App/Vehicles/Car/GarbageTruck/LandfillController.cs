using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class LandfillController : MonoBehaviour
	{
		public GameObject Effect;

		private SphereCollider sphereCollider;

		private GarbageTruckManager garbagetruckManager;

		private void Awake()
		{
			sphereCollider = GetComponent<SphereCollider>();
			DeactivateEffect();
		}

		private void OnTriggerEnter(Collider coll)
		{
			GarbageTruckManager component = coll.gameObject.GetComponent<GarbageTruckManager>();
			if ((bool)component)
			{
				if (Vector3.Distance(component.Front.position, base.transform.position) < Vector3.Distance(component.Back.position, base.transform.position))
				{
					component.InFront = true;
				}
				else
				{
					component.InFront = false;
				}
				garbagetruckManager = component;
				garbagetruckManager.InLandfill = true;
			}
		}

		private void OnTriggerExit(Collider coll)
		{
			GarbageTruckManager component = coll.gameObject.GetComponent<GarbageTruckManager>();
			if ((bool)component && (bool)garbagetruckManager && garbagetruckManager.gameObject.GetInstanceID() == component.gameObject.GetInstanceID())
			{
				garbagetruckManager.InLandfill = false;
				garbagetruckManager = null;
			}
		}

		public void ActivateEffect()
		{
			Effect.SetActive(value: true);
			sphereCollider.enabled = true;
		}

		public void DeactivateEffect()
		{
			Effect.SetActive(value: false);
			sphereCollider.enabled = false;
		}
	}
}
