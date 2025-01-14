using UnityEngine;

namespace App.Vehicles.Car.Navigation.Roads
{
	public class RoadsGeneratorConfig : ScriptableObject
	{
		public static bool showOnlyActiveRoutes = true;

		public static bool showOnlyFirstSegmentGizmo = true;

		public GameObject roadVPrefab;

		public GameObject roadHPrefab;

		public GameObject roadTLPrefab;

		public GameObject roadTRPrefab;

		public GameObject roadBLPrefab;

		public GameObject roadBRPrefab;

		[Space]
		public GameObject connectorPrefab;
	}
}
