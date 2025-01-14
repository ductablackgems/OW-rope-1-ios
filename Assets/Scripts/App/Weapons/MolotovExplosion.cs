using App.Player.SkeletonEffect;
using App.Spawn;
using App.Util;
using App.Vehicles;
using System;
using UnityEngine;

namespace App.Weapons
{
	public class MolotovExplosion : MonoBehaviour, IResetable
	{
		public float explosionRadius = 10f;

		public float explosionDelay = 3f;

		public float minimalExplosionImpulse = 5f;

		public GameObject unbreakedObject;

		public GameObject explosionObject;

		[NonSerialized]
		public GameObject owner;

		private Rigidbody _rigidbody;

		private Collider _collider;

		private DurationTimer explosionTimer = new DurationTimer();

		public void ResetStates()
		{
			_rigidbody.isKinematic = false;
			_collider.isTrigger = false;
			base.enabled = true;
			unbreakedObject.SetActive(value: true);
			explosionObject.SetActive(value: false);
			owner = null;
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			_collider = this.GetComponentSafe<Collider>();
		}

		private void OnEnable()
		{
			explosionTimer.Run(explosionDelay);
		}

		private void Update()
		{
			if (explosionTimer.Done())
			{
				Explode();
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (base.enabled && collision.impulse.magnitude > minimalExplosionImpulse)
			{
				Explode();
			}
		}

		private void Explode()
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, explosionRadius);
			foreach (Collider collider in array)
			{
				WhoIsResult whoIsResult = WhoIs.Resolve(collider, WhoIs.Masks.MolotovExplosion);
				if (whoIsResult.IsEmpty)
				{
					continue;
				}
				if (whoIsResult.Compare(WhoIs.Masks.AllRagdollableHumans))
				{
					HumanFireManager component = whoIsResult.gameObject.GetComponent<HumanFireManager>();
					if (!(component == null))
					{
						component.StartFire(owner);
					}
				}
				else
				{
					if (!whoIsResult.Compare(WhoIs.Masks.AllVehicles))
					{
						continue;
					}
					VehicleComponents component2 = whoIsResult.gameObject.GetComponent<VehicleComponents>();
					if (component2 == null)
					{
						continue;
					}
					GameObject personByCollider = component2.GetPersonByCollider(collider);
					if (personByCollider != null)
					{
						HumanFireManager component3 = personByCollider.GetComponent<HumanFireManager>();
						if (component3 != null)
						{
							component3.StartFire(owner);
						}
					}
					if (!whoIsResult.Compare(WhoIs.Masks.AllStreetVehicles))
					{
						whoIsResult.gameObject.GetComponentSafe<Health>().ApplyDamage(150f, 2, owner);
					}
				}
			}
			explosionTimer.Stop();
			_rigidbody.isKinematic = true;
			_collider.isTrigger = true;
			base.enabled = false;
			unbreakedObject.SetActive(value: false);
			explosionObject.SetActive(value: true);
			DecalInstanced[] componentsInChildren = explosionObject.GetComponentsInChildren<DecalInstanced>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Ignition();
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, explosionRadius);
		}
	}
}
