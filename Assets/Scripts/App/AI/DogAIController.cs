using App.Dogs;
using App.Spawn.Pooling;
using App.Util;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class DogAIController : AIController, IAIEntity
	{
		[SerializeField]
		private float attackTime = 0.3f;

		[SerializeField]
		private Poolable impactEffect;

		private NavMeshAgent navMeshAgent;

		private AIStateMachine stateMachine;

		private Animator animator;

		private DogTargetManager targetManager;

		private DurationTimer attackTimer = new DurationTimer();

		private SmartPooler smartPooler;

		private bool isAttack;

		private int speedParamID = Animator.StringToHash("Speed");

		private int deadParamID = Animator.StringToHash("IsDead");

		private int attackParamID = Animator.StringToHash("Attack");

		public Dog Dog
		{
			get;
			private set;
		}

		GameObject IAIEntity.Owner => base.gameObject;

		ITargetManager IAIEntity.TargetManager => targetManager;

		NavMeshAgent IAIEntity.NavMeshAgent => navMeshAgent;

		float IAIEntity.Health => Dog.Health.GetCurrentHealth();

		public void AttackTargetsInDirection()
		{
			targetManager.SetTargetInDirection();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			Dog = GetComponent<Dog>();
			navMeshAgent = GetComponent<NavMeshAgent>();
			animator = GetComponentInChildren<Animator>();
			targetManager = new DogTargetManager(Dog);
			stateMachine = new DogAIStateMachine(this);
			smartPooler = ServiceLocator.Get<SmartPooler>();
			stateMachine.Initialize();
			navMeshAgent.speed = Dog.Settings.WalkSpeed;
		}

		protected override void OnSpawn(Vector3 position)
		{
			base.OnSpawn(position);
			Spawn_Internal(position);
		}

		protected override void OnDespawn()
		{
			base.OnDespawn();
			targetManager.Clean();
			stateMachine.IsRunning = false;
		}

		protected override void OnRespawn()
		{
			base.OnRespawn();
			targetManager.Clean();
			Spawn_Internal(Dog.Spot.Position);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (stateMachine.IsRunning)
			{
				UpdateStateMachine();
				UpdateAnimState();
				UpdateAttack();
				targetManager.Update();
				animator.SetBool(attackParamID, isAttack);
				animator.SetBool(deadParamID, !Dog.IsAlive);
				isAttack = false;
			}
		}

		void IAIEntity.Move(Vector3 dir)
		{
		}

		void IAIEntity.Attack(GameObject target)
		{
			if (!attackTimer.Running())
			{
				attackTimer.Run(attackTime);
				isAttack = true;
			}
		}

		float IAIEntity.GetWeaponRange()
		{
			return Dog.Settings.AttackRange;
		}

		void IAIEntity.RotateTo(Vector3 position)
		{
			if (!(position == Vector3.zero))
			{
				Vector3 forward = position - base.transform.position;
				Quaternion a = Quaternion.LookRotation(base.transform.forward, Vector3.up);
				Quaternion b = Quaternion.LookRotation(forward, Vector3.up);
				base.transform.rotation = Quaternion.Lerp(a, b, Time.deltaTime * 30f);
			}
		}

		private void UpdateStateMachine()
		{
			if (stateMachine != null)
			{
				stateMachine.Update();
			}
		}

		private void UpdateAnimState()
		{
			if (!(animator == null))
			{
				float value = navMeshAgent.velocity.magnitude / Dog.Settings.RunSpeed;
				animator.SetFloat(speedParamID, value);
			}
		}

		private void UpdateAttack()
		{
			if (!attackTimer.Done())
			{
				return;
			}
			attackTimer.Stop();
			Health targetHealth = targetManager.TargetHealth;
			if (!(targetHealth == null))
			{
				float damage = Dog.Damage;
				float damage2 = Dog.Damage;
				targetHealth.ApplyDamage(damage, 11, base.gameObject);
				targetManager.StopActiveDriver(targetHealth.Dead());
				if (!(impactEffect == null))
				{
					Vector3 position = targetHealth.transform.position + Vector3.up;
					smartPooler.Pop(impactEffect.gameObject, position, impactEffect.transform.rotation);
				}
			}
		}

		private void Spawn_Internal(Vector3 position)
		{
			navMeshAgent.enabled = true;
			navMeshAgent.Warp(position);
			stateMachine.IsRunning = true;
		}
	}
}
