using UnityEngine;

namespace Forge3D
{
	public class F3DMissile : MonoBehaviour
	{
		public enum MissileType
		{
			Unguided,
			Guided,
			Predictive
		}

		public MissileType missileType;

		public Transform target;

		public LayerMask layerMask;

		public float detonationDistance;

		public float lifeTime = 5f;

		public float despawnDelay;

		public float velocity = 300f;

		public float alignSpeed = 1f;

		public float RaycastAdvance = 2f;

		public bool DelayDespawn;

		public ParticleSystem[] delayedParticles;

		private ParticleSystem[] particles;

		private new Transform transform;

		private bool isHit;

		private bool isFXSpawned;

		private float timer;

		private Vector3 targetLastPos;

		private Vector3 step;

		private MeshRenderer meshRenderer;

		private void Awake()
		{
			transform = GetComponent<Transform>();
			particles = GetComponentsInChildren<ParticleSystem>();
			meshRenderer = GetComponent<MeshRenderer>();
		}

		public void OnSpawned()
		{
			isHit = false;
			isFXSpawned = false;
			timer = 0f;
			targetLastPos = Vector3.zero;
			step = Vector3.zero;
			meshRenderer.enabled = true;
		}

		public void OnDespawned()
		{
		}

		private void Delay()
		{
			if (particles.Length == 0 || delayedParticles.Length == 0)
			{
				return;
			}
			for (int i = 0; i < particles.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < delayedParticles.Length; j++)
				{
					if (particles[i] == delayedParticles[j])
					{
						flag = true;
						break;
					}
				}
				particles[i].Stop(withChildren: false);
				if (!flag)
				{
					particles[i].Clear(withChildren: false);
				}
			}
		}

		private void OnMissileDestroy()
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(transform);
		}

		private void OnHit()
		{
			meshRenderer.enabled = false;
			isHit = true;
			if (DelayDespawn)
			{
				timer = 0f;
				Delay();
			}
		}

		private void Update()
		{
			if (isHit)
			{
				if (!isFXSpawned)
				{
					F3DMissileLauncher.instance.SpawnExplosion(transform.position);
					isFXSpawned = true;
				}
				if (!DelayDespawn || (DelayDespawn && timer >= despawnDelay))
				{
					OnMissileDestroy();
				}
			}
			else
			{
				if (target != null)
				{
					if (missileType == MissileType.Predictive)
					{
						Vector3 a = F3DPredictTrajectory.Predict(transform.position, target.position, targetLastPos, velocity);
						targetLastPos = target.position;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(a - transform.position), Time.deltaTime * alignSpeed);
					}
					else if (missileType == MissileType.Guided)
					{
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), Time.deltaTime * alignSpeed);
					}
				}
				step = transform.forward * Time.deltaTime * velocity;
				if (target != null && missileType != 0 && Vector3.SqrMagnitude(transform.position - target.position) <= detonationDistance)
				{
					OnHit();
				}
				else if (missileType == MissileType.Unguided && Physics.Raycast(transform.position, transform.forward, step.magnitude * RaycastAdvance, layerMask))
				{
					OnHit();
				}
				else if (timer >= lifeTime)
				{
					isFXSpawned = true;
					OnHit();
				}
				transform.position += step;
			}
			timer += Time.deltaTime;
		}
	}
}
