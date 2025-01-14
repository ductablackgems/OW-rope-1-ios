using App;
using App.Spawn.Pooling;
using App.Weapons;
using UnityEngine;

public class DamageBase : MonoBehaviour
{
	public GameObject Effect;

	[HideInInspector]
	public GameObject Owner;

	public int Damage = 20;

	public bool cyclic;

	public int CountInSec;

	public string[] TargetTag = new string[1]
	{
		"Enemy"
	};

	public string[] IgnoreTag;

	public SmartPooler smartPooler;

	public bool emitImpactParticles = true;

	public bool DoDamageCheck(GameObject gob)
	{
		for (int i = 0; i < IgnoreTag.Length; i++)
		{
			if (gob.CompareTag(IgnoreTag[i]))
			{
				return false;
			}
		}
		return true;
	}

	protected GameObject ResolveImpactPrefab(Collider collider, GameObject explosionPrefab, ProjectileImpactScheme impactScheme, WhoIsResult whoIs)
	{
		if (impactScheme == null)
		{
			return explosionPrefab;
		}
		return ResolveImpactPrefabRaw(whoIs, explosionPrefab, impactScheme);
	}

	protected GameObject ResolveImpactPrefab(Collision collision, GameObject explosionPrefab, ProjectileImpactScheme impactScheme)
	{
		if (impactScheme == null)
		{
			return explosionPrefab;
		}
		WhoIsResult whoIs = WhoIs.ResolveCollision(collision, WhoIs.Masks.BulletImpactResolving);
		return ResolveImpactPrefabRaw(whoIs, explosionPrefab, impactScheme);
	}

	private GameObject ResolveImpactPrefabRaw(WhoIsResult whoIs, GameObject explosionPrefab, ProjectileImpactScheme impactScheme)
	{
		if (impactScheme != null)
		{
			if (whoIs.Compare(WhoIs.Masks.AllHumans) || whoIs.Compare(WhoIs.Entities.RagdollPart) || whoIs.Compare(WhoIs.Masks.AllAnimals))
			{
				return impactScheme.bloodImpactPrefab;
			}
			if (!emitImpactParticles)
			{
				return null;
			}
			if (whoIs.Compare(WhoIs.Masks.AllVehicles) || whoIs.Compare(WhoIs.Entities.Lamp) || whoIs.Compare(WhoIs.Entities.FallenVehiclePart))
			{
				return impactScheme.metalImpactPrefab;
			}
			return impactScheme.concreteImpactPrefab;
		}
		return explosionPrefab;
	}
}
