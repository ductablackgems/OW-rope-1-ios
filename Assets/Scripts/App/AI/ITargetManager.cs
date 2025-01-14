using UnityEngine;

namespace App.AI
{
	public interface ITargetManager
	{
		GameObject GetTarget();

		GameObject GetVisibleTargetInRange();
	}
}
