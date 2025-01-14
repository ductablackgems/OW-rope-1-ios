using App.AI;
using App.Vehicles.Car.Navigation.Roads;
using UnityEngine;

namespace App.Vehicles.Car.Navigation.Modes.Curve
{
	public class RoadSeeker : AbstractAIScript
	{
		public Transform referencePoint;

		private VehiclesManager vehiclesManager;

		private RoadSegment nextRoadSegment;

		private TrafficRoute nextRoute;

		private TrafficRoute waiterRoute;

		private RoadSeekProgress seekProgress = new RoadSeekProgress();

		private bool routeReservationEnabled = true;

		public Road Road
		{
			get;
			private set;
		}

		public RoadSegment RoadSegment
		{
			get;
			private set;
		}

		public TrafficRoute Route
		{
			get;
			private set;
		}

		public TrafficRoute NextRoute
		{
			get;
			private set;
		}

		public bool RouteReservationEnabled
		{
			get
			{
				return routeReservationEnabled;
			}
			set
			{
				EnableRouteReservation(value);
			}
		}

		public void Connect(RoadSegment roadSegment, TrafficRoute route)
		{
			if (Route != null && base.enabled)
			{
				Route.VehicleCount--;
			}
			Road = route.road;
			RoadSegment = roadSegment;
			Route = route;
			if (base.enabled)
			{
				Route.VehicleCount++;
			}
			UpdateStates();
		}

		public Vector3 UpdateStates()
		{
			return UpdateStates(GetSegmentRemainDistance());
		}

		public Vector3 UpdateStates(Vector3 segmentRemainDistance)
		{
			if (segmentRemainDistance.z < 0f)
			{
				RoadSegment roadSegment = Route.GetNextSegment(RoadSegment);
				if (roadSegment == null && nextRoute != null)
				{
					if (base.enabled)
					{
						Route.VehicleCount--;
					}
					Route = nextRoute;
					if (base.enabled)
					{
						Route.VehicleCount++;
					}
					roadSegment = Route.GetBeginSegment();
					Road = Route.road;
				}
				if (roadSegment != null)
				{
					RoadSegment = roadSegment;
					segmentRemainDistance = GetSegmentRemainDistance();
				}
				RefreshNext();
				if (waiterRoute != null && waiterRoute != nextRoute)
				{
					if (routeReservationEnabled && base.enabled)
					{
						waiterRoute.WaiterCount--;
					}
					waiterRoute = null;
				}
			}
			if (nextRoute != null && waiterRoute == null && segmentRemainDistance.z < 20f && IsOnLastSegment())
			{
				waiterRoute = nextRoute;
				if (routeReservationEnabled && base.enabled)
				{
					waiterRoute.WaiterCount++;
				}
			}
			seekProgress.UpdateStates(RoadSegment, Route, referencePoint.position);
			return segmentRemainDistance;
		}

		public void EnableRouteReservation(bool enable)
		{
			if (routeReservationEnabled != enable)
			{
				routeReservationEnabled = enable;
				if (enable && waiterRoute != null)
				{
					waiterRoute.WaiterCount++;
				}
				if (!enable && waiterRoute != null)
				{
					waiterRoute.WaiterCount--;
				}
			}
		}

		public Vector3 GetSegmentRemainDistance()
		{
			return Route.GetSegmentRemainDistance(RoadSegment, referencePoint.position);
		}

		public float GetSeekPosition()
		{
			return seekProgress.Position;
		}

		public float GetNextTresholdSpeed(float maxSpeed)
		{
			if (nextRoadSegment == null)
			{
				return 0f;
			}
			if (nextRoute != null && RoadSegment.Equals(Route.GetEndSegment()))
			{
				if (!nextRoute.CanEnter())
				{
					return 0f;
				}
				if (nextRoute.sharpCurved)
				{
					float num = nextRoadSegment.maxSpeed * 0.75f;
					if (!(num > maxSpeed))
					{
						return num;
					}
					return maxSpeed;
				}
			}
			if (!(nextRoadSegment.maxSpeed > maxSpeed))
			{
				return nextRoadSegment.maxSpeed;
			}
			return maxSpeed;
		}

		public float GetTresholdSpeed(float maxSpeed)
		{
			if (Route.sharpCurved)
			{
				float num = RoadSegment.maxSpeed * 0.75f;
				if (!(num > maxSpeed))
				{
					return num;
				}
				return maxSpeed;
			}
			if (!(RoadSegment.maxSpeed > maxSpeed))
			{
				return RoadSegment.maxSpeed;
			}
			return maxSpeed;
		}

		public float GetDistanceAngle(Vector3 direction)
		{
			if (RoadSegment == null)
			{
				return 0f;
			}
			return Route.GetDistanceAngle(RoadSegment, direction);
		}

		public Vector3 GetDirectionDistance(Vector3 forward)
		{
			return RoadSegment.transform.InverseTransformDirection(forward);
		}

		public Transform GetNextVehicle(out RoadSeeker nextSeeker)
		{
			nextSeeker = vehiclesManager.GetNext(this, 30f);
			if (!(nextSeeker == null))
			{
				return nextSeeker.ComponentsRoot.transform;
			}
			return null;
		}

		public Vector3 GetForwardPosition(float distance, out TrafficRoute route, out RoadSegment segment)
		{
			float num = seekProgress.Position + distance;
			if (num > Road.Length)
			{
				if (nextRoute == null)
				{
					route = Route;
					segment = Route.GetEndSegment();
					return Route.GetEndPosition();
				}
				num -= Road.Length;
				route = nextRoute;
				return nextRoute.GetPosition(num, out segment);
			}
			route = Route;
			return Route.GetPosition(num, out segment);
		}

		private void Awake()
		{
			vehiclesManager = ServiceLocator.Get<VehiclesManager>();
		}

		private void Start()
		{
			Road = Route.road;
			RefreshNext();
			seekProgress.UpdateStates(RoadSegment, Route, referencePoint.position);
		}

		private void OnEnable()
		{
			Route.VehicleCount++;
		}

		private void OnDisable()
		{
			Route.VehicleCount--;
			if (waiterRoute != null)
			{
				if (routeReservationEnabled)
				{
					waiterRoute.WaiterCount--;
				}
				waiterRoute = null;
			}
		}

		private void RefreshNext()
		{
			nextRoadSegment = ((Route == null) ? null : Route.GetNextSegment(RoadSegment));
			if (Route == null)
			{
				nextRoute = null;
			}
			else
			{
				int nextRouteCount = Route.GetNextRouteCount();
				nextRoute = ((nextRouteCount <= 1) ? Route.GetNextRoute() : Route.GetNextRoute(Random.Range(0, nextRouteCount)));
			}
			if (nextRoute != null && nextRoadSegment == null)
			{
				nextRoadSegment = nextRoute.GetBeginSegment();
			}
		}

		private bool IsOnLastSegment()
		{
			RoadSegment endSegment;
			if (Route != null && (endSegment = Route.GetEndSegment()) != null)
			{
				return endSegment.Equals(RoadSegment);
			}
			return false;
		}
	}
}
