using App.Vehicles.Car.Navigation.Modes.Curve;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles
{
	public class VehiclesManager : MonoBehaviour
	{
		private HashSet<RoadSeeker> seekers = new HashSet<RoadSeeker>();

		public void Register(RoadSeeker seeker)
		{
			seekers.Add(seeker);
		}

		public void Unregister(RoadSeeker seeker)
		{
			seekers.Remove(seeker);
		}

		public RoadSeeker GetNext(RoadSeeker seeker, float maxDistance)
		{
			float seekPosition = seeker.GetSeekPosition();
			RoadSeeker roadSeeker = null;
			float num = maxDistance;
			Vector3 position = seeker.ComponentsRoot.position;
			foreach (RoadSeeker seeker2 in seekers)
			{
				if (!(Vector3.Distance(seeker2.ComponentsRoot.position, position) > maxDistance) && !seeker.Equals(seeker2) && seeker.Route.Equals(seeker2.Route))
				{
					float seekPosition2 = seeker2.GetSeekPosition();
					if (!(seekPosition2 < seekPosition))
					{
						float num2 = seekPosition2 - seekPosition;
						if (!(num2 > num))
						{
							num = num2;
							roadSeeker = seeker2;
						}
					}
				}
			}
			if (roadSeeker != null)
			{
				return roadSeeker;
			}
			if (seeker.NextRoute == null)
			{
				return null;
			}
			float num3 = seeker.Road.Length - seekPosition;
			if (num3 > maxDistance)
			{
				return null;
			}
			float num4 = maxDistance - num3;
			foreach (RoadSeeker seeker3 in seekers)
			{
				if (seeker.NextRoute.Equals(seeker3.Route))
				{
					float seekPosition3 = seeker3.GetSeekPosition();
					if (!(seekPosition3 > num4))
					{
						num4 = seekPosition3;
						roadSeeker = seeker3;
					}
				}
			}
			return roadSeeker;
		}
	}
}
