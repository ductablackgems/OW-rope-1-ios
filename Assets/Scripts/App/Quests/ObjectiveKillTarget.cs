using App.AI;
using App.Spawn;
using App.Spawn.Pooling;
using App.Util;
using UnityEngine;
using UnityEngine.AI;

namespace App.Quests
{
	public class ObjectiveKillTarget : GameplayObjective
	{
		[Header("Objective Kill Target")]
		[SerializeField]
		private EnemyType enemyType;

		[SerializeField]
		private bool isLaserImmunity;

		[SerializeField]
		private bool isWalking;

		private Health target;

		private DestroyGameObject targetDestroyer;

		private HumanSpawner spawner;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			spawner = ServiceLocator.Get<HumanSpawner>();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			SpawnTarget();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (!(target == null) && !(target.GetCurrentHealth() > 0f))
			{
				Finish();
			}
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			CleanTarget();
		}

		protected override Vector3 GetNavigationPosition()
		{
			if (!(target == null))
			{
				return target.transform.position;
			}
			return base.Position;
		}

		private void SpawnTarget()
		{
			Resetor resetor = spawner.Spawn(enemyType, GetNavMeshPosition(), isWalking, force: true);
			target = resetor.GetComponent<Health>();
			target.IsLaserImmunity = isLaserImmunity;
			targetDestroyer = resetor.GetComponent<DestroyGameObject>();
			EnableDestroyer(enable: false);
		}

		private Vector3 GetNavMeshPosition()
		{
			NavMeshHit hit = default(NavMeshHit);
			if (!NavMesh.SamplePosition(base.Position, out hit, 5f, -1))
			{
				UnityEngine.Debug.LogError("Unable to find NavMesh Position");
				return Vector3.zero;
			}
			return hit.position;
		}

		private void EnableDestroyer(bool enable)
		{
			if (!(targetDestroyer == null))
			{
				targetDestroyer.enabled = enable;
			}
		}

		private void CleanTarget()
		{
			if (!(target == null))
			{
				target.IsLaserImmunity = false;
				EnableDestroyer(enable: true);
				if (!isWalking)
				{
					target.GetComponentInChildrenSafe<FreeWalkAIModule>().enabled = true;
				}
				target = null;
			}
		}
	}
}
