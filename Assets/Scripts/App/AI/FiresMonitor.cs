using App.Util;
using System.Collections.Generic;
using UnityEngine;

namespace App.AI
{
	public class FiresMonitor : MonoBehaviour
	{
		private DurationTimer checkTimer = new DurationTimer();

		private List<FireManager> fires = new List<FireManager>();

		public FireManager FindFireWithinDistance(Vector3 origin, float maxDistance)
		{
			foreach (FireManager fire in fires)
			{
				if (fire != null && (origin - fire.transform.position).magnitude <= maxDistance)
				{
					return fire;
				}
			}
			return null;
		}

		private void Awake()
		{
			checkTimer.Run(4f);
		}

		private void Update()
		{
			if (!checkTimer.Done())
			{
				return;
			}
			checkTimer.Run(4f);
			fires.Clear();
			GameObject[] gameObjects = ServiceLocator.GetGameObjects("Vehicle");
			foreach (GameObject gameObject in gameObjects)
			{
				FireManager component = gameObject.GetComponent<FireManager>();
				if (!(component != null) || !component.IsInFire())
				{
					continue;
				}
				if (component.IsWreck())
				{
					fires.Add(component);
					continue;
				}
				DestroyGameObject component2 = gameObject.GetComponent<DestroyGameObject>();
				if (component2 != null && component2.enabled)
				{
					fires.Add(component);
				}
			}
		}
	}
}
