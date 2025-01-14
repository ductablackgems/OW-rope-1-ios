using UnityEngine;

namespace App.Player
{
	public struct RopeResult
	{
		public bool targetReached;

		public RaycastHit hit;

		public RopeResult(bool targetReached, RaycastHit hit)
		{
			this.targetReached = targetReached;
			this.hit = hit;
		}
	}
}
