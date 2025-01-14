using App.Util;
using UnityEngine;

namespace App.AI.Monitor
{
	public struct DeathTarget
	{
		public bool HasHealth
		{
			get;
			private set;
		}

		public Health Health
		{
			get;
			private set;
		}

		public GameObject GameObject
		{
			get;
			private set;
		}

		public Transform TargetTransform
		{
			get;
			private set;
		}

		public DeathTarget(Health health)
		{
			HasHealth = true;
			Health = health;
			GameObject = health.gameObject;
			TargetTransform = health.GetTargetBone();
		}

		public DeathTarget(GameObject gameObject)
		{
			HasHealth = false;
			Health = null;
			GameObject = gameObject;
			TargetTransform = gameObject.transform;
		}

		public void Clear()
		{
			HasHealth = false;
			Health = null;
			GameObject = null;
			TargetTransform = null;
		}

		public bool IsValid()
		{
			if (GameObject != null && GameObject.activeInHierarchy)
			{
				if (HasHealth)
				{
					if (GameObject.transform.parent == null)
					{
						return Health.Dead();
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}
}
