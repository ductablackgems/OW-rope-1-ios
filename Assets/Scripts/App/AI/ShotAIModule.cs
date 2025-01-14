using App.AI.Scanner;
using App.Player;
using App.Player.FightSystem;
using App.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class ShotAIModule : AbstractAIScript
	{
		public const float ShootableDistance = 17f;

		public float shotInterval = 1f;

		public GunType gunType;

		private NavMeshAgent agent;

		private ShotController shotController;

		private NavmeshWalker walker;

		private AIScanner scanner;

		private GameObject player;

		private AdvancedFightController playerFightController;

		private DurationTimer shotTimer = new DurationTimer();

		private ITargetManager targetManager;

		protected void Awake()
		{
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			shotController = base.ComponentsRoot.GetComponentSafe<ShotController>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			scanner = this.GetComponentSafe<AIScanner>();
			targetManager = GetComponent<ITargetManager>();
			PlayerModel playerModel = ServiceLocator.GetPlayerModel();
			player = playerModel.GameObject;
			playerFightController = playerModel.FightController;
		}

		private void Start()
		{
			IWeapon[] allWeapons = shotController.GetAllWeapons();
			for (int i = 0; i < allWeapons.Length; i++)
			{
				allWeapons[i].SetAmmoReserve(int.MaxValue);
			}
		}

		protected void OnEnable()
		{
			GameObject gameObject = (targetManager != null) ? targetManager.GetTarget() : player;
			shotController.Run();
			walker.FollowTransform(this, gameObject.transform, NavmeshWalkerSpeed.Run);
			shotTimer.FakeDone(shotInterval);
		}

		protected void OnDisable()
		{
			shotController.Stop();
			walker.Stop();
		}

		protected void Update()
		{
			UpdateAttack();
		}

		private void UpdateAttack()
		{
			GameObject gameObject = (targetManager != null) ? targetManager.GetTarget() : player;
			if (gameObject == null)
			{
				return;
			}
			shotController.gunType = gunType;
			Transform transform = gameObject.transform;
			if ((base.ComponentsRoot.position - transform.position).magnitude > 17f || !scanner.Shootable)
			{
				if (walker.TargetTransform != transform)
				{
					walker.FollowTransform(this, transform, NavmeshWalkerSpeed.Run);
				}
				shotController.Control(attackPressed: false, attackDownPressed: false);
				return;
			}
			walker.Stop();
			Vector3 position = transform.position;
			position.y = base.transform.position.y;
			base.ComponentsRoot.LookAt(position);
			bool flag = !(gameObject == player) || !playerFightController.RunningCustomMovement();
			shotController.Control(flag, flag && shotTimer.Done());
			if (shotTimer.Done())
			{
				shotTimer.Run(shotInterval);
			}
		}
	}
}
