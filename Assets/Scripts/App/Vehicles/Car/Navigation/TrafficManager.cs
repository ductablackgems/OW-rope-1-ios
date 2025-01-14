using App.Settings;
using App.Vehicles.Car.Navigation.Roads;
using System;
using System.Linq;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class TrafficManager : MonoBehaviour
	{
		private const int MaxSegmentCount = 30;

		public GameObject trafficTesterPrefab;

		public Vector3 spawnTestHalfSize = new Vector3(3f, 1.8f, 10f);

		public int maxFindPositionCount = 10;

		public float findPositionInterval = 0.1f;

		private Transform inactiveParent;

		private Collider[] colliders = new Collider[15];

		private System.Random random = new System.Random();

		private TrafficTester trafficTester;

		private VehicleSpawnScheme scheme;

		private Transform player;

		private DurationTimer findPositionTimer = new DurationTimer();

		private int findPositionCounter;

		private RoadSegment[] segments = new RoadSegment[30];

		public event TrafficTester.PositionFoundEventhandler OnPositionFound;

		public bool FindSpawnPosition(int minRouteCount, bool firstCall = true)
		{
			findPositionTimer.Stop();
			int num = Physics.OverlapSphereNonAlloc(player.position, scheme.maxDistance, colliders, 256);
			int num2 = num;
			while (num2 > 1)
			{
				num2--;
				int num3 = random.Next(num2 + 1);
				Collider collider = colliders[num3];
				colliders[num3] = colliders[num2];
				colliders[num2] = collider;
			}
			num2 = ((num > 30) ? 30 : num);
			int num4 = 0;
			while (num2 > 0)
			{
				num2--;
				RoadSegment componentSafe = colliders[num2].GetComponentSafe<RoadSegment>();
				segments[num2] = componentSafe;
				num4 += componentSafe.road.routes.Length;
			}
			if (num4 < minRouteCount)
			{
				return false;
			}
			num2 = ((num > 30) ? 30 : num);
			bool flag = false;
			while (num2 > 0)
			{
				num2--;
				RoadSegment roadSegment = segments[num2];
				Vector3 endPosition = roadSegment.GetEndPosition();
				Vector3[] array = MathHelper.LineCircleIntersection(player.position, scheme.maxDistance, roadSegment.transform.position, endPosition);
				if (array == null || array.Count() < 2 || !MathHelper.GetParallelIntersection(roadSegment.transform.position, endPosition, array[0], array[1], out Vector3 begin, out Vector3 end))
				{
					continue;
				}
				TrafficRoute route = roadSegment.road.routes[UnityEngine.Random.Range(0, roadSegment.road.routes.Length)];
				if (trafficTester.Prepare(roadSegment, route, begin, end, player.position))
				{
					flag = true;
					if (firstCall)
					{
						findPositionCounter = maxFindPositionCount;
					}
					break;
				}
			}
			if (!flag)
			{
				findPositionCounter = 0;
			}
			return true;
		}

		public bool LookingForSpawnPosition()
		{
			return findPositionCounter > 0;
		}

		private void Awake()
		{
			trafficTester = UnityEngine.Object.Instantiate(trafficTesterPrefab).GetComponentSafe<TrafficTester>();
			scheme = ServiceLocator.Get<QualitySchemeManager>().GetScheme().vehicleSpawn;
			player = ServiceLocator.GetGameObject("Player").transform;
			inactiveParent = new GameObject("~GameManagerHelper").transform;
			inactiveParent.gameObject.SetActive(value: false);
			trafficTester.OnPositionFound += _OnPositionFound;
			trafficTester.OnFailed += OnFailPositionFind;
		}

		private void OnDestroy()
		{
			trafficTester.OnPositionFound -= _OnPositionFound;
			trafficTester.OnFailed -= OnFailPositionFind;
		}

		private void FixedUpdate()
		{
			if (findPositionTimer.Done())
			{
				findPositionTimer.Stop();
				FindSpawnPosition(0, firstCall: false);
			}
		}

		private void OnDrawGizmos()
		{
			if (!(player == null))
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(player.position, scheme.maxDistance);
			}
		}

		private void _OnPositionFound(Vector3 position, Quaternion rotation, RoadSegment roadSegment, TrafficRoute route)
		{
			findPositionCounter = 0;
			if (this.OnPositionFound != null)
			{
				this.OnPositionFound(position, rotation, roadSegment, route);
			}
		}

		private void OnFailPositionFind()
		{
			findPositionCounter--;
			if (findPositionCounter > 0)
			{
				findPositionTimer.Run(findPositionInterval);
			}
		}
	}
}
