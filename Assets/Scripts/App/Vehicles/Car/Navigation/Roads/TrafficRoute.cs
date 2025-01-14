using UnityEngine;

namespace App.Vehicles.Car.Navigation.Roads
{
	public class TrafficRoute : MonoBehaviour
	{
		public int VehicleCount;

		public int WaiterCount;

		public TrafficRoute[] mustBeEmptyRoutes;

		public TrafficRoute[] noWaiterRoutes;

		public TrafficRoutePolarity polarity;

		public bool sharpCurved;

		public float offset;

		public Road road;

		[Space]
		public RouteConnector beginConnector;

		public RouteConnector endConnector;

		public bool CanEnter()
		{
			TrafficRoute[] array = mustBeEmptyRoutes;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].VehicleCount > 0)
				{
					return false;
				}
			}
			array = noWaiterRoutes;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].WaiterCount > 0)
				{
					return false;
				}
			}
			return true;
		}

		public RoadSegment GetBeginSegment()
		{
			if (polarity != 0)
			{
				return road.lastRoadSegment;
			}
			return road.firstRoadSegment;
		}

		public RoadSegment GetEndSegment()
		{
			if (polarity != TrafficRoutePolarity.Opposite)
			{
				return road.lastRoadSegment;
			}
			return road.firstRoadSegment;
		}

		public RoadSegment GetNextSegment(RoadSegment segment)
		{
			if (polarity != 0)
			{
				return segment.previousSegment;
			}
			return segment.nextSegment;
		}

		public Vector3 GetBeginPosition()
		{
			RoadSegment beginSegment = GetBeginSegment();
			Vector3 b = offset * beginSegment.transform.right;
			if (polarity == TrafficRoutePolarity.Positive)
			{
				return beginSegment.transform.position + b - beginSegment.transform.forward * beginSegment.beginTangent * offset;
			}
			return beginSegment.GetEndPosition() + b - beginSegment.transform.forward * beginSegment.endTangent * offset;
		}

		public Vector3 GetEndPosition()
		{
			RoadSegment endSegment = GetEndSegment();
			Vector3 b = offset * endSegment.transform.right;
			if (polarity == TrafficRoutePolarity.Opposite)
			{
				return endSegment.transform.position + b - endSegment.transform.forward * endSegment.beginTangent * offset;
			}
			return endSegment.GetEndPosition() + b - endSegment.transform.forward * endSegment.endTangent * offset;
		}

		public int GetNextRouteCount()
		{
			if (endConnector == null)
			{
				return 0;
			}
			if (!endConnector.motherRoute.Equals(this))
			{
				return 1;
			}
			return endConnector.childRoutes.Length;
		}

		public TrafficRoute GetNextRoute(int connectedRouteIndex = 0)
		{
			if (endConnector == null)
			{
				return null;
			}
			if (!endConnector.motherRoute.Equals(this))
			{
				return endConnector.motherRoute;
			}
			return endConnector.childRoutes[connectedRouteIndex];
		}

		public Vector3 GetSegmentRemainDistance(RoadSegment segment, Vector3 position)
		{
			Vector3 vector = segment.transform.InverseTransformPoint(position) - offset * Vector3.right;
			if (polarity == TrafficRoutePolarity.Positive)
			{
				vector.z = segment.Length - vector.z - segment.endTangent * offset;
				return vector;
			}
			vector.x *= -1f;
			vector.z += segment.beginTangent * offset;
			return vector;
		}

		public float GetDistanceAngle(RoadSegment segment, Vector3 direction)
		{
			Vector3 vector = segment.transform.InverseTransformDirection(direction);
			float num = Vector3.Angle(Vector3.forward, vector);
			if (polarity == TrafficRoutePolarity.Opposite)
			{
				num -= 180f;
			}
			if (!(vector.x < 0f))
			{
				return num;
			}
			return 0f - num;
		}

		public Vector3 GetClosestPosition(Vector3 referencePosition, out RoadSegment segment)
		{
			RoadSegment roadSegment = road.firstRoadSegment;
			Vector3 a = Vector3.zero;
			segment = null;
			float num = float.MaxValue;
			while (roadSegment != null)
			{
				Vector3 closestPosition = roadSegment.GetClosestPosition(referencePosition);
				float num2 = Vector3.Distance(closestPosition, referencePosition);
				if (segment == null || num2 < num)
				{
					segment = roadSegment;
					num = num2;
					a = closestPosition;
				}
				roadSegment = roadSegment.nextSegment;
			}
			return a + offset * segment.transform.right;
		}

		public Vector3 GetPosition(float position, out RoadSegment segment)
		{
			if (polarity == TrafficRoutePolarity.Positive)
			{
				segment = road.firstRoadSegment;
				while (!(segment.Length + segment.PreviousLength > position) && !(segment.nextSegment == null))
				{
					segment = segment.nextSegment;
				}
				return segment.transform.position + (position - segment.PreviousLength) * segment.transform.forward + offset * segment.transform.right;
			}
			segment = road.lastRoadSegment;
			while (!(segment.Length + segment.NextLength > position) && !(segment.previousSegment == null))
			{
				segment = segment.previousSegment;
			}
			return segment.transform.position + (segment.Length - position + segment.NextLength) * segment.transform.forward + offset * segment.transform.right;
		}

		public Vector3 GetRelativeDistance(RoadSegment segment, Vector3 position1, Vector3 position2)
		{
			Vector3 vector = segment.transform.InverseTransformPoint(position1) - segment.transform.InverseTransformPoint(position2);
			if (polarity == TrafficRoutePolarity.Opposite)
			{
				return -vector;
			}
			return vector;
		}

		private void OnDrawGizmos()
		{
			Transform obj = null;
			bool flag = base.transform.Equals(obj);
			Transform parent = base.transform.parent;
			while (parent != null && !flag)
			{
				if (parent.Equals(obj))
				{
					flag = true;
				}
				parent = parent.parent;
			}
			bool flag2 = flag || (beginConnector != null && beginConnector.transform.Equals(obj)) || (endConnector != null && endConnector.transform.Equals(obj));
			if (RoadsGeneratorConfig.showOnlyActiveRoutes && !flag2)
			{
				return;
			}
			Vector3 b = new Vector3(0f, 0.15f);
			Color color = new Color(0f, 38f / 255f, 148f / 255f);
			Color color2 = new Color(46f / 85f, 0f, 7f / 51f);
			Color cyan = Color.cyan;
			Color magenta = Color.magenta;
			RoadSegment roadSegment = road.firstRoadSegment;
			int num = 0;
			while (roadSegment != null && num < 200)
			{
				bool flag3 = flag || roadSegment.transform.Equals(obj);
				Color color4 = Gizmos.color = ((polarity != 0) ? ((flag3 | flag2) ? magenta : color2) : ((flag3 | flag2) ? cyan : color));
				Vector3 a = ((polarity == TrafficRoutePolarity.Positive) ? (-2) : 2) * roadSegment.transform.forward;
				Vector3 b2 = offset * roadSegment.transform.right;
				Vector3 vector = roadSegment.transform.position + b2 - roadSegment.transform.forward * roadSegment.beginTangent * offset;
				Vector3 a2 = roadSegment.GetEndPosition() + b2 - roadSegment.transform.forward * roadSegment.endTangent * offset;
				if ((a2 - vector).magnitude < 8f)
				{
					a *= 0.5f;
				}
				Gizmos.DrawLine(vector + b, a2 + b);
				roadSegment = roadSegment.nextSegment;
				num++;
			}
		}
	}
}
