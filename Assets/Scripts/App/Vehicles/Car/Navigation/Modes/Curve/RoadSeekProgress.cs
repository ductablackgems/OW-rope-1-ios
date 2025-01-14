using App.Vehicles.Car.Navigation.Roads;
using UnityEngine;

namespace App.Vehicles.Car.Navigation.Modes.Curve
{
	public class RoadSeekProgress
	{
		public float Position
		{
			get;
			private set;
		}

		public void UpdateStates(RoadSegment roadSegment, TrafficRoute route, Vector3 position)
		{
			float z = roadSegment.GetDistance(position).z;
			if (route.polarity == TrafficRoutePolarity.Positive)
			{
				Position = roadSegment.PreviousLength + z;
			}
			else
			{
				Position = roadSegment.NextLength + roadSegment.Length - z;
			}
		}
	}
}
