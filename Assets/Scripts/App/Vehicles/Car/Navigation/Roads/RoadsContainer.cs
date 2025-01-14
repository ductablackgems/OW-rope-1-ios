using UnityEngine;

namespace App.Vehicles.Car.Navigation.Roads
{
	public class RoadsContainer : MonoBehaviour
	{
		private Road[] roads;

		public bool GetCarInFirePosition(Vector3 playerPosition, float minDistance, out RoadSegment segment, out TrafficRoute route, out Vector3 position, out Quaternion rotation)
		{
			for (int i = 0; i <= 15; i++)
			{
				Road road = roads[Random.Range(0, roads.Length)];
				segment = road.firstRoadSegment;
				while (segment != null)
				{
					Vector3 vector = (segment.transform.position + segment.GetEndPosition()) / 2f;
					if (Vector3.Distance(playerPosition, vector) > minDistance)
					{
						route = road.routes[Random.Range(0, road.routes.Length)];
						if (route.polarity == TrafficRoutePolarity.Positive)
						{
							rotation = segment.transform.rotation;
						}
						else
						{
							Vector3 eulerAngles = segment.transform.eulerAngles;
							eulerAngles.y += 180f;
							rotation = Quaternion.Euler(eulerAngles);
						}
						position = vector + segment.transform.right * route.offset;
						return true;
					}
					segment = segment.nextSegment;
				}
			}
			segment = null;
			route = null;
			position = Vector3.zero;
			rotation = Quaternion.identity;
			return false;
		}

		private void Awake()
		{
			roads = GetComponentsInChildren<Road>();
		}
	}
}
