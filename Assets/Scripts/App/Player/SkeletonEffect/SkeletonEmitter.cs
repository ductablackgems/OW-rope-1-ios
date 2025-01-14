using App.Spawn;
using App.Spawn.Pooling;
using App.Util;
using UnityEngine;

namespace App.Player.SkeletonEffect
{
	public class SkeletonEmitter : MonoBehaviour, IResetable
	{
		public GameObject mappingSkeleton;

		public SkeletonMappingScriptableObject mapping;

		public Transform fireParent;

		private Animator animator;

		private Resetor resetor;

		private Health health;

		private HumanFireManager fireManager;

		private SmartPooler smartPooler;

		private EnemyPooler enemyPooler;

		private DurationTimer emitTimer = new DurationTimer();

		private Transform fire;

		public void ResetStates()
		{
			emitTimer.Stop();
			fire = null;
		}

		private void Awake()
		{
			animator = this.GetComponentSafe<Animator>();
			resetor = this.GetComponentSafe<Resetor>();
			health = this.GetComponentSafe<Health>();
			fireManager = this.GetComponentSafe<HumanFireManager>();
			smartPooler = ServiceLocator.Get<SmartPooler>();
			enemyPooler = ServiceLocator.Get<EnemyPooler>();
			health.OnDie += OnDie;
		}

		private void OnDestroy()
		{
			health.OnDie -= OnDie;
		}

		private void Update()
		{
			if (!emitTimer.Running())
			{
				if (!health.Dead())
				{
					base.enabled = false;
					return;
				}
				if (fireManager.IsBurning())
				{
					StartEmitProccess(fireManager.IsBurningByAcid());
				}
			}
			if (emitTimer.Done())
			{
				emitTimer.Stop();
				Emit();
			}
		}

		private void Emit()
		{
			emitTimer.Stop();
			GameObject prefab = (health.DeathDamageType == 5) ? mapping.burnedSkeletonPrefab : ((health.DeathDamageType == 7) ? mapping.greenSkeletonPrefab : ((health.DeathDamageType != 10) ? mapping.skeletonPrefab : mapping.electrifiedSkeletonPrefab));
			GameObject gameObject = smartPooler.Pop(prefab);
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = base.transform.rotation;
			Animator componentSafe = gameObject.GetComponentSafe<Animator>();
			for (int i = 0; i < mapping.items.Length; i++)
			{
				SkeletonMappingItem mappingItem = mapping.items[i];
				CopyBone(mappingItem, componentSafe);
			}
			if (fire != null)
			{
				fire.transform.parent = gameObject.GetComponentSafe<Skeleton>().fireParent;
				fire.transform.localPosition = Vector3.zero;
				fire.transform.localRotation = Quaternion.identity;
			}
			enemyPooler.Push(resetor);
		}

		private void CopyBone(SkeletonMappingItem mappingItem, Animator skeletonAnimator)
		{
			Transform boneTransform = animator.GetBoneTransform(mappingItem.bone);
			Transform boneTransform2 = skeletonAnimator.GetBoneTransform(mappingItem.bone);
			CopyTransform(boneTransform, boneTransform2, mappingItem.difference);
			Rigidbody component = boneTransform.GetComponent<Rigidbody>();
			if (component != null)
			{
				Rigidbody component2 = boneTransform2.GetComponent<Rigidbody>();
				if (component2 != null)
				{
					component2.velocity = component.velocity;
				}
			}
		}

		private void CopyTransform(Transform origin, Transform target, Quaternion difference)
		{
			target.position = origin.position;
			target.rotation = origin.rotation * difference;
		}

		private void OnDie()
		{
			base.enabled = true;
			if (health.DeathDamageType == 4 || health.DeathDamageType == 5 || health.DeathDamageType == 7 || health.DeathDamageType == 10)
			{
				StartEmitProccess(health.DeathDamageType);
			}
		}

		private void StartEmitProccess(bool isAcid)
		{
			int damageType = isAcid ? 7 : 5;
			StartEmitProccess(damageType);
		}

		private void StartEmitProccess(int damageType)
		{
			emitTimer.Run(1f);
			switch (damageType)
			{
			case 7:
				fire = smartPooler.Pop(mapping.greenSkeletonFirePrefab).transform;
				break;
			case 10:
				fire = smartPooler.Pop(mapping.skeletonFirePrefab).transform;
				break;
			case 5:
				fire = smartPooler.Pop(mapping.skeletonFirePrefab).transform;
				break;
			case 4:
				fire = smartPooler.Pop(mapping.skeletonFirePrefab).transform;
				break;
			default:
				fire = smartPooler.Pop(mapping.skeletonFirePrefab).transform;
				break;
			}
			if (!(fire == null))
			{
				fire.transform.parent = fireParent;
				fire.transform.localPosition = Vector3.zero;
				fire.transform.localRotation = Quaternion.identity;
			}
		}
	}
}
