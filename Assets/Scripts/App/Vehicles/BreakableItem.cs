using System;
using UnityEngine;

namespace App.Vehicles
{
	public class BreakableItem : MonoBehaviour
	{
		private const float BreakMomentum = 4000f;

		private const float DestroyTime = 30f;

		private bool broken;

		private Transform player;

		public event Action<BreakableItem> Breaked;

		private void OnCollisionEnter(Collision collision)
		{
			if (!collision.gameObject.CompareTag("Vehicle") || broken)
			{
				return;
			}
			float num = collision.relativeVelocity.magnitude * collision.rigidbody.mass;
			if (num > 4000f)
			{
				Rigidbody componentSafe = this.GetComponentSafe<Rigidbody>();
				componentSafe.isKinematic = false;
				Mathf.Clamp(4000f / num, 0f, 1f);
				if (num > 40000f)
				{
					num = 40000f;
				}
				componentSafe.AddForce(UnityEngine.Camera.main.transform.forward * num / 50f, ForceMode.Impulse);
				GetComponent<AudioSource>().Play();
				broken = true;
				UnityEngine.Object.Destroy(base.gameObject, 30f);
			}
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
			Rigidbody componentInChildren = GetComponentInChildren<Rigidbody>();
			componentInChildren.isKinematic = false;
			AudioSource componentInChildren2 = GetComponentInChildren<AudioSource>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.Play();
			}
			float d = 1f;
			if (massModifier > 0f)
			{
				d = componentInChildren.mass * massModifier;
			}
			componentInChildren.AddForce(force * d);
			UnityEngine.Object.Destroy(base.gameObject, 30f);
		}
	}
}
