using App;
using App.Player;
using App.Spawn.Pooling;
using App.Util;
using App.Weapons;
using System.Collections.Generic;
using UnityEngine;

public class Damage : DamageBase
{
	public ProjectileImpactScheme impactScheme;

	public bool Explosive;

	public float ExplosionRadius = 20f;

	public float ExplosionForce = 1000f;

	public List<string> tagsForMultiplier;

	public float multiplier = 5f;

	public bool HitedActive = true;

	public float TimeActive;

	private float timetemp;

	private float lastActivateTime;

	private HashSet<Transform> checkedCollisionTransforms = new HashSet<Transform>();

	private void Start()
	{
		if (!Owner || !Owner.GetComponent<Collider>())
		{
			return;
		}
		Physics.IgnoreCollision(GetComponent<Collider>(), Owner.GetComponent<Collider>());
		if ((bool)Owner.transform.root)
		{
			Collider[] componentsInChildren = Owner.transform.root.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Physics.IgnoreCollision(GetComponent<Collider>(), collider);
			}
		}
	}

	private void OnEnable()
	{
		timetemp = Time.time;
		checkedCollisionTransforms.Clear();
	}

	private void Update()
	{
		if ((!HitedActive || TimeActive > 0f) && Time.time >= timetemp + TimeActive)
		{
			Active(Effect);
		}
		if (cyclic && Time.time - lastActivateTime >= lastActivateTime + (float)(1 / CountInSec))
		{
			Active(Effect);
		}
	}

	private void FixedUpdate()
	{
		checkedCollisionTransforms.Clear();
	}

	public void Active(GameObject impactPrefab)
	{
		if (impactPrefab != null)
		{
			if (smartPooler == null)
			{
				smartPooler = ServiceLocator.Get<SmartPooler>();
			}
			smartPooler.Pop(impactPrefab, base.transform.position, impactPrefab.transform.rotation);
		}
		lastActivateTime = Time.time;
		if (Explosive)
		{
			ExplosionDamage();
		}
		if (!cyclic)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Deactivate()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void ExplosionDamage()
	{
		HashSet<Transform> usedTransforms = new HashSet<Transform>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, ExplosionRadius);
		foreach (Collider collider in array)
		{
			if (!collider || !DoDamageCheck(collider.gameObject))
			{
				continue;
			}
			WhoIsResult whoIsResult = WhoIs.Resolve(collider, WhoIs.Masks.BulletImpactResolving, usedTransforms);
			Health health = whoIsResult.IsEmpty ? null : whoIsResult.transform.GetComponent<Health>();
			if (!(health != null))
			{
				continue;
			}
			health.ApplyDamage(Damage, 2, Owner);
			if (health.GetComponent<RagdollHelper>() != null)
			{
				health.GetComponent<RagdollHelper>().Explosion(base.transform.position);
			}
			if ((bool)health.GetComponent<Rigidbody>())
			{
				float explosionForce = ExplosionForce;
				if (tagsForMultiplier != null && tagsForMultiplier.Count > 0 && tagsForMultiplier.Contains(health.gameObject.tag))
				{
					explosionForce = ExplosionForce * multiplier;
				}
				health.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, base.transform.position, ExplosionRadius, 3f);
			}
		}
	}

	private void NormalDamage(GameObject hitObject)
	{
		Health component = hitObject.GetComponent<Health>();
		if (component != null)
		{
			component.ApplyDamage(Damage, 2, Owner);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!HitedActive)
		{
			return;
		}
		WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Masks.AllStreetVehicles, checkedCollisionTransforms);
		if (whoIsResult.IsEmpty || !DoDamageCheck(whoIsResult.gameObject))
		{
			return;
		}
		GameObject impactPrefab = Effect;
		if (!Explosive)
		{
			if (impactScheme != null)
			{
				impactPrefab = impactScheme.metalImpactPrefab;
			}
			NormalDamage(whoIsResult.gameObject);
		}
		Active(impactPrefab);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (HitedActive && DoDamageCheck(collision.gameObject) && !collision.gameObject.CompareTag(base.gameObject.tag) && !WhoIs.ResolveCollision(collision, WhoIs.Masks.AllStreetVehicles, checkedCollisionTransforms).skipped)
		{
			GameObject impactPrefab = Effect;
			if (!Explosive)
			{
				impactPrefab = ResolveImpactPrefab(collision, Effect, impactScheme);
				NormalDamage(collision.gameObject);
			}
			Active(impactPrefab);
		}
	}
}
