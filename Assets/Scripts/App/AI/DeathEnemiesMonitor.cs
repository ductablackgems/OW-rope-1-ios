using App.AI.Monitor;
using App.Util;
using System.Collections.Generic;
using UnityEngine;

namespace App.AI
{
	public class DeathEnemiesMonitor : MonoBehaviour
	{
		private DurationTimer checkTimer = new DurationTimer();

		private List<DeathTarget> deathTargets = new List<DeathTarget>();

		public DeathTarget FindHealthTarget(Vector3 origin, float maxDistance)
		{
			foreach (DeathTarget deathTarget in deathTargets)
			{
				if (!deathTarget.HasHealth)
				{
					break;
				}
				if (deathTarget.IsValid() && (origin - deathTarget.TargetTransform.position).magnitude <= maxDistance)
				{
					return deathTarget;
				}
			}
			return default(DeathTarget);
		}

		public DeathTarget FindAnyTarget(Vector3 origin, float maxDistance)
		{
			foreach (DeathTarget deathTarget in deathTargets)
			{
				if (deathTarget.IsValid() && (origin - deathTarget.TargetTransform.position).magnitude <= maxDistance)
				{
					return deathTarget;
				}
			}
			return default(DeathTarget);
		}

		private void Awake()
		{
			checkTimer.Run(5f);
		}

		private void Update()
		{
			if (!checkTimer.Done())
			{
				return;
			}
			checkTimer.Run(5f);
			deathTargets.Clear();
			GameObject[] gameObjects = ServiceLocator.GetGameObjects("Enemy");
			foreach (GameObject gameObject in gameObjects)
			{
				if (!(gameObject.transform.parent != null))
				{
					Health component = gameObject.GetComponent<Health>();
					if (component != null && component.Dead())
					{
						deathTargets.Add(new DeathTarget(component));
					}
				}
			}
			gameObjects = ServiceLocator.GetGameObjects("Skeleton");
			foreach (GameObject gameObject2 in gameObjects)
			{
				deathTargets.Add(new DeathTarget(gameObject2));
			}
		}
	}
}
