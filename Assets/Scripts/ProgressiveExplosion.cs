using App;
using App.Player;
using App.Util;
using App.Vehicles;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveExplosion : MonoBehaviour
{
	public float Radius = 3f;

	public float Speed = 20f;

	public float Force = 200f;

	public float Damage = 500f;

	[Tooltip("Verticall offset of the origin of the explosion. This will influence the direction of the acting force on target.")]
	public float VerticalOffset = -1f;

	[Tooltip("Min value for linear scale calculated from distance between explosion origin and target position. Set 1 to disable scale.")]
	[Range(0f, 1f)]
	public float MinDistanceScaleRatio = 0.25f;

	[Header("Mass Modifiers")]
	public float HumanMassMod = 0.5f;

	public float VehicleMassMod = 1.5f;

	public float BreakableObjMassMod = 0.5f;

	private float currentRadius;

	private float maxRadius;

	private bool isRunning;

	private Vector3 origin;

	private List<Health> targets = new List<Health>(128);

	private Collider[] rayCastResults = new Collider[256];

	private HashSet<Transform> usedTransforms = new HashSet<Transform>();

	public GameObject Owner
	{
		get;
		private set;
	}

	public void Explode(Vector3 position, GameObject owner)
	{
		origin = position;
		isRunning = true;
		currentRadius = 0f;
		Owner = owner;
		maxRadius = Radius;
		FindTargets();
	}

	private void Update()
	{
		UpdateTargets(Time.deltaTime);
	}

	private void FindTargets()
	{
		targets.Clear();
		usedTransforms.Clear();
		int num = Physics.OverlapSphereNonAlloc(origin, Radius, rayCastResults);
		for (int i = 0; i < num; i++)
		{
			Collider collider = rayCastResults[i];
			if (collider.gameObject == Owner)
			{
				continue;
			}
			WhoIsResult whoIsResult = WhoIs.Resolve(collider, WhoIs.Masks.BulletImpactResolving, usedTransforms);
			if (whoIsResult.IsEmpty)
			{
				continue;
			}
			GameObject gameObject = whoIsResult.gameObject;
			Health component = gameObject.GetComponent<Health>();
			if (!(component == null) && !targets.Contains(component))
			{
				float num2 = Vector3.Distance(gameObject.transform.position, origin);
				if (num2 > maxRadius)
				{
					maxRadius = num2;
				}
				targets.Add(component);
			}
		}
	}

	private void UpdateTargets(float deltaTime)
	{
		if (!isRunning)
		{
			return;
		}
		if (currentRadius > maxRadius)
		{
			Deactivate();
			return;
		}
		currentRadius += Speed * deltaTime;
		int num = targets.Count;
		while (num-- > 0)
		{
			Health health = targets[num];
			if (!(Vector3.Distance(origin, health.transform.position) > currentRadius))
			{
				ProcessDamage(health);
				targets.RemoveAt(num);
			}
		}
	}

	private void ProcessDamage(Health target)
	{
		Vector3 vector = target.transform.position - (origin + Vector3.up * VerticalOffset);
		float num = Mathf.Max(1f - vector.magnitude / maxRadius, MinDistanceScaleRatio);
		float num2 = Force * num;
		Vector3 force = vector.normalized * num2;
		float damage = Damage * num;
		target.ApplyDamage(damage, 2, Owner);
		if (!AddExplosionToHuman(target.gameObject, force) && !AddExplosionToVehicle(target.gameObject, num2))
		{
			AddExplosionToBreakable(target.gameObject, force);
		}
	}

	private void Deactivate()
	{
		isRunning = false;
		targets.Clear();
	}

	private bool AddExplosionToHuman(GameObject target, Vector3 force)
	{
		RagdollHelper component = target.GetComponent<RagdollHelper>();
		if (component == null)
		{
			return false;
		}
		Transform pelvis = component.pelvis;
		if (pelvis == null)
		{
			return false;
		}
		component.Ragdolled = true;
		Rigidbody component2 = pelvis.GetComponent<Rigidbody>();
		component2.AddForce(force * component2.mass * HumanMassMod, ForceMode.Impulse);
		return true;
	}

	private bool AddExplosionToVehicle(GameObject target, float force)
	{
		if (target.GetComponent<VehicleComponents>() == null)
		{
			return false;
		}
		Rigidbody component = target.GetComponent<Rigidbody>();
		if (component == null)
		{
			return false;
		}
		float explosionForce = force * component.mass * VehicleMassMod;
		component.AddExplosionForce(explosionForce, origin, maxRadius, 2f);
		return true;
	}

	private bool AddExplosionToBreakable(GameObject target, Vector3 force)
	{
		BreakableItemInstantiate component = target.GetComponent<BreakableItemInstantiate>();
		if (component == null)
		{
			return false;
		}
		component.DestroyByForce(force, BreakableObjMassMod);
		return true;
	}

	private void OnDrawGizmos()
	{
		if (isRunning)
		{
			Gizmos.DrawWireSphere(origin, currentRadius);
		}
	}
}
