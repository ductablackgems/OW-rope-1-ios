using App.Vehicles.Tank;
using UnityEngine;

namespace App.Spawn
{
	public class TankSpawnPoint : MonoBehaviour
	{
		public delegate void TankSpawnPointEventHandler(TankSpawnPoint point);

		public const float SpawnDistance = 250f;

		private GameObject tank;

		private Transform player;

		private bool spawn;

		private GameObject prefab;

		private DurationTimer checkTimer = new DurationTimer();

		private bool lostDistant;

		private Vector3 lostDistantPosition;

		public event TankSpawnPointEventHandler OnActivate;

		public event TankSpawnPointEventHandler OnDeactivate;

		public void Activate(GameObject prefab)
		{
			tank = null;
			spawn = false;
			lostDistant = false;
			this.prefab = prefab;
			checkTimer.FakeDone(1f);
			base.gameObject.SetActive(value: true);
			if (this.OnActivate != null)
			{
				this.OnActivate(this);
			}
		}

		public bool Activated()
		{
			return base.gameObject.activeSelf;
		}

		public float GetPlayerDistance()
		{
			return (player.position - base.transform.position).magnitude;
		}

		public void SetLostDistant(Vector3 currentPosition)
		{
			lostDistant = true;
			lostDistantPosition = currentPosition;
		}

		private void Awake()
		{
			player = ServiceLocator.GetGameObject("Player").transform;
			base.gameObject.SetActive(value: false);
		}

		private void OnDisable()
		{
			if (this.OnDeactivate != null)
			{
				this.OnDeactivate(this);
			}
		}

		private void Update()
		{
			if (!checkTimer.Done())
			{
				return;
			}
			checkTimer.Run(1.7f);
			float magnitude = (player.position - base.transform.position).magnitude;
			if (spawn)
			{
				if (tank == null)
				{
					if (lostDistant && (base.transform.position - lostDistantPosition).magnitude < 15f)
					{
						spawn = false;
					}
					else
					{
						base.gameObject.SetActive(value: false);
					}
				}
			}
			else if (magnitude < 250f)
			{
				spawn = true;
				tank = UnityEngine.Object.Instantiate(prefab, base.transform.position, base.transform.rotation);
				tank.GetComponentSafe<DistantTankDestroyer>().SpawnPoint = this;
			}
			lostDistant = false;
		}
	}
}
