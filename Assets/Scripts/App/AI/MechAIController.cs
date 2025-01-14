using App.Player;
using App.Util;
using App.Vehicles.Mech;
using App.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class MechAIController : AIController, IAIEntity, ITargetManager
	{
		[SerializeField]
		private Transform raycastPosition;

		private float maxFixAngle = 30f;

		private const float PLAYER_VERTICAL_OFFSET = 1f;

		private MechCabinRotator cabineRotator;

		private MechController mechController;

		private WeaponController[] weaponControllers;

		private NavMeshAgent navmeshAgent;

		private MechAIStateMachine aiStateMachine;

		private PlayerModel player;

		private Health health;

		private float weaponRange;

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
			cabineRotator = GetComponentInChildren<MechCabinRotator>();
			mechController = GetComponent<MechController>();
			navmeshAgent = GetComponent<NavMeshAgent>();
			weaponControllers = GetComponentsInChildren<WeaponController>();
			health = GetComponent<Health>();
			player = ServiceLocator.GetPlayerModel();
			bool flag = GetComponent<MechJetpack>() != null;
			navmeshAgent.enabled = !flag;
			navmeshAgent.speed = mechController.MaxForwardSpeed;
			cabineRotator.Type = MechCabinRotator.ControlType.Target;
			aiStateMachine = new MechAIStateMachine(this, flag);
			aiStateMachine.Initialize();
			aiStateMachine.IsRunning = true;
			weaponRange = GetWeaponRange();
			mechController.IsActive = true;
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
			}
		}

		void IAIEntity.Attack(GameObject target)
		{
			for (int i = 0; i < weaponControllers.Length; i++)
			{
				WeaponController controller = weaponControllers[i];
				WeaponAttack(controller, target);
			}
		}

		void IAIEntity.Move(Vector3 dir)
		{
			float f = Vector3.SignedAngle(base.transform.forward.normalized, dir, Vector3.up);
			float x = 0f;
			if (Mathf.Abs(f) > 10f)
			{
				x = Mathf.Sign(f);
			}
			float magnitude = dir.magnitude;
			Vector2 move = new Vector2(x, dir.normalized.magnitude);
			GameObject visibleTargetInRange = GetVisibleTargetInRange(50f);
			Vector3 position = base.transform.position + base.transform.forward;
			if (visibleTargetInRange != null)
			{
				position = visibleTargetInRange.transform.position;
			}
			cabineRotator.RotateTo(position);
			mechController.Move(move);
		}

		void IAIEntity.RotateTo(Vector3 position)
		{
			Vector3 to = position - base.transform.position;
			float f = Vector3.SignedAngle(base.transform.forward, to, Vector3.up);
			cabineRotator.RotateTo(position);
			if (!(Mathf.Abs(f) < cabineRotator.maxAngle * 0.5f))
			{
				float x = Mathf.Sign(f);
				mechController.Move(new Vector2(x, 0f));
			}
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
				transform.LookAt(target.transform.position + Vector3.up * 1f);
				transform.localRotation = Quaternion.RotateTowards(Quaternion.identity, transform.localRotation, maxFixAngle);
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
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < weaponControllers.Length; i++)
			{
				num += (weaponControllers[i].GetCurrentWeapon()?.Range ?? 0f);
			}
			return num / (float)weaponControllers.Length;
		}
	}
}
