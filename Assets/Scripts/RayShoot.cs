using App;
using App.Player;
using App.Spawn.Pooling;
using App.Util;
using App.Weapons;
using UnityEngine;

public class RayShoot : DamageBase
{
	public int Range = 10000;

	public Vector3 AimPoint;

	public GameObject Explosion;

	public ProjectileImpactScheme impactScheme;

	public float LifeTime = 1f;

	public LineRenderer Trail;

	private Poolable poolable;

	private bool firstUpdate;

	public void ForceUpdate()
	{
		Update();
	}

	private void OnEnable()
	{
		firstUpdate = true;
	}

	private void OnDisable()
	{
		firstUpdate = true;
	}

	private void Start()
	{
		Update();
	}

	private void Update()
	{
		if (!firstUpdate)
		{
			return;
		}
		firstUpdate = false;
		GameObject gameObject = null;
		int layerMask = 1063937;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out RaycastHit hitInfo, Range, layerMask))
		{
			AimPoint = hitInfo.point;
			WhoIsResult whoIsResult = WhoIs.Resolve(hitInfo.collider, WhoIs.Masks.BulletImpactResolving);
			GameObject gameObject2 = ResolveImpactPrefab(hitInfo.collider, Explosion, impactScheme, whoIsResult);
			if (gameObject2 != null)
			{
				gameObject = smartPooler.Pop(gameObject2, out Poolable poolable, AimPoint, Quaternion.LookRotation(hitInfo.normal));
				if (poolable.IsNew)
				{
					DamageBase component = gameObject.GetComponent<DamageBase>();
					if (component != null)
					{
						component.smartPooler = smartPooler;
					}
				}
			}
			if (!whoIsResult.IsEmpty)
			{
				Health component2 = whoIsResult.transform.GetComponent<Health>();
				if (component2 != null)
				{
					RagdollHelper component3 = hitInfo.collider.GetComponent<RagdollHelper>();
					if (component3 != null)
					{
						if (base.gameObject.name == "ShotgunBullet")
						{
							component3.AddForceBody(Damage * 10);
						}
						else
						{
							component3.AddForceBody(Damage);
						}
					}
					component2.ApplyDamage(Damage, 2, Owner);
				}
			}
		}
		else
		{
			AimPoint = base.transform.forward * Range;
			if (Explosion != null)
			{
				gameObject = smartPooler.Pop(Explosion);
				gameObject.transform.position = AimPoint;
				gameObject.transform.rotation = base.transform.rotation;
			}
		}
		if ((bool)gameObject)
		{
			DamageBase component4 = gameObject.GetComponent<DamageBase>();
			if ((bool)component4)
			{
				component4.smartPooler = smartPooler;
				component4.Owner = Owner;
				component4.TargetTag = TargetTag;
				component4.IgnoreTag = IgnoreTag;
			}
		}
		if ((bool)Trail)
		{
			Trail.SetPosition(0, base.transform.position);
			Trail.SetPosition(1, AimPoint);
		}
		if (this.poolable == null)
		{
			this.poolable = GetComponent<Poolable>();
			if (this.poolable == null)
			{
				UnityEngine.Object.Destroy(base.gameObject, LifeTime);
			}
			else
			{
				this.poolable.UpdatePushTime(LifeTime);
			}
		}
	}
}
