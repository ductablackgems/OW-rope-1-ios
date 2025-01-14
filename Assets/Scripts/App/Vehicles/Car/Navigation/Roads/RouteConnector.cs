using UnityEngine;

namespace App.Vehicles.Car.Navigation.Roads
{
	public class RouteConnector : MonoBehaviour
	{
		public TrafficRoute motherRoute;

		public TrafficRoute[] childRoutes;
	}
}
