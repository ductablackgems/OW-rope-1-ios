using App.Player;
using App.Util;
using App.Vehicles.Tank;
using App.Vehicles.Tanks;
using App.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public sealed class TankAIController : AIController, IAIEntity, ITargetManager
	{
		[SerializeField]
		private Transform raycastPosition;

		private const float DEFAULT_WEAPON_RANGE = 20f;

		private const float PLAYER_VERTICAL_OFFSET = 1f;

		private TankTowerControl tankTower;

		private TankController tankController;

		private TankManager tankManager;

		private WeaponController[] weaponControllers;

		private NavMeshAgent navmeshAgent;

		private TankAIStateMachine aiStateMachine;

		private PlayerModel player;

		private Health health;

		private float weaponRange;

		private float velocity;

		private float rotation;

		private GameObject cachedTarget;

		GameObject IAIEntity.Owner => base.gameObject;

		ITargetManager IAIEntity.TargetManager => this;

		NavMeshAgent IAIEntity.NavMeshAgent => navmeshAgent;

		float IAIEntity.Health
		{
			get
			{
				if (!(health != null))
				{
					return 0f;
				}
				return health.GetCurrentHealth();
			}
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			tankTower = GetComponentInChildren<TankTowerControl>();
			tankController = GetComponent<TankController>();
			tankManager = GetComponent<TankManager>();
			navmeshAgent = GetComponent<NavMeshAgent>();
			weaponControllers = GetComponentsInChildren<WeaponController>();
			health = GetComponent<Health>();
			player = ServiceLocator.GetPlayerModel();
			navmeshAgent.speed = tankController.maxSpeed / 3.6f;
			tankTower.SetState(TankTowerState.Target);
			aiStateMachine = new TankAIStateMachine(this);
			aiStateMachine.Initialize();
			aiStateMachine.IsRunning = true;
			weaponRange = GetWeaponRange();
			navmeshAgent.updateRotation = false;
			tankManager.Active = true;
		}

		protected override void OnSpawn(Vector3 position)
		{
			base.OnSpawn(position);
			base.transform.position = position;
			if (navmeshAgent.enabled)
			{
				navmeshAgent.Warp(position);
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (aiStateMachine != null)
			{
				aiStateMachine.Update();
				cachedTarget = null;
				tankController.MoveAI(velocity, rotation);
				velocity = 0f;
				rotation = 0f;
			}
		}

		void IAIEntity.Attack(GameObject target)
		{
			Vector3 to = target.transform.position - base.transform.position;
			if (!(Mathf.Abs(Vector3.SignedAngle(tankTower.transform.forward, to, Vector3.up)) > 10f))
			{
				for (int i = 0; i < weaponControllers.Length; i++)
				{
					WeaponController controller = weaponControllers[i];
					WeaponAttack(controller, target);
				}
			}
		}

		void IAIEntity.Move(Vector3 dir)
		{
			rotation = 0f;
			velocity = 0f;
			GameObject visibleTargetInRange = GetVisibleTargetInRange(weaponRange);
			if (visibleTargetInRange != null)
			{
				tankTower.RotateToTarget(visibleTargetInRange.transform.position);
			}
			else
			{
				tankTower.SetState(TankTowerState.Home);
			}
			if (dir != Vector3.zero)
			{
				Quaternion b = Quaternion.LookRotation(dir, Vector3.up);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, Time.deltaTime * 0.7f);
				rotation = b.eulerAngles.x;
				velocity = navmeshAgent.velocity.normalized.magnitude;
			}
		}

		void IAIEntity.RotateTo(Vector3 position)
		{
			tankTower.RotateToTarget(position);
		}

		float IAIEntity.GetWeaponRange()
		{
			return weaponRange;
		}

		GameObject ITargetManager.GetTarget()
		{
			return GetTarget();
		}

		GameObject ITargetManager.GetVisibleTargetInRange()
		{
			return GetVisibleTargetInRange(weaponRange);
		}

		private GameObject GetTarget()
		{
			if (player.GameObject == null)
			{
				return null;
			}
			if (!(player.Health.GetCurrentHealth() > 0f))
			{
				return null;
			}
			return player.GameObject;
		}

		private void WeaponAttack(WeaponController controller, GameObject target)
		{
			IWeapon currentWeapon = controller.GetCurrentWeapon();
			float num = Vector3.Distance(target.transform.position, base.transform.position);
			if (!(currentWeapon.Range > 0f) || !(num > currentWeapon.Range))
			{
				Transform transform = controller.transform;
				Vector3 position = target.transform.position;
				position = ((currentWeapon.GetGunType() == GunType.Rpg) ? position : (position + Vector3.up * 1f));
				transform.LookAt(position);
				transform.localRotation = Quaternion.RotateTowards(Quaternion.identity, transform.localRotation, 30f);
				controller.LaunchWeapon();
			}
		}

		private GameObject GetVisibleTargetInRange(float range)
		{
			if (cachedTarget != null)
			{
				return cachedTarget;
			}
			GameObject target = GetTarget();
			if (target == null)
			{
				return null;
			}
			if (Vector3.Distance(target.transform.position, base.transform.position) > range)
			{
				return null;
			}
			bool vehicleHit = false;
			if (!AIUtils.CanSeePlayer(raycastPosition.position, player, range, 1f, out vehicleHit))
			{
				return null;
			}
			return player.GameObject;
		}

		private float GetWeaponRange()
		{
			if (weaponControllers.Length == 0)
			{
				return 20f;
			}
			float num = 0f;
			for (int i = 0; i < weaponControllers.Length; i++)
			{
				num += (weaponControllers[i].GetCurrentWeapon()?.Range ?? 0f);
			}
			return (num > 0f) ? (num / (float)weaponControllers.Length) : 20f;
		}
	}
}
