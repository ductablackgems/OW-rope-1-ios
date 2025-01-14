using System;
using UnityEngine;

namespace App.Vehicles
{
	public class BreakableItemInstantiate : MonoBehaviour
	{
		private const float BreakMomentum = 4000f;

		private bool broken;

		private Transform player;

		public bool tankObject;

		public GameObject Prefab;

		private static GameObject DynamicObject;

		private static float momentum;

		public event Action<BreakableItemInstantiate> Breaked;

		private void OnCollisionEnter(Collision collision)
		{
			if (broken || !collision.gameObject.CompareTag("Vehicle"))
			{
				return;
			}
			momentum = collision.relativeVelocity.magnitude * collision.rigidbody.mass;
			if (tankObject)
			{
				momentum /= 8f;
			}
			if (momentum > 4000f)
			{
				this.GetComponentSafe<Rigidbody>().isKinematic = false;
				Mathf.Clamp(4000f / momentum, 0f, 1f);
				if (momentum > 40000f)
				{
					momentum = 40000f;
				}
				broken = true;
				DynamicState();
			}
		}

		private void DynamicState()
		{
			if (Prefab != null && !broken)
			{
				broken = true;
				DynamicObject = UnityEngine.Object.Instantiate(Prefab, base.transform.position, base.transform.rotation);
				OnBreaked();
				DynamicObject.GetComponentSafe<Rigidbody>().AddForce(UnityEngine.Camera.main.transform.forward * momentum / 50f, ForceMode.Impulse);
				DynamicObject.GetComponentSafe<AudioSource>().Play();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void OnLaserDamage()
		{
			momentum = 20000f;
			DynamicState();
		}

		private void OnBreaked()
		{
			if (this.Breaked != null)
			{
				this.Breaked(this);
				this.Breaked = null;
			}
		}

		public void DestroyByForce(Vector3 force, float massModifier = 0.5f)
		{
			if (Prefab != null)
			{
				DynamicObject = UnityEngine.Object.Instantiate(Prefab, base.transform.position, base.transform.rotation);
				OnBreaked();
				BreakableItemInstantiate component = DynamicObject.GetComponent<BreakableItemInstantiate>();
				if (!(component == null))
				{
					component.DestroyByForce(force, massModifier);
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
