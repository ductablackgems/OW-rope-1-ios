using System;
using UnityEngine;

namespace App.Settings
{
	[Serializable]
	public class VehicleSpawnScheme
	{
		public float minDistance;

		public float maxDistance;

		public float destroyDistance;

		[Space]
		public int maxBikeCount;

		[Space]
		public int maxVehicleCount1;

		public int maxVehicleCount2;

		public int maxVehicleCount3;

		[Space]
		public int minRouteCount1;

		public int minRouteCount2;
	}
}
