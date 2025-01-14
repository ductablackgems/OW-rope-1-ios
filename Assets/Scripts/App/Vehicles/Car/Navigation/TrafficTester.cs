using App.Settings;
using App.Vehicles.Car.Navigation.Roads;
using System;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class TrafficTester : MonoBehaviour
	{
		public delegate void PositionFoundEventhandler(Vector3 position, Quaternion rotation, RoadSegment roadSegment, TrafficRoute route);

		private VehicleSpawnScheme scheme;

		public Vector3 awayPosition = new Vector3(999f, 999f, 999f);

		public Vector3 halfSize = new Vector3(3f, 2f, 6f);

		private float prepareTime = -1f;

		private int preparedNodeCount;

		private RoadSegment preparedSegment;

		private TrafficRoute preparedRoute;

		private TrafficTesterNode[] nodes;

		public event PositionFoundEventhandler OnPositionFound;

		public event Action OnFailed;

		public bool Prepare(RoadSegment roadSegment, TrafficRoute route, Vector3 beginPosition, Vector3 endPosition, Vector3 playerPosition)
		{
			float num = Vector3.Distance(roadSegment.transform.position, beginPosition);
			float num2 = Vector3.Distance(roadSegment.transform.position, endPosition);
			bool flag = num < num2;
			if (route.polarity == TrafficRoutePolarity.Positive == flag)
			{
				Vector3 vector = beginPosition;
				beginPosition = endPosition;
				endPosition = vector;
			}
			base.transform.position = beginPosition + roadSegment.transform.right * route.offset;
			base.transform.LookAt(endPosition + roadSegment.transform.right * route.offset);
			preparedNodeCount = Mathf.CeilToInt(Vector3.Distance(beginPosition, endPosition) / halfSize.z / 2f);
			int num3 = 0;
			int num4 = 0;
			bool flag2 = false;
			TrafficTesterNode[] array = nodes;
			foreach (TrafficTesterNode trafficTesterNode in array)
			{
				trafficTesterNode.gameObject.SetActive(num3 < preparedNodeCount);
				trafficTesterNode.ResetStates();
				if (Vector3.Distance(trafficTesterNode.transform.position, playerPosition) < scheme.minDistance)
				{
					trafficTesterNode.FakeMovingVehicleHit();
				}
				if (trafficTesterNode.gameObject.activeSelf)
				{
					num4++;
					if (num4 == 2)
					{
						flag2 = true;
					}
				}
				else
				{
					num4 = 0;
				}
				num3++;
			}
			if (flag2)
			{
				prepareTime = Time.fixedTime;
				preparedSegment = roadSegment;
				preparedRoute = route;
			}
			else
			{
				GoAway();
			}
			return flag2;
		}

		private void Awake()
		{
			scheme = ServiceLocator.Get<QualitySchemeManager>().GetScheme().vehicleSpawn;
			base.transform.position = awayPosition;
			int num = Mathf.CeilToInt(scheme.maxDistance / halfSize.z);
			nodes = new TrafficTesterNode[num];
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = new GameObject("TrafficTesterNode");
				gameObject.layer = 2;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3(0f, halfSize.y, halfSize.z * (float)(i * 2 + 1));
				gameObject.transform.localScale = halfSize * 2f;
				gameObject.AddComponent<BoxCollider>().isTrigger = true;
				nodes[i] = gameObject.AddComponent<TrafficTesterNode>();
			}
		}

		private void FixedUpdate()
		{
			if (prepareTime < 0f || prepareTime + Time.fixedDeltaTime * 2f >= Time.fixedTime)
			{
				return;
			}
			prepareTime = -1f;
			for (int num = preparedNodeCount - 2; num >= 0; num--)
			{
				if (!nodes[num].HitObstacle && !nodes[num + 1].HitMovingVehicle)
				{
					Transform transform = nodes[num].transform;
					RaycastHit hitInfo;
					Vector3 position = (!Physics.Raycast(transform.position, -Vector3.up, out hitInfo, halfSize.y + 2f, 1048576)) ? transform.position : hitInfo.point;
					Quaternion rotation = Quaternion.LookRotation(transform.forward * -1f);
					GoAway();
					if (this.OnPositionFound != null)
					{
						this.OnPositionFound(position, rotation, preparedSegment, preparedRoute);
					}
					return;
				}
			}
			GoAway();
			if (this.OnFailed != null)
			{
				this.OnFailed();
			}
		}

		private void GoAway()
		{
			base.transform.position = awayPosition;
			TrafficTesterNode[] array = nodes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}
	}
}
