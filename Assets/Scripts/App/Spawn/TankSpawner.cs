using UnityEngine;

namespace App.Spawn
{
	public class TankSpawner : MonoBehaviour
	{
		public const int MaxTankCount = 2;

		public TankSpawnPoint[] points;

		public GameObject[] prefabs;

		private GameObject prefab;

		private DurationTimer checkTimer = new DurationTimer();

		public event TankSpawnPoint.TankSpawnPointEventHandler OnActivatePoint;

		public event TankSpawnPoint.TankSpawnPointEventHandler OnDeactivatePoint;

		public GameObject SpawnVehicle(Vector3 position, GameObject prefab)
		{
			if (prefab == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, null);
			gameObject.SetActive(value: true);
			gameObject.transform.position = position;
			return gameObject;
		}

		private void Awake()
		{
			TankSpawnPoint[] array = points;
			foreach (TankSpawnPoint obj in array)
			{
				obj.OnActivate += _OnActivatePoint;
				obj.OnDeactivate += _OnDeactivatePoint;
			}
		}

		private void OnDestroy()
		{
			TankSpawnPoint[] array = points;
			foreach (TankSpawnPoint obj in array)
			{
				obj.OnActivate -= _OnActivatePoint;
				obj.OnDeactivate -= _OnDeactivatePoint;
			}
		}

		private void Start()
		{
			checkTimer.Run(1.9f);
		}

		private void Update()
		{
			if (!checkTimer.Done())
			{
				return;
			}
			checkTimer.Run(1.9f);
			int num = 0;
			TankSpawnPoint[] array = points;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Activated())
				{
					num++;
				}
			}
			if (num < 2)
			{
				ActivateRandomPosition();
			}
		}

		private void ActivateRandomPosition()
		{
			int num = UnityEngine.Random.Range(0, points.Length);
			prefab = prefabs[Random.Range(0, prefabs.Length)];
			TankSpawnPoint tankSpawnPoint = points[num];
			int num2 = 0;
			if (!tankSpawnPoint.Activated() && tankSpawnPoint.GetPlayerDistance() > 200f)
			{
				tankSpawnPoint.Activate(prefab);
				return;
			}
			while (true)
			{
				if (num2 < points.Length)
				{
					num++;
					if (num >= points.Length)
					{
						num = 0;
					}
					tankSpawnPoint = points[num];
					if (!tankSpawnPoint.Activated() && tankSpawnPoint.GetPlayerDistance() > 200f)
					{
						break;
					}
					num2++;
					continue;
				}
				return;
			}
			tankSpawnPoint.Activate(prefab);
		}

		private void _OnActivatePoint(TankSpawnPoint point)
		{
			if (this.OnActivatePoint != null)
			{
				this.OnActivatePoint(point);
			}
		}

		private void _OnDeactivatePoint(TankSpawnPoint point)
		{
			if (this.OnDeactivatePoint != null)
			{
				this.OnDeactivatePoint(point);
			}
		}
	}
}
