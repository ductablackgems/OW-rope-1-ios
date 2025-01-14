using System;
using UnityEngine;

namespace App.Player.Climbing
{
	[Serializable]
	public class ClimbRaycast
	{
		public Transform transform;

		[NonSerialized]
		public LayerMask mask;

		[NonSerialized]
		public float distance;

		[NonSerialized]
		public float delay;

		private float lastCheckTime = -1f;

		private bool hit;

		private RaycastHit raycastHit;

		private float hitDistance = 999f;

		public bool Hit(bool keepOldValue = false)
		{
			if (!keepOldValue && lastCheckTime + delay < Time.fixedTime)
			{
				UpdateHit();
			}
			return hit;
		}

		public bool HitClimbable(bool keepOldValue = false)
		{
			if (lastCheckTime + delay < Time.fixedTime)
			{
				UpdateHit();
			}
			if (hit && raycastHit.transform != null)
			{
				return raycastHit.transform.gameObject.layer == 13;
			}
			return false;
		}

		public RaycastHit GetRaycastHit()
		{
			if (lastCheckTime + delay < Time.fixedTime)
			{
				UpdateHit();
			}
			return raycastHit;
		}

		public float GetHitDistance(bool keepOldValue = false)
		{
			if (!keepOldValue && lastCheckTime + delay < Time.fixedTime)
			{
				UpdateHit();
			}
			return hitDistance;
		}

		private void UpdateHit()
		{
			if (!(transform == null))
			{
				lastCheckTime = Time.fixedTime;
				hit = Physics.Raycast(new Ray(transform.position, transform.forward), out raycastHit, distance, mask);
				if (hit)
				{
					hitDistance = (transform.position - raycastHit.point).magnitude;
				}
				else
				{
					hitDistance = 999f;
				}
			}
		}
	}
}
