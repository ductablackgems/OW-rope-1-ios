using App.Spawn;
using UnityEngine;

namespace App.Vehicles.Tank
{
	public class DistantTankDestroyer : MonoBehaviour
	{
		public const float DestroyDistance = 300f;

		private Transform player;

		public TankSpawnPoint SpawnPoint
		{
			get;
			set;
		}

		private void Awake()
		{
			player = ServiceLocator.GetGameObject("Player").transform;
		}

		private void Update()
		{
			if ((player.position - base.transform.position).magnitude > 300f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				if (SpawnPoint != null)
				{
					SpawnPoint.SetLostDistant(base.transform.position);
				}
			}
		}
	}
}
