using UnityEngine;

namespace App.Vehicles.Car.Navigation.Roads
{
	public class Road : MonoBehaviour
	{
		public RoadSegment firstRoadSegment;

		public RoadSegment lastRoadSegment;

		public TrafficRoute[] routes;

		public float Length
		{
			get;
			private set;
		}

		private void Awake()
		{
			int num = 0;
			RoadSegment nextSegment = firstRoadSegment;
			while (nextSegment != null)
			{
				nextSegment.PreviousLength = Length;
				Length += nextSegment.Length;
				nextSegment = nextSegment.nextSegment;
				num++;
				if (num > 300)
				{
					UnityEngine.Debug.LogError("Limit 300 segments per road exceeded.");
					return;
				}
			}
			num = 0;
			nextSegment = lastRoadSegment;
			float num2 = 0f;
			do
			{
				if (nextSegment != null)
				{
					nextSegment.NextLength = num2;
					num2 += nextSegment.Length;
					nextSegment = nextSegment.previousSegment;
					num++;
					continue;
				}
				return;
			}
			while (num <= 300);
			UnityEngine.Debug.LogError("Limit 300 segments per road exceeded.");
		}
	}
}
