using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public interface IAIEntity
	{
		GameObject Owner
		{
			get;
		}

		ITargetManager TargetManager
		{
			get;
		}

		NavMeshAgent NavMeshAgent
		{
			get;
		}

		float Health
		{
			get;
		}

		void Move(Vector3 dir);

		void RotateTo(Vector3 position);

		void Attack(GameObject target);

		float GetWeaponRange();
	}
}
